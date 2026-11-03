using RoyalVilla_Api.Services.IServices;

namespace RoyalVilla_Api.Services
{
    public class ImageService : IImageService
    {
        private const long MaxFileSize = 5*1024*1024;
        private readonly string[] AllowedExtention = { ".jpg", ".png", ".jpeg" };
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment=webHostEnvironment;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return false;
                }

                var filename=Path.GetFileName(imageUrl);
                var filePeth = Path.Combine(_webHostEnvironment.WebRootPath, "images", "villas", filename);
                if (File.Exists(filePeth))
                { 
                await Task.Run(() => { File.Delete(filePeth); });
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (!ValidateImage(file))
                {
                    throw new InvalidOperationException("Invalid image file");
                }

               
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images","villas");
                if (!Directory.Exists(uploadFolder))
                { 
                Directory.CreateDirectory(uploadFolder);
                }
                var fileExtention = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtention}";
                var filePath= Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                };
               
                var imageUrl = $"/images/villas/{uniqueFileName}";
                return imageUrl;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ValidateImage(IFormFile file)
        {
            if (file==null||file.Length==0)
            {
                return false;

            }
            if (file.Length>MaxFileSize)
            {
                return false;
            }
            var fileExtention=Path.GetExtension(file.FileName);

            if (!AllowedExtention.Contains(fileExtention))
            { 
            return false ;

            }
            return true;
        }
    }
}
