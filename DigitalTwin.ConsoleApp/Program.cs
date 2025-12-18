using DigitalTwin.ConsoleApp.Models;
using DigitalTwin.ConsoleApp.Models.Orders;
using DigitalTwin.ConsoleApp.Domain;
using DigitalTwin.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();

var ollamaSettings = new OllamaSettings();
configuration.GetSection("Ollama").Bind(ollamaSettings);
services.AddSingleton(ollamaSettings);

var qdrantSettings = new QdrantSettings();
configuration.GetSection("Qdrant").Bind(qdrantSettings);
services.AddSingleton(qdrantSettings);

services.AddHttpClient<OllamaService>();
services.AddSingleton<OllamaService>();
services.AddSingleton<QdrantService>();
services.AddSingleton<IOrderService, OrderService>();

var serviceProvider = services.BuildServiceProvider();

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

var orders = new List<Order>();
for (int i = 0; i < 5; i++)
{
    orders.Add(Order.Create());
}

Console.WriteLine($"\nCreated {orders.Count} orders.");

for (int i = 0; i < orders.Count; i++)
{
    var order = orders[i];
    Console.WriteLine($"\n--- Processing Order {i + 1}: {order.OrderNumber} ---");
    
    try
    {
        var unused2 = orderService.Create(order);
        Console.WriteLine($"✓ OrderCreated event generated and loaded into RAG");
        
        switch (i)
        {
            case 0:
                order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                var unused = orderService.Confirm(order, OrderStatus.Pending, "admin001");
                Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Shipped, Version = 3 };
                var unused1 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse001", "TRACK-001", "DHL");
                Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                break;
                
            case 1:
                order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                var unused3 = orderService.Confirm(order, OrderStatus.Pending, "admin002");
                Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Shipped, Version = 3 };
                var unused4 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse002", "TRACK-002", "UPS");
                Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Delivered, Version = 4 };
                var unused5 = orderService.Deliver(order, OrderStatus.Shipped, "courier001", "SIGN-001", "Consegnato al piano terra");
                Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                break;
                
            case 2:
                order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                var unused6 = orderService.Confirm(order, OrderStatus.Pending, "admin003");
                Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Cancelled, Version = 3 };
                var unused7 = orderService.Cancel(order, OrderStatus.Confirmed, "admin003", "Cliente ha richiesto cancellazione");
                Console.WriteLine($"✓ OrderCancelled event generated and loaded into RAG");
                break;
                
            case 3:
                order = order with { Status = OrderStatus.Processing, Version = 2 };
                order = order with { Status = OrderStatus.Shipped, Version = 3 };
                var unused8 = orderService.Ship(order, OrderStatus.Processing, "warehouse003", "TRACK-003", "FedEx");
                Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Delivered, Version = 4 };
                var unused9 = orderService.Deliver(order, OrderStatus.Shipped, "courier002", "SIGN-002", "Consegnato in ufficio");
                Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                break;
                
            case 4:
                order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                var unused10 = orderService.Confirm(order, OrderStatus.Pending, "admin004");
                Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Shipped, Version = 3 };
                var unused11 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse004", "TRACK-004", "Poste Italiane");
                Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                
                order = order with { Status = OrderStatus.Delivered, Version = 4 };
                var unused12 = orderService.Deliver(order, OrderStatus.Shipped, "courier003", "SIGN-003", "Consegnato al destinatario");
                Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error processing order {order.OrderNumber}: {ex.Message}");
    }
}

Console.WriteLine("\n=== All Orders Processed ===");
Console.WriteLine("Events have been generated and loaded into the RAG system.");

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
