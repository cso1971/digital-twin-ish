using System.Text;
using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public static class OrderEventEmbeddingExtensions
{
    public static EmbeddingDocument ToEmbeddingDocument(this IOrderEvent orderEvent)
    {
        var embeddingText = orderEvent switch
        {
            OrderCreated created => created.ToEmbeddingText(),
            OrderConfirmed confirmed => confirmed.ToEmbeddingText(),
            OrderShipped shipped => shipped.ToEmbeddingText(),
            OrderDelivered delivered => delivered.ToEmbeddingText(),
            OrderCancelled cancelled => cancelled.ToEmbeddingText(),
            _ => throw new NotSupportedException($"Event type {orderEvent.GetType().Name} is not supported")
        };

        return EmbeddingDocument.FromEvent(orderEvent, embeddingText);
    }
}

public static class OrderEventExtensions
{
    public static string ToEmbeddingText(this OrderCreated orderCreated)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Evento: Ordine Creato");
        sb.AppendLine($"Numero Ordine: {orderCreated.OrderNumber}");
        sb.AppendLine($"Versione: {orderCreated.Version}");
        sb.AppendLine($"Data Ordine: {orderCreated.OrderDate:yyyy-MM-dd HH:mm:ss}");
        
        if (orderCreated.DeliveryDate.HasValue)
        {
            sb.AppendLine($"Data Consegna Prevista: {orderCreated.DeliveryDate.Value:yyyy-MM-dd HH:mm:ss}");
        }

        sb.AppendLine($"Cliente ID: {orderCreated.CustomerId}");
        sb.AppendLine($"Cliente: {orderCreated.CustomerName}");
        if (!string.IsNullOrWhiteSpace(orderCreated.CustomerEmail))
        {
            sb.AppendLine($"Email Cliente: {orderCreated.CustomerEmail}");
        }

        if (orderCreated.ShippingAddress != null)
        {
            sb.AppendLine($"Indirizzo Spedizione: {FormatAddress(orderCreated.ShippingAddress)}");
        }

        if (orderCreated.BillingAddress != null && 
            !AreAddressesEqual(orderCreated.ShippingAddress, orderCreated.BillingAddress))
        {
            sb.AppendLine($"Indirizzo Fatturazione: {FormatAddress(orderCreated.BillingAddress)}");
        }

        sb.AppendLine($"Stato Iniziale: {orderCreated.Status}");
        sb.AppendLine($"Metodo Pagamento: {orderCreated.PaymentMethod}");

        sb.AppendLine($"Righe Ordine ({orderCreated.OrderLines.Count}):");
        foreach (var line in orderCreated.OrderLines.OrderBy(l => l.LineNumber))
        {
            sb.AppendLine($"  Riga {line.LineNumber}:");
            sb.AppendLine($"    Prodotto ID: {line.ProductId}");
            sb.AppendLine($"    Codice Prodotto: {line.ProductCode}");
            sb.AppendLine($"    Nome Prodotto: {line.ProductName}");
            
            if (!string.IsNullOrWhiteSpace(line.ProductDescription))
            {
                sb.AppendLine($"    Descrizione: {line.ProductDescription}");
            }

            sb.AppendLine($"    Quantità: {line.Quantity} {line.UnitOfMeasure}");
            sb.AppendLine($"    Prezzo Unitario: {line.UnitPrice:F2} {orderCreated.Currency}");
            
            if (line.DiscountPercentage > 0)
            {
                sb.AppendLine($"    Sconto: {line.DiscountPercentage:F2}% ({line.DiscountAmount:F2} {orderCreated.Currency})");
            }

            if (line.TaxPercentage > 0)
            {
                sb.AppendLine($"    IVA: {line.TaxPercentage:F2}% ({line.TaxAmount:F2} {orderCreated.Currency})");
            }

            sb.AppendLine($"    Totale Riga: {line.LineTotal:F2} {orderCreated.Currency}");

            if (!string.IsNullOrWhiteSpace(line.Notes))
            {
                sb.AppendLine($"    Note: {line.Notes}");
            }
        }

        sb.AppendLine("Totali:");
        sb.AppendLine($"  Subtotale: {orderCreated.SubTotal:F2} {orderCreated.Currency}");
        
        if (orderCreated.DiscountAmount > 0)
        {
            sb.AppendLine($"  Sconto Totale: {orderCreated.DiscountAmount:F2} {orderCreated.Currency}");
        }

        if (orderCreated.ShippingCost > 0)
        {
            sb.AppendLine($"  Spese Spedizione: {orderCreated.ShippingCost:F2} {orderCreated.Currency}");
        }

        if (orderCreated.TaxAmount > 0)
        {
            sb.AppendLine($"  IVA Totale: {orderCreated.TaxAmount:F2} {orderCreated.Currency}");
        }

        sb.AppendLine($"  Totale Ordine: {orderCreated.TotalAmount:F2} {orderCreated.Currency}");

