using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Mvc;
using CamplusBetaBackend.Services.Interfaces;
using CamplusBetaBackend.Services.Implentations;

namespace CamplusBetaBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class HostController : Controller {
        private readonly ILogger<HostController> _logger;
        private readonly IHostService _hostService;

        public HostController(ILogger<HostController> logger, IHostService hostService) {
            _logger = logger;
            _hostService = hostService;
        }

        [HttpGet("GetHosts")]
        public async Task<ActionResult<Models.Host[]>> GetHosts() {
            Models.Host[]? hosts = await _hostService.GetHostsFromDB();

            if (hosts == null) {
                return NotFound();
            }
            return Ok(hosts);
        }

        [HttpGet("GetHost/{id}")]
        public async Task<ActionResult<Models.Host>> GetHost(Guid id) {
            Models.Host? host = await _hostService.GetHostFromDB(id);

            if (host == null) {
                return NotFound();
            }
            return Ok(host);
        }

        [HttpPost("AddNewHost")]
        public async Task<ActionResult> AddNewHost([FromBody] Models.Host host) {
            try {
                host.HostId = Guid.NewGuid();
                await _hostService.AddNewHostToDB(host);
                return Ok();
            }
            catch (Exception e) {
                return StatusCode(500, "Internal server error: " + e.Message);
            }
        }

        [HttpDelete("DeleteHost")]
        public async Task<ActionResult> DeleteHost(Guid id) {
            Models.Host? response = await _hostService.DeleteHostFromDB(id);

            if (response == null) {
                return NotFound("Host not found.");
            }
            return Ok(response);
        }
    }
}
