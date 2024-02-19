using CamplusBetaBackend.Data;
using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using static Amazon.S3.Util.S3EventNotification;

namespace CamplusBetaBackend.Services.Implentations
{
    public class ClubService : IClubService {
        private readonly ApplicationDbContext _context;

        public ClubService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Club[]?> GetClubsFromDB() {
            ClubEntity[]? clubEntitys = await _context.clubs.ToArrayAsync();
        
            if (clubEntitys == null) {
                return null;
            }
            return clubEntitys.Select(clubEntity => EntityToClub(clubEntity)).ToArray();
        }

        public async Task<Club?> GetClubFromDB(Guid id) {
            ClubEntity? clubEntity = await _context.clubs.FindAsync(id);
            if (clubEntity == null) {
                return null;
            }
            return EntityToClub(clubEntity);
        }

        public async Task<Club?> GetClubFromDB(string name) {
            ClubEntity? clubEntity = await _context.clubs.FirstOrDefaultAsync(c => c.club_name == name);
            if (clubEntity == null) {
                return null;
            }
            _context.Entry(clubEntity).State = EntityState.Detached;
            return EntityToClub(clubEntity);
        }

        public async Task AddNewClubToDB(Club club) {
            _context.clubs.Add(ClubToEntity(club));
            await _context.SaveChangesAsync();
        }

        public async Task<Club?> DeleteClubFromDB(Guid id) {
            ClubEntity? clubEntity = await _context.clubs.FindAsync(id);

            if (clubEntity == null) {
                return null;
            }
            _context.clubs.Remove(clubEntity);
            await _context.SaveChangesAsync();
            return (EntityToClub(clubEntity));
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
