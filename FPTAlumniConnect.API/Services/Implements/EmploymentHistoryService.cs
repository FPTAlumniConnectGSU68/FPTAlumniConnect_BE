using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EmploymentHistoryService : BaseService<EmploymentHistoryService>, IEmploymentHistoryService
    {
        public EmploymentHistoryService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EmploymentHistoryService> logger,
            IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateEmploymentHistory(EmploymentHistoryInfo request)
        {
            _logger.LogInformation("Creating new EmploymentHistory for CV: {CvId}", request.CvId);

            if (string.IsNullOrWhiteSpace(request.CompanyName))
                throw new BadHttpRequestException("Company name cannot be empty");

            if (string.IsNullOrWhiteSpace(request.PrimaryDuties))
                throw new BadHttpRequestException("Primary duties cannot be empty");

            if (string.IsNullOrWhiteSpace(request.JobLevel))
                throw new BadHttpRequestException("Job level cannot be empty");

            // Validate CV exists
            var cvExists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == request.CvId);
            if (!cvExists)
                throw new BadHttpRequestException("CV not found");

            EmploymentHistory newHistory = _mapper.Map<EmploymentHistory>(request);

            await _unitOfWork.GetRepository<EmploymentHistory>().InsertAsync(newHistory);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            _logger.LogInformation("EmploymentHistory created successfully with ID: {EmploymentHistoryId}, Company: {CompanyName}",
                newHistory.EmploymentHistoryId, newHistory.CompanyName);
            return newHistory.EmploymentHistoryId;
        }

        public async Task<EmploymentHistoryResponse> GetEmploymentHistoryById(int employmentHistoryId)
        {
            _logger.LogInformation("Getting EmploymentHistory by ID: {EmploymentHistoryId}", employmentHistoryId);

            EmploymentHistory history = await _unitOfWork.GetRepository<EmploymentHistory>().SingleOrDefaultAsync(
                predicate: x => x.EmploymentHistoryId == employmentHistoryId,
                include: x => x.Include(e => e.Cv)) ??
                throw new BadHttpRequestException("EmploymentHistoryNotFound");

            EmploymentHistoryResponse result = _mapper.Map<EmploymentHistoryResponse>(history);
            return result;
        }

        public async Task<bool> UpdateEmploymentHistory(int employmentHistoryId, EmploymentHistoryInfo request)
        {
            _logger.LogInformation("Updating EmploymentHistory with ID: {EmploymentHistoryId}", employmentHistoryId);

            EmploymentHistory history = await _unitOfWork.GetRepository<EmploymentHistory>().SingleOrDefaultAsync(
                predicate: x => x.EmploymentHistoryId == employmentHistoryId) ??
                throw new BadHttpRequestException("EmploymentHistoryNotFound");

            if (string.IsNullOrWhiteSpace(request.CompanyName))
                throw new BadHttpRequestException("Company name cannot be empty");

            if (string.IsNullOrWhiteSpace(request.PrimaryDuties))
                throw new BadHttpRequestException("Primary duties cannot be empty");

            if (string.IsNullOrWhiteSpace(request.JobLevel))
                throw new BadHttpRequestException("Job level cannot be empty");

            // Validate CV exists if changing CV
            if (history.CvId != request.CvId)
            {
                var cvExists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == request.CvId);
                if (!cvExists)
                    throw new BadHttpRequestException("CV not found");
            }

            _mapper.Map(request, history);

            _unitOfWork.GetRepository<EmploymentHistory>().UpdateAsync(history);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("EmploymentHistory updated successfully: ID: {EmploymentHistoryId}", employmentHistoryId);
            else
                _logger.LogWarning("EmploymentHistory update failed: ID: {EmploymentHistoryId}", employmentHistoryId);

            return isSuccessful;
        }

        public async Task<IPaginate<EmploymentHistoryResponse>> ViewAllEmploymentHistories(EmploymentHistoryFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all EmploymentHistories with paging: Page {Page}, Size {Size}",
                pagingModel.page, pagingModel.size);

            IPaginate<EmploymentHistoryResponse> response = await _unitOfWork.GetRepository<EmploymentHistory>().GetPagingListAsync(
                selector: x => _mapper.Map<EmploymentHistoryResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.EmploymentHistoryId),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteEmploymentHistory(int employmentHistoryId)
        {
            _logger.LogInformation("Deleting EmploymentHistory with ID: {EmploymentHistoryId}", employmentHistoryId);

            EmploymentHistory history = await _unitOfWork.GetRepository<EmploymentHistory>().SingleOrDefaultAsync(
                predicate: x => x.EmploymentHistoryId == employmentHistoryId) ??
                throw new BadHttpRequestException("EmploymentHistoryNotFound");

            _unitOfWork.GetRepository<EmploymentHistory>().DeleteAsync(history);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
                _logger.LogInformation("EmploymentHistory deleted successfully: ID: {EmploymentHistoryId}", employmentHistoryId);
            else
                _logger.LogWarning("EmploymentHistory deletion failed: ID: {EmploymentHistoryId}", employmentHistoryId);

            return isSuccessful;
        }

        public async Task<IPaginate<EmploymentHistoryResponse>> GetEmploymentHistoriesByCvId(int cvId, PagingModel pagingModel)
        {
            _logger.LogInformation("Getting EmploymentHistories for CV: {CvId}", cvId);

            IPaginate<EmploymentHistoryResponse> response = await _unitOfWork.GetRepository<EmploymentHistory>().GetPagingListAsync(
                selector: x => _mapper.Map<EmploymentHistoryResponse>(x),
                predicate: x => x.CvId == cvId,
                orderBy: x => x.OrderByDescending(x => x.StartDate),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}