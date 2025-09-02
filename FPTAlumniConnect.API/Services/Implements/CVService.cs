using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Enums;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class CVService : BaseService<CVService>, ICVService
    {
        public CVService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<CVService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewCV(CVInfo request)
        {
            if (request.Birthday.HasValue)
            {
                int age = TimeHelper.NowInVietnam().Year - request.Birthday.Value.Year;
                if (request.Birthday.Value.Date > TimeHelper.NowInVietnam().Date.AddYears(-age)) age--;
                if (age < 18) throw new BadHttpRequestException("Birthday must indicate age 18 or older.");
            }

            if (request.StartAt.HasValue && request.StartAt.Value > TimeHelper.NowInVietnam())
                throw new BadHttpRequestException("StartAt cannot be in the future.");
            if (request.EndAt.HasValue && request.StartAt.HasValue && request.EndAt.Value < request.StartAt.Value)
                throw new BadHttpRequestException("EndAt cannot be earlier than StartAt.");

            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.UserId)
                ?? throw new BadHttpRequestException("UserNotFound");

            if (request.MajorId.HasValue)
            {
                _ = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                    predicate: x => x.MajorId == request.MajorId)
                    ?? throw new BadHttpRequestException("MajorNotFound");
            }

            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    _ = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                        predicate: x => x.SkillId == skillId)
                        ?? throw new BadHttpRequestException($"Skill ID {skillId} not found.");
                }
            }

            Cv newCV = _mapper.Map<Cv>(request);
            newCV.CreatedAt = TimeHelper.NowInVietnam();
            newCV.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            newCV.CvSkills = new List<CvSkill>();

            await _unitOfWork.GetRepository<Cv>().InsertAsync(newCV);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

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

            return newCV.Id;
        }

        public async Task<CVResponse> GetCVById(int id)
        {
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == id,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major))
                ?? throw new BadHttpRequestException("CVNotFound");

            return _mapper.Map<CVResponse>(cv);
        }

        public async Task<CVResponse> GetCVByUserId(int id)
        {
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.UserId == id,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major))
                ?? throw new BadHttpRequestException("CVNotFound");

            return _mapper.Map<CVResponse>(cv);
        }

        public async Task<bool> UpdateCVInfo(int id, CVInfo request)
        {
            // Load CV without eager loading CvSkills here to avoid duplicate tracking
            Cv cv = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id == id,
                include: null // <-- Don't include CvSkills here
            ) ?? throw new BadHttpRequestException("CVNotFound");

            if (request.MajorId.HasValue)
            {
                _ = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                    predicate: x => x.MajorId == request.MajorId)
                    ?? throw new BadHttpRequestException("MajorNotFound");
            }

            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    _ = await _unitOfWork.GetRepository<Skill>().SingleOrDefaultAsync(
                        predicate: x => x.SkillId == skillId)
                        ?? throw new BadHttpRequestException($"Skill ID {skillId} not found.");
                }
            }

            if (request.Birthday.HasValue)
            {
                int age = TimeHelper.NowInVietnam().Year - request.Birthday.Value.Year;
                if (request.Birthday.Value.Date > TimeHelper.NowInVietnam().Date.AddYears(-age)) age--;
                if (age < 18) throw new BadHttpRequestException("Birthday must indicate age 18 or older.");
                cv.Birthday = request.Birthday.Value;
            }

            if (request.StartAt.HasValue && request.StartAt.Value > TimeHelper.NowInVietnam())
                throw new BadHttpRequestException("StartAt cannot be in the future.");
            if (request.EndAt.HasValue && request.StartAt.HasValue && request.EndAt.Value < request.StartAt.Value)
                throw new BadHttpRequestException("EndAt cannot be earlier than StartAt.");

            if (request.MinSalary.HasValue && request.MinSalary < 0)
                throw new BadHttpRequestException("MinSalary cannot be negative.");
            if (request.MaxSalary.HasValue && request.MaxSalary < 0)
                throw new BadHttpRequestException("MaxSalary cannot be negative.");
            if (request.MinSalary.HasValue && request.MaxSalary.HasValue && request.MinSalary > request.MaxSalary)
                throw new BadHttpRequestException("MaxSalary cannot be less than MinSalary.");

            // Update fields
            cv.FullName = string.IsNullOrEmpty(request.FullName) ? cv.FullName : request.FullName;
            cv.Address = string.IsNullOrEmpty(request.Address) ? cv.Address : request.Address;
            cv.Gender = string.IsNullOrEmpty(request.Gender) ? cv.Gender : request.Gender;
            cv.Email = string.IsNullOrEmpty(request.Email) ? cv.Email : request.Email;
            cv.Phone = string.IsNullOrEmpty(request.Phone) ? cv.Phone : request.Phone;
            cv.City = string.IsNullOrEmpty(request.City) ? cv.City : request.City;
            cv.Company = string.IsNullOrEmpty(request.Company) ? cv.Company : request.Company;
            cv.PrimaryDuties = string.IsNullOrEmpty(request.PrimaryDuties) ? cv.PrimaryDuties : request.PrimaryDuties;
            cv.JobLevel = string.IsNullOrEmpty(request.JobLevel) ? cv.JobLevel : request.JobLevel;
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

            if (request.SkillIds != null)
            {
                // Delete skills without tracking to avoid duplicate tracking issues
                var existingSkills = await _unitOfWork
                    .GetRepository<CvSkill>()
                    .GetListAsync(predicate: x => x.CvId == id);


                foreach (var skill in existingSkills)
                {
                    _unitOfWork.GetRepository<CvSkill>().DeleteAsync(skill);
                    await _unitOfWork.CommitAsync();
                }

                foreach (var skillId in request.SkillIds.Distinct()) // distinct to avoid key conflicts
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
            }

            cv.UpdatedAt = TimeHelper.NowInVietnam();
            cv.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Cv>().UpdateAsync(cv);
            return await _unitOfWork.CommitAsync() > 0;
        }


        public async Task<IPaginate<CVResponse>> ViewAllCV(CVFilter filter, PagingModel pagingModel)
        {
            if (filter.EndAt.HasValue && filter.StartAt.HasValue && filter.EndAt.Value < filter.StartAt.Value)
                throw new BadHttpRequestException("EndAt cannot be earlier than StartAt.");

            return await _unitOfWork.GetRepository<Cv>().GetPagingListAsync(
                selector: x => _mapper.Map<CVResponse>(x),
                predicate: filter.BuildPredicate(),
                orderBy: x => x.OrderBy(c => c.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size,
                include: query => query
                    .Include(c => c.CvSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Major));
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
                include: query => query.Include(c => c.CvSkills))
                ?? throw new BadHttpRequestException("CVNotFound");

            string skills = string.Join(", ", cv.CvSkills.Select(cs => cs.SkillId));
            string content = $"Name: {cv.FullName}\nEmail: {cv.Email}\nPhone: {cv.Phone}\nSkills: {skills}\nExperience: {cv.PrimaryDuties}";

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
