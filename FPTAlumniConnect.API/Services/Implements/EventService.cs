using AutoMapper;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EventService : BaseService<EventService>, IEventService
    {
        private readonly IUserJoinEventService _userJoinEventService;
        public EventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IUserJoinEventService userJoinEventService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _userJoinEventService = userJoinEventService;
        }

        public async Task<int> CreateNewEvent(EventInfo request)
        {
            // Kiểm tra lịch trùng
            var duplicateEvent = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventName == request.EventName
                             && x.StartDate == request.StartDate);

            if (duplicateEvent != null)
                throw new BadHttpRequestException("An event with the same name and start date already exists.");

            if (request.StartDate.HasValue && request.StartDate.Value < TimeHelper.NowInVietnam())
                throw new BadHttpRequestException("StartDate cannot be in the past.");

            if (request.EndDate.HasValue && request.StartDate.HasValue &&
                request.EndDate.Value < request.StartDate.Value)
                throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");

            // Nếu không truyền OrganizerId thì lấy từ người dùng đăng nhập
            if (!request.OrganizerId.HasValue)
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId");
                if (userIdClaim == null)
                    throw new UnauthorizedAccessException("User not authenticated");

                request.OrganizerId = int.Parse(userIdClaim.Value);
            }

            // Kiểm tra OrganizerId hợp lệ
            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId == request.OrganizerId)
                ?? throw new NotFoundException("UserNotFound");

            var majorId = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId == request.MajorId)
                ?? throw new NotFoundException("MajorIdNotFound");

            var newEvent = _mapper.Map<Event>(request);
            newEvent.CreatedAt = TimeHelper.NowInVietnam();
            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            newEvent.UpdatedBy = string.IsNullOrEmpty(userName) ? "system" : userName;

            await _unitOfWork.GetRepository<Event>().InsertAsync(newEvent);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEvent.EventId;
        }

        public async Task<GetEventResponse> GetEventById(int id)
        {
            var ev = await _unitOfWork.GetRepository<Event>()
                .SingleOrDefaultAsync(
                    predicate: e => e.EventId == id,
                    include: e => e.Include(ev => ev.UserJoinEvents)
                );

            if (ev == null)
            {
                throw new NotFoundException("Event not found");
            }

            return _mapper.Map<GetEventResponse>(ev);
        }

        public async Task<EventDetailResponse> GetEventByIdAsync(int eventId)
        {
            // Get event with related data
            var eventEntity = await _unitOfWork.GetRepository<Event>()
                .SingleOrDefaultAsync(
                    predicate: e => e.EventId == eventId,
                    include: e => e
                        .Include(ev => ev.Organizer)
                        .Include(ev => ev.Major)
                        .Include(ev => ev.EventTimeLines)
                        .Include(ev => ev.UserJoinEvents)
                ) ?? throw new NotFoundException("Event not found");

            // Map to EventDetailResponse
            var result = _mapper.Map<EventDetailResponse>(eventEntity);
            result.Total = await _userJoinEventService.GetTotalParticipants(eventId);
            result.AverageRating = await _userJoinEventService.GetAverageRatingOfEvent(eventId);

            return result;
        }

        public async Task<IPaginate<GetEventResponse>> GetEventsUserJoined(int userId, EventFilter filter, PagingModel pagingModel)
        {
            _ = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(userId))
                ?? throw new ConflictException("UserNotFound");

            Expression<Func<UserJoinEvent, bool>> predicate = uje =>
                uje.UserId == userId &&
                (string.IsNullOrEmpty(filter.EventName) || uje.Event.EventName.Contains(filter.EventName)) &&
                (!filter.StartDate.HasValue || uje.Event.EndDate >= filter.StartDate) &&
                (!filter.EndDate.HasValue || uje.Event.StartDate <= filter.EndDate) &&
                (string.IsNullOrEmpty(filter.Location) || uje.Event.Location.Contains(filter.Location));

            return await _unitOfWork.GetRepository<UserJoinEvent>().GetPagingListAsync(
                selector: uje => _mapper.Map<GetEventResponse>(uje),
                predicate: predicate,
                include: q => q.Include(uje => uje.Event)
                               .ThenInclude(e => e.UserJoinEvents),
                orderBy: q => q.OrderByDescending(uje => uje.Event.StartDate),
                page: pagingModel.page,
                size: pagingModel.size
            );
        }


        public async Task<bool> UpdateEventInfo(int id, EventInfo request)
        {
            // Include related timelines for update
            Func<IQueryable<Event>, IIncludableQueryable<Event, object>> include = q => q.Include(e => e.EventTimeLines);

            // Fetch event or throw
            Event eventToUpdate = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(id),
                include: include)
                ?? throw new BadHttpRequestException("EventNotFound");

            // Validate dates
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.EndDate.Value < request.StartDate.Value)
                    throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");
            }
            else if (request.StartDate.HasValue)
            {
                if (request.StartDate.Value > eventToUpdate.EndDate)
                    throw new BadHttpRequestException("New StartDate cannot be later than the existing EndDate.");
            }
            else if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value < eventToUpdate.StartDate)
                    throw new BadHttpRequestException("New EndDate cannot be earlier than the existing StartDate.");
            }

            // Update OrganizerId if provided
            if (request.OrganizerId.HasValue)
            {
                var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    predicate: x => x.UserId.Equals(request.OrganizerId.Value))
                    ?? throw new BadHttpRequestException("UserNotFound");

                eventToUpdate.OrganizerId = request.OrganizerId.Value;
            }

            // Update MajorId if provided
            if (request.MajorId.HasValue)
            {
                var major = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                    predicate: x => x.MajorId.Equals(request.MajorId.Value))
                    ?? throw new BadHttpRequestException("MajorNotFound");

                eventToUpdate.MajorId = request.MajorId.Value;
            }

            // Update basic fields with validation
            if (!string.IsNullOrWhiteSpace(request.EventName))
            {
                if (request.EventName.Length < 3 || request.EventName.Length > 100)
                    throw new BadHttpRequestException("Event name must be between 3 and 100 characters.");

                eventToUpdate.EventName = request.EventName;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                if (request.Description.Length > 1000)
                    throw new BadHttpRequestException("Description cannot exceed 1000 characters.");

                eventToUpdate.Description = request.Description;
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                if (request.Location.Length > 200)
                    throw new BadHttpRequestException("Location cannot exceed 200 characters.");

                eventToUpdate.Location = request.Location;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                eventToUpdate.Status = request.Status;
            }

            // Apply date and other non-validated updates
            eventToUpdate.StartDate = request.StartDate ?? eventToUpdate.StartDate;
            eventToUpdate.EndDate = request.EndDate ?? eventToUpdate.EndDate;
            eventToUpdate.Img = string.IsNullOrEmpty(request.Img) ? eventToUpdate.Img : request.Img;
            eventToUpdate.UpdatedAt = TimeHelper.NowInVietnam();
            eventToUpdate.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "System";  // Fallback if no context

            // --- Helper local functions ---

            // Parse string to TimeSpan with validation (time-of-day: 00:00 to 23:59:59)
            TimeSpan ParseTimeSpanOrThrow(string input, string fieldName)
            {
                if (TimeSpan.TryParse(input, out var ts))
                {
                    ValidateTimeSpan(ts, fieldName);
                    return ts;
                }

                // Try exact formats
                var formats = new[] { "h\\:mm", "hh\\:mm", "hh\\:mm\\:ss", "h\\:mm\\:ss" };
                foreach (var format in formats)
                {
                    if (TimeSpan.TryParseExact(input, format, null, out ts))
                    {
                        ValidateTimeSpan(ts, fieldName);
                        return ts;
                    }
                }

                throw new BadHttpRequestException($"Invalid {fieldName} format: '{input}'. Expected formats: 'HH:mm' or 'HH:mm:ss'.");
            }

            // Validate TimeSpan is a valid time-of-day (non-negative, < 24 hours)
            void ValidateTimeSpan(TimeSpan ts, string fieldName)
            {
                if (ts < TimeSpan.Zero)
                    throw new BadHttpRequestException($"{fieldName} cannot be negative.");

                if (ts.TotalHours >= 24)
                    throw new BadHttpRequestException($"{fieldName} must be less than 24 hours (use format for time-of-day).");
            }

            // Resolve the next valid DateTime for a given time-of-day, ensuring it's >= minAllowed and within event range
            DateTime GetNextDateTimeForTime(Event e, TimeSpan timeOfDay, DateTime minAllowed)
            {
                // Start from minAllowed's date with the time-of-day applied
                var candidate = minAllowed.Date.Add(timeOfDay);

                // Advance by full days if candidate < minAllowed
                while (candidate < minAllowed)
                    candidate = candidate.AddDays(1);

                // Ensure it's within the event's overall range
                if (candidate < e.StartDate || candidate > e.EndDate)
                    throw new BadHttpRequestException($"Timeline time {timeOfDay} (resolved to {candidate:u}) is outside event range [{e.StartDate:u} - {e.EndDate:u}].");

                return candidate;
            }

            // Handle timelines: update existing, add new, remove not provided
            if (request.TimeLines != null)
            {
                var existingTimeLines = eventToUpdate.EventTimeLines.ToList();
                var timelineRepo = _unitOfWork.GetRepository<EventTimeLine>();

                foreach (var tlInfo in request.TimeLines)
                {
                    if (tlInfo.EventTimeLineId.HasValue)
                    {
                        // Update existing timeline
                        var existing = existingTimeLines.FirstOrDefault(t => t.EventTimeLineId == tlInfo.EventTimeLineId.Value)
                            ?? throw new BadHttpRequestException("TimeLineNotFound");

                        existing.Title = string.IsNullOrEmpty(tlInfo.Title) ? existing.Title : tlInfo.Title;
                        existing.Description = string.IsNullOrEmpty(tlInfo.Description) ? existing.Description : tlInfo.Description;

                        // Parse new times if provided, otherwise keep existing
                        var startTs = existing.StartTime;
                        var endTs = existing.EndTime;

                        DateTime startDt;
                        DateTime endDt;

                        if (!string.IsNullOrWhiteSpace(tlInfo.StartTime))
                        {
                            startTs = ParseTimeSpanOrThrow(tlInfo.StartTime, "StartTime");
                        }
                        startDt = GetNextDateTimeForTime(eventToUpdate, startTs, eventToUpdate.StartDate);

                        if (!string.IsNullOrWhiteSpace(tlInfo.EndTime))
                        {
                            endTs = ParseTimeSpanOrThrow(tlInfo.EndTime, "EndTime");
                        }
                        endDt = GetNextDateTimeForTime(eventToUpdate, endTs, startDt);

                        if (endDt < startDt)
                            throw new BadHttpRequestException("Timeline EndTime cannot be earlier than StartTime.");

                        // Update TimeSpan values
                        existing.StartTime = startTs;
                        existing.EndTime = endTs;

                        timelineRepo.UpdateAsync(existing);
                    }
                    else
                    {
                        // Add new timeline (require both StartTime and EndTime)
                        if (string.IsNullOrWhiteSpace(tlInfo.StartTime) || string.IsNullOrWhiteSpace(tlInfo.EndTime))
                            throw new BadHttpRequestException("StartTime and EndTime are required for new timeline entries and must be non-empty strings.");

                        var startTsNew = ParseTimeSpanOrThrow(tlInfo.StartTime, "StartTime");
                        var endTsNew = ParseTimeSpanOrThrow(tlInfo.EndTime, "EndTime");

                        var startDtNew = GetNextDateTimeForTime(eventToUpdate, startTsNew, eventToUpdate.StartDate);
                        var endDtNew = GetNextDateTimeForTime(eventToUpdate, endTsNew, startDtNew);

                        if (endDtNew < startDtNew)
                            throw new BadHttpRequestException("Timeline EndTime cannot be earlier than StartTime.");

                        var newTL = new EventTimeLine
                        {
                            EventId = id,
                            Title = tlInfo.Title,
                            Description = tlInfo.Description,
                            StartTime = startTsNew,
                            EndTime = endTsNew
                        };

                        await timelineRepo.InsertAsync(newTL);
                        eventToUpdate.EventTimeLines.Add(newTL);
                    }
                }

                // Remove timelines not in request
                var providedIds = request.TimeLines
                    .Where(t => t.EventTimeLineId.HasValue)
                    .Select(t => t.EventTimeLineId.Value)
                    .ToHashSet();  // For efficient lookup

                var toRemove = existingTimeLines.Where(t => !providedIds.Contains(t.EventTimeLineId)).ToList();

                foreach (var tr in toRemove)
                {
                    timelineRepo.DeleteAsync(tr);
                    eventToUpdate.EventTimeLines.Remove(tr);
                }
            }

            // Update the event entity
            _unitOfWork.GetRepository<Event>().UpdateAsync(eventToUpdate);

            // Commit changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetEventResponse>> ViewAllEvent(EventFilter filter, PagingModel pagingModel)
        {
            if (filter.EndDate.HasValue && filter.StartDate.HasValue && filter.EndDate.Value < filter.StartDate.Value)
            {
                throw new BadHttpRequestException("EndDate cannot be earlier than StartDate.");
            }

            Expression<Func<Event, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.EventName) || x.EventName.Contains(filter.EventName)) &&
                (string.IsNullOrEmpty(filter.Img) || x.Img == filter.Img) &&
                (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                (!filter.StartDate.HasValue || x.EndDate >= filter.StartDate) &&
                (!filter.EndDate.HasValue || x.StartDate <= filter.EndDate) &&
                (!filter.OrganizerId.HasValue || x.OrganizerId == filter.OrganizerId) &&
                (!filter.MajorId.HasValue || x.MajorId == filter.MajorId) &&
                (string.IsNullOrEmpty(filter.Location) || x.Location.Contains(filter.Location)) &&
                (string.IsNullOrEmpty(filter.CreatedBy) || x.CreatedBy.Contains(filter.CreatedBy));

            return await _unitOfWork.GetRepository<Event>().GetPagingListAsync(
                selector: x => _mapper.Map<GetEventResponse>(x),
                predicate: predicate,
                orderBy: x => x.OrderByDescending(x => x.CreatedAt),
                include: q => q.Include(e => e.UserJoinEvents),
                page: pagingModel.page,
                size: pagingModel.size
            );
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var ev = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId == id) ??
                throw new NotFoundException("EventNotFound");

            _unitOfWork.GetRepository<Event>().DeleteAsync(ev);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<int> CountAllEvents()
        {
            ICollection<EventDetailResponse> events = await _unitOfWork.GetRepository<Event>().GetListAsync(
            selector: x => _mapper.Map<EventDetailResponse>(x));
            int count = events.Count();
            return count;
        }

        public async Task<ICollection<CountByMonthResponse>> CountEventsByMonth(int? month, int? year)
        {
            int targetYear = (year == null || year == 0) ? TimeHelper.NowInVietnam().Year : year.Value;
            int startMonth = (month.HasValue && month > 0 && month <= 12) ? month.Value : 1;
            int endMonth = (targetYear == TimeHelper.NowInVietnam().Year) ? TimeHelper.NowInVietnam().Month : 12;
            var result = new List<CountByMonthResponse>();
            for (int m = startMonth; m <= endMonth; m++)
            {
                var users = await _unitOfWork.GetRepository<Event>().GetListAsync(
                    selector: x => _mapper.Map<EventDetailResponse>(x),
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


        // Đếm số lượng sự kiện theo trạng thái(tận dụng trường Status)
        public async Task<Dictionary<string, int>> GetEventCountByStatus()
        {
            var events = await _unitOfWork.GetRepository<Event>()
                .GetListAsync();
            return events.GroupBy(e => e.Status ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Kiểm tra trùng lịch sự kiện (dùng StartDate và EndDate)
        public async Task<bool> CheckEventConflict(int eventId, DateTime newStart, DateTime newEnd)
        {
            var existingEvent = await _unitOfWork.GetRepository<Event>()
                .SingleOrDefaultAsync(predicate: e => e.EventId == eventId);
            if (existingEvent == null) return false;
            return await _unitOfWork.GetRepository<Event>()
                .AnyAsync(e => e.EventId != eventId &&
                              e.OrganizerId == existingEvent.OrganizerId &&
                              ((newStart >= e.StartDate && newStart <= e.EndDate) ||
                               (newEnd >= e.StartDate && newEnd <= e.EndDate) ||
                               (newStart <= e.StartDate && newEnd >= e.EndDate)));
        }

        // Gợi ý sự kiện tương tự (dựa trên MajorId và Description)
        public async Task<IEnumerable<GetEventResponse>> GetSimilarEvents(int eventId, int count)
        {
            // Lấy thông tin sự kiện hiện tại
            var currentEvent = await _unitOfWork.GetRepository<Event>()
                .SingleOrDefaultAsync(predicate: e => e.EventId == eventId);

            if (currentEvent == null)
                throw new NotFoundException("EventNotFound");

            // Tìm các sự kiện tương tự
            var similarEvents = await _unitOfWork.GetRepository<Event>().GetListAsync(
                predicate: x => x.EventId != eventId &&
                                (x.MajorId == currentEvent.MajorId ||
                                 x.Description.Contains(currentEvent.EventName)),
                orderBy: x => x.OrderByDescending(e => e.StartDate)
            );

            // Map kết quả và giới hạn số lượng
            var mappedEvents = _mapper.Map<IEnumerable<GetEventResponse>>(similarEvents);

            return mappedEvents.Take(count);
        }

        // Lấy events sắp xếp theo độ phổ biến (dựa vào số lượng người tham gia)
        public async Task<IEnumerable<EventPopularityDto>> GetEventsByPopularity(int top)
        {
            if (top <= 0) throw new BadHttpRequestException("Số lượng lớn hơn 0");
            var paginatedEvents = await _unitOfWork.GetRepository<Event>()
                .GetPagingListAsync(
                    include: x => x.Include(e => e.UserJoinEvents),
                    selector: x => new EventPopularityDto
                    {
                        EventId = x.EventId,
                        EventName = x.EventName,
                        ParticipantCount = x.UserJoinEvents.Count,
                        PopularityScore = CalculatePopularityScore(x)
                    },
                    orderBy: x => x.OrderByDescending(e => e.UserJoinEvents.Count),
                    size: top
                );
            // Sử dụng Items để lấy danh sách các mục
            return paginatedEvents.Items; // Trả về danh sách các EventPopularityDto
        }

        private static float CalculatePopularityScore(Event e)
        {
            // Tính điểm phổ biến dựa trên số người tham gia và thời gian còn lại
            var timeFactor = (e.StartDate - TimeHelper.NowInVietnam()).TotalDays > 30 ? 1.2f : 1.0f;
            return e.UserJoinEvents.Count * timeFactor;
        }

        public async Task<BestEventTimeDto> SuggestBestTimeForNewEvent(int organizerId, int durationHours)
        {
            var organizerEvents = await _unitOfWork.GetRepository<Event>()
                .GetListAsync(predicate: x => x.OrganizerId == organizerId);
            var timeSlots = new Dictionary<DateTime, int>();
            var startDate = TimeHelper.NowInVietnam().AddDays(7);
            var endDate = startDate.AddDays(30);
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue; // Bỏ qua cuối tuần
                for (var hour = 9; hour <= 17; hour += 2) // Xét các khung giờ hàng 2 tiếng
                {
                    var slotStart = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                    var slotEnd = slotStart.AddHours(durationHours); // Tính thời gian kết thúc dựa trên durationHours
                    var conflictExists = organizerEvents.Any(e =>
                        (slotStart < e.EndDate && slotEnd > e.StartDate)); // Kiểm tra xung đột với thời gian mới

                    if (!conflictExists)
                    {
                        timeSlots[slotStart] = CalculateTimeSlotScore(slotStart);
                    }
                }
            }
            var bestTime = timeSlots.OrderByDescending(x => x.Value).FirstOrDefault();
            return new BestEventTimeDto
            {
                SuggestedTime = bestTime.Key,
                Score = bestTime.Value,
                AlternativeTimes = timeSlots
                    .Where(x => x.Value >= bestTime.Value * 0.8)
                    .OrderByDescending(x => x.Value)
                    .Take(3)
                    .ToDictionary(x => x.Key, x => x.Value)
            };
        }

        private int CalculateTimeSlotScore(DateTime timeSlot)
        {
            // Điểm cao hơn cho các khung giờ 10-11AM và 2-3PM
            return timeSlot.Hour switch
            {
                10 or 11 => 5,
                14 or 15 => 4,
                9 or 16 => 3,
                13 or 17 => 2,
                _ => 1
            };
        }

        public List<SuggestedTimelineDto> GetSuggestedTimelines(DateTime eventStartTime, int eventDurationHours)
        {
            var suggestions = new List<SuggestedTimelineDto>();

            // Example timeline templates that can be adjusted based on event duration
            var baseTemplates = new List<SuggestedTimelineDto>
    {
        new SuggestedTimelineDto
        {
            Title = "Opening Ceremony",
            StartTime = eventStartTime.AddHours(0),
            EndTime = eventStartTime.AddHours(1),
            Description = "Welcome speech and introductions"
        },
        new SuggestedTimelineDto
        {
            Title = "Keynote Session",
            StartTime = eventStartTime.AddHours(1),
            EndTime = eventStartTime.AddHours(2),
            Description = "Main presentation by keynote speaker"
        },
        new SuggestedTimelineDto
        {
            Title     = "Break",
            StartTime = eventStartTime.AddHours(2),
            EndTime = eventStartTime.AddHours(2.5),
            Description = "Networking and refreshments"
        },
        new SuggestedTimelineDto
        {
            Title     = "Workshop Session",
            StartTime = eventStartTime.AddHours(2.5),
            EndTime = eventStartTime.AddHours(4),
            Description = "Interactive workshop activities"
        },
        new SuggestedTimelineDto
        {
            Title = "Closing Remarks",
            StartTime = eventStartTime.AddHours(4),
            EndTime = eventStartTime.AddHours(4.5),
            Description = "Final thoughts and thank yous"
        }
    };

            // Adjust timelines based on event duration
            var scaleFactor = eventDurationHours / 4.5; // 4.5 is total hours in our template
            foreach (var template in baseTemplates)
            {
                suggestions.Add(new SuggestedTimelineDto
                {
                    Title = template.Title,
                    StartTime = eventStartTime.AddHours(template.StartTime.Hour * scaleFactor),
                    EndTime = eventStartTime.AddHours(template.EndTime.Hour * scaleFactor),
                    Description = template.Description
                });
            }

            return suggestions;
        }


    }
}