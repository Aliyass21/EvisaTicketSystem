using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace EVisaTicketSystem.Infrastructure.Services
{
    public class PhotoService : IPhotoService, IDisposable
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        // Use network storage path:
         //private readonly string _networkStoragePath = @"\\172.16.108.26\samba";
        // Alternatively, use the local storage path:
        private readonly string _networkStoragePath = @"C:\Uploads";
        private readonly string _attachmentFolder;

        private const int MaxImageDimension = 1920; // Maximum allowed dimension
        private const int ImageQuality = 75; // JPEG quality

        public PhotoService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            // Define a storage folder for ticket attachments under the network storage path.
            _attachmentFolder = Path.Combine(_networkStoragePath, "ticketattachments");
            Directory.CreateDirectory(_attachmentFolder);
        }

        public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file, Guid ticketId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            // Generate a unique filename for the attachment.
            string uniqueFileName = $"{ticketId}_{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(_attachmentFolder, uniqueFileName);

            // Process image files
            if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                using Image image = await Image.LoadAsync(file.OpenReadStream());
                if (image.Width > MaxImageDimension || image.Height > MaxImageDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxImageDimension, MaxImageDimension)
                    }));
                }
                var encoder = new JpegEncoder { Quality = ImageQuality };
                await image.SaveAsync(filePath, encoder);
            }
            else
            {
                // For non-image files, simply copy the file.
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            // Create a relative file path for client consumption.
            // This assumes that the network storage is accessible via a relative URL,
            // otherwise adjust accordingly.
            string relativePath = Path.Combine("ticketattachments", uniqueFileName).Replace("\\", "/");
            return new PhotoUploadResult { FilePath = "/" + relativePath };
        }

        public async Task<bool> DeletePhotoAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.");

            // Build the full path using the network storage path.
            string trimmedPath = filePath.TrimStart('/');
            string fullPath = Path.Combine(_networkStoragePath, trimmedPath);

            if (!File.Exists(fullPath))
                return false;

            try
            {
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                // e.g., _logger.LogError(ex, "Failed to delete file: {FilePath}", fullPath);
                return false;
            }
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
