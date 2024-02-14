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

        [HttpGet("GetClub/{id}")]
        public async Task<ActionResult<Club>> GetClub(Guid id) {
            Club? club = await _clubService.GetClubFromDB(id);

            if (club == null) {
                return NotFound();
            }
            return club;
        }

        [HttpPost("AddNewClub")]
        public async Task<ActionResult> AddNewClub([FromBody] Club club) {
            try {
                club.ClubId = Guid.NewGuid();
                await _clubService.AddNewClubToDB(club);
                return Ok();
            }
            catch (Exception e) {
                return StatusCode(500, "Internal server error: " + e.Message);
            }
        }   
    }
}
