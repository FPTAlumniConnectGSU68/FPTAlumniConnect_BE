using AutoMapper;
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

            // Check content appropriateness using perspective API
            if (!await _perspectiveService.IsContentAppropriate(request.Content))
            {
                throw new BadHttpRequestException("Comment contains inappropriate content.");
            }

            // Map to entity and insert into database
            Comment newComment = _mapper.Map<Comment>(request);
            newComment.CreatedAt = TimeHelper.NowInVietnam();
            newComment.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            await _unitOfWork.GetRepository<Comment>().InsertAsync(newComment);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            // Get the target post
            var post = await _postService.GetPostById(request.PostId);
            if (post == null || post.AuthorId == null)
                throw new BadHttpRequestException("Post not found");

            // Get the commenting user
            var user = await _userService.GetUserById(request.AuthorId);
            if (user == null)
                throw new BadHttpRequestException("User not found");

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
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id))
                ?? throw new BadHttpRequestException("CommentNotFound");

            return _mapper.Map<CommentReponse>(comment);
        }

        // Update existing comment
        public async Task<bool> UpdateCommentInfo(int id, CommentInfo request)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id))
                ?? throw new BadHttpRequestException("CommentNotFound");

            // PostId, AuthorId, and ParentCommentId should not be changed
            comment.Content = string.IsNullOrEmpty(request.Content) ? comment.Content : request.Content;
            comment.UpdatedAt = TimeHelper.NowInVietnam();
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // View paginated comments with validation of parent-child relationship
        public async Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            //var comments = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
            //    selector: x => _mapper.Map<CommentReponse>(x),
            //    filter: filter,
            //    orderBy: x => x.OrderBy(x => x.CreatedAt),
            //    page: pagingModel.page,
            //    size: pagingModel.size
            //);
            Expression<Func<Comment, bool>> predicate = x =>
        (!filter.PostId.HasValue || x.PostId == filter.PostId) &&
        (!filter.AuthorId.HasValue || x.AuthorId == filter.AuthorId) &&
        (x.ParentCommentId == filter.ParentCommentId);

            var comments = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
                selector: x => _mapper.Map<CommentReponse>(x),
                predicate: predicate,
                include: q => q.Include(c => c.Author),
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            // If getting root comments, include their direct replies in ChildComments
            if (filter.ParentCommentId == null)
            {
                var commentIds = comments.Items.Select(c => c.CommentId).ToList();

                var childComments = await _unitOfWork.GetRepository<Comment>().GetListAsync(
                    predicate: x => commentIds.Contains(x.ParentCommentId.Value),
                    include: q => q.Include(c => c.Author)
                );

                var mappedChildComments = childComments
                    .Select(x => _mapper.Map<CommentReponse>(x))
                    .ToList();

                // Group child comments by ParentCommentId
                var groupedReplies = mappedChildComments
                    .GroupBy(x => x.ParentCommentId)
                    .ToDictionary(g => g.Key!.Value, g => g.ToList());

                // Assign replies to corresponding root comment
                foreach (var parent in comments.Items)
                {
                    if (groupedReplies.TryGetValue(parent.CommentId, out var replies))
                    {
                        parent.ChildComments = replies;
                    }
                }
            }

            var errorMessages = new List<string>();

            foreach (var commentResponse in comments.Items)
            {
                // If comment has a parent, validate its existence and PostId
                if (commentResponse.ParentCommentId.HasValue)
                {
                    var parentComment = await _unitOfWork.GetRepository<Comment>()
                        .FindAsync(x => x.CommentId == commentResponse.ParentCommentId.Value);

                    if (parentComment == null)
                    {
                        errorMessages.Add($"Parent comment not found for CommentId = {commentResponse.CommentId} (ParentCommentId = {commentResponse.ParentCommentId})");
                        continue;
                    }

                    if (parentComment.PostId != commentResponse.PostId)
                    {
                        errorMessages.Add($"PostId mismatch between comment (ID = {commentResponse.CommentId}, PostId = {commentResponse.PostId}) and its parent (ID = {parentComment.CommentId}, PostId = {parentComment.PostId})");
                        continue;
                    }
                }
            }

            // Optional: Log the validation errors
            if (errorMessages.Any())
            {
                foreach (var message in errorMessages)
                {
                    Console.WriteLine(message); // Or use logger
                }
            }

            return comments;
        }
    }
}
