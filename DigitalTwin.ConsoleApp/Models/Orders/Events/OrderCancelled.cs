using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public record OrderCancelled : IOrderEvent
{
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus PreviousStatus { get; init; }
    public OrderStatus NewStatus { get; init; } = OrderStatus.Cancelled;
    public DateTime CancelledAt { get; init; } = DateTime.UtcNow;
    public string? CancelledBy { get; init; }
    public string? CancellationReason { get; init; }
    public int Version { get; init; }

    string IOrderEvent.OrderNumber => OrderNumber;
    int IOrderEvent.Version => Version;
    DateTime IOrderEvent.EventTimestamp => CancelledAt;
    string IOrderEvent.EventType => "OrderCancelled";
}

