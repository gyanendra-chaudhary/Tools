using Microsoft.AspNetCore.Mvc;
using TextExtractor.Helpers;

namespace TextExtractor.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TextExtractorController : ControllerBase
    {
        private readonly ITextExtractorHelper _textExtractorHelper;
        private readonly string[] imgType = new string[]
        {
            "jpeg", "jpg", "png", "gif", "bmp", "tiff", "webp", "heic", // Raster
            "svg", "ai", "eps", // Vector
            "obj", "stl", // 3D
            "raw", "cr2", "nef", "psd" // Specialized
        };
        public TextExtractorController(ITextExtractorHelper textExtractorHelper)
        {
            _textExtractorHelper = textExtractorHelper;
        }
        [HttpGet("extracted-files")]
        public IActionResult GetFiles()
        {

            return Ok();
        }


        [HttpPost("extract-text")]
        public async Task<IActionResult> ExtractText([FromForm] IFormFile file)
        {
            try
            {
                var fileUrl = await FileHelper.UploadFile(file, "Files");
                var fileExtension = Path.GetExtension(file.FileName)?.ToLower().TrimStart('.');
                if (imgType.Contains(fileExtension))
                {
                    var textFileUrl = _textExtractorHelper.ExtractTextFromImageUsingTesseract(fileUrl);
                }
                else if (fileExtension == "pdf")
                {
                    var textFileUrl = _textExtractorHelper.ExtractPdfTextToFile(fileUrl);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Ok();
        }
    }
}
