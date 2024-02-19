using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Data;
using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CamplusBetaBackend.Services.Interfaces;
using System.Runtime.CompilerServices;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using System.Net;
using CamplusBetaBackend.Services.Implentations;

namespace CamplusBetaBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller {
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
        public async Task<ActionResult<Event[]>> GetEvents() {
            Event[]? events = await _eventService.GetEventsFromDB();

            if (events == null) {
                return NotFound();
            }
            return Ok(events);
        }

        [HttpPost("AddNewEvent")]
        public async Task<ActionResult> AddNewEvent([FromBody] Event @event) {
            try {
                @event.EventId = Guid.NewGuid();
                await _eventService.AddNewEventToDB(@event);
                return Ok();
            } catch (Exception e) {
                return StatusCode(500, "Internal server error: " + e.Message);
            }
        }

        [HttpDelete("DeleteEvent")]
        public async Task<ActionResult> DeleteEvent(Guid id) {
            Event? response = await _eventService.DeleteEventFromDB(id);

            if (response == null) {
                return NotFound("Event not found.");
            }
            return Ok(response);
        }

        [HttpPost("UploadEventImage")]
        public async Task<ActionResult> UploadEventImage(IFormFile image) {
            if (image == null || image.Length == 0) {
                return StatusCode(500, "Internal server error: Image file is empty");
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
        public async Task<ActionResult> DeleteEventImage(string imageLink) {
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
    }
}
