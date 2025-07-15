using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Education;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EducationService : BaseService<EducationService>, IEducationService
    {
        public EducationService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<EducationService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateEducationAsync(EducationInfo request)
        {
            var isDuplicate = await _unitOfWork.GetRepository<Education>().AnyAsync(
                predicate: x => x.UserId == request.UserId &&
                    x.SchoolName == request.SchoolName &&
                    x.Major == request.Major &&
                    x.StartDate == request.StartDate);

            if (isDuplicate)
            {
                throw new BadHttpRequestException("DuplicateEducationEntry");
            }

            Education newEducation = _mapper.Map<Education>(request);
            await _unitOfWork.GetRepository<Education>().InsertAsync(newEducation);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEducation.Id;
        }

        public async Task<EducationResponse> GetEducationByIdAsync(int id)
        {
            Education education = await _unitOfWork.GetRepository<Education>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("EducationNotFound");

            EducationResponse response = _mapper.Map<EducationResponse>(education);
            return response;
        }

        public async Task<bool> UpdateEducationAsync(int id, EducationInfo request)
        {
            Education education = await _unitOfWork.GetRepository<Education>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("EducationNotFound");

            education.SchoolName = string.IsNullOrEmpty(request.SchoolName) ? education.SchoolName : request.SchoolName;
            education.Major = string.IsNullOrEmpty(request.Major) ? education.Major : request.Major;
            education.StartDate = request.StartDate == default ? education.StartDate : request.StartDate;
            education.EndDate = request.EndDate ?? education.EndDate;
            education.SchoolWebsite = string.IsNullOrEmpty(request.SchoolWebsite) ? education.SchoolWebsite : request.SchoolWebsite;
            education.Achievements = string.IsNullOrEmpty(request.Achievements) ? education.Achievements : request.Achievements;
            education.Location = string.IsNullOrEmpty(request.Location) ? education.Location : request.Location;
            education.LogoUrl = request.LogoUrl ?? education.LogoUrl;
            education.UserId = request.UserId;

            _unitOfWork.GetRepository<Education>().UpdateAsync(education);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteEducationAsync(int id)
        {
            Education education = await _unitOfWork.GetRepository<Education>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("EducationNotFound");

            _unitOfWork.GetRepository<Education>().DeleteAsync(education);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<EducationResponse>> ViewAllEducationAsync(EducationFilter filter, PagingModel pagingModel)
        {
            IPaginate<EducationResponse> response = await _unitOfWork.GetRepository<Education>().GetPagingListAsync(
                selector: x => _mapper.Map<EducationResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.StartDate),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<List<EducationStatisticsDto>> GetEducationStatsByUser(int userId, string groupBy = "SchoolName")
        {
            var educations = await _unitOfWork.GetRepository<Education>().GetListAsync(
                predicate: x => x.UserId == userId
                //, disableTracking: true
            );

            if (educations == null || !educations.Any())
                return new List<EducationStatisticsDto>();

            List<EducationStatisticsDto> stats;

            switch (groupBy.ToLower())
            {
                case "major":
                    stats = educations.GroupBy(x => x.Major)
                        .Select(g => new EducationStatisticsDto
                        {
                            GroupByField = g.Key,
                            TotalCount = g.Count()
                        }).ToList();
                    break;

                case "location":
                    stats = educations.GroupBy(x => x.Location)
                        .Select(g => new EducationStatisticsDto
                        {
                            GroupByField = g.Key,
                            TotalCount = g.Count()
                        }).ToList();
                    break;

                default: // SchoolName
                    stats = educations.GroupBy(x => x.SchoolName)
                        .Select(g => new EducationStatisticsDto
                        {
                            GroupByField = g.Key,
                            TotalCount = g.Count()
                        }).ToList();
                    break;
            }

            return stats;
        }


    }
}