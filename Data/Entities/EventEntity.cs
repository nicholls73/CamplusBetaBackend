using System.ComponentModel.DataAnnotations;

namespace CamplusBetaBackend.Data.Entities {
    public class EventEntity {
        [Key]
        public Guid event_id { get; set; }
        public required string title { get; set; }
        public required DateTime start_date_time { get; set; }
        public DateTime? end_date_time { get; set; }
        public required string event_location { get; set; }
        public required string event_link { get; set; }
        public required string image_link { get; set; }
        public Guid? host_id { get; set; }
        public Guid? club_id { get; set; }
        public required string description { get; set; }
        //public required Guid created_by { get; set; }
    }
}
