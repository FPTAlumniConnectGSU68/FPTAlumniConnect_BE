using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class ScheduleService : BaseService<ScheduleService>, IScheduleService
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly IScheduleSettingsService _settingsService;

        public ScheduleService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<ScheduleService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IMentorshipService mentorshipService,
            IScheduleSettingsService settingsService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _mentorshipService = mentorshipService;
            _settingsService = settingsService;
        }

        // Accept mentorship and create a new schedule
        public async Task<int> AcceptMentorShip(ScheduleInfo request)
        {
            // Validate mentorship ID
            if (!request.MentorShipId.HasValue || request.MentorShipId.Value == 0)
                throw new BadHttpRequestException("MentorShipId is required.");
            var mentorship = await EnsureMentorshipExists(request.MentorShipId.Value);

            // Validate mentor ID
            if (!request.MentorId.HasValue || request.MentorId.Value == 0)
                throw new BadHttpRequestException("MentorId is required.");
            await EnsureUserExists(request.MentorId.Value);

            // Validate StartTime
            if (!request.StartTime.HasValue)
                throw new BadHttpRequestException("StartTime is required.");

            if (request.StartTime.Value < TimeHelper.NowInVietnam())
                throw new BadHttpRequestException("StartTime cannot be in the past.");

            // Validate EndTime
            if (request.EndTime.HasValue && request.EndTime.Value < request.StartTime.Value)
                throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");

            // Check max schedules per day
            var date = request.StartTime.Value.Date;
            var scheduleRepo = _unitOfWork.GetRepository<Schedule>();

            int countForDay = await scheduleRepo.CountAsync(s =>
                s.MentorShipId == request.MentorShipId &&
                s.StartTime.Date == date);

            var maxPerDay = _settingsService.GetMaxPerDay();
            if (countForDay >= maxPerDay)
                throw new BadHttpRequestException(
                    $"You can only create up to {maxPerDay} schedules for this mentorship on the same day."
                );

            // Create new schedule
            var newSchedule = _mapper.Map<Schedule>(request);
            await scheduleRepo.InsertAsync(newSchedule);

            // Update mentorship status
            mentorship.Status = "Active";
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);

            // Commit
            if (await _unitOfWork.CommitAsync() <= 0)
                throw new BadHttpRequestException("Failed to create schedule or update mentorship status.");

            return newSchedule.ScheduleId;
        }

        public async Task<int> CreateNewSchedule(ScheduleInfo request)
        {
            await EnsureMentorshipExists(request.MentorShipId ?? 0);
            await EnsureUserExists(request.MentorId ?? 0);

            if (request.StartTime.HasValue && request.StartTime.Value < TimeHelper.NowInVietnam())
                throw new BadHttpRequestException("StartTime cannot be in the past.");

            if (request.StartTime.HasValue && request.EndTime.HasValue &&
                request.EndTime.Value < request.StartTime.Value)
                throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");

            if (!request.StartTime.HasValue)
                throw new BadHttpRequestException("StartTime is required.");

            var date = request.StartTime.Value.Date;
            var scheduleRepo = _unitOfWork.GetRepository<Schedule>();

            int countForDay = await scheduleRepo.CountAsync(s =>
                s.MentorShipId == request.MentorShipId &&
                s.StartTime.Date == date);

            var maxPerDay = _settingsService.GetMaxPerDay();
            if (countForDay >= maxPerDay)
                throw new BadHttpRequestException(
                    $"You can only create up to {maxPerDay} schedules for this mentorship on the same day."
                );

            var newSchedule = _mapper.Map<Schedule>(request);
            await scheduleRepo.InsertAsync(newSchedule);

            if (await _unitOfWork.CommitAsync() <= 0)
                throw new BadHttpRequestException("CreateFailed");

            return newSchedule.ScheduleId;
        }

        // Get schedule details by ID
        public async Task<ScheduleReponse> GetScheduleById(int id)
        {
            Schedule schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == id,
                include: q => q.Include(x => x.Mentor)
                              .Include(x => x.MentorShip)
                              .ThenInclude(m => m.Aumni))
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            return _mapper.Map<ScheduleReponse>(schedule);
        }

        // Get all schedules for a given mentor
        public async Task<ICollection<ScheduleReponse>> GetSchedulesByMentorId(int id)
        {
            ICollection<ScheduleReponse> schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                predicate: x => x.MentorId == id,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                include: q => q.Include(x => x.Mentor)
                              .Include(x => x.MentorShip)
                              .ThenInclude(m => m.Aumni))
                ?? throw new BadHttpRequestException("MentorNotFound");

            return schedules;
        }

        // Update a schedule by ID
        public async Task<bool> UpdateScheduleInfo(int id, ScheduleInfo request)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == id)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            // Update mentorship and mentor if provided
            if (request.MentorShipId.HasValue)
            {
                await EnsureMentorshipExists(request.MentorShipId.Value);
                schedule.MentorShipId = request.MentorShipId.Value;
            }

            if (request.MentorId.HasValue)
            {
                await EnsureUserExists(request.MentorId.Value);
                schedule.MentorId = request.MentorId.Value;
            }

            // Update content and status if available
            if (!string.IsNullOrEmpty(request.Content))
                schedule.Content = request.Content;

            if (!string.IsNullOrEmpty(request.Status))
                schedule.Status = request.Status;

            // Update rating with validation
            if (request.Rating.HasValue)
            {
                if (request.Rating.Value < 0 || request.Rating.Value > 5)
                    throw new BadHttpRequestException("Rating must be between 0 and 5.");

                schedule.Rating = request.Rating;
            }

            // Update audit info
            schedule.UpdatedAt = TimeHelper.NowInVietnam();
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Complete a schedule by updating status to Completed for both schedule and mentorship
        public async Task<bool> CompleteSchedule(int id)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == id)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            var mentorship = await EnsureMentorshipExists(schedule.MentorShipId ?? 0);

            schedule.Status = "Completed";
            schedule.UpdatedAt = TimeHelper.NowInVietnam();
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);

            mentorship.Status = "Completed";
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> FailSchedule(int id, string message)
        {
            var scheduleRepo = _unitOfWork.GetRepository<Schedule>();
            var schedule = await scheduleRepo.SingleOrDefaultAsync(predicate: s => s.ScheduleId == id)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            // Mark schedule as failed
            schedule.Status = "Failed";
            schedule.Comment = message;
            schedule.UpdatedAt = TimeHelper.NowInVietnam();
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            scheduleRepo.UpdateAsync(schedule);

            // Cancel mentorship using existing method
            await _mentorshipService.CancelRequest(schedule.MentorShipId ?? 0, message);

            return await _unitOfWork.CommitAsync() > 0;
        }

        // View paginated list of schedules
        public async Task<IPaginate<ScheduleReponse>> ViewAllSchedule(ScheduleFilter filter, PagingModel pagingModel)
        {
            return await _unitOfWork.GetRepository<Schedule>().GetPagingListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                include: q => q.Include(x => x.Mentor)
                              .Include(x => x.MentorShip)
                              .ThenInclude(m => m.Aumni),
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);
        }

        public async Task<int> CountAllSchedules()
        {
            ICollection<ScheduleReponse> schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x));
            int count = schedules.Count();
            return count;
        }

        public async Task<ICollection<CountByMonthResponse>> CountSchedulesByMonth(int? month, int? year)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByMonthResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                    selector: x => _mapper.Map<ScheduleReponse>(x),
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


        // Rate mentor for a schedule
        public async Task<bool> RateMentor(int scheduleId, string comment, int rate)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == scheduleId)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            if (rate < 0 || rate > 5)
                throw new BadHttpRequestException("Rating must be between 0 and 5.");

            schedule.Comment = comment;
            schedule.Rating = rate;
            schedule.UpdatedAt = TimeHelper.NowInVietnam();
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Ensure mentorship exists
        private async Task<Mentorship> EnsureMentorshipExists(int id)
        {
            return await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id == id)
                ?? throw new BadHttpRequestException("MentorshipNotFound");
        }

        // Ensure user exists
        private async Task<User> EnsureUserExists(int userId)
        {
            return await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == userId)
                ?? throw new BadHttpRequestException("UserNotFound");
        }
    }
}
