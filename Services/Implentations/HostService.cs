using CamplusBetaBackend.Data;
using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CamplusBetaBackend.Services.Implentations {
    public class HostService : IHostService {
        private readonly ApplicationDbContext _context;

        public HostService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Models.Host[]?> GetHostsFromDB() {
            HostEntity[]? hostEntitys = await _context.hosts.ToArrayAsync();

            if (hostEntitys == null) {
                return null;
            }
            return hostEntitys.Select(hostEntity => EntityToHost(hostEntity)).ToArray();
        }

        public async Task<Models.Host?> GetHostFromDB(Guid id) {
            HostEntity? hostEntity = await _context.hosts.FindAsync(id);
            if (hostEntity == null) {
                return null;
            }
            return EntityToHost(hostEntity);
        }

        public async Task<Models.Host?> GetHostFromDB(string name) {
            HostEntity? hostEntity = await _context.hosts.FirstOrDefaultAsync(h => h.host_name == name);
            if (hostEntity == null) {
                return null;
            }
            _context.Entry(hostEntity).State = EntityState.Detached;
            return EntityToHost(hostEntity);
        }

        public async Task AddNewHostToDB(Models.Host host) {
            _context.hosts.Add(HostToEntity(host));
            await _context.SaveChangesAsync();
        }

        public async Task<Models.Host?> DeleteHostFromDB(Guid id) {
            HostEntity? hostEntity = await _context.hosts.FindAsync(id);

            if (hostEntity == null) {
                return null;
            }
            _context.hosts.Remove(hostEntity);
            await _context.SaveChangesAsync();
            return (EntityToHost(hostEntity));
        }

        private static Models.Host EntityToHost(HostEntity hostEntity) {
            return new Models.Host {
                HostId = hostEntity.host_id,
                Name = hostEntity.host_name
            };
        }

        private static HostEntity HostToEntity(Models.Host host) {
            return new HostEntity {
                host_id = host.HostId,
                host_name = host.Name
            };
        }
    }
}
