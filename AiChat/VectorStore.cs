namespace AiChat
{
    public class VectorStore
    {
        public List<KnowledgeEntry> Entries { get; } = new();

        public void AddEntry(string text, float[] embedding)
        {
            Entries.Add(new KnowledgeEntry(text, embedding));
        }

        public List<string> FindSimilar(float[] queryEmbedding, int topK = 3)
        {
            return Entries
                .OrderBy(e => CosineSimilarity(e.Embedding, queryEmbedding))
                .Take(topK)
                .Select(e => e.Text)
                .ToList();
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            var dot = a.Zip(b, (x, y) => x * y).Sum();
            var magA = Math.Sqrt(a.Sum(x => x * x));
            var magB = Math.Sqrt(b.Sum(x => x * x));
            return (float)(dot / (magA * magB));
        }

        public record KnowledgeEntry(string Text, float[] Embedding);

    }
}
