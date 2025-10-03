using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    // Service for managing social links
    public class SocialLinkService : BaseService<SocialLinkService>, ISocialLinkService
    {
        public SocialLinkService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SocialLinkService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateSocialLink(SocialLinkInfo request)
        {
            // Check if user exists
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.UserId))
                ?? throw new BadHttpRequestException("UserNotFound");

            // Validate link content and format
            await ValidateSocialLinkAsync(request.Link);

            // Check for duplicate link
            var existing = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: s => s.UserId == request.UserId && s.Link == request.Link);
            if (existing != null)
                throw new BadHttpRequestException("This link already exists for the user.");

            // Create and insert new link
            var newLink = _mapper.Map<SoicalLink>(request);
            newLink.CreatedAt = TimeHelper.NowInVietnam();
            newLink.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<SoicalLink>().InsertAsync(newLink);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newLink.Slid;
        }

        private async Task ValidateSocialLinkAsync(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                throw new BadHttpRequestException("Link cannot be empty.");

            if (link.Length > 500)
                throw new BadHttpRequestException("Link is too long. Max 500 characters.");

            if (!Uri.TryCreate(link, UriKind.Absolute, out var uriResult))
                throw new BadHttpRequestException("Invalid URL format.");

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
                throw new BadHttpRequestException("Only HTTP and HTTPS are allowed.");

            // Optional: Allow only specific domains
            string[] allowedDomains = { "facebook.com", "linkedin.com", "twitter.com", "github.com", "instagram.com" };

            bool isAllowed = allowedDomains.Any(domain =>
                uriResult.Host.Equals(domain, StringComparison.OrdinalIgnoreCase) ||
                uriResult.Host.EndsWith($".{domain}", StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
                throw new BadHttpRequestException("This social platform is not supported.");

            // Optional: Check URL availability
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, link));
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to verify the link.");
            }
            catch
            {
                throw new BadHttpRequestException("Error when verifying the link.");
            }
        }

        public async Task<GetSocialLinkResponse> GetSocialLinkById(int id)
        {
            var link = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid == id)
                ?? throw new BadHttpRequestException("SocialLinkNotFound");

            return _mapper.Map<GetSocialLinkResponse>(link);
        }

        public async Task<bool> UpdateSocialLink(int id, SocialLinkInfo request)
        {
            var link = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid == id)
                ?? throw new BadHttpRequestException("SocialLinkNotFound");

            // If link is changed, validate and check duplicate
            if (request.Link != link.Link)
            {
                var exists = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                    predicate: s => s.UserId == request.UserId && s.Link == request.Link);

                if (exists != null)
                    throw new BadHttpRequestException("This link already exists.");

                await ValidateSocialLinkAsync(request.Link);
                link.Link = request.Link;
            }

            link.UpdatedAt = TimeHelper.NowInVietnam();
            link.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<SoicalLink>().UpdateAsync(link);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<GetSocialLinkResponse>> ViewAllSocialLinks(SocialLinkFilter filter, PagingModel pagingModel)
        {
            return await _unitOfWork.GetRepository<SoicalLink>().GetPagingListAsync(
                selector: x => _mapper.Map<GetSocialLinkResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size);
        }

        public async Task<bool> DeleteSocialLink(int id)
        {
            var link = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid == id)
                ?? throw new BadHttpRequestException("SocialLinkNotFound");

            _unitOfWork.GetRepository<SoicalLink>().DeleteAsync(link);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<ICollection<GetSocialLinkResponse>> GetLinksByUserId(int userId)
        {
            return await _unitOfWork.GetRepository<SoicalLink>().GetListAsync(
                selector: x => _mapper.Map<GetSocialLinkResponse>(x),
                predicate: x => x.UserId == userId);
        }

        public async Task<bool> ApproveSocialLinkAsync(int id)
        {
            var link = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid == id)
                ?? throw new BadHttpRequestException("SocialLinkNotFound");

            //link.IsApproved = true;
            link.UpdatedAt = TimeHelper.NowInVietnam();

            _unitOfWork.GetRepository<SoicalLink>().UpdateAsync(link);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> ReportSocialLinkAsync(int id)
        {
            var link = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid == id)
                ?? throw new BadHttpRequestException("SocialLinkNotFound");

            //link.ReportedCount += 1;
            link.UpdatedAt = TimeHelper.NowInVietnam();

            _unitOfWork.GetRepository<SoicalLink>().UpdateAsync(link);
            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
