using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public record OrderDelivered : IOrderEvent
{
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus PreviousStatus { get; init; } = OrderStatus.Shipped;
    public OrderStatus NewStatus { get; init; } = OrderStatus.Delivered;
    public DateTime DeliveredAt { get; init; } = DateTime.UtcNow;
    public string? DeliveredBy { get; init; }
    public string? Signature { get; init; }
    public string? DeliveryNotes { get; init; }
    public int Version { get; init; }

    string IOrderEvent.OrderNumber => OrderNumber;
    int IOrderEvent.Version => Version;
    DateTime IOrderEvent.EventTimestamp => DeliveredAt;
    string IOrderEvent.EventType => "OrderDelivered";
}

