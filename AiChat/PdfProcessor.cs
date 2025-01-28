// PDFProcessor.cs
using UglyToad.PdfPig;
using System.Text;
using System.Text.RegularExpressions;

public class PDFProcessor
{
    public string ProcessPDF(string filePath)
    {
        try
        {
            using var document = PdfDocument.Open(filePath);
            var text = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                text.AppendLine(CleanText(page.Text));
            }

            return text.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing PDF: {ex.Message}");
            return string.Empty;
        }
    }

    private string CleanText(string text)
    {
        // Remove headers/footers/page numbers
        text = Regex.Replace(text, @"\bPage\s\d+\b", "");
        text = Regex.Replace(text, @"\n\d+\n", "\n");

        // Normalize whitespace
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }
}