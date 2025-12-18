using System.Net.Http.Json;
using System.Text.Json;
using DigitalTwin.ConsoleApp.Models;

namespace DigitalTwin.ConsoleApp.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;

    public OllamaService(HttpClient httpClient, OllamaSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    public async Task<string> GenerateAsync(string prompt, string? model = null, CancellationToken cancellationToken = default)
    {
        model ??= _settings.DefaultModel;

        var request = new
        {
            model = model,
            prompt = prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken: cancellationToken);
        return result?.Response ?? string.Empty;
    }

    public async Task<OllamaGenerateDetailedResponse> GenerateDetailedAsync(string prompt, string? model = null, CancellationToken cancellationToken = default)
    {
        model ??= _settings.DefaultModel;

        var request = new
        {
            model = model,
            prompt = prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaGenerateDetailedResponse>(cancellationToken: cancellationToken);
        
        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize Ollama response");
        }

        return result;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>(cancellationToken: cancellationToken);
            return result?.Models?.Select(m => m.Name).ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, string? model = null, CancellationToken cancellationToken = default)
    {
        var embeddingModels = new[] { "nomic-embed-text", "mxbai-embed-large", "all-minilm" };
        model ??= embeddingModels[0];

        var request = new
        {
            model = model,
            prompt = text
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/embeddings", request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Ollama embeddings API error with model '{model}': {response.StatusCode} - {errorContent}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound && model == embeddingModels[0])
                {
                    Console.WriteLine($"Model '{model}' not found. Trying alternative models...");
                    
                    foreach (var altModel in embeddingModels.Skip(1))
                    {
                        Console.WriteLine($"Trying model: {altModel}");
                        var altRequest = new { model = altModel, prompt = text };
                        var altResponse = await _httpClient.PostAsJsonAsync("/api/embeddings", altRequest, cancellationToken);
                        
                        if (altResponse.IsSuccessStatusCode)
                        {
                            var altResult = await altResponse.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
                            if (altResult?.Embedding != null && altResult.Embedding.Length > 0)
                            {
                                Console.WriteLine($"✓ Successfully generated embedding using model '{altModel}' ({altResult.Embedding.Length} dimensions)");
                                return altResult.Embedding;
                            }
                        }
                        else
                        {
                            var altError = await altResponse.Content.ReadAsStringAsync(cancellationToken);
                            Console.WriteLine($"  Model '{altModel}' also failed: {altError}");
                        }
                    }
                    
                    Console.WriteLine($"\n⚠ No embedding models available. Please install one of these models:");
                    Console.WriteLine($"   ollama pull {embeddingModels[0]}");
                    Console.WriteLine($"   ollama pull {embeddingModels[1]}");
                    Console.WriteLine($"   ollama pull {embeddingModels[2]}");
                }
                
                return Array.Empty<float>();
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Ollama embeddings response: {jsonContent.Substring(0, Math.Min(200, jsonContent.Length))}...");
            
            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
            
            if (result?.Embedding == null || result.Embedding.Length == 0)
            {
                Console.WriteLine("Warning: Ollama returned empty embedding array");
                return Array.Empty<float>();
            }
            
            Console.WriteLine($"✓ Generated embedding with {result.Embedding.Length} dimensions using model '{model}'");
            return result.Embedding;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating embedding: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
            }
            return Array.Empty<float>();
        }
    }

    private class OllamaEmbeddingResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
    }

    private class OllamaGenerateResponse
    {
        public string Response { get; set; } = string.Empty;
    }

    public class OllamaGenerateDetailedResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("done")]
        public bool Done { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("context")]
        public long[]? Context { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("total_duration")]
        public long? TotalDuration { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("load_duration")]
        public long? LoadDuration { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("prompt_eval_count")]
        public int? PromptEvalCount { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("prompt_eval_duration")]
        public long? PromptEvalDuration { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("eval_count")]
        public int? EvalCount { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("eval_duration")]
        public long? EvalDuration { get; set; }
    }

    private class OllamaTagsResponse
    {
        public List<OllamaModel> Models { get; set; } = new();
    }

    private class OllamaModel
    {
        public string Name { get; set; } = string.Empty;
    }
}

