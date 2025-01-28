using Microsoft.AspNetCore.Http;

namespace TextExtractor.Helpers
{
    public class FileHelper
    {
        public static async Task<string> UploadFile(IFormFile file, string uploadFolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("The file is null or empty.");
            }
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadFolder);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var uniqueFileName = file.FileName + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"File upload failed: {ex.Message}", ex);
            }
        }

    }
}
