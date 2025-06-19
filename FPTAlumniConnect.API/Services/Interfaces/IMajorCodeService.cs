using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IMajorCodeService
    {
        Task<int> CreateNewMajorCode(MajorCodeInfo request);
        Task<IPaginate<MajorCodeReponse>> ViewAllMajorCode(MajorCodeFilter filter, PagingModel pagingModel);
        Task<bool> UpdateMajorCodeInfo(int id, MajorCodeInfo request);
        Task<MajorCodeReponse> GetMajorCodeById(int id);
    }
}
