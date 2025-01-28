// QdrantVectorStore.cs
using System.Net.Http.Json;

public class QdrantVectorStore
{
    private readonly HttpClient _client;
    private readonly string _collection;
    private readonly int _dimensions;

    public QdrantVectorStore(Config config, int dimensions)
    {
        _client = new HttpClient { BaseAddress = new Uri(config.QdrantUrl) };
        _collection = config.CollectionName;
        _dimensions = dimensions;
    }

    public async Task InitializeCollectionAsync()
    {
        var response = await _client.PutAsJsonAsync($"/collections/{_collection}", new
        {
            vectors = new { size = _dimensions, distance = "Cosine" }
        });

        response.EnsureSuccessStatusCode();

    }

    public async Task BulkUpsertAsync(IEnumerable<(string Text, float[] Embedding)> items)
    {
        var points = items.Select(x => new
        {
            id = Guid.NewGuid().ToString(),
            vector = x.Embedding.Select(e => (double)e).ToArray(),
            payload = new { text = x.Text }
        }).ToArray();

        var response = await _client.PutAsJsonAsync(
            $"/collections/{_collection}/points?wait=true",
            new { points }
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<string>> SearchAsync(float[] queryEmbedding, int topK = 3)
    {
        var response = await _client.PostAsJsonAsync(
            $"/collections/{_collection}/points/search",
            new
            {
                vector = queryEmbedding.Select(e => (double)e).ToArray(),
                top = topK,
                with_payload = true
            });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QdrantSearchResponse>();
        return result?.Result.Select(r => r.Payload.text.ToString() ?? "").ToList() ?? new();
    }

    private class QdrantSearchResponse
    {
        public List<SearchResult> Result { get; set; } = new();
        public class SearchResult
        {
            public PayloadData Payload { get; set; } = new();
        }
        public class PayloadData
        {
            public string text { get; set; } = "";
        }
    }
    // QdrantVectorStore.cs additions
    public async Task<bool> CollectionExistsAsync()
    {
        var response = await _client.GetAsync($"/collections/{_collection}");
        return response.IsSuccessStatusCode;
    }

    public async Task CreateCollectionAsync()
    {
        var response = await _client.PutAsJsonAsync($"/collections/{_collection}", new
        {
            vectors = new
            {
                size = _dimensions,
                distance = "Cosine"
            }
        });
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCollectionAsync()
    {
        var response = await _client.DeleteAsync($"/collections/{_collection}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<(bool IsHealthy, string Message)> CheckCollectionHealthAsync()
    {
        try
        {
            var response = await _client.GetAsync($"/collections/{_collection}");
            return (response.IsSuccessStatusCode, "Collection is healthy");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}