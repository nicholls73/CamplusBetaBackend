using CamplusBetaBackend.Models;

namespace CamplusBetaBackend.Services.Interfaces {
    public interface IHostService
    {
        Task<Models.Host?> GetHostFromDB(Guid id);
        Task AddNewHostToDB(Models.Host host);
    }
}
