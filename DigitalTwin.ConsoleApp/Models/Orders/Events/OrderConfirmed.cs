using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public record OrderConfirmed : IOrderEvent
{
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus PreviousStatus { get; init; } = OrderStatus.Pending;
    public OrderStatus NewStatus { get; init; } = OrderStatus.Confirmed;
    public DateTime ConfirmedAt { get; init; } = DateTime.UtcNow;
    public string? ConfirmedBy { get; init; }
    public int Version { get; init; }

    string IOrderEvent.OrderNumber => OrderNumber;
    int IOrderEvent.Version => Version;
    DateTime IOrderEvent.EventTimestamp => ConfirmedAt;
    string IOrderEvent.EventType => "OrderConfirmed";
}

