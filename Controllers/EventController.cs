using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Mvc;
using CamplusBetaBackend.Services.Interfaces;
using Amazon.S3.Model;
using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using System.Net;
using CamplusBetaBackend.DTOs;

namespace CamplusBetaBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller {
        private readonly string? ADMIN_API_KEY = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        private readonly string? RO_API_KEY = Environment.GetEnvironmentVariable("RO_API_KEY");
        private readonly ILogger<EventController> _logger;
        private readonly IEventService _eventService;
        private readonly IAmazonS3 _s3Client;

        private const string AWSAccessKey = "AKIA2UC27X2SLL7OXSZK";
        private const string AWSSecretKey = "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";
        private const string BucketName = "camplus-beta-images-bucket";

        public EventController(ILogger<EventController> logger, IEventService eventService) {
            _logger = logger;
            _eventService = eventService;
            _s3Client = new AmazonS3Client(new BasicAWSCredentials(AWSAccessKey, AWSSecretKey), RegionEndpoint.APSoutheast2);
        }

        [HttpGet("GetEvents")]
        public async Task<ActionResult<Event[]>> GetEvents([FromHeader(Name = "X-API-Key")] string apiKey) {
        //public async Task<ActionResult<Event[]>> GetEvents() {
            if (!IsApiKeyValid(apiKey, false)) {
                return Unauthorized("Invalid API Key");
            }

            Event[]? events = await _eventService.GetEventsFromDB();

            if (events == null) {
                return BadRequest("There is no events.");
            }
            return Ok(events);
        }

        [HttpPost("AddNewEvent")]
        public async Task<ActionResult> AddNewEvent([FromBody] EventDTO eventDTO, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Event newEvent = new Event {
                EventId = Guid.NewGuid(),
                Title = eventDTO.Title,
                StartDateTime = eventDTO.StartDateTime,
                EndDateTime = eventDTO.EndDateTime,
                Location = eventDTO.Location,
                Link = eventDTO.Link,
                ImageLink = eventDTO.ImageLink,
                HostId = eventDTO.HostId,
                ClubId = eventDTO.ClubId,
                Description = eventDTO.Description,
            };
            await _eventService.AddNewEventToDB(newEvent);
            return Ok(newEvent);
        }

        [HttpDelete("DeleteEvent")]
        public async Task<ActionResult> DeleteEvent(Guid id, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Event? response = await _eventService.DeleteEventFromDB(id);

            if (response == null) {
                return BadRequest("Event not found.");
            }
            return Ok(response);
        }

        [HttpPost("UploadEventImage")]
        public async Task<ActionResult> UploadEventImage(IFormFile image, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            if (image == null || image.Length == 0) {
                return BadRequest("Internal server error: Image file is empty");
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            using (var stream = new MemoryStream()) {
                await image.CopyToAsync(stream);

                PutObjectRequest putObjectRequest = new PutObjectRequest {
                    BucketName = BucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = image.ContentType,
                };

                var response = await _s3Client.PutObjectAsync(putObjectRequest);

                if (response.HttpStatusCode == HttpStatusCode.OK) {
                    var imageUrl = $"https://{BucketName}.s3.amazonaws.com/{fileName}";
                    return Ok(imageUrl);
                }
                else {
                    return BadRequest("Internal server error: Error uploading image");
                }
            }
        }

        [HttpDelete("DeleteEventImage")]
        public async Task<ActionResult> DeleteEventImage(string imageLink, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            Uri uri = new Uri(imageLink);
            string fileName = Path.GetFileName(uri.AbsolutePath);

            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest {
                BucketName = BucketName,
                Key = fileName
            };

            try {
                var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);
                return Ok();
            } catch (AmazonS3Exception e) {
                return BadRequest($"Internal server error: {e}");
            }
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
