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
            return events;
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

        [HttpPost("UploadEventImage")]
        public async Task<ActionResult> UploadEventImage(IFormFile image) {
            if (image == null || image.Length == 0) {
                return StatusCode(500, "Internal server error: Image file is empty");
            }

            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            using (var stream = new MemoryStream()) {
                await image.CopyToAsync(stream);

                PutObjectRequest putObjectRequest = new PutObjectRequest {
                    BucketName = BucketName,
                    Key = imageName,
                    InputStream = stream,
                    ContentType = image.ContentType,
                    //CannedACL = S3CannedACL.PublicRead
                };

                var response = await _s3Client.PutObjectAsync(putObjectRequest);

                if (response.HttpStatusCode == HttpStatusCode.OK) {
                    var imageUrl = $"https://{BucketName}.s3.amazonaws.com/{imageName}";
                    return Ok(imageUrl);
                }
                else {
                    return BadRequest("Internal server error: Error uploading image");
                }
            }
        }
    }
}
