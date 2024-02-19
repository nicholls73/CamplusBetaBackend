using CamplusBetaBackend.Models;

namespace CamplusBetaBackend.Services.Interfaces
{
    public interface IClubService
    {
        Task<Club[]?> GetClubsFromDB();
        Task<Club?> GetClubFromDB(Guid id);
        Task<Club?> GetClubFromDB(string name);
        Task AddNewClubToDB(Club club);
        Task<Club?> DeleteClubFromDB(Guid id);

    }
}
