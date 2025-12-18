using DigitalTwin.ConsoleApp.Domain;
using DigitalTwin.ConsoleApp.Models;
using DigitalTwin.ConsoleApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace DigitalTwin.ConsoleApp.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDigitalTwinServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        // Ollama Settings
        var ollamaSettings = new OllamaSettings();
        configuration.GetSection("Ollama").Bind(ollamaSettings);
        builder.Services.AddSingleton(ollamaSettings);

        // Qdrant Settings
        var qdrantSettings = new QdrantSettings();
        configuration.GetSection("Qdrant").Bind(qdrantSettings);
        builder.Services.AddSingleton(qdrantSettings);

        // Services
        builder.Services.AddHttpClient<OllamaService>();
        builder.Services.AddSingleton<OllamaService>();
        builder.Services.AddSingleton<QdrantService>();
        builder.Services.AddSingleton<IOrderService, OrderService>();

        return builder;
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication OpenSwaggerInBrowserOnStart(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        // Open browser when server is ready
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                // Wait a bit longer to ensure server and Swagger are fully ready
                await Task.Delay(2000);
                
                // Get the actual URL the server is listening on
                var swaggerUrl = "http://localhost:5000";
                if (app.Urls.Any())
                {
                    swaggerUrl = app.Urls.First();
                }
                
                Console.WriteLine($"Attempting to open Swagger UI at: {swaggerUrl}");
                
                try
                {
                    // Try to verify the endpoint is accessible first
                    using var httpClient = new System.Net.Http.HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(2);
                    var response = await httpClient.GetAsync($"{swaggerUrl}/swagger/v1/swagger.json");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var processInfo = new System.Diagnostics.ProcessStartInfo 
                        { 
                            FileName = swaggerUrl, 
                            UseShellExecute = true 
                        };
                        System.Diagnostics.Process.Start(processInfo);
                        Console.WriteLine($"✓ Swagger UI opened successfully at: {swaggerUrl}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠ Swagger endpoint returned status {response.StatusCode}");
                        Console.WriteLine($"Please navigate manually to: {swaggerUrl}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Could not verify Swagger endpoint: {ex.Message}");
                    Console.WriteLine($"Attempting to open browser anyway at: {swaggerUrl}");
                    
                    try
                    {
                        var processInfo = new System.Diagnostics.ProcessStartInfo 
                        { 
                            FileName = swaggerUrl, 
                            UseShellExecute = true 
                        };
                        System.Diagnostics.Process.Start(processInfo);
                        Console.WriteLine($"Browser opened at: {swaggerUrl}");
                    }
                    catch (Exception browserEx)
                    {
                        Console.WriteLine($"✗ Could not open browser automatically: {browserEx.Message}");
                        Console.WriteLine($"Please navigate manually to: {swaggerUrl}");
                    }
                }
            });
        });
        
        return app;
    }
}

