using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
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
        public ScheduleService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<ScheduleService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Accept mentorship and create a new schedule
        public async Task<int> AcceptMentorShip(ScheduleInfo request)
        {
            // Validate mentorship ID in the request
            if (!request.MentorShipId.HasValue || request.MentorShipId.Value == 0)
                throw new BadHttpRequestException("MentorShipId is required.");

            // Validate mentorship existence
            var mentorship = await EnsureMentorshipExists(request.MentorShipId.Value);

            // Ensure mentor ID is provided and valid
            if (!request.MentorId.HasValue || request.MentorId.Value == 0)
                throw new BadHttpRequestException("MentorId is required.");
            await EnsureUserExists(request.MentorId.Value);

            // Create new schedule
            var newSchedule = _mapper.Map<Schedule>(request);
            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);

            // Update mentorship status
            mentorship.Status = "Active";
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);

            // Commit both operations
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("Failed to create schedule or update mentorship status.");

            return newSchedule.ScheduleId;
        }

        // Create a new schedule
        public async Task<int> CreateNewSchedule(ScheduleInfo request)
        {
            await EnsureMentorshipExists(request.MentorShipId ?? 0);
            await EnsureUserExists(request.MentorId ?? 0);

            // Validate StartTime and EndTime
            if (request.StartTime.HasValue && request.StartTime.Value < DateTime.UtcNow)
                throw new BadHttpRequestException("StartTime cannot be in the past.");

            if (request.StartTime.HasValue && request.EndTime.HasValue &&
                request.EndTime.Value < request.StartTime.Value)
                throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");

            var newSchedule = _mapper.Map<Schedule>(request);
            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
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
            schedule.UpdatedAt = DateTime.Now;
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
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);

            mentorship.Status = "Completed";
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);

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
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);
        }

        // Rate mentor for a schedule
        public async Task<bool> RateMentor(int scheduleId, string content, int rate)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == scheduleId)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

            if (rate < 0 || rate > 5)
                throw new BadHttpRequestException("Rating must be between 0 and 5.");

            schedule.Comment = content;
            schedule.Rating = rate;
            schedule.UpdatedAt = DateTime.UtcNow;
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
