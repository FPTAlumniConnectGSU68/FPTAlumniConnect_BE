using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
            newPost.CreatedAt = TimeHelper.NowInVietnam();
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
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<int> CountAllPosts()
        {
            ICollection<PostReponse> posts = await _unitOfWork.GetRepository<Post>().GetListAsync(
                selector: x => _mapper.Map<PostReponse>(x));
            int count = posts.Count();
            return count;
        }

        public async Task<ICollection<CountByMonthResponse>> CountPostsByMonth(int? month, int? year)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByMonthResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<Post>().GetListAsync(
                    selector: x => _mapper.Map<PostReponse>(x),
                    predicate: x => x.CreatedAt.HasValue
                                    && x.CreatedAt.Value.Year == targetYear
                                    && x.CreatedAt.Value.Month == m
                );
                result.Add(new CountByMonthResponse
                {
                    Month = m,
                    Year = targetYear,
                    Count = users.Count()
                });
            }
            return result;
        }



        // Helper: Update post fields from request
        private void UpdatePostFields(Post post, PostInfo request)
        {
            post.Title = string.IsNullOrEmpty(request.Title) ? post.Title : request.Title;
            post.Content = string.IsNullOrEmpty(request.Content) ? post.Content : request.Content;
            post.IsPrivate = request.IsPrivate ?? post.IsPrivate;
            post.MajorId = request.MajorId ?? post.MajorId;
            post.AuthorId = request.AuthorId ?? post.AuthorId;
            post.UpdatedAt = TimeHelper.NowInVietnam();
            post.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }

        // Get top users by number of posts
        public async Task<IEnumerable<object>> GetTopUsersByNumberOfPosts(int topN = 10)
        {
            _logger.LogInformation("Getting top {TopN} users by number of posts", topN);

            var query = _unitOfWork.GetRepository<Post>().GetQueryable()
                .Include(p => p.Author);

            var topUsers = await query
                .GroupBy(p => p.AuthorId)
                .Select(g => new
                {
                    UserId = g.Key,
                    UserAvatar = g.FirstOrDefault().Author != null ? g.FirstOrDefault().Author.ProfilePicture : "default-avatar.png",
                    UserCode = g.FirstOrDefault().Author != null ? g.FirstOrDefault().Author.Code : "Unknown",
                    UserName = g.FirstOrDefault().Author != null
                        ? g.FirstOrDefault().Author.FirstName + " " + g.FirstOrDefault().Author.LastName
                        : "Unknown",
                    PostCount = g.Count()
                })
                .OrderByDescending(x => x.PostCount)
                .Take(topN)
                .ToListAsync();

            return topUsers;
        }

        // Default include function (currently only includes Major)
        private Func<IQueryable<Post>, IIncludableQueryable<Post, object>> DefaultIncludes()
        {
            return q => q.Include(x => x.Major);
        }
    }
}
