using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ICVService
    {
        Task<int> CreateNewCV(CVInfo request);
        Task<IPaginate<CVReponse>> ViewAllCV(CVFilter filter, PagingModel pagingModel);
        Task<bool> UpdateCVInfo(int id, CVInfo request);
        Task<CVReponse> GetCVById(int id);
        Task<CVReponse> GetCVByUserId(int id);
    }
}
