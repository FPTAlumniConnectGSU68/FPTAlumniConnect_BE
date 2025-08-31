using AutoMapper;
using FPTAlumniConnect.API;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class UserJoinEventService : BaseService<UserJoinEventService>, IUserJoinEventService
    {
        private readonly IPerspectiveService _perspectiveService;

        public UserJoinEventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<UserJoinEventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPerspectiveService perspectiveService)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _perspectiveService = perspectiveService;
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
                throw new ConflictException("This user already joined this event!");

            // Create record
            UserJoinEvent newJoinEvent = _mapper.Map<UserJoinEvent>(request);
            newJoinEvent.CreatedAt = TimeHelper.NowInVietnam();
            newJoinEvent.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<UserJoinEvent>().InsertAsync(newJoinEvent);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJoinEvent.Id;
        }

        public async Task<GetUserJoinEventResponse> GetUserJoinEventById(int id)
        {
            UserJoinEvent userJoinEvent = await _unitOfWork.GetRepository<UserJoinEvent>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == id,
                    include: source => source.Include(x => x.User).ThenInclude(u => u.Role))
                ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            GetUserJoinEventResponse result = _mapper.Map<GetUserJoinEventResponse>(userJoinEvent);
            return result;
        }

        public async Task<bool> UpdateUserJoinEvent(int id, UserJoinEventInfo request)
        {
            UserJoinEvent userJoinEventToUpdate = await _unitOfWork.GetRepository<UserJoinEvent>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == id,
                    include: source => source.Include(x => x.User).ThenInclude(u => u.Role))
                ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            await UpdateUserJoinEventFields(userJoinEventToUpdate, request);

            _unitOfWork.GetRepository<UserJoinEvent>().UpdateAsync(userJoinEventToUpdate);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetUserJoinEventResponse>> ViewAllUserJoinEvents(UserJoinEventFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetUserJoinEventResponse> response = await _unitOfWork.GetRepository<UserJoinEvent>()
                .GetPagingListAsync(
                    selector: x => _mapper.Map<GetUserJoinEventResponse>(x),
                    predicate: x =>
                        (!filter.UserId.HasValue || x.UserId == filter.UserId) &&
                        (!filter.EventId.HasValue || x.EventId == filter.EventId) &&
                        (!filter.Rating.HasValue || x.Rating == filter.Rating),
                    include: source => source.Include(x => x.User).ThenInclude(u => u.Role),
                    orderBy: x => x.OrderBy(x => x.Id),
                    page: pagingModel.page,
                    size: pagingModel.size
                );

            return response;
        }

        private async Task UpdateUserJoinEventFields(UserJoinEvent target, UserJoinEventInfo request)
        {
            if (!request.Rating.HasValue || request.Rating < 1 || request.Rating > 5)
            {
                throw new BadHttpRequestException("Rating is required and must be between 1 and 5.");
            }

            // Check content appropriateness using perspective API
            if (!string.IsNullOrWhiteSpace(request.Content) &&
                !await _perspectiveService.IsContentAppropriate(request.Content))
            {
                throw new BadHttpRequestException("Comment contains inappropriate content.");
            }

            target.Content = string.IsNullOrWhiteSpace(request.Content) ? target.Content : request.Content;
            target.Rating = request.Rating ?? target.Rating;
            target.UpdatedAt = TimeHelper.NowInVietnam();
            target.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }

        public async Task<double> GetAverageRatingOfEvent(int eventId)
        {
            var ratings = await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId && x.Rating.HasValue,
                selector: x => x.Rating.Value);

            return ratings.Count == 0 ? 0 : ratings.Average();
        }

        // General Statistics

        public async Task<int> GetTotalParticipants(int eventId)
        {
            return await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId,
                selector: x => x.Id).ContinueWith(t => t.Result.Count);
        }

        public async Task<Dictionary<string, int>> GetTotalParticipantsByRole(int eventId)
        {
            var participants = await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId,
                include: source => source.Include(x => x.User).ThenInclude(u => u.Role),
                selector: x => new { RoleName = x.User.Role.Name});

            return participants
                .GroupBy(p => p.RoleName)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        // Daily Statistics

        public async Task<Dictionary<string, int>> GetTotalParticipantsByDay(int eventId)
        {
            var participants = await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId,
                selector: x => x.CreatedAt ?? TimeHelper.NowInVietnam());

            return participants
                .GroupBy(d => d.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Feedback and Reviews

        public async Task<IEnumerable<GetUserJoinEventResponse>> GetEvaluations(int eventId)
        {
            return await _unitOfWork.GetRepository<UserJoinEvent>().GetListAsync(
                predicate: x => x.EventId == eventId && (x.Rating.HasValue || !string.IsNullOrEmpty(x.Content)),
                include: source => source.Include(x => x.User).ThenInclude(u => u.Role),
                selector: x => _mapper.Map<GetUserJoinEventResponse>(x));
        }

        // API Functionality

        // Checks if a user has joined a specific event and returns the participation details
        public async Task<GetUserJoinEventResponse> CheckUserParticipation(int userId, int eventId)
        {
            try
            {
                return await _unitOfWork.GetRepository<UserJoinEvent>().SingleOrDefaultAsync(
                    predicate: x => x.EventId == eventId && x.UserId == userId,
                    include: source => source.Include(x => x.User).ThenInclude(u => u.Role),
                    selector: x => _mapper.Map<GetUserJoinEventResponse>(x));
            }
            catch (Exception ex)
            {
                // Log the error (use your preferred logging mechanism, e.g., ILogger)
                throw new Exception($"Error checking user participation for UserId {userId} and EventId {eventId}: {ex.Message}", ex);
            }
        }
    }
}