using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class UserJoinEventService : BaseService<UserJoinEventService>, IUserJoinEventService
    {
        public UserJoinEventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<UserJoinEventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewUserJoinEvent(UserJoinEventInfo request)
        {
            // Validate User and Event
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: x => x.UserId == request.UserId)
                ?? throw new BadHttpRequestException("UserNotFound");

            var eventDetails = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(predicate: x => x.EventId == request.EventId)
                ?? throw new BadHttpRequestException("EventNotFound");

            // Prevent duplicate join
            bool alreadyJoined = await _unitOfWork.GetRepository<UserJoinEvent>().AnyAsync(
                x => x.UserId == request.UserId && x.EventId == request.EventId);
            if (alreadyJoined)
                throw new BadHttpRequestException("This user already joined this event!");

            // Create record
            UserJoinEvent newJoinEvent = _mapper.Map<UserJoinEvent>(request);
            newJoinEvent.CreatedAt = DateTime.Now;
            newJoinEvent.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<UserJoinEvent>().InsertAsync(newJoinEvent);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJoinEvent.Id;
        }

        public async Task<GetUserJoinEventResponse> GetUserJoinEventById(int id)
        {
            UserJoinEvent userJoinEvent = await _unitOfWork.GetRepository<UserJoinEvent>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            GetUserJoinEventResponse result = _mapper.Map<GetUserJoinEventResponse>(userJoinEvent);
            return result;
        }

        public async Task<bool> UpdateUserJoinEvent(int id, UserJoinEventInfo request)
        {
            UserJoinEvent userJoinEventToUpdate = await _unitOfWork.GetRepository<UserJoinEvent>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            // Phân quyền chỉnh sửa
            //var currentUsername = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            //if (!string.Equals(userJoinEventToUpdate.CreatedBy, currentUsername, StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new UnauthorizedAccessException("You are not allowed to update this record.");
            //}

            UpdateUserJoinEventFields(userJoinEventToUpdate, request);

            _unitOfWork.GetRepository<UserJoinEvent>().UpdateAsync(userJoinEventToUpdate);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetUserJoinEventResponse>> ViewAllUserJoinEvents(UserJoinEventFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetUserJoinEventResponse> response = await _unitOfWork.GetRepository<UserJoinEvent>().GetPagingListAsync(
                selector: x => _mapper.Map<GetUserJoinEventResponse>(x),
                  filter: filter,
                orderBy: x => x.OrderBy(x => x.Id),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        private void UpdateUserJoinEventFields(UserJoinEvent target, UserJoinEventInfo request)
        {
            if (request.Rating.HasValue && (request.Rating < 1 || request.Rating > 5))
            {
                throw new BadHttpRequestException("Rating must be between 1 and 5.");
            }

            target.Content = string.IsNullOrWhiteSpace(request.Content) ? target.Content : request.Content;
            target.Rating = request.Rating ?? target.Rating;
            target.UpdatedAt = DateTime.Now;
            target.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }

        public async Task<double> GetAverageRatingOfEvent(int eventId)
        {
            var ratings = await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId && x.Rating.HasValue,
                selector: x => x.Rating.Value);

            return ratings.Count == 0 ? 0 : ratings.Average();
        }


    }
}
