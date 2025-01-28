// Program.cs (updated)
using System.Diagnostics;
using System.Text.Json;

// Configuration
const string PdfDirectory = "pdf_docs";
const string CollectionName = "pdf_chunks";
const int EmbeddingDimensions = 3072;  // Matches OpenAI/text-embedding-3-large
const int ChunkSize = 200;            // 60% smaller than document models
const int BatchSize = 12;             // Fits most 16GB GPUs
const int Overlap = 80;               // 40% overlap for CV continuity

var config = new Config(
    OllamaUrl: "http://localhost:11434",
    QdrantUrl: "http://localhost:6333",
    ModelName: "phi3"
);

var processor = new PDFProcessor();
var splitter = new TextSplitter(ChunkSize, Overlap);
var embedder = new EmbeddingService(config);
var vectorStore = new QdrantVectorStore(config, EmbeddingDimensions);
var chatService = new ChatService(config);

// Initialize Vector Database with error handling
try
{
    Console.WriteLine("Initializing vector database...");
    await InitializeVectorDatabase(vectorStore);
    Console.WriteLine("Vector database ready");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to initialize vector database: {ex.Message}");
    return;
}

// Process PDFs
var pdfFiles = Directory.GetFiles(PdfDirectory, "*.pdf");
foreach (var pdfFile in pdfFiles)
{
    Console.WriteLine($"Processing {Path.GetFileName(pdfFile)}...");
    var text = processor.ProcessPDF(pdfFile);
    var chunks = splitter.Split(text);

    foreach (var batch in chunks.Chunk(BatchSize))
    {
        var embeddings = await embedder.GetBatchEmbeddingsAsync(batch.ToList());
        await vectorStore.BulkUpsertAsync(batch.Zip(embeddings));
    }
}

// Chat loop
Console.WriteLine("Chat with your documents (type 'exit' to quit):");
while (true)
{
    Console.Write("\nQuestion: ");
    var question = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(question) || question.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    try
    {
        var questionEmbedding = await embedder.GetEmbeddingAsync(question);
        var context = await vectorStore.SearchAsync(questionEmbedding);
        var answer = await chatService.GetResponseAsync(question, context);

        Console.WriteLine($"\nAnswer: {answer}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task InitializeVectorDatabase(QdrantVectorStore store)
{
    // Check collection existence
    var exists = await store.CollectionExistsAsync();

    if (exists)
    {
        Console.WriteLine("Found existing collection");
        // Optional: Uncomment to reset collection for fresh starts
        // await store.DeleteCollectionAsync();
        // await store.CreateCollectionAsync();
    }
    else
    {
        Console.WriteLine("Creating new collection");
        await store.CreateCollectionAsync();
    }

    // Verify collection health
    var health = await store.CheckCollectionHealthAsync();
    if (!health.IsHealthy)
    {
        throw new Exception($"Collection health check failed: {health.Message}");
    }
}