using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    // Service for managing post report operations (create, update, view, get by id)
    public class PostReportService : BaseService<PostReportService>, IPostReportService
    {
        public PostReportService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<PostReportService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Creates a new post report
        public async Task<int> CreateNewReport(PostReportInfo request)
        {
            // Validate PostId exists
            Post checkPostId = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: s => s.PostId == request.PostId);
            if (checkPostId == null)
            {
                throw new BadHttpRequestException("PostIdNotFound");
            }

            // Validate UserId exists
            User checkUserId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: s => s.UserId == request.UserId);
            if (checkUserId == null)
            {
                throw new BadHttpRequestException("UserIdNotFound");
            }

            // Map and insert
            PostReport newRp = _mapper.Map<PostReport>(request);
            await _unitOfWork.GetRepository<PostReport>().InsertAsync(newRp);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newRp.RpId;
        }

        // Updates a report's information
        public async Task<bool> UpdateReportInfo(int id, PostReportInfo request)
        {
            // Find report by PostId (note: should likely be RpId instead?)
            PostReport rp = await _unitOfWork.GetRepository<PostReport>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id))
                ?? throw new BadHttpRequestException("ReportNotFound");

            // Validate new PostId exists
            Post checkPostId = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: s => s.PostId == request.PostId);
            if (checkPostId == null)
            {
                throw new BadHttpRequestException("PostIdNotFound");
            }

            // Validate new UserId exists
            User checkAuthorId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: s => s.UserId == request.UserId);
            if (checkAuthorId == null)
            {
                throw new BadHttpRequestException("UserIdNotFound");
            }

            // Update content
            rp.TypeOfReport = string.IsNullOrEmpty(request.TypeOfReport) ? rp.TypeOfReport : request.TypeOfReport;
            rp.UpdatedAt = DateTime.Now;
            rp.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<PostReport>().UpdateAsync(rp);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Retrieves report details by report ID
        public async Task<PostReportReponse> GetReportById(int id)
        {
            PostReport rp = await _unitOfWork.GetRepository<PostReport>().SingleOrDefaultAsync(
                predicate: x => x.RpId.Equals(id))
                ?? throw new BadHttpRequestException("PostNotFound");

            return _mapper.Map<PostReportReponse>(rp);
        }

        // Returns paginated list of post reports based on filter
        public async Task<IPaginate<PostReportReponse>> ViewAllReport(PostReportFilter filter, PagingModel pagingModel)
        {
            var response = await _unitOfWork.GetRepository<PostReport>().GetPagingListAsync(
                selector: x => _mapper.Map<PostReportReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
