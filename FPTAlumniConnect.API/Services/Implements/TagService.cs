using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.TagJob;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            ValidateCvExists(request.CvID);

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

        public async Task<IEnumerable<TagJobReponse>> GetTagsByCvId(int cvId)
        {
            var tags = await _unitOfWork.GetRepository<TagJob>().GetListAsync(
                predicate: x => x.CvID == cvId,
                selector: x => _mapper.Map<TagJobReponse>(x)
            );

            return tags;
        }

        public async Task<bool> UpdateTagInfo(int id, TagJobInfo request)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
            predicate: x => x.TagJobId.Equals(id)) ??
            throw new BadHttpRequestException("TagNotFound");

            // Check CvId
            ValidateCvExists(request.CvID);

            // Check if the user already has this tag
            TagJob existingTagJob = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
            predicate: s => s.Tag == request.Tag && s.CvID == request.CvID && s.TagJobId != id);

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

        public async Task<bool> DeleteTag(int id)
        {
            var tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.TagJobId == id) ?? throw new BadHttpRequestException("TagNotFound");

            _unitOfWork.GetRepository<TagJob>().DeleteAsync(tag);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<int> CountTagsByCvId(int cvId)
        {
            return await _unitOfWork.GetRepository<TagJob>()
                .GetQueryable()
                .CountAsync(x => x.CvID == cvId);
        }


        private async Task ValidateCvExists(int cvId)
        {
            var exists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == cvId);
            if (!exists)
                throw new BadHttpRequestException("CvIdNotFound");
        }

    }
}
