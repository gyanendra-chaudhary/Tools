// EmbeddingService.cs
using System.Net.Http.Json;

public class EmbeddingService
{
    private readonly HttpClient _client;
    private readonly string _model;

    public EmbeddingService(Config config)
    {
        _client = new HttpClient { BaseAddress = new Uri(config.OllamaUrl) };
        _model = config.ModelName;
    }

    //public async Task<float[]> GetEmbeddingAsync(string text)
    //{
    //    var response = await _client.PostAsJsonAsync("/api/embeddings", new
    //    {
    //        model = _model,
    //        prompt = text
    //    });

    //    response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadFromJsonAsync<float[]>() ?? Array.Empty<float>();
    //}

    //public async Task<List<float[]>> GetBatchEmbeddingsAsync(List<string> chunks)
    //{
    //    var response = await _client.PostAsJsonAsync("/api/embeddings", new
    //    {
    //        model = _model,
    //        prompts = chunks
    //    });

    //    response.EnsureSuccessStatusCode();
    //    return await response.Content.ReadFromJsonAsync<List<float[]>>() ?? new();
    //}
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/api/embeddings", new
            {
                model = _model,
                prompt = text
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            return result?.Embedding ?? throw new Exception("Failed to get embedding");
        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<List<float[]>> GetBatchEmbeddingsAsync(List<string> chunks)
    {
        var embeddings = new List<float[]>();

        foreach (var chunk in chunks)
        {
            embeddings.Add(await GetEmbeddingAsync(chunk));
        }

        return embeddings;
    }

    private class EmbeddingResponse
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}