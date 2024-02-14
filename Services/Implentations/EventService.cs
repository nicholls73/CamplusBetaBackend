using CamplusBetaBackend.Data;
using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CamplusBetaBackend.Services.Implentations {
    public class EventService : IEventService {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<Event[]?> GetEventsFromDB() {
            EventEntity[]? eventEntitys = await _context.events.ToArrayAsync();

            if (eventEntitys == null) {
                return null;
            }
            return eventEntitys.Select(eventEntity => EntityToEvent(eventEntity)).ToArray();
        }

        public async Task AddNewEventToDB(Event @event) {
            _context.events.Add(EventToEntity(@event));
            await _context.SaveChangesAsync();
        }

        private static Event EntityToEvent(EventEntity eventEntity) {
            return new Event {
                EventId = eventEntity.event_id,
                Title = eventEntity.title,
                StartDateTime = eventEntity.start_date_time,
                EndDateTime = eventEntity.end_date_time,
                Location = eventEntity.event_location,
                Link = eventEntity.event_link,
                ImageLink = eventEntity.image_link,
                HostId = eventEntity.host_id,
                ClubId = eventEntity.club_id,
                Description = eventEntity.description,
                CreatedBy = eventEntity.created_by
            };
        }

        private static EventEntity EventToEntity(Event event_) {
            return new EventEntity {
                event_id = event_.EventId,
                title = event_.Title,
                start_date_time = event_.StartDateTime,
                end_date_time = event_.EndDateTime,
                event_location = event_.Location,
                event_link = event_.Link,
                image_link = event_.ImageLink,
                host_id = event_.HostId,
                club_id = event_.ClubId,
                description = event_.Description,
                created_by = event_.CreatedBy
            };
        }
    }
}
