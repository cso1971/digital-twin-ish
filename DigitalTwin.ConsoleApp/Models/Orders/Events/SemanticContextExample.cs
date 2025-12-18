namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public static class SemanticContextExample
{
    public static void UsageExample()
    {
        var order = new Order
        {
            OrderNumber = "ORD-2024-001",
            Version = 1
        };

        var orderCreated = new OrderCreated
        {
            OrderNumber = "ORD-2024-001",
            Version = 1
        };

        var orderConfirmed = new OrderConfirmed
        {
            OrderNumber = "ORD-2024-001",
            Version = 2
        };

        var orderDocument = order.ToEmbeddingDocument();
        var createdDocument = orderCreated.ToEmbeddingDocument();
        var confirmedDocument = orderConfirmed.ToEmbeddingDocument();

        Console.WriteLine($"Order Document ID: {orderDocument.Id}");
        Console.WriteLine($"Order Context Key: {orderDocument.Context.GetOrderContextKey()}");
        Console.WriteLine($"Event Type Context Key: {orderDocument.Context.GetEventTypeContextKey()}");

        Console.WriteLine($"\nCreated Event Document ID: {createdDocument.Id}");
        Console.WriteLine($"Created Event Context Key: {createdDocument.Context.GetOrderContextKey()}");

        Console.WriteLine($"\nConfirmed Event Document ID: {confirmedDocument.Id}");
        Console.WriteLine($"Confirmed Event Context Key: {confirmedDocument.Context.GetOrderContextKey()}");

        Console.WriteLine("\nAll documents share the same order context key:");
        Console.WriteLine($"  {orderDocument.Context.GetOrderContextKey()}");
        Console.WriteLine($"  {createdDocument.Context.GetOrderContextKey()}");
        Console.WriteLine($"  {confirmedDocument.Context.GetOrderContextKey()}");
    }
}

