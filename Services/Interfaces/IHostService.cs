using CamplusBetaBackend.Models;

namespace CamplusBetaBackend.Services.Interfaces {
    public interface IHostService {
        Task<Models.Host[]?> GetHostsFromDB();
        Task<Models.Host?> GetHostFromDB(Guid id);
        Task<Models.Host?> GetHostFromDB(string name);
        Task AddNewHostToDB(Models.Host host);
        Task<Models.Host?> DeleteHostFromDB(Guid id);
    }
}
