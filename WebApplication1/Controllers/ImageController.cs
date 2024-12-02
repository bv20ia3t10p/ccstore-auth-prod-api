using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CcStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _bucketName = "ccstorecomments"; // Your Google Cloud Storage bucket name
        private StorageClient storageClient;
        private readonly string _serviceUrl;

        public ImageController(IConfiguration configuration)
        {
            // Path to your service account JSON file
            var credential = GoogleCredential.FromJson(configuration.GetValue<string>(Environment.GetEnvironmentVariable("GOOGLECRED")));
            _serviceUrl = Environment.GetEnvironmentVariable("SERVICEURL");
            storageClient = StorageClient.Create(credential);
        }


        // POST: api/image/upload
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Get the username from the authenticated user's claims
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Username not found in the authentication token.");
            }

            // Generate a unique file name by prepending the username
            var fileName = $"{username}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            try
            {
                // Upload file to Google Cloud Storage
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Upload to the cloud
                    await storageClient.UploadObjectAsync(
                        _bucketName,
                        fileName,
                        file.ContentType,
                        memoryStream
                    );

                    // Generate the image URL
                    return Ok(new { ImageUrl = _serviceUrl + "/api/Image/" + fileName });
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., network issues, Google Cloud issues)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {

            var memoryStream = new MemoryStream();

            try
            {
                // Download the file from Google Cloud Storage
                await storageClient.DownloadObjectAsync(_bucketName, fileName, memoryStream);
                memoryStream.Position = 0;

                // Return the image in the response
                return File(memoryStream, "image/jpeg"); // Adjust MIME type if needed (e.g., png, gif, etc.)
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "Image not found.", Error = ex.Message });
            }
        }
    }
}   
