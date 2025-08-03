using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ICVService
    {
        Task<int> CreateNewCV(CVInfo request);
        Task<CVResponse> GetCVById(int id);
        Task<CVResponse> GetCVByUserId(int id);
        Task<bool> UpdateCVInfo(int id, CVInfo request);
        Task<IPaginate<CVResponse>> ViewAllCV(CVFilter filter, PagingModel pagingModel);
        Task<bool> ToggleIsLookingForJobAsync(int cvId);
        Task<byte[]> ExportCvToPdfAsync(int cvId);
        Task<List<int>> GetCVSkills(int cvId);
        Task ShareCvByEmailAsync(ShareCvRequest request);
    }
}