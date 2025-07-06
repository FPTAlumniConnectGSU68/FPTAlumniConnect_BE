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
    // Service class to manage Education History records for users
    public class EducationHistoryService : BaseService<EducationHistoryService>, IEducationHistoryService
    {
        public EducationHistoryService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EducationHistoryService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Creates a new education history record
        public async Task<int> CreateNewEducationHistory(EducationHistoryInfo request)
        {
            // Ensure user exists
            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.Iduser))
                ?? throw new BadHttpRequestException("UserNotFound");

            // Validate Name (optional)
            if (request.Name != null && (request.Name.Length < 2 || request.Name.Length > 200))
            {
                throw new BadHttpRequestException("Name must be between 2 and 200 characters.");
            }

            // Validate ReceivedAt is not a future date
            if (request.ReceivedAt.HasValue && request.ReceivedAt.Value > DateTime.UtcNow)
            {
                throw new BadHttpRequestException("ReceivedAt cannot be in the future.");
            }

            // Prevent duplicate entry for the same user and education history
            EducationHistory alreadyHad = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.Iduser == request.Iduser &&
                                x.Name == request.Name &&
                                x.ReceivedAt == request.ReceivedAt);
            if (alreadyHad != null)
            {
                throw new BadHttpRequestException("UserAlreadyHadThisEduHis");
            }

            // Map and insert to DB
            EducationHistory newEducationHistory = _mapper.Map<EducationHistory>(request);
            await _unitOfWork.GetRepository<EducationHistory>().InsertAsync(newEducationHistory);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEducationHistory.EduHistoryId;
        }

        // Gets education history record by ID
        public async Task<GetEducationHistoryResponse> GetEducationHistoryById(int id)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id))
                ?? throw new BadHttpRequestException("EducationHistoryNotFound");

            return _mapper.Map<GetEducationHistoryResponse>(educationHistory);
        }

        // Updates an existing education history record
        public async Task<bool> UpdateEducationHistory(int id, EducationHistoryInfo request)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id))
                ?? throw new BadHttpRequestException("EducationHistoryNotFound");

            // Update Name with validation
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                if (request.Name.Length < 2 || request.Name.Length > 200)
                {
                    throw new BadHttpRequestException("Name must be between 2 and 200 characters.");
                }
                educationHistory.Name = request.Name;
            }

            // Update ReceivedAt with validation
            if (request.ReceivedAt.HasValue)
            {
                if (request.ReceivedAt.Value > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("ReceivedAt cannot be a future date.");
                }
                educationHistory.ReceivedAt = request.ReceivedAt.Value;
            }

            educationHistory.UpdatedAt = DateTime.Now;
            educationHistory.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<EducationHistory>().UpdateAsync(educationHistory);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Views paginated list of education history with filters
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

            // Build filtering expression
            Expression<Func<EducationHistory, bool>> predicate = x =>
                (filter.Iduser == null || x.Iduser == filter.Iduser) &&
                (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                (!filter.ReceivedAtFrom.HasValue || x.ReceivedAt >= filter.ReceivedAtFrom.Value) &&
                (!filter.ReceivedAtTo.HasValue || x.ReceivedAt <= filter.ReceivedAtTo.Value) &&
                (!filter.CreatedAtFrom.HasValue || x.CreatedAt >= filter.CreatedAtFrom.Value) &&
                (!filter.CreatedAtTo.HasValue || x.CreatedAt <= filter.CreatedAtTo.Value);

            // Execute paged query
            var response = await _unitOfWork.GetRepository<EducationHistory>().GetPagingListAsync(
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
