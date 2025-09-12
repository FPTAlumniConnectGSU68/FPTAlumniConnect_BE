using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.BusinessTier.Payload.EmploymentHistory;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class CVService : BaseService<CVService>, ICVService
    {
        private readonly IEmploymentHistoryService _employmentHistoryService;

        public CVService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<CVService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IEmploymentHistoryService employmentHistoryService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _employmentHistoryService = employmentHistoryService;
        }

        public async Task<int> CreateNewCV(CVInfo request)
        {
            // Validate User
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId)
                ?? throw new BadHttpRequestException("UserNotFound");

            // Validate Major
            if (request.MajorId.HasValue)
            {
                _ = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                    predicate: x => x.MajorId == request.MajorId)
                    ?? throw new BadHttpRequestException("MajorNotFound");
            }

            // Validate Skills
            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    _ = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                        predicate: x => x.SkillId == skillId)
                        ?? throw new BadHttpRequestException($"Skill ID {skillId} not found.");
                }
            }

            // Validate Birthday
            if (request.Birthday.HasValue)
            {
                int age = TimeHelper.NowInVietnam().Year - request.Birthday.Value.Year;
                if (request.Birthday.Value.Date > TimeHelper.NowInVietnam().Date.AddYears(-age)) age--;
                if (age < 18) throw new BadHttpRequestException("Birthday must indicate age 18 or older.");
            }

            // Validate Employment Histories
            if (request.EmploymentHistories != null && request.EmploymentHistories.Any())
            {
                foreach (var history in request.EmploymentHistories)
                {
                    if (string.IsNullOrWhiteSpace(history.CompanyName))
                        throw new BadHttpRequestException("Company name cannot be empty.");
                    if (string.IsNullOrWhiteSpace(history.PrimaryDuties))
                        throw new BadHttpRequestException("Primary duties cannot be empty.");
                    if (string.IsNullOrWhiteSpace(history.JobLevel))
                        throw new BadHttpRequestException("Job level cannot be empty.");
                    if (history.StartDate.HasValue && history.StartDate.Value > TimeHelper.NowInVietnam())
                        throw new BadHttpRequestException("Start date cannot be in the future.");
                    if (history.EndDate.HasValue && history.StartDate.HasValue && history.EndDate.Value < history.StartDate.Value)
                        throw new BadHttpRequestException("End date cannot be earlier than start date.");
                }
            }

            // Map and create CV
            Cv newCV = _mapper.Map<Cv>(request);
            newCV.CreatedAt = TimeHelper.NowInVietnam();
            newCV.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            newCV.CvSkills = new List<CvSkill>();

            await _unitOfWork.GetRepository<Cv>().InsertAsync(newCV);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            // Add Skills
            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    var cvSkill = new CvSkill
                    {
                        CvId = newCV.Id,
                        SkillId = skillId,
                        CreatedAt = TimeHelper.NowInVietnam(),
                        UpdatedAt = TimeHelper.NowInVietnam()
                    };
                    await _unitOfWork.GetRepository<CvSkill>().InsertAsync(cvSkill);
                }
                isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful) throw new BadHttpRequestException("Failed to add CV skills");
            }

            // Add Employment Histories
            if (request.EmploymentHistories != null && request.EmploymentHistories.Any())
            {
                foreach (var history in request.EmploymentHistories)
                {
                    var employmentHistoryInfo = new EmploymentHistoryInfo
                    {
                        CvId = newCV.Id,
                        CompanyName = history.CompanyName,
                        PrimaryDuties = history.PrimaryDuties,
                        JobLevel = history.JobLevel,
                        StartDate = history.StartDate,
                        EndDate = history.EndDate,
                        IsCurrentJob = history.IsCurrentJob
                    };
                    await _employmentHistoryService.CreateEmploymentHistory(employmentHistoryInfo);
                }
            }

            return newCV.Id;
        }

        public async Task<CVResponse> GetCVById(int id)
        {
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == id,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major)
                    .Include(c => c.EmploymentHistories))
                ?? throw new BadHttpRequestException("CVNotFound");

            return _mapper.Map<CVResponse>(cv);
        }

        public async Task<CVResponse> GetCVByUserId(int id)
        {
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.UserId == id,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major)
                    .Include(c => c.EmploymentHistories))
                ?? throw new BadHttpRequestException("CVNotFound");

            return _mapper.Map<CVResponse>(cv);
        }

        public async Task<bool> UpdateCVInfo(int id, CVInfo request)
        {
            // Load CV with EmploymentHistories to compare with request
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == id)
                ?? throw new BadHttpRequestException("CVNotFound");

            // Validate Major
            if (request.MajorId.HasValue)
            {
                _ = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                    predicate: x => x.MajorId == request.MajorId)
                    ?? throw new BadHttpRequestException("MajorNotFound");
            }

            // Validate Skills
            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    _ = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                        predicate: x => x.SkillId == skillId)
                        ?? throw new BadHttpRequestException($"Skill ID {skillId} not found.");
                }
            }

            // Validate Birthday
            if (request.Birthday.HasValue)
            {
                int age = TimeHelper.NowInVietnam().Year - request.Birthday.Value.Year;
                if (request.Birthday.Value.Date > TimeHelper.NowInVietnam().Date.AddYears(-age)) age--;
                if (age < 18) throw new BadHttpRequestException("Birthday must indicate age 18 or older.");
            }

            // Validate Employment Histories
            if (request.EmploymentHistories != null && request.EmploymentHistories.Any())
            {
                foreach (var history in request.EmploymentHistories)
                {
                    if (string.IsNullOrWhiteSpace(history.CompanyName))
                        throw new BadHttpRequestException("Company name cannot be empty.");
                    if (string.IsNullOrWhiteSpace(history.PrimaryDuties))
                        throw new BadHttpRequestException("Primary duties cannot be empty.");
                    if (string.IsNullOrWhiteSpace(history.JobLevel))
                        throw new BadHttpRequestException("Job level cannot be empty.");
                    if (history.StartDate.HasValue && history.StartDate.Value > TimeHelper.NowInVietnam())
                        throw new BadHttpRequestException("Start date cannot be in the future.");
                    if (history.EndDate.HasValue && history.StartDate.HasValue && history.EndDate.Value < history.StartDate.Value)
                        throw new BadHttpRequestException("End date cannot be earlier than start date.");
                }
            }

            // Validate Salary
            if (request.MinSalary.HasValue && request.MinSalary < 0)
                throw new BadHttpRequestException("MinSalary cannot be negative.");
            if (request.MaxSalary.HasValue && request.MaxSalary < 0)
                throw new BadHttpRequestException("MaxSalary cannot be negative.");
            if (request.MinSalary.HasValue && request.MaxSalary.HasValue && request.MinSalary > request.MaxSalary)
                throw new BadHttpRequestException("MaxSalary cannot be less than MinSalary.");          

            // Update Skills
            if (request.SkillIds != null)
            {
                var existingSkills = await _unitOfWork.GetRepository<CvSkill>().GetListAsync(predicate: x => x.CvId == id);
                foreach (var skill in existingSkills)
                {
                    _unitOfWork.GetRepository<CvSkill>().DeleteAsync(skill);
                    await _unitOfWork.CommitAsync();
                }

                foreach (var skillId in request.SkillIds.Distinct())
                {
                    var cvSkill = new CvSkill
                    {
                        CvId = id,
                        SkillId = skillId,
                        CreatedAt = TimeHelper.NowInVietnam(),
                        UpdatedAt = TimeHelper.NowInVietnam()
                    };
                    await _unitOfWork.GetRepository<CvSkill>().InsertAsync(cvSkill);
                }

                await _unitOfWork.CommitAsync();
            }

            // Update Employment Histories
            if (request.EmploymentHistories != null)
            {
                var existingHistories = cv.EmploymentHistories.ToList();
                var newHistories = request.EmploymentHistories ?? new List<EmploymentHistoryInfo>();

                // Helper function to check if two employment histories match
                bool HistoriesMatch(EmploymentHistory existing, EmploymentHistoryInfo newHistory)
                {
                    return existing.CompanyName == newHistory.CompanyName &&
                           existing.PrimaryDuties == newHistory.PrimaryDuties &&
                           existing.JobLevel == newHistory.JobLevel &&
                           existing.StartDate == newHistory.StartDate &&
                           existing.EndDate == newHistory.EndDate &&
                           existing.IsCurrentJob == newHistory.IsCurrentJob;
                }

                // Update or reuse existing employment histories
                var matchedIndices = new HashSet<int>();
                for (int i = 0; i < newHistories.Count && i < existingHistories.Count; i++)
                {
                    var newHistory = newHistories[i];
                    var existingHistory = existingHistories[i];

                    // If they match, skip update; otherwise, update the existing record
                    if (!HistoriesMatch(existingHistory, newHistory))
                    {
                        var employmentHistoryInfo = new EmploymentHistoryInfo
                        {
                            CvId = id,
                            CompanyName = newHistory.CompanyName,
                            PrimaryDuties = newHistory.PrimaryDuties,
                            JobLevel = newHistory.JobLevel,
                            StartDate = newHistory.StartDate,
                            EndDate = newHistory.EndDate,
                            IsCurrentJob = newHistory.IsCurrentJob
                        };
                        await _employmentHistoryService.UpdateEmploymentHistory(existingHistory.EmploymentHistoryId, employmentHistoryInfo);
                    }
                    matchedIndices.Add(i);
                }

                // Delete extra existing employment histories
                for (int i = 0; i < existingHistories.Count; i++)
                {
                    if (!matchedIndices.Contains(i))
                    {
                        await _employmentHistoryService.DeleteEmploymentHistory(existingHistories[i].EmploymentHistoryId);
                    }
                }

                // Create new employment histories if there are more in the request
                for (int i = existingHistories.Count; i < newHistories.Count; i++)
                {
                    var newHistory = newHistories[i];
                    var employmentHistoryInfo = new EmploymentHistoryInfo
                    {
                        CvId = id,
                        CompanyName = newHistory.CompanyName,
                        PrimaryDuties = newHistory.PrimaryDuties,
                        JobLevel = newHistory.JobLevel,
                        StartDate = newHistory.StartDate,
                        EndDate = newHistory.EndDate,
                        IsCurrentJob = newHistory.IsCurrentJob
                    };
                    await _employmentHistoryService.CreateEmploymentHistory(employmentHistoryInfo);
                }
            }
            else
            {
                // If request.EmploymentHistories is null, delete all existing employment histories
                var existingHistories = cv.EmploymentHistories.ToList();
                foreach (var history in existingHistories)
                {
                    await _employmentHistoryService.DeleteEmploymentHistory(history.EmploymentHistoryId);
                }
            }
            // Update CV fields
            cv.FullName = string.IsNullOrEmpty(request.FullName) ? cv.FullName : request.FullName;
            cv.Address = string.IsNullOrEmpty(request.Address) ? cv.Address : request.Address;
            cv.Birthday = request.Birthday ?? cv.Birthday;
            cv.Gender = string.IsNullOrEmpty(request.Gender) ? cv.Gender : request.Gender;
            cv.Email = string.IsNullOrEmpty(request.Email) ? cv.Email : request.Email;
            cv.Phone = string.IsNullOrEmpty(request.Phone) ? cv.Phone : request.Phone;
            cv.City = string.IsNullOrEmpty(request.City) ? cv.City : request.City;
            cv.Language = string.IsNullOrEmpty(request.Language) ? cv.Language : request.Language;
            cv.LanguageLevel = string.IsNullOrEmpty(request.LanguageLevel) ? cv.LanguageLevel : request.LanguageLevel;
            cv.MinSalary = request.MinSalary ?? cv.MinSalary;
            cv.MaxSalary = request.MaxSalary ?? cv.MaxSalary;
            cv.IsDeal = request.IsDeal ?? cv.IsDeal;
            cv.DesiredJob = string.IsNullOrEmpty(request.DesiredJob) ? cv.DesiredJob : request.DesiredJob;
            cv.Position = string.IsNullOrEmpty(request.Position) ? cv.Position : request.Position;
            cv.MajorId = request.MajorId ?? cv.MajorId;
            cv.AdditionalContent = string.IsNullOrEmpty(request.AdditionalContent) ? cv.AdditionalContent : request.AdditionalContent;
            cv.Status = request.Status != null ? Enum.Parse<CVStatus>(request.Status) : cv.Status;
            cv.UpdatedAt = TimeHelper.NowInVietnam();
            cv.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Cv>().UpdateAsync(cv);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<CVResponse>> ViewAllCV(CVFilter filter, PagingModel pagingModel)
        {
            return await _unitOfWork.GetRepository<Cv>().GetPagingListAsync(
                selector: x => _mapper.Map<CVResponse>(x),
                predicate: filter.BuildPredicate(),
                orderBy: x => x.OrderBy(c => c.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major)
                    .Include(c => c.EmploymentHistories));
        }

        public async Task<bool> ToggleIsLookingForJobAsync(int cvId)
        {
            if (cvId <= 0) throw new BadHttpRequestException("Invalid CV ID.");

            var cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == cvId)
                ?? throw new BadHttpRequestException("CVNotFound");

            cv.IsDeal = !cv.IsDeal;
            _unitOfWork.GetRepository<Cv>().UpdateAsync(cv);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<byte[]> ExportCvToPdfAsync(int cvId)
        {
            var cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == cvId,
                include: query => query
                    .Include(c => c.CvSkills)
                    .Include(c => c.EmploymentHistories))
                ?? throw new BadHttpRequestException("CVNotFound");

            string skills = string.Join(", ", cv.CvSkills.Select(cs => cs.SkillId));
            string employmentHistories = string.Join("\n", cv.EmploymentHistories.Select(h =>
                $"Company: {h.CompanyName}, Role: {h.JobLevel}, Duties: {h.PrimaryDuties}, " +
                $"Period: {h.StartDate:yyyy-MM-dd} - {(h.EndDate.HasValue ? h.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}"));

            string content = $"Name: {cv.FullName}\nEmail: {cv.Email}\nPhone: {cv.Phone}\nSkills: {skills}\nExperience:\n{employmentHistories}";

            // Placeholder for PDF generation
            return null; // Replace with actual PDF bytes later
        }

        public async Task<List<int>> GetCVSkills(int cvId)
        {
            if (cvId <= 0) throw new BadHttpRequestException("Invalid CV ID.");

            var cvSkills = await _unitOfWork.GetRepository<CvSkill>().GetListAsync(
                predicate: x => x.CvId == cvId);
            return cvSkills.Select(cs => cs.SkillId).ToList();
        }

        public async Task ShareCvByEmailAsync(ShareCvRequest request)
        {
            throw new NotImplementedException("ShareCvByEmailAsync not implemented");
        }
    }
}