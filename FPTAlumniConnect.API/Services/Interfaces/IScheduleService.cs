using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<int> CreateNewSchedule(ScheduleInfo request);
        Task<IPaginate<ScheduleReponse>> ViewAllSchedule(ScheduleFilter filter, PagingModel pagingModel);
        Task<bool> UpdateScheduleInfo(int id, ScheduleInfo request);
        Task<ScheduleReponse> GetScheduleById(int id);
        Task<ICollection<ScheduleReponse>> GetSchedulesByMentorId(int id);
        Task<int> AcceptMentorShip(ScheduleInfo request);
        Task<bool> CompleteSchedule(int id);
        Task<bool> RateMentor(int scheduleId, string content, int rate);
        Task<int> CountAllSchedules();
        Task<int> CountSchedulesByMonth(int month, int year);
    }
}