public static class ServiceProviderExtensions
{
    public static async Task RunConsoleLogicAsync(this IServiceProvider serviceProvider)
    {
        await Task.Delay(1000); // Give web server time to start
        
        Console.WriteLine("=== Digital Twin Console App ===\n");

        Console.WriteLine("Testing Ollama connection...");
        var ollamaService = serviceProvider.GetRequiredService<OllamaService>();
        var ollamaAvailable = await ollamaService.IsAvailableAsync();
        if (ollamaAvailable)
        {
            Console.WriteLine("✓ Ollama is available!");
            var models = await ollamaService.ListModelsAsync();
            Console.WriteLine($"  Available models: {string.Join(", ", models)}");
            
            Console.WriteLine("\nTesting text generation...");
            try
            {
                var response = await ollamaService.GenerateAsync("Say hello in Italian");
                Console.WriteLine($"  Response: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("✗ Ollama is not available. Make sure it's running on Docker Desktop.");
        }

        Console.WriteLine();

        var qdrantSettings = serviceProvider.GetRequiredService<QdrantSettings>();
        Console.WriteLine("Testing Qdrant connection...");
        Console.WriteLine($"  Connecting to {qdrantSettings.Host}:{qdrantSettings.GrpcPort} (gRPC)");
        var qdrantService = serviceProvider.GetRequiredService<QdrantService>();
        var qdrantAvailable = await qdrantService.IsAvailableAsync();
        if (qdrantAvailable)
        {
            Console.WriteLine("✓ Qdrant is available!");
            var collections = await qdrantService.ListCollectionsAsync();
            Console.WriteLine($"  Available collections: {string.Join(", ", collections)}");
            
            if (collections.Count == 0)
            {
                Console.WriteLine("  No collections found. You can create one using the QdrantService.");
            }
        }
        else
        {
            Console.WriteLine("✗ Qdrant is not available. Make sure it's running on Docker Desktop.");
            Console.WriteLine($"  Expected connection: {qdrantSettings.Host}:{qdrantSettings.GrpcPort} (gRPC)");
            Console.WriteLine("  Tip: Verify that Qdrant container is running and port 6333 is exposed.");
        }

        Console.WriteLine("\n=== Setup Complete ===");
        Console.WriteLine("You can now use OllamaService and QdrantService in your application.");
        Console.WriteLine("Web API is available at http://localhost:5000");
        Console.WriteLine("Try: POST /api/prompt with { \"prompt\": \"your question\", \"model\": \"optional-model\" }");

        Console.WriteLine("\n=== Creating Orders and Events ===");

        var orderService = serviceProvider.GetRequiredService<IOrderService>();
        const string collectionName = "orders";
        const uint vectorSize = 768;

        if (qdrantAvailable)
        {
            if (!await qdrantService.CollectionExistsAsync(collectionName))
            {
                Console.WriteLine($"Creating collection '{collectionName}' with vector size {vectorSize}...");
                await qdrantService.CreateCollectionAsync(collectionName, vectorSize);
                Console.WriteLine($"✓ Collection '{collectionName}' created.");
            }
            else
            {
                Console.WriteLine($"✓ Collection '{collectionName}' already exists.");
                Console.WriteLine($"Clearing all existing items from collection '{collectionName}'...");
                try
                {
                    await qdrantService.DeleteAllPointsAsync(collectionName);
                    Console.WriteLine($"✓ All items deleted from collection '{collectionName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error deleting items: {ex.Message}");
                }
            }
        }

        OrderProcessingHelper.ProcessOrders(orderService);

        if (qdrantAvailable && ollamaAvailable)
        {
            Console.WriteLine("\n=== Natural Language Query ===");
            Console.WriteLine("You can now query the order data using natural language.");
            Console.WriteLine("Example queries:");
            Console.WriteLine("  - 'Quali ordini sono stati spediti?'");
            Console.WriteLine("  - 'Mostrami tutti gli ordini confermati'");
            Console.WriteLine("  - 'Quali ordini sono stati consegnati?'");
            Console.WriteLine("  - 'Dimmi informazioni sugli ordini cancellati'");
            Console.WriteLine();
            
            var availableModels = await ollamaService.ListModelsAsync();
            Console.WriteLine($"Available models: {string.Join(", ", availableModels)}");
            
            var embeddingModels = new[] { "nomic-embed-text", "mxbai-embed-large", "all-minilm" };
            var hasEmbeddingModel = embeddingModels.Any(m => availableModels.Contains(m));
            
            if (!hasEmbeddingModel)
            {
                Console.WriteLine("\n⚠ Warning: No embedding models found!");
                Console.WriteLine("Please install one of these models:");
                Console.WriteLine("  ollama pull nomic-embed-text");
                Console.WriteLine("  ollama pull mxbai-embed-large");
                Console.WriteLine("  ollama pull all-minilm");
                Console.WriteLine("\nThe system will try to use available models, but may fail.");
            }
            
            var query = "Mi dai tutte le informazioni che sull'ordine ORD-2025-000004?";
            Console.WriteLine($"Query: {query}");
            Console.WriteLine("Processing query...");
            
            try
            {
                Console.WriteLine("Generating embedding for query...");
                var queryEmbedding = await ollamaService.GenerateEmbeddingAsync(query);
                
                Console.WriteLine($"Embedding generated: Length = {queryEmbedding.Length}");
                
                if (queryEmbedding.Length > 0)
                {
                    var searchResults = await qdrantService.SearchAsync(collectionName, queryEmbedding, limit: 5);
                    
                    if (searchResults.Count > 0)
                    {
                        Console.WriteLine($"Found {searchResults.Count} relevant documents.");
                        
                        var contextBuilder = new System.Text.StringBuilder();
                        contextBuilder.AppendLine("Contesto sugli ordini trovati nel sistema:");
                        contextBuilder.AppendLine();
                        
                        foreach (var result in searchResults)
                        {
                            if (result.Payload.TryGetValue("text", out var textValue))
                            {
                                var text = textValue.StringValue;
                                contextBuilder.AppendLine(text);
                                contextBuilder.AppendLine("---");
                            }
                        }
                        
                        var context = contextBuilder.ToString();
                        
                        var prompt = $@"Sei un assistente che aiuta a rispondere a domande sugli ordini.
Usa il seguente contesto per rispondere alla domanda dell'utente.
Se non trovi informazioni rilevanti nel contesto, dillo chiaramente.

Contesto:
{context}

Domanda dell'utente: {query}

Risposta:";
                        
                        var response = await ollamaService.GenerateAsync(prompt);
                        Console.WriteLine("\n=== Response ===");
                        Console.WriteLine(response);
                    }
                    else
                    {
                        Console.WriteLine("No relevant documents found in the RAG system.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to generate embedding for the query.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error processing query: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("\n⚠ Natural language query skipped: Qdrant or Ollama not available.");
        }
    }
}

