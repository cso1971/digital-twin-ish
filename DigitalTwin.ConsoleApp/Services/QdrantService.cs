using Qdrant.Client;
using Qdrant.Client.Grpc;
using DigitalTwin.ConsoleApp.Models;
using DigitalTwin.ConsoleApp.Models.Orders.Events;

namespace DigitalTwin.ConsoleApp.Services;

public class QdrantService
{
    private readonly QdrantClient _client;
    private readonly QdrantSettings _settings;

    public QdrantService(QdrantSettings settings)
    {
        _settings = settings;
        _client = new QdrantClient(
            host: _settings.Host,
            port: _settings.GrpcPort,
            https: _settings.UseHttps
        );
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
            
            await _client.ListCollectionsAsync(linkedCts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"  Qdrant connection timeout: Could not connect to {_settings.Host}:{_settings.GrpcPort} within 5 seconds");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Qdrant connection error: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }

    public async Task<List<string>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var collections = await _client.ListCollectionsAsync(cancellationToken);
            return collections.ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<bool> CollectionExistsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var collections = await _client.ListCollectionsAsync(cancellationToken);
            return collections.Contains(collectionName);
        }
        catch
        {
            return false;
        }
    }

    public async Task CreateCollectionAsync(string collectionName, uint vectorSize, CancellationToken cancellationToken = default)
    {
        var collectionParams = new VectorParams
        {
            Size = vectorSize,
            Distance = Distance.Cosine
        };

        await _client.CreateCollectionAsync(
            collectionName: collectionName,
            vectorsConfig: collectionParams,
            cancellationToken: cancellationToken
        );
    }

    public async Task UpsertPointsAsync(
        string collectionName,
        IEnumerable<PointStruct> points,
        CancellationToken cancellationToken = default)
    {
        var pointsList = points.ToList();
        await _client.UpsertAsync(
            collectionName: collectionName,
            points: pointsList,
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<ScoredPoint>> SearchAsync(
        string collectionName,
        float[] vector,
        ulong limit = 10,
        CancellationToken cancellationToken = default)
    {
        var results = await _client.SearchAsync(
            collectionName: collectionName,
            vector: vector,
            limit: limit,
            cancellationToken: cancellationToken
        );

        return results.ToList();
    }

    public async Task UpsertEmbeddingDocumentAsync(
        string collectionName,
        EmbeddingDocument document,
        float[] embedding,
        CancellationToken cancellationToken = default)
    {
        var pointIdHash = document.Id.GetHashCode();
        var pointId = pointIdHash < 0 ? (ulong)Math.Abs(pointIdHash) : (ulong)pointIdHash;

        var payload = new Dictionary<string, Value>();
        foreach (var (key, value) in document.Metadata)
        {
            payload[key] = value switch
            {
                string s => new Value { StringValue = s },
                int i => new Value { IntegerValue = i },
                long l => new Value { IntegerValue = l },
                float f => new Value { DoubleValue = f },
                double d => new Value { DoubleValue = d },
                decimal dec => new Value { DoubleValue = (double)dec },
                DateTime dt => new Value { StringValue = dt.ToString("O") },
                _ => new Value { StringValue = value?.ToString() ?? string.Empty }
            };
        }

        payload["text"] = new Value { StringValue = document.Text };
        payload["id"] = new Value { StringValue = document.Id };
        payload["orderNumber"] = new Value { StringValue = document.Context.OrderNumber };
        payload["eventType"] = new Value { StringValue = document.Context.EventType };

        var point = new PointStruct
        {
            Id = pointId,
            Vectors = embedding,
            Payload = { payload }
        };

        await _client.UpsertAsync(
            collectionName: collectionName,
            points: new[] { point },
            cancellationToken: cancellationToken
        );
    }

    public async Task DeleteAllPointsAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new Filter();
            await _client.DeleteAsync(
                collectionName: collectionName,
                filter: filter,
                wait: true,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting all points from collection '{collectionName}': {ex.Message}");
            throw;
        }
    }

    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.DeleteCollectionAsync(
                collectionName: collectionName,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting collection '{collectionName}': {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

