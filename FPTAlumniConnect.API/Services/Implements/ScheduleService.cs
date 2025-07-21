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

        public async Task<int> CreateNewSchedule(ScheduleInfo request)
        {
            await EnsureMentorshipExists(request.MentorShipId ?? 0);
            await EnsureUserExists(request.MentorId ?? 0);

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

        public async Task<bool> UpdateScheduleInfo(int id, ScheduleInfo request)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId == id)
                ?? throw new BadHttpRequestException("ScheduleNotFound");

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

            if (request.StartTime.HasValue)
            {
                if (request.StartTime.Value < DateTime.UtcNow)
                    throw new BadHttpRequestException("StartTime cannot be in the past.");

                schedule.StartTime = request.StartTime.Value;
            }

            if (request.EndTime.HasValue)
            {
                DateTime compareStart = request.StartTime ?? schedule.StartTime;
                if (request.EndTime.Value < compareStart)
                    throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");

                schedule.EndTime = request.EndTime.Value;
            }

            if (!string.IsNullOrEmpty(request.Content))
                schedule.Content = request.Content;

            if (!string.IsNullOrEmpty(request.Status))
                schedule.Status = request.Status;

            if (request.Rating.HasValue)
            {
                if (request.Rating.Value < 0 || request.Rating.Value > 5)
                    throw new BadHttpRequestException("Rating must be between 0 and 5.");

                schedule.Rating = request.Rating;
            }

            schedule.UpdatedAt = DateTime.Now;
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            return await _unitOfWork.CommitAsync() > 0;
        }

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

        private async Task<Mentorship> EnsureMentorshipExists(int id)
        {
            return await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id == id)
                ?? throw new BadHttpRequestException("MentorshipNotFound");
        }

        private async Task<User> EnsureUserExists(int userId)
        {
            return await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == userId)
                ?? throw new BadHttpRequestException("UserNotFound");
        }
    }
}
