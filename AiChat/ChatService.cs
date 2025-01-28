// ChatService.cs
using System.Net.Http.Json;
using System.Text.Json;

public class ChatService
{
    private readonly HttpClient _client;
    private readonly string _model;
    private readonly JsonSerializerOptions _jsonOptions;

    public ChatService(Config config)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(config.OllamaUrl),
            Timeout = TimeSpan.FromMinutes(1)
        };
        _model = config.ModelName;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<string> GetResponseAsync(string question, List<string> context)
    {
        try
        {
            var prompt = $$"""
                Answer the question based only on this context:
                {{string.Join("\n\n", context)}}
                
                Question: {{question}}
                Answer:
                """;

            var response = await _client.PostAsJsonAsync("/api/chat", new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                stream = false
            });

            response.EnsureSuccessStatusCode();

            // Debugging: Log raw response
            var responseString = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"Chat Response: {responseString}");

            return JsonSerializer.Deserialize<ChatResponse>(responseString, _jsonOptions)?.Message.Content?.Trim()
                ?? "No response generated";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chat Error: {ex.Message}");
            return "Error generating response";
        }
    }

    private class ChatResponse
    {
        public MessageData Message { get; set; } = new();
        public class MessageData
        {
            public string Content { get; set; } = "";
        }
    }
}