namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public class OrderEventContext
{
    public string OrderNumber { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public int Version { get; init; }
    public DateTime EventTimestamp { get; init; }
    public string? EventId { get; init; }

    public string SemanticId => EventId ?? GenerateSemanticId();

    private string GenerateSemanticId()
    {
        var timestamp = EventTimestamp.ToString("yyyyMMddHHmmss");
        return $"{OrderNumber}:{EventType}:v{Version}:{timestamp}";
    }

    public static OrderEventContext FromEvent(IOrderEvent orderEvent)
    {
        return new OrderEventContext
        {
            OrderNumber = orderEvent.OrderNumber,
            EventType = orderEvent.EventType,
            Version = orderEvent.Version,
            EventTimestamp = orderEvent.EventTimestamp
        };
    }

    public string GetOrderContextKey() => $"order:{OrderNumber}";
    
    public string GetEventTypeContextKey() => $"event-type:{EventType}";
    
    public string GetOrderVersionContextKey() => $"order:{OrderNumber}:v{Version}";
}

