using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MentorshipService : BaseService<MentorshipService>, IMentorshipService
    {
        public MentorshipService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<MentorshipService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Create new mentorship record
        public async Task<int> CreateNewMentorship(MentorshipInfo request)
        {
            await EnsureAlumniExists(request.AumniId);

            var newMentorship = _mapper.Map<Mentorship>(request);

            await _unitOfWork.GetRepository<Mentorship>().InsertAsync(newMentorship);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newMentorship.Id;
        }

        // Get mentorship by ID
        public async Task<MentorshipReponse> GetMentorshipById(int id)
        {
            var mentorship = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: q => q.Include(x => x.Aumni)) ??
                throw new BadHttpRequestException("MentorshipNotFound");

            return _mapper.Map<MentorshipReponse>(mentorship);
        }

        // Get mentorships by alumni ID
        public async Task<List<MentorshipReponse>> GetMentorshipsByAlumniId(int alumniId)
        {
            var mentorships = await _unitOfWork.GetRepository<Mentorship>().GetListAsync(
                predicate: x => x.AumniId == alumniId,
                include: q => q.Include(x => x.Aumni),
                orderBy: q => q.OrderByDescending(x => x.CreatedAt)
            );

            return mentorships.Select(x => _mapper.Map<MentorshipReponse>(x)).ToList();
        }

        // Update mentorship info by ID
        public async Task<bool> UpdateMentorshipInfo(int id, MentorshipInfo request)
        {
            var mentorship = await _unitOfWork.GetRepository<Mentorship>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("MentorshipNotFound");

            if (request.AumniId != mentorship.AumniId)
            {
                await EnsureAlumniExists(request.AumniId);
                mentorship.AumniId = request.AumniId;
            }

            mentorship.RequestMessage = string.IsNullOrEmpty(request.RequestMessage)
                ? mentorship.RequestMessage
                : request.RequestMessage;

            mentorship.Status = string.IsNullOrEmpty(request.Status)
                ? mentorship.Status
                : request.Status;

            // Set updated by current user
            mentorship.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Mentorship>().UpdateAsync(mentorship);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Get paginated list of mentorships
        public async Task<IPaginate<MentorshipReponse>> ViewAllMentorship(MentorshipFilter filter, PagingModel pagingModel)
        {
            IPaginate<MentorshipReponse> response = await _unitOfWork.GetRepository<Mentorship>().GetPagingListAsync(
                selector: x => _mapper.Map<MentorshipReponse>(x),
                include: q => q.Include(x => x.Aumni),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }

        // Get mentorship statistics by status
        public async Task<Dictionary<string, int>> GetMentorshipStatusStatistics()
        {
            var query = _unitOfWork.GetRepository<Mentorship>().GetQueryable();

            var result = await query
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status ?? "Unknown", x => x.Count);

            return result;
        }

        // Automatically cancel expired mentorships after 2 days
        public async Task<int> AutoCancelExpiredMentorships()
        {
            var now = DateTime.UtcNow;
            var expiredMentorships = await _unitOfWork.GetRepository<Mentorship>().FindAllAsync(
                predicate: x => x.Status == "Pending" && x.CreatedAt.HasValue && x.CreatedAt.Value.AddDays(2) < now
            );

            foreach (var item in expiredMentorships)
            {
                item.Status = "Cancelled";
                item.UpdatedBy = "System";
                _unitOfWork.GetRepository<Mentorship>().UpdateAsync(item);
            }

            return await _unitOfWork.CommitAsync();
        }

        // Validate alumni existence by ID and role
        private async Task<User> EnsureAlumniExists(int? alumniId)
        {
            if (!alumniId.HasValue)
                throw new BadHttpRequestException("AlumniId is required");

            return await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: s => s.UserId == alumniId.Value && s.RoleId == 2
            ) ?? throw new BadHttpRequestException("AlumniIdNotFound");
        }
    }
}
