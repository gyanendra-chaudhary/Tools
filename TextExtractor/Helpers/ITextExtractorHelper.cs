namespace TextExtractor.Helpers
{
    public interface ITextExtractorHelper
    {
        string ExtractPdfTextToFile(string pdfFilePath);
        string ExtractTextFromImage(string filePath);
        string ExtractTextFromImageUsingTesseract(string filePath);
    }
}
