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

        public async Task<int> CreateNewPost(PostInfo request)
        {
            Post newPost = _mapper.Map<Post>(request);

            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newPost.PostId;
        }

        public async Task<PostReponse> GetPostById(int id)
        {
            Func<IQueryable<Post>, IIncludableQueryable<Post, object>> include = q => q.Include(u => u.Major);

            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id), include: include) ??
                throw new BadHttpRequestException("PostNotFound");

            PostReponse result = _mapper.Map<PostReponse>(post);
            return result;
        }

        public async Task<bool> UpdatePostInfo(int id, PostInfo request)
        {
            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id)) ??
                throw new BadHttpRequestException("PostNotFound");

            post.Title = string.IsNullOrEmpty(request.Title) ? post.Title : request.Title;
            post.Content = string.IsNullOrEmpty(request.Content) ? post.Content : request.Content;
            if (request.IsPrivate.HasValue)
            {
                post.IsPrivate = request.IsPrivate.Value;
            }
            if (request.MajorId.HasValue)
            {
                post.MajorId = request.MajorId.Value;
            }
            if (request.AuthorId.HasValue)
            {
                post.AuthorId = request.AuthorId.Value;
            }
            post.UpdatedAt = DateTime.Now;
            post.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<PostReponse>> ViewAllPost(PostFilter filter, PagingModel pagingModel)
        {
            Func<IQueryable<Post>, IIncludableQueryable<Post, object>> include = q => q.Include(u => u.Major);

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
    }
}