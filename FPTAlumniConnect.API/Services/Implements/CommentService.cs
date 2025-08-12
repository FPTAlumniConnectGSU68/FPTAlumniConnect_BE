using AutoMapper;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FPTAlumniConnect.API.Services.Implements
{
    // Service for managing comment-related actions
    public class CommentService : BaseService<CommentService>, ICommentService
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IPerspectiveService _perspectiveService;

        public CommentService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<CommentService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPostService postService,
            INotificationService notificationService,
            IPerspectiveService perspectiveService,
            IUserService userService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _postService = postService;
            _notificationService = notificationService;
            _userService = userService;
            _perspectiveService = perspectiveService;
        }

        // Create a new comment
        public async Task<int> CreateNewComment(CommentInfo request)
        {
            // Validate the comment info
            ValidateCommentInfo(request);

            // Get the target post
            var post = await _postService.GetPostById(request.PostId);
            if (post == null || post.AuthorId == null)
                throw new BadHttpRequestException("Post not found");

            // Get the parent Comment or not
            if (request.ParentCommentId != null)
            {
                Comment parentComment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId == request.ParentCommentId)
                ?? throw new NotFoundException("ParentCommentNotFound");

                // Kiểm tra PostId của comment cha có trùng với PostId gửi lên không
                if (parentComment.PostId != request.PostId)
                {
                    throw new BadHttpRequestException("ParentCommentPostMismatch");
                }
            }

            // Get the commenting user
            var user = await _userService.GetUserById(request.AuthorId);
            if (user == null)
                throw new BadHttpRequestException("User not found");

            // Check content appropriateness using perspective API
            if (!await _perspectiveService.IsContentAppropriate(request.Content))
            {
                throw new BadHttpRequestException("Comment contains inappropriate content.");
            }

            // Map to entity and insert into database
            Comment newComment = _mapper.Map<Comment>(request);
            await _unitOfWork.GetRepository<Comment>().InsertAsync(newComment);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            // Prepare notification for the post author
            var commenter = await _userService.GetUserById(request.AuthorId);
            if (commenter == null)
                throw new BadHttpRequestException("Notification preparation failed");

            var notificationPayload = new NotificationPayload
            {
                UserId = post.AuthorId.Value,
                Message = $"{commenter.FirstName} commented on your post: {post.Title}",
                IsRead = false
            };

            // Send notification
            await _notificationService.SendNotificationAsync(notificationPayload);

            return newComment.CommentId;
        }

        // Validate comment data
        private void ValidateCommentInfo(CommentInfo request)
        {
            List<string> errors = new List<string>();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Comment information cannot be null");
            }
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                errors.Add("Content is required");
            }
            if (errors.Any())
            {
                throw new BadHttpRequestException($"Validation failed: {string.Join(", ", errors)}");
            }
        }

        // Get comment by ID
        public async Task<CommentReponse> GetCommentById(int id)
        {
            // Lấy comment gốc
            var comment = await _unitOfWork.GetRepository<Comment>()
                .SingleOrDefaultAsync(
                    predicate: x => x.CommentId == id,
                    include: q => q.Include(c => c.Author)
                ) ?? throw new BadHttpRequestException("CommentNotFound");

            // Map sang response
            var response = _mapper.Map<CommentReponse>(comment);

            // Lấy child comments (nhiều cấp)
            response.ChildComments = await GetChildCommentsAsync(id);

            return response;
        }

        // Hàm đệ quy lấy child comments
        private async Task<List<CommentReponse>> GetChildCommentsAsync(int parentId)
        {
            // Lấy danh sách comment con của parentId
            var childComments = await _unitOfWork.GetRepository<Comment>()
                .GetListAsync(
                    predicate: x => x.ParentCommentId == parentId,
                    include: q => q.Include(c => c.Author)
                );

            // Map sang response
            var childResponses = _mapper.Map<List<CommentReponse>>(childComments);

            // Với mỗi comment con, lấy tiếp comment cháu (nếu có)
            foreach (var child in childResponses)
            {
                child.ChildComments = await GetChildCommentsAsync(child.CommentId);
            }

            return childResponses;
        }

        // Update existing comment
        public async Task<bool> UpdateCommentInfo(int id, CommentInfo request)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id))
                ?? throw new BadHttpRequestException("CommentNotFound");

            // PostId, AuthorId, and ParentCommentId should not be changed
            comment.Content = string.IsNullOrEmpty(request.Content) ? comment.Content : request.Content;
            comment.UpdatedAt = DateTime.Now;
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // View paginated comments with validation of parent-child relationship
        public async Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            // 1) Lấy root comments theo filter + paging
            Expression<Func<Comment, bool>> rootPredicate = x =>
                (!filter.PostId.HasValue || x.PostId == filter.PostId) &&
                (!filter.AuthorId.HasValue || x.AuthorId == filter.AuthorId) &&
                (x.ParentCommentId == filter.ParentCommentId);

            var rootComments = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
                selector: x => _mapper.Map<CommentReponse>(x),
                predicate: rootPredicate,
                include: q => q.Include(c => c.Author),
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            if (!rootComments.Items.Any())
                return rootComments;

            // 2) Xác định postId (dùng filter nếu có, nếu không thì lấy từ một root comment)
            var postId = filter.PostId ?? rootComments.Items.First().PostId;

            // 3) Lấy tất cả comment của post trong 1 query (để dựng full tree)
            var allCommentsEntities = await _unitOfWork.GetRepository<Comment>().GetListAsync(
                predicate: x => x.PostId == postId,
                include: q => q.Include(c => c.Author)
            );

            // 4) Map tất cả sang CommentReponse và tạo dictionary cho lookup nhanh
            var allMapped = allCommentsEntities
                .Select(c => _mapper.Map<CommentReponse>(c))
                .OrderBy(c => c.CreatedAt)
                .ToDictionary(c => c.CommentId, c => c);

            // 5) Dựng cây: thêm mỗi comment vào ChildComments của parent (nếu có)
            foreach (var comment in allMapped.Values)
            {
                if (comment.ParentCommentId.HasValue)
                {
                    if (allMapped.TryGetValue(comment.ParentCommentId.Value, out var parent))
                    {
                        parent.ChildComments ??= new List<CommentReponse>();
                        parent.ChildComments.Add(comment);
                    }
                    else
                    {
                        _logger.LogWarning("Parent comment not found for CommentId = {CommentId}", comment.CommentId);
                    }
                }
            }

            // 6) Thay từng phần tử root trong Items bằng bản full tree
            for (int i = 0; i < rootComments.Items.Count; i++)
            {
                var rootId = rootComments.Items[i].CommentId;
                if (allMapped.TryGetValue(rootId, out var fullTree))
                {
                    rootComments.Items[i] = fullTree;
                }
            }

            // 7) Sắp xếp con theo CreatedAt đệ quy
            void SortChildrenRecursive(CommentReponse node)
            {
                if (node.ChildComments == null || !node.ChildComments.Any()) return;
                node.ChildComments = node.ChildComments.OrderBy(c => c.CreatedAt).ToList();
                foreach (var child in node.ChildComments) SortChildrenRecursive(child);
            }

            foreach (var root in rootComments.Items)
            {
                SortChildrenRecursive(root);
            }

            return rootComments;
        }
    }
}
