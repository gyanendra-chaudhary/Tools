public class TextSplitter
{
    private readonly int _chunkSize;
    private readonly int _overlap;

    public TextSplitter(int chunkSize = 512, int overlap = 128)
    {
        if (overlap >= chunkSize)
            throw new ArgumentException("Overlap must be smaller than chunk size");

        _chunkSize = chunkSize;
        _overlap = overlap;
    }

    public List<string> Split(string text)
    {
        var chunks = new List<string>();
        int position = 0;
        int previousPosition = -1;

        while (position < text.Length)
        {
            var end = Math.Min(position + _chunkSize, text.Length);

            // Add the current chunk
            var chunk = text.Substring(position, end - position).Trim();
            if (chunk.Length > 10)
            {
                chunks.Add(chunk);
            }

            // Break if we've reached the end of the text
            if (end == text.Length)
                break;

            // Calculate new position with overlap
            var newPosition = end - _overlap;

            // Prevent infinite loop by ensuring forward progress
            if (newPosition <= position)
            {
                throw new InvalidOperationException(
                    $"No progress made - chunk: {_chunkSize}, overlap: {_overlap}. " +
                    $"Position stuck at {position}"
                );
            }

            // Update tracking variables
            previousPosition = position;
            position = newPosition;
        }

        return chunks;
    }
}