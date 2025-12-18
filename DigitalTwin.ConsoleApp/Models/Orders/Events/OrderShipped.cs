using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public record OrderShipped : IOrderEvent
{
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus PreviousStatus { get; init; } = OrderStatus.Processing;
    public OrderStatus NewStatus { get; init; } = OrderStatus.Shipped;
    public DateTime ShippedAt { get; init; } = DateTime.UtcNow;
    public string? ShippedBy { get; init; }
    public string? TrackingNumber { get; init; }
    public string? Carrier { get; init; }
    public int Version { get; init; }

    string IOrderEvent.OrderNumber => OrderNumber;
    int IOrderEvent.Version => Version;
    DateTime IOrderEvent.EventTimestamp => ShippedAt;
    string IOrderEvent.EventType => "OrderShipped";
}

