using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EducationHistory;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EducationHistoryService : BaseService<EducationHistoryService>, IEducationHistoryService
    {
        public EducationHistoryService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EducationHistoryService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewEducationHistory(EducationHistoryInfo request)
        {
            // Kiểm tra xem Iduser có tồn tại trong bảng User không
            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: x => x.UserId.Equals(request.Iduser)) ??
                    throw new BadHttpRequestException("UserNotFound");

            if (request.Name != null)
            {
                // Optional: Add name validation
                if (request.Name.Length < 2 || request.Name.Length > 200)
                {
                    throw new BadHttpRequestException("Name must be between 2 and 200 characters.");
                }
            }

            // Kiểm tra điều kiện thời gian
            if (request.ReceivedAt.HasValue)
            {
                if (request.ReceivedAt.Value > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("ReceivedAt cannot be in the future.");
                }
            }

            // Kiểm tra Người dùng đã có EduHis này
            EducationHistory alreadyHad = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.Iduser == request.Iduser &&
                            x.Name == request.Name &&
                            x.ReceivedAt == request.ReceivedAt);
            if (alreadyHad != null)
            {
                throw new BadHttpRequestException("UserAlreadyHadThisEduHis");
            }

            // Ánh xạ request sang EducationHistory
            EducationHistory newEducationHistory = _mapper.Map<EducationHistory>(request);
            //newEducationHistory.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Thêm EducationHistory vào database
            await _unitOfWork.GetRepository<EducationHistory>().InsertAsync(newEducationHistory);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEducationHistory.EduHistoryId;
        }


        public async Task<GetEducationHistoryResponse> GetEducationHistoryById(int id)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id)) ??
                throw new BadHttpRequestException("EducationHistoryNotFound");

            GetEducationHistoryResponse result = _mapper.Map<GetEducationHistoryResponse>(educationHistory);
            return result;
        }

        public async Task<bool> UpdateEducationHistory(int id, EducationHistoryInfo request)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id)) ??
                throw new BadHttpRequestException("EducationHistoryNotFound");

            // Update Name
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                // Optional: Add name validation
                if (request.Name.Length < 2 || request.Name.Length > 200)
                {
                    throw new BadHttpRequestException("Name must be between 2 and 200 characters.");
                }
                educationHistory.Name = request.Name;
            }

            // Update ReceivedAt
            if (request.ReceivedAt.HasValue)
            {
                // Optional: Add date validation
                if (request.ReceivedAt.Value > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("ReceivedAt cannot be a future date.");
                }
                educationHistory.ReceivedAt = request.ReceivedAt.Value;
            }

            educationHistory.UpdatedAt = DateTime.Now;
            educationHistory.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
                //?? throw new UnauthorizedAccessException("User not authenticated");

            _unitOfWork.GetRepository<EducationHistory>().UpdateAsync(educationHistory);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetEducationHistoryResponse>> ViewAllEducationHistory(EducationHistoryFilter filter, PagingModel pagingModel)
        {
            // Validate date ranges
            if (filter.CreatedAtTo.HasValue && filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.Value < filter.CreatedAtFrom.Value)
            {
                throw new BadHttpRequestException("CreatedAtTo cannot be earlier than CreatedAtFrom.");
            }

            if (filter.ReceivedAtTo.HasValue && filter.ReceivedAtFrom.HasValue && filter.ReceivedAtTo.Value < filter.ReceivedAtFrom.Value)
            {
                throw new BadHttpRequestException("ReceivedAtTo cannot be earlier than ReceivedAtFrom.");
            }

            // Build filter expression
            Expression<Func<EducationHistory, bool>> predicate = x =>
                (filter.Iduser == null || x.Iduser == filter.Iduser) &&
                (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                (!filter.ReceivedAtFrom.HasValue || x.ReceivedAt >= filter.ReceivedAtFrom.Value) &&
                (!filter.ReceivedAtTo.HasValue || x.ReceivedAt <= filter.ReceivedAtTo.Value) &&
                (!filter.CreatedAtFrom.HasValue || x.CreatedAt >= filter.CreatedAtFrom.Value) &&
                (!filter.CreatedAtTo.HasValue || x.CreatedAt <= filter.CreatedAtTo.Value);

            IPaginate<GetEducationHistoryResponse> response = await _unitOfWork.GetRepository<EducationHistory>().GetPagingListAsync(
                selector: x => _mapper.Map<GetEducationHistoryResponse>(x),
                predicate: predicate,
                orderBy: x => x.OrderBy(x => x.ReceivedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
