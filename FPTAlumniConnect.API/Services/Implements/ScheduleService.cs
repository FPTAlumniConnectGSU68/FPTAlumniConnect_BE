using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class ScheduleService : BaseService<ScheduleService>, IScheduleService
    {

        public ScheduleService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<ScheduleService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewSchedule(ScheduleInfo request)
        {
            Mentorship mentorShipId = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.MentorShipId)) ??
                throw new BadHttpRequestException("MentorshipNotFound");

            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.MentorId)) ??
                throw new BadHttpRequestException("UserNotFound");

            // Kiểm tra điều kiện thời gian
            if (request.StartTime.HasValue)
            {
                if (request.StartTime.Value < DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("StartTime cannot be in the past.");
                }
            }

            if (request.EndTime.HasValue)
            {
                if (request.StartTime.HasValue && request.EndTime.Value < request.StartTime.Value)
                {
                    throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");
                }
            }

            Schedule newSchedule = _mapper.Map<Schedule>(request);

            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newSchedule.ScheduleId;
        }

        public async Task<ScheduleReponse> GetScheduleById(int id)
        {
            Schedule schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId.Equals(id)) ??
                throw new BadHttpRequestException("ScheduleNotFound");

            ScheduleReponse result = _mapper.Map<ScheduleReponse>(schedule);
            return result;
        }

        public async Task<ICollection<ScheduleReponse>> GetSchedulesByMentorId(int id)
        {
            ICollection<ScheduleReponse> schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                predicate: x => x.MentorId.Equals(id)) ??
                  throw new BadHttpRequestException("MentorNotFound");
            return schedules;
        }

        public async Task<double> GetAverageRatingByMentorId(int id)
        {
            ICollection<ScheduleReponse> schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                predicate: x => x.MentorId.Equals(id)) ??
                  throw new BadHttpRequestException("MentorNotFound");
            double averageRating = schedules.Average(s => s.Rating ?? 0);

            return averageRating;
        }

        public async Task<bool> UpdateScheduleInfo(int id, ScheduleInfo request)
        {
            Schedule schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId.Equals(id)) ??
                throw new BadHttpRequestException("ScheduleNotFound");

            if (request.MentorShipId.HasValue)
            {
                Mentorship mentorShipId = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(request.MentorShipId)) ??
                    throw new BadHttpRequestException("MentorshipNotFound");
                schedule.MentorShipId = request.MentorShipId.Value;
            }

            if (request.MentorId.HasValue)
            {
                User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: x => x.UserId.Equals(request.MentorId)) ??
                    throw new BadHttpRequestException("UserNotFound");
                schedule.MentorId = request.MentorId.Value;
            }

            // Validate ngày tháng
            if (request.StartTime.HasValue && request.EndTime.HasValue)
            {
                // Validate khi cả StartTime và EndTime đều được cập nhật
                if (request.EndTime.Value < request.StartTime.Value)
                {
                    throw new BadHttpRequestException("EndTime cannot be earlier than StartTime.");
                }
            }
            else if (request.EndTime.HasValue)
            {
                // Validate khi chỉ EndTime được cập nhật
                // Nếu StartTime cũ không thay đổi, EndTime mới không được là ngày quá khứ so với StartTime cũ
                if (request.EndTime.Value < schedule.StartTime)
                {
                    throw new BadHttpRequestException("New EndTime cannot be earlier than the existing StartTime.");
                }
            }

            schedule.Content = string.IsNullOrEmpty(request.Content) ? schedule.Content : request.Content;
            if (request.StartTime.HasValue)
            {
                schedule.StartTime = request.StartTime.Value;
            }
            if (request.EndTime.HasValue)
            {
                schedule.EndTime = request.EndTime.Value;
            }
            schedule.Status = string.IsNullOrEmpty(request.Status) ? schedule.Status : request.Status;

            // Nếu muốn validate
            if (request.Rating.HasValue)
            {
                // Validate rating
                if (request.Rating.Value < 0 || request.Rating.Value > 5)
                    throw new BadHttpRequestException("Rating must be between 0 and 5.");

                schedule.Rating = request.Rating;
            }

            schedule.UpdatedAt = DateTime.Now;
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            /*?? throw new UnauthorizedAccessException("User not authenticated")*/

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<ScheduleReponse>> ViewAllSchedule(ScheduleFilter filter, PagingModel pagingModel)
        {
            IPaginate<ScheduleReponse> response = await _unitOfWork.GetRepository<Schedule>().GetPagingListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
