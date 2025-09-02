using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

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
            if (request == null || string.IsNullOrWhiteSpace(request.MajorName))
            {
                throw new BadHttpRequestException("Major name is required.");
            }
            var existingMajorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorName == request.MajorName);
            if (existingMajorCode != null)
            {
                throw new BadHttpRequestException("Major already exists.");
            }

            MajorCode newMajorCode = _mapper.Map<MajorCode>(request);
            newMajorCode.CreatedAt = TimeHelper.NowInVietnam();
            newMajorCode.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<MajorCode>().InsertAsync(newMajorCode);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("Create failed");

            return newMajorCode.MajorId;
        }

        public async Task<MajorCodeReponse> GetMajorCodeById(int id)
        {
            MajorCode majorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException($"MajorCode with ID {id} not found.");
            return _mapper.Map<MajorCodeReponse>(majorCode);
        }

        public async Task<bool> UpdateMajorCodeInfo(int id, MajorCodeInfo request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MajorName))
            {
                throw new BadHttpRequestException("Major name is required.");
            }

            var majorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException($"MajorCode with ID {id} not found.");
            // Kiểm tra nếu tên đã tồn tại nhưng thuộc Major khác
            var existingMajorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorName == request.MajorName && x.MajorId != id);

            if (existingMajorCode != null)
            {
                throw new BadHttpRequestException("Another major with the same name already exists.");
            }

            majorCode.MajorName = request.MajorName; // Cập nhật tên chuyên ngành
            majorCode.UpdatedAt = TimeHelper.NowInVietnam();
            majorCode.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<MajorCode>().UpdateAsync(majorCode);
            return await _unitOfWork.CommitAsync() > 0;
        }


        public async Task<IPaginate<MajorCodeReponse>> ViewAllMajorCode(MajorCodeFilter filter, PagingModel pagingModel)
        {
            var response = await _unitOfWork.GetRepository<MajorCode>().GetPagingListAsync(
                selector: x => _mapper.Map<MajorCodeReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );
            if (response == null || !response.Items.Any())
            {
                throw new BadHttpRequestException("No major codes found.");
            }
            return response;
        }

        public async Task<bool> DeleteMajorCodeAsync(int id)
        {
            var majorCode = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId == id) ??
                throw new BadHttpRequestException($"MajorCode with ID {id} not found.");

            // Kiểm tra xem majorCode có đang được sử dụng hay không (nếu cần)
            // if (IsMajorCodeInUse(majorCode)) // Implement this method if necessary
            // {
            //     throw new BadHttpRequestException("Cannot delete major code that is in use.");
            // }

            _unitOfWork.GetRepository<MajorCode>().DeleteAsync(majorCode);
            return await _unitOfWork.CommitAsync() > 0;
        }


        public async Task<List<string>> GetAllMajorNames()
        {
            var majors = await _unitOfWork.GetRepository<MajorCode>().GetListAsync(
                selector: x => x.MajorName
            );
            if (majors == null || !majors.Any())
            {
                throw new BadHttpRequestException("No major names found.");
            }
            return majors.ToList();
        }

        public async Task<int> CountMajorCodesAsync()
        {
            int count = await _unitOfWork.GetRepository<MajorCode>().CountAsync(x => true);
            if (count == 0)
            {
                throw new BadHttpRequestException("No major codes available.");
            }
            return count;
        }
    }
}