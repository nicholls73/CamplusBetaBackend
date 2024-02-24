using CamplusBetaBackend.Services.Interfaces;
using CamplusBetaBackend.DTOs;
using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace CamplusBetaBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class HostController : Controller {
        private readonly string? ADMIN_API_KEY = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        private readonly string? RO_API_KEY = Environment.GetEnvironmentVariable("RO_API_KEY");
        private readonly ILogger<HostController> _logger;
        private readonly IHostService _hostService;

        public HostController(ILogger<HostController> logger, IHostService hostService) {
            _logger = logger;
            _hostService = hostService;
        }

        [HttpGet("GetHosts")]
        public async Task<ActionResult<Models.Host[]>> GetHosts([FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, false)) {
                return Unauthorized("Invalid API Key");
            }

            Models.Host[]? hosts = await _hostService.GetHostsFromDB();

            if (hosts == null) {
                return BadRequest("There is no hosts.");
            }
            return Ok(hosts);
        }

        [HttpGet("GetHost/{id}")]
        public async Task<ActionResult<Models.Host>> GetHost(Guid id, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, false)) {
                return Unauthorized("Invalid API Key");
            }

            Models.Host? host = await _hostService.GetHostFromDB(id);

            if (host == null) {
                return BadRequest("Could not find host.");
            }
            return Ok(host);
        }

        [HttpPost("AddNewHost")]
        public async Task<ActionResult> AddNewHost([FromBody] HostDTO hostDTO, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Models.Host? existingHost = await _hostService.GetHostFromDB(hostDTO.Name);
        
            if (existingHost == null) {
                Models.Host newHost = new Models.Host {
                    HostId = Guid.NewGuid(),
                    Name = hostDTO.Name,
                };
                await _hostService.AddNewHostToDB(newHost);
                return Ok(newHost);
            } else {
                return BadRequest($"Host {hostDTO.Name} already exists.");
            }
        }

        [HttpDelete("DeleteHost")]
        public async Task<ActionResult> DeleteHost(Guid id, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Models.Host? response = await _hostService.DeleteHostFromDB(id);

            if (response == null) {
                return BadRequest("Host not found.");
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
