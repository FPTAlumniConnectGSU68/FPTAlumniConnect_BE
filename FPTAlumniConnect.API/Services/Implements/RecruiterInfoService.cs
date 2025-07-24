using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.RecruiterInfo;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class RecruiterInfoService : BaseService<RecruiterInfoService>, IRecruiterInfoService
    {
        public RecruiterInfoService(IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<RecruiterInfoService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Create a new recruiter info (one per user)
        public async Task<int> CreateNewRecruiterInfo(RecruiterInfoInfo request)
        {
            // Ensure user exists
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (user == null)
                throw new BadHttpRequestException("UserNotFound");

            // Ensure user does not already have recruiter info
            var existing = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId);
            if (existing != null)
                throw new BadHttpRequestException("RecruiterInfoAlreadyExists");

            var entity = _mapper.Map<RecruiterInfo>(request);
            entity.CreatedAt = DateTime.UtcNow;
            //entity.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<RecruiterInfo>().InsertAsync(entity);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
                throw new BadHttpRequestException("CreateFailed");

            return entity.RecruiterInfoId;
        }

        // Get recruiter info by ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoById(int id)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Get recruiter info by user ID
        public async Task<RecruiterInfoResponse> GetRecruiterInfoByUserId(int userId)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.UserId == userId)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            return _mapper.Map<RecruiterInfoResponse>(entity);
        }

        // Update recruiter info
        public async Task<bool> UpdateRecruiterInfo(int id, RecruiterInfoInfo request)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            entity.CompanyName = request.CompanyName ?? entity.CompanyName;
            entity.CompanyEmail = request.CompanyEmail ?? entity.CompanyEmail;
            entity.CompanyPhone = request.CompanyPhone ?? entity.CompanyPhone;
            entity.CompanyLogoUrl = request.CompanyLogoUrl ?? entity.CompanyLogoUrl;
            entity.CompanyCertificateUrl = request.CompanyCertificateUrl ?? entity.CompanyCertificateUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            //entity.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<RecruiterInfo>().UpdateAsync(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Get all recruiter info with filter & pagination
        public async Task<IPaginate<RecruiterInfoResponse>> ViewAllRecruiters(RecruiterInfoFilter filter, PagingModel paging)
        {
            var response = await _unitOfWork.GetRepository<RecruiterInfo>().GetPagingListAsync(
                selector: x => _mapper.Map<RecruiterInfoResponse>(x),
                filter: filter,
                page: paging.page,
                size: paging.size
            );

            return response;
        }

        // Delete recruiter info by ID
        public async Task<bool> DeleteRecruiterInfo(int id)
        {
            var entity = await _unitOfWork.GetRepository<RecruiterInfo>().SingleOrDefaultAsync(
                predicate: x => x.RecruiterInfoId == id)
                ?? throw new BadHttpRequestException("RecruiterInfoNotFound");

            _unitOfWork.GetRepository<RecruiterInfo>().DeleteAsync(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Optional: Count all recruiter infos in system
        public async Task<int> CountAllRecruiters()
        {
            return await _unitOfWork.GetRepository<RecruiterInfo>()
                .GetQueryable()
                .CountAsync();
        }
    }
}
