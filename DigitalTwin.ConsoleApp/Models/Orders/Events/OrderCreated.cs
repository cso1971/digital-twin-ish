using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public record OrderCreated : IOrderEvent
{
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; } = DateTime.UtcNow;
    public DateTime? DeliveryDate { get; init; }
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public Address? ShippingAddress { get; init; }
    public Address? BillingAddress { get; init; }
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; init; } = PaymentMethod.Cash;
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = "EUR";
    public string? Notes { get; init; }
    public List<OrderLine> OrderLines { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
    public int Version { get; set; }

    string IOrderEvent.OrderNumber => OrderNumber;
    int IOrderEvent.Version => Version;
    DateTime IOrderEvent.EventTimestamp => CreatedAt;
    string IOrderEvent.EventType => "OrderCreated";
}