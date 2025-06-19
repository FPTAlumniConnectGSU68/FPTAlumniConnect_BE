    using AutoMapper;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using System.Security.Claims;

namespace FPTAlumniConnect.API.Services.Implements
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<AlumniConnectContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        protected BaseService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<T> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetEmailFromJwt()
        {
            string username = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return username;
        }

        protected string GetRoleFromJwt()
        {
            string role = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return role;
        }
    }
}