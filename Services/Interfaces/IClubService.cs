using CamplusBetaBackend.Models;

namespace CamplusBetaBackend.Services.Interfaces
{
    public interface IClubService
    {
        Task<Club?> GetClubFromDB(Guid id);
        Task AddNewClubToDB(Club club);
    }
}
