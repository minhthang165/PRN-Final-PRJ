using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryController> _logger;

        public CloudinaryController(Cloudinary cloudinary, ILogger<CloudinaryController> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        [HttpPost("upload/uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                _logger.LogInformation($"Uploading file: {file.FileName}, Size: {file.Length} bytes");

                using var stream = file.OpenReadStream();
                
                // Xác ??nh lo?i file ?? dùng upload params phù h?p
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                var isRawFile = fileExtension != ".jpg" && fileExtension != ".jpeg" && 
                                fileExtension != ".png" && fileExtension != ".gif" && 
                                fileExtension != ".bmp" && fileExtension != ".webp";

                UploadResult uploadResult;

                if (isRawFile)
                {
                    // Upload file d?ng raw (PDF, DOC, etc.)
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "task_attachments",
                        UseFilename = true,
                        UniqueFilename = true,
                        Overwrite = false,
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                else
                {
                    // Upload file hình ?nh
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "task_attachments",
                        UseFilename = true,
                        UniqueFilename = true,
                        Overwrite = false,
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

                if (uploadResult.Error != null)
                {
                    _logger.LogError($"Cloudinary upload error: {uploadResult.Error.Message}");
                    return StatusCode(500, new { message = "Upload error: " + uploadResult.Error.Message });
                }

                if (uploadResult.SecureUrl == null)
                {
                    _logger.LogError("Upload succeeded but SecureUrl is null");
                    return StatusCode(500, new { message = "Upload succeeded but no URL returned" });
                }

                _logger.LogInformation($"File uploaded successfully. URL: {uploadResult.SecureUrl}");

                var response = new
                {
                    secure_url = uploadResult.SecureUrl.AbsoluteUri,
                    url = uploadResult.Url?.AbsoluteUri,
                    public_id = uploadResult.PublicId,
                    format = uploadResult.Format
                };

                _logger.LogInformation($"Returning response: {System.Text.Json.JsonSerializer.Serialize(response)}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during file upload: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Internal server error: " + ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "avatars",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return StatusCode(500, new { message = "Upload error: " + uploadResult.Error.Message });
                }

                return Ok(new
                {
                    url = uploadResult.SecureUrl.AbsoluteUri,
                    public_id = uploadResult.PublicId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during avatar upload: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error: " + ex.Message });
            }
        }
    }
}
