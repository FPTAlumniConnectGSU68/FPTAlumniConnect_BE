using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MajorCodeService : BaseService<MajorCodeService>, IMajorCodeService
    {

        public MajorCodeService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<MajorCodeService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewMajorCode(MajorCodeInfo request)
        {
            var existingMajorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorName == request.MajorName);
            if (existingMajorCode != null)
            {
                throw new BadHttpRequestException("Major already exists.");
            }
            MajorCode newMajorCode = _mapper.Map<MajorCode>(request);

            await _unitOfWork.GetRepository<MajorCode>().InsertAsync(newMajorCode);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newMajorCode.MajorId;
        }

        public async Task<MajorCodeReponse> GetMajorCodeById(int id)
        {
            MajorCode majorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException("MajorCodeNotFound");

            MajorCodeReponse result = _mapper.Map<MajorCodeReponse>(majorCode);
            return result;
        }

        public async Task<bool> UpdateMajorCodeInfo(int id, MajorCodeInfo request)
        {
            var existingMajorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorName == request.MajorName);
            if (existingMajorCode != null)
            {
                throw new BadHttpRequestException("Major already exists.");
            }
            MajorCode majorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException("MajorCodeNotFound");

            majorCode.MajorName = string.IsNullOrEmpty(request.MajorName) ? majorCode.MajorName : request.MajorName;
            majorCode.UpdatedAt = DateTime.Now;
            majorCode.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<MajorCode>().UpdateAsync(majorCode);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<MajorCodeReponse>> ViewAllMajorCode(MajorCodeFilter filter, PagingModel pagingModel)
        {


            IPaginate<MajorCodeReponse> response = await _unitOfWork.GetRepository<MajorCode>().GetPagingListAsync(
                selector: x => _mapper.Map<MajorCodeReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}