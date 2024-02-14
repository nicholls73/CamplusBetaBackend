using CamplusBetaBackend.Data;
using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Services.Interfaces;

namespace CamplusBetaBackend.Services.Implentations {
    public class HostService : IHostService {
        private readonly ApplicationDbContext _context;

        public HostService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Models.Host?> GetHostFromDB(Guid id) {
            HostEntity? hostEntity = await _context.hosts.FindAsync(id);
            if (hostEntity == null) {
                return null;
            }
            return EntityToHost(hostEntity);
        }

        public async Task AddNewHostToDB(Models.Host host) {
            _context.hosts.Add(HostToEntity(host));
            await _context.SaveChangesAsync();
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
