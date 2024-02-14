using System.ComponentModel.DataAnnotations;

namespace CamplusBetaBackend.Data.Entities {
    public class HostEntity {
        [Key]
        public required Guid host_id { get; set; }
        public required string host_name { get; set; }
    }
}
