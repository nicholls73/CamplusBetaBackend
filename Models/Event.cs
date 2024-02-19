namespace CamplusBetaBackend.Models {
    public class Event {
        public required Guid EventId { get; set; }
        public required string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public required string Location { get; set; }
        public required string Link { get; set; }
        public required string ImageLink { get; set; }
        public Guid? HostId { get; set; }
        public Guid? ClubId { get; set; }
        public required string Description { get; set; }
        //public required Guid? CreatedBy { get; set; }
    }
}