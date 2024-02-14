using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CamplusBetaBackend.Data.Entities {
    public class ClubEntity {
        [Key]
        public required Guid club_id { get; set; }
        public required string club_name { get; set; }
    }
}
