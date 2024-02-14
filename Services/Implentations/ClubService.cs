using CamplusBetaBackend.Data;
using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;

namespace CamplusBetaBackend.Services.Implentations
{
    public class ClubService : IClubService {
        private readonly ApplicationDbContext _context;

        public ClubService(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<Club?> GetClubFromDB(Guid id) {
            ClubEntity? clubEntity = await _context.clubs.FindAsync(id);
            if (clubEntity == null) {
                return null;
            }
            return EntityToClub(clubEntity);
        }

        public async Task AddNewClubToDB(Club club) {
            _context.clubs.Add(ClubToEntity(club));
            await _context.SaveChangesAsync();
        }

        private static Club EntityToClub(ClubEntity clubEntity) {
            return new Club {
                ClubId = clubEntity.club_id,
                Name = clubEntity.club_name
            };
        }

        private static ClubEntity ClubToEntity(Club club) {
            return new ClubEntity {
                club_id = club.ClubId,
                club_name = club.Name
            };
        }
    }
}
