using CamplusBetaBackend.DTOs;
using CamplusBetaBackend.Models;
using CamplusBetaBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CamplusBetaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : Controller {
        private readonly string? ADMIN_API_KEY = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        private readonly string? RO_API_KEY = Environment.GetEnvironmentVariable("RO_API_KEY");
        private readonly ILogger<ClubController> _logger;
        private readonly IClubService _clubService;

        public ClubController(ILogger<ClubController> logger, IClubService clubService) {
            _logger = logger;
            _clubService = clubService;
        }

        [HttpGet("GetClubs")]
        public async Task<ActionResult<Club[]>> GetClubs([FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, false)) {
                return Unauthorized("Invalid API Key");
            }

            Club[]? clubs = await _clubService.GetClubsFromDB();

            if (clubs == null) {
                return BadRequest("There is no clubs.");
            }
            return Ok(clubs);
        }

        [HttpGet("GetClub/{id}")]
        public async Task<ActionResult<Club>> GetClub(Guid id, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, false)) {
                return Unauthorized("Invalid API Key");
            }

            Club? club = await _clubService.GetClubFromDB(id);

            if (club == null) {
                return BadRequest("Could not find club.");
            }
            return Ok(club);
        }

        [HttpPost("AddNewClub")]
        public async Task<ActionResult> AddNewClub([FromBody] ClubDTO clubDTO, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Club? existingClub = await _clubService.GetClubFromDB(clubDTO.Name);

            if (existingClub == null) {
                Club newClub = new Club {
                    ClubId = Guid.NewGuid(),
                    Name = clubDTO.Name,
                };
                await _clubService.AddNewClubToDB(newClub);
                return Ok(newClub);
            }
            else {
                return BadRequest($"Club {clubDTO.Name} already exists.");
            }
        }

        [HttpDelete("DeleteClub")]
        public async Task<ActionResult> DeleteClub(Guid id, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Club? response = await _clubService.DeleteClubFromDB(id);

            if (response == null) {
                return BadRequest("Club not found.");
            }
            return Ok(response);
        }

        private bool IsApiKeyValid(string apiKey, bool isAdmin) {
            if (isAdmin) {
                return apiKey == ADMIN_API_KEY;
            }
            else {
                return apiKey == ADMIN_API_KEY || apiKey == RO_API_KEY;
            }
        }
    }
}
