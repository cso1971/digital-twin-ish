using DigitalTwin.ConsoleApp.Models.Orders;
using DigitalTwin.ConsoleApp.Models.Orders.Events;
using DigitalTwin.ConsoleApp.Services;

namespace DigitalTwin.ConsoleApp.Domain;

public class OrderService : IOrderService
{
    private readonly OllamaService _ollamaService;
    private readonly QdrantService _qdrantService;
    private readonly string _collectionName;

    public OrderService(OllamaService ollamaService, QdrantService qdrantService, string collectionName = "orders")
    {
        _ollamaService = ollamaService ?? throw new ArgumentNullException(nameof(ollamaService));
        _qdrantService = qdrantService ?? throw new ArgumentNullException(nameof(qdrantService));
        _collectionName = collectionName;
    }

    private async Task LoadEventIntoRagAsync(IOrderEvent orderEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var embeddingDocument = orderEvent.ToEmbeddingDocument();
            var embedding = await _ollamaService.GenerateEmbeddingAsync(embeddingDocument.Text, cancellationToken: cancellationToken);
            
            if (embedding.Length > 0)
            {
                await _qdrantService.UpsertEmbeddingDocumentAsync(_collectionName, embeddingDocument, embedding, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore nel caricamento dell'evento nel RAG: {ex.Message}");
        }
    }

    public async Task<OrderCreated> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var orderCreated = order.ToOrderCreated();
        await LoadEventIntoRagAsync(orderCreated, cancellationToken);
        return orderCreated;
    }

    public OrderCreated Create(Order order)
    {
        return CreateAsync(order).GetAwaiter().GetResult();
    }

    public async Task<OrderConfirmed> ConfirmAsync(Order order, OrderStatus? previousStatus = null, string? confirmedBy = null, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"L'ordine {order.OrderNumber} non è nello stato Confirmed. Stato attuale: {order.Status}");

        var orderConfirmed = order.ToOrderConfirmed(previousStatus);
        
        if (!string.IsNullOrWhiteSpace(confirmedBy))
        {
            orderConfirmed = orderConfirmed with { ConfirmedBy = confirmedBy };
        }

        await LoadEventIntoRagAsync(orderConfirmed, cancellationToken);
        return orderConfirmed;
    }

    public OrderConfirmed Confirm(Order order, OrderStatus? previousStatus = null, string? confirmedBy = null)
    {
        return ConfirmAsync(order, previousStatus, confirmedBy).GetAwaiter().GetResult();
    }

    public async Task<OrderShipped> ShipAsync(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? shippedBy = null,
        string? trackingNumber = null, 
        string? carrier = null,
        CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Status != OrderStatus.Shipped)
            throw new InvalidOperationException($"L'ordine {order.OrderNumber} non è nello stato Shipped. Stato attuale: {order.Status}");

        var orderShipped = order.ToOrderShipped(previousStatus, trackingNumber, carrier);
        
        if (!string.IsNullOrWhiteSpace(shippedBy))
        {
            orderShipped = orderShipped with { ShippedBy = shippedBy };
        }

        await LoadEventIntoRagAsync(orderShipped, cancellationToken);
        return orderShipped;
    }

    public OrderShipped Ship(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? shippedBy = null,
        string? trackingNumber = null, 
        string? carrier = null)
    {
        return ShipAsync(order, previousStatus, shippedBy, trackingNumber, carrier).GetAwaiter().GetResult();
    }

    public async Task<OrderDelivered> DeliverAsync(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? deliveredBy = null,
        string? signature = null, 
        string? deliveryNotes = null,
        CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Status != OrderStatus.Delivered)
            throw new InvalidOperationException($"L'ordine {order.OrderNumber} non è nello stato Delivered. Stato attuale: {order.Status}");

        var orderDelivered = order.ToOrderDelivered(previousStatus, signature, deliveryNotes);
        
        if (!string.IsNullOrWhiteSpace(deliveredBy))
        {
            orderDelivered = orderDelivered with { DeliveredBy = deliveredBy };
        }

        await LoadEventIntoRagAsync(orderDelivered, cancellationToken);
        return orderDelivered;
    }

    public OrderDelivered Deliver(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? deliveredBy = null,
        string? signature = null, 
        string? deliveryNotes = null)
    {
        return DeliverAsync(order, previousStatus, deliveredBy, signature, deliveryNotes).GetAwaiter().GetResult();
    }

    public async Task<OrderCancelled> CancelAsync(
        Order order, 
        OrderStatus previousStatus, 
        string? cancelledBy = null,
        string? cancellationReason = null,
        CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.Status != OrderStatus.Cancelled)
            throw new InvalidOperationException($"L'ordine {order.OrderNumber} non è nello stato Cancelled. Stato attuale: {order.Status}");

        var orderCancelled = order.ToOrderCancelled(previousStatus, cancellationReason);
        
        if (!string.IsNullOrWhiteSpace(cancelledBy))
        {
            orderCancelled = orderCancelled with { CancelledBy = cancelledBy };
        }

        await LoadEventIntoRagAsync(orderCancelled, cancellationToken);
        return orderCancelled;
    }

    public OrderCancelled Cancel(
        Order order, 
        OrderStatus previousStatus, 
        string? cancelledBy = null,
        string? cancellationReason = null)
    {
        return CancelAsync(order, previousStatus, cancelledBy, cancellationReason).GetAwaiter().GetResult();
    }
}

