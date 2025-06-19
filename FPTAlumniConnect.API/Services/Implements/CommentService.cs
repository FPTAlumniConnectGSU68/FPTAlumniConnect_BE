using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
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

        public async Task<int> CreateNewComment(CommentInfo request)
        {
            // Validate input
            ValidateCommentInfo(request);

            // Kiểm tra nội dung bình luận
            if (!await _perspectiveService.IsContentAppropriate(request.Content))
            {
                throw new BadHttpRequestException("Bình luận chứa nội dung không phù hợp.");
            }

            // Map to entity and save comment
            Comment newComment = _mapper.Map<Comment>(request);
            await _unitOfWork.GetRepository<Comment>().InsertAsync(newComment);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            // Find post
            var post = await _postService.GetPostById(request.PostId);
            if (post == null || post.AuthorId == null)
                throw new BadHttpRequestException("find post failed");

            //find user
            var user = await _userService.GetUserById(request.AuthorId);
            if (user == null)
            throw new BadHttpRequestException("find user failed");

            //Prepare notifcation
            var commenter = await _userService.GetUserById(request.AuthorId);
            if (commenter == null)
                throw new BadHttpRequestException("prepare notification failed");
            var notificationPayload = new NotificationPayload
            {
                UserId = post.AuthorId.Value, // Tác giả bài viết
                Message = $"{commenter.FirstName} đã bình luận vào bài viết của bạn: {post.Title}",
                IsRead = false
            };

            // Send notification
            await _notificationService.SendNotificationAsync(notificationPayload);

            return newComment.CommentId;
        }

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


        public async Task<CommentReponse> GetCommentById(int id)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id)) ??
                throw new BadHttpRequestException("CommentNotFound");

            CommentReponse result = _mapper.Map<CommentReponse>(comment);
            return result;
        }

        public async Task<bool> UpdateCommentInfo(int id, CommentInfo request)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id)) ??
                throw new BadHttpRequestException("CommentNotFound");

            // Không được thay đổi PostId, AuthorId, ParentCommentId

            comment.Content = string.IsNullOrEmpty(request.Content) ? comment.Content : request.Content;
            comment.Type = string.IsNullOrEmpty(request.Type) ? comment.Type : request.Type;
            comment.UpdatedAt = DateTime.Now;
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            // Lấy danh sách comment từ repository
            var comments = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
                selector: x => _mapper.Map<CommentReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            var errorMessages = new List<string>();

            foreach (var commentResponse in comments.Items)
            {
                // Kiểm tra parentCommentId cho mỗi comment
                if (commentResponse.ParentCommentId.HasValue)
                {
                    // Tìm comment cha dựa trên parentCommentId
                    var parentComment = await _unitOfWork.GetRepository<Comment>()
                        .FindAsync(x => x.CommentId == commentResponse.ParentCommentId.Value);

                    // Kiểm tra xem comment cha có tồn tại không
                    if (parentComment == null)
                    {
                        errorMessages.Add($"Không tìm thấy comment cha cho CommentId = {commentResponse.CommentId} với ParentCommentId = {commentResponse.ParentCommentId}");
                        continue; // Bỏ qua comment hiện tại và tiếp tục với comment tiếp theo
                    }

                    // Kiểm tra postId của comment cha và con
                    if (parentComment.PostId != commentResponse.PostId)
                    {
                        errorMessages.Add($"postId của CommentId = {commentResponse.CommentId} (postId: {commentResponse.PostId}) không khớp với postId của comment cha (CommentId = {parentComment.CommentId}, postId: {parentComment.PostId})");
                        continue; // Bỏ qua comment hiện tại và tiếp tục với comment tiếp theo
                    }
                }
            }

            // Trả về dữ liệu bao gồm danh sách comment hợp lệ và thông báo lỗi
            if (errorMessages.Any())
            {
                // Thêm thông báo lỗi vào response nếu cần thiết
                foreach (var message in errorMessages)
                {
                    Console.WriteLine(message); // Hoặc ghi lại lỗi vào log
                }
            }
            return comments;
        }
    }
}
