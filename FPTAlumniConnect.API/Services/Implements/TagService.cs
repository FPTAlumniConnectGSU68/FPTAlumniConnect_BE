using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.TagJob;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class TagService : BaseService<TagService>, ITagService
    {

        public TagService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<TagService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewTag(TagJobInfo request)
        {
            // Check CvId
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
            predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            // Check if the user already has this tag
            TagJob existingTagJob = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: s => s.Tag == request.Tag && s.CvID == request.CvID);

            if (existingTagJob != null)
            {
                throw new BadHttpRequestException("Tag already exists.");
            }

            var newTag = _mapper.Map<TagJob>(request);
            newTag.CreatedAt = DateTime.Now;
            newTag.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;


            await _unitOfWork.GetRepository<TagJob>().InsertAsync(newTag);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newTag.TagJobId;
        }

        public async Task<TagJobReponse> GetTagById(int id)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.TagJobId.Equals(id)) ??
                throw new BadHttpRequestException("TagNotFound");

            TagJobReponse result = _mapper.Map<TagJobReponse>(tag);
            return result;
        }

        public async Task<TagJobReponse> GetTagByCvId(int id)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.CvID.Equals(id)) ??
                throw new BadHttpRequestException("TagNotFound");

            TagJobReponse result = _mapper.Map<TagJobReponse>(tag);
            return result;
        }

        public async Task<bool> UpdateTagInfo(int id, TagJobInfo request)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
            predicate: x => x.TagJobId.Equals(id)) ??
            throw new BadHttpRequestException("TagNotFound");

            // Check CvId
            Cv checkCvId = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
            predicate: s => s.Id == request.CvID);
            if (checkCvId == null)
            {
                throw new BadHttpRequestException("CvIdNotFound");
            }

            // Check if the user already has this tag
            TagJob existingTagJob = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: s => s.Tag == request.Tag && s.CvID == request.CvID);
            if (existingTagJob != null)
            {
                throw new BadHttpRequestException("Tag already exists.");
            }

            tag.Tag = string.IsNullOrEmpty(request.Tag) ? tag.Tag : request.Tag;
            tag.UpdatedAt = DateTime.Now;
            tag.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<TagJob>().UpdateAsync(tag);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<TagJobReponse>> ViewAllTag(TagJobFilter filter, PagingModel pagingModel)
        {
            IPaginate<TagJobReponse> response = await _unitOfWork.GetRepository<TagJob>().GetPagingListAsync(
                selector: x => _mapper.Map<TagJobReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
