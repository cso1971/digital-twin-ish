using DigitalTwin.ConsoleApp.Models.Orders;

namespace DigitalTwin.ConsoleApp.Domain;

public static class OrderProcessingHelper
{
    public static void ProcessOrders(IOrderService orderService)
    {
        var orders = new List<Order>();
        for (int i = 0; i < 5; i++)
        {
            orders.Add(Order.Create());
        }

        Console.WriteLine($"\nCreated {orders.Count} orders.");

        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            Console.WriteLine($"\n--- Processing Order {i + 1}: {order.OrderNumber} ---");
            
            try
            {
                var unused2 = orderService.Create(order);
                Console.WriteLine($"✓ OrderCreated event generated and loaded into RAG");
                
                switch (i)
                {
                    case 0:
                        order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                        var unused = orderService.Confirm(order, OrderStatus.Pending, "admin001");
                        Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Shipped, Version = 3 };
                        var unused1 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse001", "TRACK-001", "DHL");
                        Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                        break;
                        
                    case 1:
                        order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                        var unused3 = orderService.Confirm(order, OrderStatus.Pending, "admin002");
                        Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Shipped, Version = 3 };
                        var unused4 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse002", "TRACK-002", "UPS");
                        Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Delivered, Version = 4 };
                        var unused5 = orderService.Deliver(order, OrderStatus.Shipped, "courier001", "SIGN-001", "Consegnato al piano terra");
                        Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                        break;
                        
                    case 2:
                        order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                        var unused6 = orderService.Confirm(order, OrderStatus.Pending, "admin003");
                        Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Cancelled, Version = 3 };
                        var unused7 = orderService.Cancel(order, OrderStatus.Confirmed, "admin003", "Cliente ha richiesto cancellazione");
                        Console.WriteLine($"✓ OrderCancelled event generated and loaded into RAG");
                        break;
                        
                    case 3:
                        order = order with { Status = OrderStatus.Processing, Version = 2 };
                        order = order with { Status = OrderStatus.Shipped, Version = 3 };
                        var unused8 = orderService.Ship(order, OrderStatus.Processing, "warehouse003", "TRACK-003", "FedEx");
                        Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Delivered, Version = 4 };
                        var unused9 = orderService.Deliver(order, OrderStatus.Shipped, "courier002", "SIGN-002", "Consegnato in ufficio");
                        Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                        break;
                        
                    case 4:
                        order = order with { Status = OrderStatus.Confirmed, Version = 2 };
                        var unused10 = orderService.Confirm(order, OrderStatus.Pending, "admin004");
                        Console.WriteLine($"✓ OrderConfirmed event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Shipped, Version = 3 };
                        var unused11 = orderService.Ship(order, OrderStatus.Confirmed, "warehouse004", "TRACK-004", "Poste Italiane");
                        Console.WriteLine($"✓ OrderShipped event generated and loaded into RAG");
                        
                        order = order with { Status = OrderStatus.Delivered, Version = 4 };
                        var unused12 = orderService.Deliver(order, OrderStatus.Shipped, "courier003", "SIGN-003", "Consegnato al destinatario");
                        Console.WriteLine($"✓ OrderDelivered event generated and loaded into RAG");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error processing order {order.OrderNumber}: {ex.Message}");
            }
        }

        Console.WriteLine("\n=== All Orders Processed ===");
        Console.WriteLine("Events have been generated and loaded into the RAG system.");
    }
}

