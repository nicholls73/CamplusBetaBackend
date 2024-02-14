using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace CamplusBetaBackend.Services.Interfaces {
    public interface IEventService {
        Task<Event[]?> GetEventsFromDB();
        Task AddNewEventToDB(Event event_);
    }
}
