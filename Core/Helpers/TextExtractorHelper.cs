using IronOcr;
using Tesseract;
using UglyToad.PdfPig;

namespace TextExtractor.Helpers
{
    public class TextExtractorHelper : ITextExtractorHelper
    {
        public string ExtractTextFromImage(string filePath)
        {
            string resultUrl = string.Empty;
            var ocr = new IronTesseract();
            using (var input = new OcrInput(filePath))
            {
                var result = ocr.Read(input);
                string newFileName = filePath.Contains('.') ? filePath.Substring(0, filePath.LastIndexOf('.')) + ".txt" : filePath + ".txt";
                result.SaveAsTextFile(newFileName);
                resultUrl = newFileName;
            }
            return resultUrl;
        }
        public string ExtractTextFromImageUsingTesseract(string filePath)
        {
            string resultUrl = string.Empty;

            string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

            if (!Directory.Exists(tessDataPath))
            {
                throw new DirectoryNotFoundException("Tessdata folder not found. Make sure the tessdata files are in the correct location.");
            }

            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(filePath))
                {
                    using (var page = engine.Process(img))
                    {
                        string extractedText = page.GetText();

                        string newFileName = filePath.Contains('.')
                            ? filePath.Substring(0, filePath.LastIndexOf('.')) + ".txt"
                            : filePath + ".txt";

                        File.WriteAllText(newFileName, extractedText);
                        resultUrl = newFileName;
                    }
                }
            }

            return resultUrl;
        }
        public string ExtractPdfTextToFile(string pdfFilePath)
        {
            try
            {
                string outputFilePath = pdfFilePath.Contains('.')
                            ? pdfFilePath.Substring(0, pdfFilePath.LastIndexOf('.')) + ".txt"
                            : pdfFilePath + ".txt";

                using (var pdfDocument = PdfDocument.Open(pdfFilePath))
                {
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
                    {
                        foreach (var page in pdfDocument.GetPages())
                        {
                            string pageText = page.Text;
                            writer.WriteLine(pageText);
                        }
                    }
                }
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
