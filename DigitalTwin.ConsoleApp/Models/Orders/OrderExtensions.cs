using System.Text;
using DigitalTwin.ConsoleApp.Models.Orders.Events;

namespace DigitalTwin.ConsoleApp.Models.Orders;

public static class OrderExtensions
{
    public static string ToEmbeddingText(this Order order)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Ordine: {order.OrderNumber}");
        sb.AppendLine($"Versione Ordine: {order.Version}");
        sb.AppendLine($"Data ordine: {order.OrderDate:yyyy-MM-dd HH:mm:ss}");
        
        if (order.DeliveryDate.HasValue)
        {
            sb.AppendLine($"Data consegna: {order.DeliveryDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        sb.AppendLine($"Cliente ID: {order.CustomerId}");
        sb.AppendLine($"Cliente: {order.CustomerName}");
        if (!string.IsNullOrWhiteSpace(order.CustomerEmail))
        {
            sb.AppendLine($"Email cliente: {order.CustomerEmail}");
        }

        if (order.ShippingAddress != null)
        {
            sb.AppendLine($"Indirizzo spedizione: {FormatAddress(order.ShippingAddress)}");
        }

        if (order.BillingAddress != null && 
            !AreAddressesEqual(order.ShippingAddress, order.BillingAddress))
        {
            sb.AppendLine($"Indirizzo fatturazione: {FormatAddress(order.BillingAddress)}");
        }

        sb.AppendLine($"Stato ordine: {order.Status}");
        sb.AppendLine($"Metodo pagamento: {order.PaymentMethod}");

        sb.AppendLine($"Righe ordine ({order.OrderLines.Count}):");
        foreach (var line in order.OrderLines.OrderBy(l => l.LineNumber))
        {
            sb.AppendLine($"  Riga {line.LineNumber}:");
            sb.AppendLine($"    Prodotto ID: {line.ProductId}");
            sb.AppendLine($"    Codice prodotto: {line.ProductCode}");
            sb.AppendLine($"    Nome prodotto: {line.ProductName}");
            
            if (!string.IsNullOrWhiteSpace(line.ProductDescription))
            {
                sb.AppendLine($"    Descrizione: {line.ProductDescription}");
            }

            sb.AppendLine($"    Quantità: {line.Quantity} {line.UnitOfMeasure}");
            sb.AppendLine($"    Prezzo unitario: {line.UnitPrice:F2} {order.Currency}");
            
            if (line.DiscountPercentage > 0)
            {
                sb.AppendLine($"    Sconto: {line.DiscountPercentage:F2}% ({line.DiscountAmount:F2} {order.Currency})");
            }

            if (line.TaxPercentage > 0)
            {
                sb.AppendLine($"    IVA: {line.TaxPercentage:F2}% ({line.TaxAmount:F2} {order.Currency})");
            }

            sb.AppendLine($"    Totale riga: {line.LineTotal:F2} {order.Currency}");

            if (!string.IsNullOrWhiteSpace(line.Notes))
            {
                sb.AppendLine($"    Note: {line.Notes}");
            }
        }

        sb.AppendLine("Totali:");
        sb.AppendLine($"  Subtotale: {order.SubTotal:F2} {order.Currency}");
        
        if (order.DiscountAmount > 0)
        {
            sb.AppendLine($"  Sconto totale: {order.DiscountAmount:F2} {order.Currency}");
        }

        if (order.ShippingCost > 0)
        {
            sb.AppendLine($"  Spese spedizione: {order.ShippingCost:F2} {order.Currency}");
        }

        if (order.TaxAmount > 0)
        {
            sb.AppendLine($"  IVA totale: {order.TaxAmount:F2} {order.Currency}");
        }

        sb.AppendLine($"  Totale ordine: {order.TotalAmount:F2} {order.Currency}");

        if (!string.IsNullOrWhiteSpace(order.Notes))
        {
            sb.AppendLine($"Note ordine: {order.Notes}");
        }

        sb.AppendLine($"Creato il: {order.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        if (order.CreatedBy != null)
        {
            sb.AppendLine($"Creato da: {order.CreatedBy}");
        }

        if (order.UpdatedAt.HasValue)
        {
            sb.AppendLine($"Aggiornato il: {order.UpdatedAt.Value:yyyy-MM-dd HH:mm:ss}");
            if (order.UpdatedBy != null)
            {
                sb.AppendLine($"Aggiornato da: {order.UpdatedBy}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static OrderCreated ToOrderCreated(this Order order)
    {
        return new OrderCreated
        {
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            ShippingAddress = order.ShippingAddress,
            BillingAddress = order.BillingAddress,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            Notes = order.Notes,
            OrderLines = order.OrderLines,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            CreatedBy = order.CreatedBy,
            UpdatedBy = order.UpdatedBy,
            Version = order.Version
        };
    }

    public static OrderConfirmed ToOrderConfirmed(this Order order, OrderStatus? previousStatus = null)
    {
        return new OrderConfirmed
        {
            OrderNumber = order.OrderNumber,
            PreviousStatus = previousStatus ?? OrderStatus.Pending,
            NewStatus = OrderStatus.Confirmed,
            ConfirmedAt = order.UpdatedAt ?? DateTime.UtcNow,
            ConfirmedBy = order.UpdatedBy,
            Version = order.Version
        };
    }

    public static OrderShipped ToOrderShipped(this Order order, OrderStatus? previousStatus = null, string? trackingNumber = null, string? carrier = null)
    {
        return new OrderShipped
        {
            OrderNumber = order.OrderNumber,
            PreviousStatus = previousStatus ?? OrderStatus.Processing,
            NewStatus = OrderStatus.Shipped,
            ShippedAt = order.UpdatedAt ?? DateTime.UtcNow,
            ShippedBy = order.UpdatedBy,
            TrackingNumber = trackingNumber,
            Carrier = carrier,
            Version = order.Version
        };
    }

    public static OrderDelivered ToOrderDelivered(this Order order, OrderStatus? previousStatus = null, string? signature = null, string? deliveryNotes = null)
    {
        return new OrderDelivered
        {
            OrderNumber = order.OrderNumber,
            PreviousStatus = previousStatus ?? OrderStatus.Shipped,
            NewStatus = OrderStatus.Delivered,
            DeliveredAt = order.UpdatedAt ?? DateTime.UtcNow,
            DeliveredBy = order.UpdatedBy,
            Signature = signature,
            DeliveryNotes = deliveryNotes,
            Version = order.Version
        };
    }

    public static OrderCancelled ToOrderCancelled(this Order order, OrderStatus previousStatus, string? cancellationReason = null)
    {
        return new OrderCancelled
        {
            OrderNumber = order.OrderNumber,
            PreviousStatus = previousStatus,
            NewStatus = OrderStatus.Cancelled,
            CancelledAt = order.UpdatedAt ?? DateTime.UtcNow,
            CancelledBy = order.UpdatedBy,
            CancellationReason = cancellationReason,
            Version = order.Version
        };
    }

    private static string FormatAddress(Address address)
    {
        var parts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(address.Street))
        {
            parts.Add(address.Street);
        }
        
        if (!string.IsNullOrWhiteSpace(address.PostalCode))
        {
            parts.Add(address.PostalCode);
        }
        
        if (!string.IsNullOrWhiteSpace(address.City))
        {
            parts.Add(address.City);
        }
        
        if (!string.IsNullOrWhiteSpace(address.Province))
        {
            parts.Add($"({address.Province})");
        }
        
        if (!string.IsNullOrWhiteSpace(address.Country))
        {
            parts.Add(address.Country);
        }

        return string.Join(" ", parts);
    }

    private static bool AreAddressesEqual(Address? address1, Address? address2)
    {
        if (address1 == null && address2 == null) return true;
        if (address1 == null || address2 == null) return false;

        return address1.Street == address2.Street &&
               address1.City == address2.City &&
               address1.PostalCode == address2.PostalCode &&
               address1.Province == address2.Province &&
               address1.Country == address2.Country;
    }

    public static EmbeddingDocument ToEmbeddingDocument(this Order order)
    {
        var embeddingText = order.ToEmbeddingText();
        return EmbeddingDocument.FromOrder(order, embeddingText);
    }
}
