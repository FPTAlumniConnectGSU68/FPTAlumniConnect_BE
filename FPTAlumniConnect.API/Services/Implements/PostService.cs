using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PostService : BaseService<PostService>, IPostService
    {
        public PostService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<PostService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Create a new post
        public async Task<int> CreateNewPost(PostInfo request)
        {
            _logger.LogInformation("Creating new post with title: {Title}", request.Title);

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new BadHttpRequestException("Title cannot be empty");

            Post newPost = _mapper.Map<Post>(request);
            newPost.CreatedAt = DateTime.Now;
            // newPost.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            _logger.LogInformation("Post created successfully with ID: {PostId}", newPost.PostId);
            return newPost.PostId;
        }

        // Get a post by its ID
        public async Task<PostReponse> GetPostById(int id)
        {
            _logger.LogInformation("Getting post by ID: {PostId}", id);

            Func<IQueryable<Post>, IIncludableQueryable<Post, object>> include =
                q => q.Include(u => u.Major).Include(u => u.Author);

            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id), include: include) ??
                throw new BadHttpRequestException("PostNotFound");

            PostReponse result = _mapper.Map<PostReponse>(post);
            return result;
        }

        // Update post information
        public async Task<bool> UpdatePostInfo(int id, PostInfo request)
        {
            _logger.LogInformation("Updating post with ID: {PostId}", id);

            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id)) ??
                throw new BadHttpRequestException("PostNotFound");

            UpdatePostFields(post, request);

            _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("Post updated successfully: {PostId}", id);
            else
                _logger.LogWarning("Post update failed: {PostId}", id);

            return isSuccessful;
        }

        // Get paginated list of posts with filters
        public async Task<IPaginate<PostReponse>> ViewAllPost(PostFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all posts with filter and paging.");

            Func<IQueryable<Post>, IIncludableQueryable<Post, object>> include =
                q => q.Include(u => u.Major).Include(u => u.Author);

            IPaginate<PostReponse> response = await _unitOfWork.GetRepository<Post>().GetPagingListAsync(
                selector: x => _mapper.Map<PostReponse>(x),
                filter: filter,
                include: include,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        // Helper: Update post fields from request
        private void UpdatePostFields(Post post, PostInfo request)
        {
            post.Title = string.IsNullOrEmpty(request.Title) ? post.Title : request.Title;
            post.Content = string.IsNullOrEmpty(request.Content) ? post.Content : request.Content;
            post.IsPrivate = request.IsPrivate ?? post.IsPrivate;
            post.MajorId = request.MajorId ?? post.MajorId;
            post.AuthorId = request.AuthorId ?? post.AuthorId;
            post.UpdatedAt = DateTime.Now;
            post.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }

        // Default include function (currently only includes Major)
        private Func<IQueryable<Post>, IIncludableQueryable<Post, object>> DefaultIncludes()
        {
            return q => q.Include(x => x.Major);
        }
    }
}
