using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CamplusBetaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : Controller {
        private readonly ILogger<ClubController> _logger;
        private readonly IClubService _clubService;

        public ClubController(ILogger<ClubController> logger, IClubService clubService) {
            _logger = logger;
            _clubService = clubService;
        }

        [HttpGet("GetClubs")]
        public async Task<ActionResult<Club[]>> GetClubs() {
            Club[]? clubs = await _clubService.GetClubsFromDB();

            if (clubs == null) {
                return NotFound();
            }
            return Ok(clubs);
        }

        [HttpGet("GetClub/{id}")]
        public async Task<ActionResult<Club>> GetClub(Guid id) {
            Club? club = await _clubService.GetClubFromDB(id);

            if (club == null) {
                return NotFound();
            }
            return Ok(club);
        }

        [HttpPost("AddNewClub")]
        public async Task<ActionResult> AddNewClub([FromBody] Club club) {
            Club? existingClub = await _clubService.GetClubFromDB(club.Name);

            if (existingClub == null) {
                club.ClubId = Guid.NewGuid();
                await _clubService.AddNewClubToDB(club);
                return Ok();
            }
            else {
                return NotFound($"Club {club.Name} already exists.");
            }
        }

        [HttpDelete("DeleteClub")]
        public async Task<ActionResult> DeleteClub(Guid id) {
            Club? response = await _clubService.DeleteClubFromDB(id);

            if (response == null) {
                return NotFound("Club not found.");
            }
            return Ok(response);
        }
    }
}
