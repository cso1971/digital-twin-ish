using DigitalTwin.ConsoleApp.Domain;
using DigitalTwin.ConsoleApp.Extensions;
using DigitalTwin.ConsoleApp.Models;
using DigitalTwin.ConsoleApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.AddDigitalTwinServices();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Swagger (always enabled for this console app)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Twin API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

// Minimal API Endpoints
app.MapPost("/api/prompt", async (
    PromptRequest request, 
    OllamaService ollamaService,
    QdrantService qdrantService,
    QdrantSettings qdrantSettings) =>
{
    try
    {
        const string collectionName = "orders";
        const int searchLimit = 5;
        
        // Step 1: Generate embedding for the query using embedding model
        var queryEmbedding = await ollamaService.GenerateEmbeddingAsync(request.Prompt);
        
        if (queryEmbedding.Length == 0)
        {
            return Results.Problem(
                detail: "Failed to generate embedding for the query. Make sure an embedding model is available (e.g., nomic-embed-text).",
                statusCode: 500);
        }
        
        // Step 2: Search in Qdrant for relevant documents
        var searchResults = await qdrantService.SearchAsync(collectionName, queryEmbedding, limit: searchLimit);
        
        // Step 3: Build context from search results
        var contextBuilder = new System.Text.StringBuilder();
        if (searchResults.Count > 0)
        {
            contextBuilder.AppendLine("Contesto sugli ordini trovati nel sistema:");
            contextBuilder.AppendLine();
            
            foreach (var point in searchResults)
            {
                if (point.Payload.TryGetValue("text", out var textValue))
                {
                    var text = textValue.StringValue;
                    contextBuilder.AppendLine(text);
                    contextBuilder.AppendLine("---");
                }
            }
        }
        
        var context = contextBuilder.ToString();
        
        // Step 4: Build prompt with context
        var enhancedPrompt = string.IsNullOrWhiteSpace(context)
            ? request.Prompt
            : $@"Sei un assistente che aiuta a rispondere a domande sugli ordini.
Usa il seguente contesto per rispondere alla domanda dell'utente.
Se non trovi informazioni rilevanti nel contesto, dillo chiaramente.

Contesto:
{context}

Domanda dell'utente: {request.Prompt}

Risposta:";
        
        // Step 5: Generate response using LLM model (not embedding model)
        var result = await ollamaService.GenerateDetailedAsync(
            enhancedPrompt, 
            request.Model, // Use the specified model or default (llama3.2)
            CancellationToken.None);
        
        // Enhance response with RAG metadata
        return Results.Ok(new
        {
            result.Response,
            result.Model,
            result.CreatedAt,
            result.Done,
            result.TotalDuration,
            result.EvalCount,
            result.EvalDuration,
            RAGContext = new
            {
                DocumentsFound = searchResults.Count,
                HasContext = !string.IsNullOrWhiteSpace(context)
            }
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500);
    }
})
.WithName("ExecutePrompt");

// Run console logic in background (optional - comment out if you don't want it)
_ = Task.Run(async () => await app.Services.RunConsoleLogicAsync());

// Configure Swagger browser opening
app.OpenSwaggerInBrowserOnStart();


// Start the web server
await app.RunAsync();
