namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPerspectiveService
    {
        Task<bool> IsContentAppropriate(string content);
    }
}
