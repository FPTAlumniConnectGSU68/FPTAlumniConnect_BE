using AutoMapper;
using Azure.Core;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class TimeLineService : BaseService<TimeLineService>, ITimeLineService
    {
        public TimeLineService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<TimeLineService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateTimeLine(TimeLineInfo request)
        {
            // Validate request
            if (request == null)
                throw new BadHttpRequestException("Request cannot be null.");

            // Fetch event
            var eventEntity = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(request.EventId))
                ?? throw new BadHttpRequestException("EventNotFound");

            // Validate Title and Description
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                if (request.Title.Length < 3 || request.Title.Length > 100)
                    throw new BadHttpRequestException("Title must be between 3 and 100 characters.");
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                if (request.Description.Length > 1000)
                    throw new BadHttpRequestException("Description cannot exceed 1000 characters.");
            }

            // Validate and parse StartTime and EndTime
            if (string.IsNullOrWhiteSpace(request.StartTime) || string.IsNullOrWhiteSpace(request.EndTime))
                throw new BadHttpRequestException("StartTime and EndTime are required and must be non-empty strings.");

            var startTs = ParseTimeSpanOrThrow(request.StartTime, "StartTime");
            var endTs = ParseTimeSpanOrThrow(request.EndTime, "EndTime");

            // Resolve DateTime for StartTime and EndTime, ensuring within event range
            var startDt = GetNextDateTimeForTime(eventEntity, startTs, eventEntity.StartDate);
            var endDt = GetNextDateTimeForTime(eventEntity, endTs, startDt);

            if (endDt < startDt)
                throw new BadHttpRequestException("Timeline EndTime cannot be earlier than StartTime.");

            // Map to EventTimeLine after validation
            var newTimeline = new EventTimeLine
            {
                EventId = request.EventId,
                Title = request.Title,
                Description = request.Description,
                StartTime = startTs,
                EndTime = endTs
            };

            // Insert timeline
            await _unitOfWork.GetRepository<EventTimeLine>().InsertAsync(newTimeline);

            // Commit changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("Failed to create timeline due to database error.");

            return newTimeline.EventTimeLineId;
        }

        // Helper functions (reused from UpdateEventInfo)
        private TimeSpan ParseTimeSpanOrThrow(string input, string fieldName)
        {
            if (TimeSpan.TryParse(input, out var ts))
            {
                ValidateTimeSpan(ts, fieldName);
                return ts;
            }

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

        private void ValidateTimeSpan(TimeSpan ts, string fieldName)
        {
            if (ts < TimeSpan.Zero)
                throw new BadHttpRequestException($"{fieldName} cannot be negative.");

            if (ts.TotalHours >= 24)
                throw new BadHttpRequestException($"{fieldName} must be less than 24 hours (use format for time-of-day).");
        }

        private DateTime GetNextDateTimeForTime(Event e, TimeSpan timeOfDay, DateTime minAllowed)
        {
            var candidate = minAllowed.Date.Add(timeOfDay);

            while (candidate < minAllowed)
                candidate = candidate.AddDays(1);

            if (candidate < e.StartDate || candidate > e.EndDate)
                throw new BadHttpRequestException($"Timeline time {timeOfDay} (resolved to {candidate:u}) is outside event range [{e.StartDate:u} - {e.EndDate:u}].");

            return candidate;
        }
        public async Task<TimeLineReponse> GetTimeLineById(int id)
        {
            EventTimeLine TimeLine = await _unitOfWork.GetRepository<EventTimeLine>().SingleOrDefaultAsync(
                predicate: x => x.EventTimeLineId.Equals(id)) ?? 
                throw new BadHttpRequestException("TimeLineNotFound");

            TimeLineReponse result = _mapper.Map<TimeLineReponse>(TimeLine);
            return result;
        }

        public async Task<bool> UpdateTimeLine(int id, TimeLineInfo request)
        {
            // Validate request
            if (request == null)
                throw new BadHttpRequestException("Request cannot be null.");

            // Fetch timeline
            var timeLine = await _unitOfWork.GetRepository<EventTimeLine>().SingleOrDefaultAsync(
                predicate: x => x.EventTimeLineId.Equals(id))
                ?? throw new BadHttpRequestException("TimeLineNotFound");

            // Fetch associated event to validate time range
            var eventEntity = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(timeLine.EventId))
                ?? throw new BadHttpRequestException("EventNotFound");

            // Validate Title and Description
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                if (request.Title.Length < 3 || request.Title.Length > 100)
                    throw new BadHttpRequestException("Title must be between 3 and 100 characters.");
                timeLine.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                if (request.Description.Length > 1000)
                    throw new BadHttpRequestException("Description cannot exceed 1000 characters.");
                timeLine.Description = request.Description;
            }

            // Validate and parse StartTime and EndTime if provided
            TimeSpan startTs = timeLine.StartTime;
            TimeSpan endTs = timeLine.EndTime;

            if (!string.IsNullOrWhiteSpace(request.StartTime))
            {
                startTs = ParseTimeSpanOrThrow(request.StartTime, "StartTime");
            }

            if (!string.IsNullOrWhiteSpace(request.EndTime))
            {
                endTs = ParseTimeSpanOrThrow(request.EndTime, "EndTime");
            }

            // Resolve DateTime for StartTime and EndTime, ensuring within event range
            var startDt = GetNextDateTimeForTime(eventEntity, startTs, eventEntity.StartDate);
            var endDt = GetNextDateTimeForTime(eventEntity, endTs, startDt);

            if (endDt < startDt)
                throw new BadHttpRequestException($"Timeline EndTime (resolved to {endDt:u}) cannot be earlier than StartTime (resolved to {startDt:u}).");

            // Update TimeSpan values
            timeLine.StartTime = startTs;
            timeLine.EndTime = endTs;

            // Update EventId if provided and valid
            if (request.EventId != 0 && request.EventId != timeLine.EventId)
            {
                var newEvent = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                    predicate: x => x.EventId.Equals(request.EventId))
                    ?? throw new BadHttpRequestException("New EventNotFound");

                // Re-validate times against new event's date range
                startDt = GetNextDateTimeForTime(newEvent, startTs, newEvent.StartDate);
                endDt = GetNextDateTimeForTime(newEvent, endTs, startDt);

                if (endDt < startDt)
                    throw new BadHttpRequestException($"Timeline EndTime (resolved to {endDt:u}) cannot be earlier than StartTime (resolved to {startDt:u}) for new event.");

                timeLine.EventId = request.EventId;
            }

            // Update timeline
            _unitOfWork.GetRepository<EventTimeLine>().UpdateAsync(timeLine);

            // Commit changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("Failed to update timeline due to database error.");

            return isSuccessful;
        }

        public async Task<IPaginate<TimeLineReponse>> ViewAllTimeLine(TimeLineFilter filter, PagingModel pagingModel)
        {
            _logger.LogInformation("Viewing all timelines with filter and paging.");

            IPaginate<TimeLineReponse> response = await _unitOfWork.GetRepository<EventTimeLine>().GetPagingListAsync(
                selector: x => _mapper.Map<TimeLineReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.EventTimeLineId),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
