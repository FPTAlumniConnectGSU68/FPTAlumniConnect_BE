using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MentorshipService : BaseService<MentorshipService>, IMentorshipService
    {

        public MentorshipService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<MentorshipService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewMentorship(MentorshipInfo request)
        {
            // Check AlumniId
            User checkAlumniId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.AlumniId && s.RoleId == 2);
            if (checkAlumniId == null)
            {
                throw new BadHttpRequestException("AlumniIdNotFound");
            }

            Mentorship newMentorship = _mapper.Map<Mentorship>(request);

            await _unitOfWork.GetRepository<Mentorship>().InsertAsync(newMentorship);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newMentorship.Id;
        }

        public async Task<MentorshipReponse> GetMentorshipById(int id)
        {
            Mentorship post = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("MentorshipNotFound");

            MentorshipReponse result = _mapper.Map<MentorshipReponse>(post);
            return result;
        }

        public async Task<bool> UpdateMentorshipInfo(int id, MentorshipInfo request)
        {
            Mentorship mentorship = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("MentorshipNotFound");

            // Check AlumniId
            User checkAlumniId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.AlumniId && s.RoleId == 2);
            if (checkAlumniId == null)
            {
                throw new BadHttpRequestException("AlumniIdNotFound");
            }

            mentorship.RequestMessage = string.IsNullOrEmpty(request.RequestMessage) ? mentorship.RequestMessage : request.RequestMessage;
            mentorship.Type = string.IsNullOrEmpty(request.Type) ? mentorship.Type : request.Type;
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<MentorshipReponse>> ViewAllMentorship(MentorshipFilter filter, PagingModel pagingModel)
        {
            IPaginate<MentorshipReponse> response = await _unitOfWork.GetRepository<Mentorship>().GetPagingListAsync(
                selector: x => _mapper.Map<MentorshipReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
