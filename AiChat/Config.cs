// Config.cs
public record Config(
    string OllamaUrl,
    string QdrantUrl,
    string ModelName,
    string CollectionName = "pdf_chunks"
);