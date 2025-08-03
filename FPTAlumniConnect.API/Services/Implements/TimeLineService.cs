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
            Event Event = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(request.EventId)) ??
                throw new BadHttpRequestException("EventNotFound");

            var newT = _mapper.Map<EventTimeLine>(request);
            await _unitOfWork.GetRepository<EventTimeLine>().InsertAsync(newT);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newT.EventTimeLineId;
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
            // Lấy timeline từ cơ sở dữ liệu
            EventTimeLine timeLine = await _unitOfWork.GetRepository<EventTimeLine>().SingleOrDefaultAsync(
                predicate: x => x.EventTimeLineId.Equals(id))
                ?? throw new BadHttpRequestException("TimeLineNotFound");

            // Cập nhật các thuộc tính nếu có giá trị mới
            if (!string.IsNullOrEmpty(request.Title)) // Cập nhật tên
            {
                timeLine.Title = request.Title;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                timeLine.Description = request.Description;
            }

            // Chuyển đổi StartTime và EndTime từ DateTime sang TimeSpan
            if (request.StartTime != default(DateTime))
            {
                timeLine.StartTime = request.StartTime.TimeOfDay; // Chuyển đổi sang TimeSpan
            }

            if (request.EndTime != default(DateTime))
            {
                timeLine.EndTime = request.EndTime.TimeOfDay; // Chuyển đổi sang TimeSpan
            }

            // Kiểm tra điều kiện StartTime phải trước EndTime
            if (timeLine.StartTime >= timeLine.EndTime)
            {
                throw new BadHttpRequestException("StartTime must be earlier than EndTime");
            }

            // Cập nhật timeline trong cơ sở dữ liệu
            _unitOfWork.GetRepository<EventTimeLine>().UpdateAsync(timeLine);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<TimeLineReponse>> ViewAllTimeLine(TimeLineFilter filter, PagingModel pagingModel)
        {
            IPaginate<TimeLineReponse> response = await _unitOfWork.GetRepository<EventTimeLine>().GetPagingListAsync(
                selector: x => _mapper.Map<TimeLineReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.EventTimeLineId),
                page: pagingModel.page,
                size: pagingModel.size
            );

            if (response == null || response.Items.Count == 0)
            {
                throw new BadHttpRequestException("No timelines found.");
            }

            return response;
        }
    }
}
