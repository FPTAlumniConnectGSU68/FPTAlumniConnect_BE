using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SpMajorCode;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ISpMajorCodeService
    {
        Task<int> CreateNewSpMajorCode(SpMajorCodeInfo request);
        Task<IPaginate<SpMajorCodeResponse>> ViewAllSpMajorCodes(SpMajorCodeFilter filter, PagingModel pagingModel);
        Task<bool> UpdateSpMajorCodeInfo(int id, SpMajorCodeInfo request);
        Task<SpMajorCodeResponse> GetSpMajorCodeById(int id);
    }
}
