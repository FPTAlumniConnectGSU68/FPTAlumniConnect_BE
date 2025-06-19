using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using System;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;

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
            var newT = _mapper.Map<EventTimeLine>(request);
            await _unitOfWork.GetRepository<EventTimeLine>().InsertAsync(newT);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newT.EventTimeLineId;
        }
        public async Task<TimeLineReponse> GetTimeLineById(int id)
        {
            EventTimeLine TimeLine = await _unitOfWork.GetRepository<EventTimeLine>().SingleOrDefaultAsync(
                predicate: x => x.EventTimeLineId.Equals(id)) ?? throw new BadHttpRequestException("TimeLineNotFound");

            return _mapper.Map<TimeLineReponse>(TimeLine);
        }

        public async Task<bool> UpdateTimeLine(int id, TimeLineInfo request)
        {
            EventTimeLine TimeLine = await _unitOfWork.GetRepository<EventTimeLine>().SingleOrDefaultAsync(
                predicate: x => x.EventTimeLineId.Equals(id)) ?? throw new BadHttpRequestException("TimeLineNotFound");

            TimeLine.Title = string.IsNullOrEmpty(request.Title) ? TimeLine.Title : request.Title;
            TimeLine.Description = string.IsNullOrEmpty(request.Description) ? TimeLine.Description : request.Description;
            TimeLine.StartTime = request.StartTime;
            TimeLine.EndTime = request.EndTime;
            if (TimeLine.StartTime >= TimeLine.EndTime)
                throw new BadHttpRequestException("StartTime must be earlier than EndTime");
            _unitOfWork.GetRepository<EventTimeLine>().UpdateAsync(TimeLine);
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

            return response;
        }
    }
}
