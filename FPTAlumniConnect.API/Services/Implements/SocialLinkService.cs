using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class SocialLinkService : BaseService<SocialLinkService>, ISocialLinkService
    {
        public SocialLinkService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SocialLinkService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateSocialLink(SocialLinkInfo request)
        {
            // Validate User
            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.UserId)) ??
                throw new BadHttpRequestException("UserNotFound");

            // Validate link format and content
            await ValidateSocialLinkAsync(request.Link);

            // Check if the user already has this link
            SoicalLink existingLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: s => s.UserId == request.UserId && s.Link == request.Link);
            if (existingLink != null)
            {
                throw new BadHttpRequestException("This link already exists for the user.");
            }

            // Map the request to a new SocialLink entity
            var newSocialLink = _mapper.Map<SoicalLink>(request);
            newSocialLink.CreatedAt = DateTime.UtcNow;
            newSocialLink.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name
            /*?? throw new UnauthorizedAccessException("User not authenticated")*/;

            // Insert the new link
            await _unitOfWork.GetRepository<SoicalLink>().InsertAsync(newSocialLink);

            // Commit changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");
            return newSocialLink.Slid;
        }

        private async Task ValidateSocialLinkAsync(string link)
        {
            // Kiểm tra null hoặc trống
            if (string.IsNullOrWhiteSpace(link))
            {
                throw new BadHttpRequestException("Link cannot be empty.");
            }

            // Kiểm tra độ dài link
            if (link.Length > 500)
            {
                throw new BadHttpRequestException("Link is too long. Maximum 500 characters allowed.");
            }

            // Kiểm tra định dạng URL
            if (!Uri.TryCreate(link, UriKind.Absolute, out Uri? uriResult))
            {
                throw new BadHttpRequestException("Invalid URL format.");
            }

            // Kiểm tra scheme (chỉ cho phép http và https)
            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                throw new BadHttpRequestException("Only HTTP and HTTPS links are allowed.");
            }

            // Danh sách các domain được phép (tuỳ chọn)
            string[] allowedDomains = new[]
            {
                "facebook.com",
                "linkedin.com",
                "twitter.com",
                "github.com",
                "instagram.com"
            };

            bool isDomainAllowed = allowedDomains.Any(domain =>
                uriResult.Host.Equals(domain, StringComparison.OrdinalIgnoreCase) ||
                uriResult.Host.EndsWith($".{domain}", StringComparison.OrdinalIgnoreCase));

            if (!isDomainAllowed)
            {
                throw new BadHttpRequestException("This social media platform is not supported.");
            }

            // Kiểm tra kết nối URL (tuỳ chọn, có thể gây chậm)
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, link));

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new BadHttpRequestException("Unable to verify the link.");
                    }
                }
            }
            catch
            {
                // Tuỳ chọn: bỏ qua lỗi kết nối hoặc log lại
                // Nếu muốn chắc chắn, có thể throw exception
                throw new BadHttpRequestException("Error when create new link");
            }
        }

        public async Task<GetSocialLinkResponse> GetSocialLinkById(int id)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            GetSocialLinkResponse result = _mapper.Map<GetSocialLinkResponse>(socialLink);
            return result;
        }

        public async Task<bool> UpdateSocialLink(int id, SocialLinkInfo request)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            // Check if the user already has this link
            SoicalLink existingLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: s => s.UserId == request.UserId && s.Link == request.Link);

            if (existingLink != null)
            {
                throw new BadHttpRequestException("This link already exists!");
            }

            //socialLink.UserId = request.UserId ?? socialLink.UserId;  // Tại sao lại cho thay đổi ???
            // Validate link format and content
            await ValidateSocialLinkAsync(request.Link);

            socialLink.Link = request.Link ?? socialLink.Link;
            socialLink.UpdatedAt = DateTime.Now;
            socialLink.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name
            /*?? throw new UnauthorizedAccessException("User not authenticated")*/;

            _unitOfWork.GetRepository<SoicalLink>().UpdateAsync(socialLink);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetSocialLinkResponse>> ViewAllSocialLinks(SocialLinkFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetSocialLinkResponse> response = await _unitOfWork.GetRepository<SoicalLink>().GetPagingListAsync(
                selector: x => _mapper.Map<GetSocialLinkResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }

        public async Task<bool> DeleteSocialLink(int id)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            _unitOfWork.GetRepository<SoicalLink>().DeleteAsync(socialLink);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}