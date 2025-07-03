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
    // Service class for managing tag-related operations (CRUD, listing, filtering) tied to a CV.
    public class TagService : BaseService<TagService>, ITagService
    {
        public TagService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<TagService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        // Creates a new tag for a CV. Throws error if tag already exists for the same CV.
        public async Task<int> CreateNewTag(TagJobInfo request)
        {
            ValidateCvExists(request.CvID);

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

        // Retrieves a tag by its ID.
        public async Task<TagJobReponse> GetTagById(int id)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.TagJobId.Equals(id))
                ?? throw new BadHttpRequestException("TagNotFound");

            return _mapper.Map<TagJobReponse>(tag);
        }

        // Gets all tags associated with a specific CV.
        public async Task<IEnumerable<TagJobReponse>> GetTagsByCvId(int cvId)
        {
            var tags = await _unitOfWork.GetRepository<TagJob>().GetListAsync(
                predicate: x => x.CvID == cvId,
                selector: x => _mapper.Map<TagJobReponse>(x)
            );

            return tags;
        }

        // Updates a tag's content. Throws error if duplicate tag is found on same CV.
        public async Task<bool> UpdateTagInfo(int id, TagJobInfo request)
        {
            TagJob tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.TagJobId.Equals(id))
                ?? throw new BadHttpRequestException("TagNotFound");

            ValidateCvExists(request.CvID);

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
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Retrieves all tags in the system with pagination and optional filters.
        public async Task<IPaginate<TagJobReponse>> ViewAllTag(TagJobFilter filter, PagingModel pagingModel)
        {
            var response = await _unitOfWork.GetRepository<TagJob>().GetPagingListAsync(
                selector: x => _mapper.Map<TagJobReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );
            return response;
        }

        // Deletes a tag by ID.
        public async Task<bool> DeleteTag(int id)
        {
            var tag = await _unitOfWork.GetRepository<TagJob>().SingleOrDefaultAsync(
                predicate: x => x.TagJobId == id)
                ?? throw new BadHttpRequestException("TagNotFound");

            _unitOfWork.GetRepository<TagJob>().DeleteAsync(tag);
            return await _unitOfWork.CommitAsync() > 0;
        }

        // Counts the number of tags associated with a specific CV.
        public async Task<int> CountTagsByCvId(int cvId)
        {
            return await _unitOfWork.GetRepository<TagJob>()
                .GetQueryable()
                .Where(x => x.CvID == cvId)
                .CountAsync();
        }

        // Helper method to check if a CV exists.
        private async Task ValidateCvExists(int cvId)
        {
            var exists = await _unitOfWork.GetRepository<Cv>().AnyAsync(x => x.Id == cvId);
            if (!exists)
                throw new BadHttpRequestException("CvIdNotFound");
        }
    }
}