        if (!string.IsNullOrWhiteSpace(orderCreated.Notes))
        {
            sb.AppendLine($"Note Ordine: {orderCreated.Notes}");
        }

        sb.AppendLine($"Creato il: {orderCreated.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        if (!string.IsNullOrWhiteSpace(orderCreated.CreatedBy))
        {
            sb.AppendLine($"Creato da: {orderCreated.CreatedBy}");
        }

        if (orderCreated.UpdatedAt.HasValue)
        {
            sb.AppendLine($"Aggiornato il: {orderCreated.UpdatedAt.Value:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrWhiteSpace(orderCreated.UpdatedBy))
            {
                sb.AppendLine($"Aggiornato da: {orderCreated.UpdatedBy}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static string ToEmbeddingText(this OrderConfirmed orderConfirmed)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Evento: Ordine Confermato");
        sb.AppendLine($"Numero Ordine: {orderConfirmed.OrderNumber}");
        sb.AppendLine($"Versione: {orderConfirmed.Version}");
        sb.AppendLine($"Stato Precedente: {orderConfirmed.PreviousStatus}");
        sb.AppendLine($"Nuovo Stato: {orderConfirmed.NewStatus}");
        sb.AppendLine($"Data Conferma: {orderConfirmed.ConfirmedAt:yyyy-MM-dd HH:mm:ss}");
        
        if (!string.IsNullOrWhiteSpace(orderConfirmed.ConfirmedBy))
        {
            sb.AppendLine($"Confermato da: {orderConfirmed.ConfirmedBy}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ToEmbeddingText(this OrderShipped orderShipped)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Evento: Ordine Spedito");
        sb.AppendLine($"Numero Ordine: {orderShipped.OrderNumber}");
        sb.AppendLine($"Versione: {orderShipped.Version}");
        sb.AppendLine($"Stato Precedente: {orderShipped.PreviousStatus}");
        sb.AppendLine($"Nuovo Stato: {orderShipped.NewStatus}");
        sb.AppendLine($"Data Spedizione: {orderShipped.ShippedAt:yyyy-MM-dd HH:mm:ss}");
        
        if (!string.IsNullOrWhiteSpace(orderShipped.ShippedBy))
        {
            sb.AppendLine($"Spedito da: {orderShipped.ShippedBy}");
        }

        if (!string.IsNullOrWhiteSpace(orderShipped.TrackingNumber))
        {
            sb.AppendLine($"Numero Tracciamento: {orderShipped.TrackingNumber}");
        }

        if (!string.IsNullOrWhiteSpace(orderShipped.Carrier))
        {
            sb.AppendLine($"Corriere: {orderShipped.Carrier}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ToEmbeddingText(this OrderDelivered orderDelivered)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Evento: Ordine Consegnato");
        sb.AppendLine($"Numero Ordine: {orderDelivered.OrderNumber}");
        sb.AppendLine($"Versione: {orderDelivered.Version}");
        sb.AppendLine($"Stato Precedente: {orderDelivered.PreviousStatus}");
        sb.AppendLine($"Nuovo Stato: {orderDelivered.NewStatus}");
        sb.AppendLine($"Data Consegna: {orderDelivered.DeliveredAt:yyyy-MM-dd HH:mm:ss}");
        
        if (!string.IsNullOrWhiteSpace(orderDelivered.DeliveredBy))
        {
            sb.AppendLine($"Consegnato da: {orderDelivered.DeliveredBy}");
        }

        if (!string.IsNullOrWhiteSpace(orderDelivered.Signature))
        {
            sb.AppendLine($"Firma: {orderDelivered.Signature}");
        }

        if (!string.IsNullOrWhiteSpace(orderDelivered.DeliveryNotes))
        {
            sb.AppendLine($"Note Consegna: {orderDelivered.DeliveryNotes}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ToEmbeddingText(this OrderCancelled orderCancelled)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Evento: Ordine Cancellato");
        sb.AppendLine($"Numero Ordine: {orderCancelled.OrderNumber}");
        sb.AppendLine($"Versione: {orderCancelled.Version}");
        sb.AppendLine($"Stato Precedente: {orderCancelled.PreviousStatus}");
        sb.AppendLine($"Nuovo Stato: {orderCancelled.NewStatus}");
        sb.AppendLine($"Data Cancellazione: {orderCancelled.CancelledAt:yyyy-MM-dd HH:mm:ss}");
        
        if (!string.IsNullOrWhiteSpace(orderCancelled.CancelledBy))
        {
            sb.AppendLine($"Cancellato da: {orderCancelled.CancelledBy}");
        }

        if (!string.IsNullOrWhiteSpace(orderCancelled.CancellationReason))
        {
            sb.AppendLine($"Motivo Cancellazione: {orderCancelled.CancellationReason}");
        }

        return sb.ToString().TrimEnd();
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
}

