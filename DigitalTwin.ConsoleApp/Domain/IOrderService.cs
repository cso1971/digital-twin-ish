using DigitalTwin.ConsoleApp.Models.Orders;
using DigitalTwin.ConsoleApp.Models.Orders.Events;

namespace DigitalTwin.ConsoleApp.Domain;

public interface IOrderService
{
    OrderCreated Create(Order order);

    OrderConfirmed Confirm(Order order, OrderStatus? previousStatus = null, string? confirmedBy = null);

    OrderShipped Ship(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? shippedBy = null,
        string? trackingNumber = null, 
        string? carrier = null);

    OrderDelivered Deliver(
        Order order, 
        OrderStatus? previousStatus = null, 
        string? deliveredBy = null,
        string? signature = null, 
        string? deliveryNotes = null);

    OrderCancelled Cancel(
        Order order, 
        OrderStatus previousStatus, 
        string? cancelledBy = null,
        string? cancellationReason = null);
}

