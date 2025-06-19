using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EventService : BaseService<EventService>, IEventService
    {

        public EventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewEvent(EventInfo request)
        {
            // Kiểm tra điều kiện thời gian
            if (request.StartDate.HasValue)
            {
                if (request.StartDate.Value < DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("StartDate cannot be in the past.");
                }
            }

            if (request.EndDate.HasValue)
            {
                if (request.StartDate.HasValue && request.EndDate.Value < request.StartDate.Value)
                {
                    throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");
                }
            }

            // Lấy tên của người đang đăng nhập (người tạo / session)
            //request.OrganizerId = ;

            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: x => x.UserId.Equals(request.OrganizerId)) ??
                    throw new BadHttpRequestException("UserNotFound");

            Event newEvent = _mapper.Map<Event>(request);

            await _unitOfWork.GetRepository<Event>().InsertAsync(newEvent);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEvent.EventId;
        }

        public async Task<GetEventResponse> GetEventById(int id)
        {
            Event Event = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(id)) ??
                throw new BadHttpRequestException("EventNotFound");

            GetEventResponse result = _mapper.Map<GetEventResponse>(Event);
            return result;
        }

        public async Task<bool> UpdateEventInfo(int id, EventInfo request)
        {
            Event eventToUpdate = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(id))
                ?? throw new BadHttpRequestException("EventNotFound");

            // Validate ngày tháng
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                // Validate khi cả StartDate và EndDate đều được cập nhật
                if (request.EndDate.Value < request.StartDate.Value)
                {
                    throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");
                }
            }
            else if (request.EndDate.HasValue)
            {
                // Validate khi chỉ EndDate được cập nhật
                // Nếu StartDate cũ không thay đổi, EndDate mới không được là ngày quá khứ so với StartDate cũ
                if (request.EndDate.Value < eventToUpdate.StartDate)
                {
                    throw new BadHttpRequestException("New EndDate cannot be earlier than the existing StartDate.");
                }
            }

            if (request.OrganizerId.HasValue)
            {
                User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: x => x.UserId.Equals(request.OrganizerId))
                    ?? throw new BadHttpRequestException("UserNotFound");
                eventToUpdate.OrganizerId = request.OrganizerId.Value;
            }

            // Validate tên sự kiện
            if (!string.IsNullOrWhiteSpace(request.EventName))
            {
                // Kiểm tra độ dài tên sự kiện
                if (request.EventName.Length < 3 || request.EventName.Length > 100)
                {
                    throw new BadHttpRequestException("Event name must be between 3 and 100 characters.");
                }
                eventToUpdate.EventName = request.EventName;
            }

            // Validate mô tả
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                // Kiểm tra độ dài mô tả
                if (request.Description.Length > 1000)
                {
                    throw new BadHttpRequestException("Description cannot exceed 1000 characters.");
                }
                eventToUpdate.Description = request.Description;
            }

            // Validate địa điểm
            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                // Kiểm tra độ dài địa điểm
                if (request.Location.Length > 200)
                {
                    throw new BadHttpRequestException("Location cannot exceed 200 characters.");
                }
                eventToUpdate.Location = request.Location;
            }

            eventToUpdate.StartDate = request.StartDate ?? eventToUpdate.StartDate;
            eventToUpdate.EndDate = request.EndDate ?? eventToUpdate.EndDate;
            eventToUpdate.Img = string.IsNullOrEmpty(request.Img) ? eventToUpdate.Img : request.Img;
            eventToUpdate.UpdatedAt = DateTime.Now;
            eventToUpdate.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name
                /*?? throw new UnauthorizedAccessException("User not authenticated")*/ ;

            _unitOfWork.GetRepository<Event>().UpdateAsync(eventToUpdate);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;

            return isSuccesful;
        }

        public async Task<IPaginate<GetEventResponse>> ViewAllEvent(EventFilter filter, PagingModel pagingModel)
        {
            if (filter.EndDate.HasValue && filter.StartDate.HasValue && filter.EndDate.Value < filter.StartDate.Value)
            {
                throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");
            }

            // Xây dựng biểu thức lọc
            Expression<Func<Event, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.EventName) || x.EventName.Contains(filter.EventName)) &&
                (string.IsNullOrEmpty(filter.Img) || x.Img == filter.Img) &&
                (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                (!filter.StartDate.HasValue || x.EndDate >= filter.StartDate) &&
                (!filter.EndDate.HasValue || x.StartDate <= filter.EndDate) &&
                (!filter.OrganizerId.HasValue || x.OrganizerId == filter.OrganizerId) &&
                (string.IsNullOrEmpty(filter.Location) || x.Location.Contains(filter.Location));

            // Thực hiện truy vấn
            IPaginate<GetEventResponse> response = await _unitOfWork.GetRepository<Event>().GetPagingListAsync(
                selector: x => _mapper.Map<GetEventResponse>(x),
                predicate: predicate,
                orderBy: x => x.OrderByDescending(x => x.EndDate),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}