namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public class EmbeddingDocument
{
    public string Id { get; init; } = string.Empty;
    public OrderEventContext Context { get; init; } = null!;
    public string Text { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();

    public static EmbeddingDocument FromEvent(IOrderEvent orderEvent, string embeddingText)
    {
        var context = OrderEventContext.FromEvent(orderEvent);
        
        var metadata = new Dictionary<string, object>
        {
            { "orderNumber", orderEvent.OrderNumber },
            { "eventType", orderEvent.EventType },
            { "version", orderEvent.Version },
            { "eventTimestamp", orderEvent.EventTimestamp },
            { "orderContextKey", context.GetOrderContextKey() },
            { "eventTypeContextKey", context.GetEventTypeContextKey() },
            { "orderVersionContextKey", context.GetOrderVersionContextKey() }
        };

        return new EmbeddingDocument
        {
            Id = context.SemanticId,
            Context = context,
            Text = embeddingText,
            Metadata = metadata
        };
    }

    public static EmbeddingDocument FromOrder(Order order, string embeddingText)
    {
        var context = new OrderEventContext
        {
            OrderNumber = order.OrderNumber,
            EventType = "Order",
            Version = order.Version,
            EventTimestamp = order.UpdatedAt ?? order.CreatedAt
        };

        var metadata = new Dictionary<string, object>
        {
            { "orderNumber", order.OrderNumber },
            { "eventType", "Order" },
            { "version", order.Version },
            { "eventTimestamp", context.EventTimestamp },
            { "orderContextKey", context.GetOrderContextKey() },
            { "eventTypeContextKey", context.GetEventTypeContextKey() },
            { "orderVersionContextKey", context.GetOrderVersionContextKey() },
            { "status", order.Status.ToString() },
            { "customerId", order.CustomerId },
            { "totalAmount", order.TotalAmount }
        };

        return new EmbeddingDocument
        {
            Id = context.SemanticId,
            Context = context,
            Text = embeddingText,
            Metadata = metadata
        };
    }
}

