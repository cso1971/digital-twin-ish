namespace DigitalTwin.ConsoleApp.Models.Orders;

public static class OrderSamples
{
    public static List<Order> CreateSampleOrders()
    {
        return new List<Order>
        {
            CreateOrder1(),
            CreateOrder2(),
            CreateOrder3(),
            CreateOrder4(),
            CreateOrder5()
        };
    }

    private static Order CreateOrder1()
    {
        return new Order
        {
            Version = 1,
            OrderNumber = "ORD-2024-001",
            OrderDate = new DateTime(2024, 1, 15, 10, 30, 0),
            DeliveryDate = new DateTime(2024, 1, 20, 14, 0, 0),
            CustomerId = "CUST-001",
            CustomerName = "Mario Rossi",
            CustomerEmail = "mario.rossi@example.com",
            ShippingAddress = new Address
            {
                Street = "Via Roma 123",
                City = "Milano",
                PostalCode = "20100",
                Province = "MI",
                Country = "IT"
            },
            BillingAddress = new Address
            {
                Street = "Via Roma 123",
                City = "Milano",
                PostalCode = "20100",
                Province = "MI",
                Country = "IT"
            },
            Status = OrderStatus.Confirmed,
            PaymentMethod = PaymentMethod.CreditCard,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineNumber = 1,
                    ProductId = "PROD-101",
                    ProductCode = "LAPTOP-001",
                    ProductName = "Laptop Dell XPS 15",
                    ProductDescription = "Laptop 15 pollici, Intel i7, 16GB RAM, 512GB SSD",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 1299.99m,
                    DiscountPercentage = 5.0m,
                    DiscountAmount = 65.00m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 271.60m,
                    LineTotal = 1506.59m
                },
                new OrderLine
                {
                    LineNumber = 2,
                    ProductId = "PROD-102",
                    ProductCode = "MOUSE-001",
                    ProductName = "Mouse Wireless Logitech",
                    ProductDescription = "Mouse wireless ergonomico, batteria inclusa",
                    Quantity = 2,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 29.99m,
                    DiscountPercentage = 0m,
                    DiscountAmount = 0m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 13.20m,
                    LineTotal = 73.18m
                }
            },
            SubTotal = 1559.98m,
            TaxAmount = 284.80m,
            ShippingCost = 15.00m,
            DiscountAmount = 65.00m,
            TotalAmount = 1794.78m,
            Currency = "EUR",
            Notes = "Consegna urgente richiesta",
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
            CreatedBy = "user001",
            UpdatedAt = new DateTime(2024, 1, 15, 11, 0, 0),
            UpdatedBy = "user001"
        };
    }

    private static Order CreateOrder2()
    {
        return new Order
        {
            Version = 1,
            
            OrderNumber = "ORD-2024-002",
            OrderDate = new DateTime(2024, 2, 10, 14, 20, 0),
            DeliveryDate = new DateTime(2024, 2, 15, 10, 0, 0),
            CustomerId = "CUST-002",
            CustomerName = "Giulia Bianchi",
            CustomerEmail = "giulia.bianchi@example.com",
            ShippingAddress = new Address
            {
                Street = "Corso Garibaldi 45",
                City = "Torino",
                PostalCode = "10121",
                Province = "TO",
                Country = "IT"
            },
            BillingAddress = new Address
            {
                Street = "Via Verdi 78",
                City = "Torino",
                PostalCode = "10122",
                Province = "TO",
                Country = "IT"
            },
            Status = OrderStatus.Shipped,
            PaymentMethod = PaymentMethod.BankTransfer,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineNumber = 1,
                    ProductId = "PROD-201",
                    ProductCode = "PHONE-001",
                    ProductName = "Smartphone Samsung Galaxy S24",
                    ProductDescription = "Smartphone 6.1 pollici, 128GB, 5G",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 899.99m,
                    DiscountPercentage = 10.0m,
                    DiscountAmount = 90.00m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 178.20m,
                    LineTotal = 988.19m
                },
                new OrderLine
                {
                    LineNumber = 2,
                    ProductId = "PROD-202",
                    ProductCode = "CASE-001",
                    ProductName = "Custodia Protettiva",
                    ProductDescription = "Custodia in silicone con protezione angoli",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 19.99m,
                    DiscountPercentage = 0m,
                    DiscountAmount = 0m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 4.40m,
                    LineTotal = 24.39m
                },
                new OrderLine
                {
                    LineNumber = 3,
                    ProductId = "PROD-203",
                    ProductCode = "CABLE-001",
                    ProductName = "Cavo USB-C",
                    ProductDescription = "Cavo USB-C da 2 metri, ricarica rapida",
                    Quantity = 2,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 12.99m,
                    DiscountPercentage = 0m,
                    DiscountAmount = 0m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 5.72m,
                    LineTotal = 31.70m
                }
            },
            SubTotal = 953.97m,
            TaxAmount = 188.32m,
            ShippingCost = 0m,
            DiscountAmount = 90.00m,
            TotalAmount = 1052.29m,
            Currency = "EUR",
            Notes = "Pagamento anticipato ricevuto",
            CreatedAt = new DateTime(2024, 2, 10, 14, 20, 0),
            CreatedBy = "user002"
        };
    }

    private static Order CreateOrder3()
    {
        return new Order
        {
            Version = 1,
            
            OrderNumber = "ORD-2024-003",
            OrderDate = new DateTime(2024, 3, 5, 9, 15, 0),
            DeliveryDate = new DateTime(2024, 3, 8, 16, 30, 0),
            CustomerId = "CUST-003",
            CustomerName = "Luca Verdi",
            CustomerEmail = "luca.verdi@example.com",
            ShippingAddress = new Address
            {
                Street = "Via Firenze 89",
                City = "Roma",
                PostalCode = "00100",
                Province = "RM",
                Country = "IT"
            },
            BillingAddress = new Address
            {
                Street = "Via Firenze 89",
                City = "Roma",
                PostalCode = "00100",
                Province = "RM",
                Country = "IT"
            },
            Status = OrderStatus.Delivered,
            PaymentMethod = PaymentMethod.PayPal,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineNumber = 1,
                    ProductId = "PROD-301",
                    ProductCode = "BOOK-001",
                    ProductName = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ProductDescription = "Libro sulla scrittura di codice pulito e manutenibile",
                    Quantity = 3,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 45.00m,
                    DiscountPercentage = 15.0m,
                    DiscountAmount = 20.25m,
                    TaxPercentage = 4.0m,
                    TaxAmount = 4.95m,
                    LineTotal = 119.70m
                },
                new OrderLine
                {
                    LineNumber = 2,
                    ProductId = "PROD-302",
                    ProductCode = "BOOK-002",
                    ProductName = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    ProductDescription = "Classico libro sui design pattern in programmazione",
                    Quantity = 2,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 55.00m,
                    DiscountPercentage = 15.0m,
                    DiscountAmount = 16.50m,
                    TaxPercentage = 4.0m,
                    TaxAmount = 3.74m,
                    LineTotal = 93.24m
                }
            },
            SubTotal = 200.00m,
            TaxAmount = 8.69m,
            ShippingCost = 5.00m,
            DiscountAmount = 36.75m,
            TotalAmount = 176.94m,
            Currency = "EUR",
            Notes = "Regalo per team di sviluppo",
            CreatedAt = new DateTime(2024, 3, 5, 9, 15, 0),
            CreatedBy = "user003",
            UpdatedAt = new DateTime(2024, 3, 8, 17, 0, 0),
            UpdatedBy = "system"
        };
    }

    private static Order CreateOrder4()
    {
        return new Order
        {
            Version = 1,
            
            OrderNumber = "ORD-2024-004",
            OrderDate = new DateTime(2024, 4, 12, 16, 45, 0),
            DeliveryDate = new DateTime(2024, 4, 18, 12, 0, 0),
            CustomerId = "CUST-004",
            CustomerName = "Anna Neri",
            CustomerEmail = "anna.neri@example.com",
            ShippingAddress = new Address
            {
                Street = "Piazza Duomo 1",
                City = "Firenze",
                PostalCode = "50122",
                Province = "FI",
                Country = "IT"
            },
            BillingAddress = new Address
            {
                Street = "Piazza Duomo 1",
                City = "Firenze",
                PostalCode = "50122",
                Province = "FI",
                Country = "IT"
            },
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.DebitCard,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineNumber = 1,
                    ProductId = "PROD-401",
                    ProductCode = "TABLET-001",
                    ProductName = "iPad Pro 12.9 pollici",
                    ProductDescription = "Tablet Apple 12.9 pollici, 256GB, Wi-Fi + Cellular",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 1299.00m,
                    DiscountPercentage = 0m,
                    DiscountAmount = 0m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 285.78m,
                    LineTotal = 1584.78m
                },
                new OrderLine
                {
                    LineNumber = 2,
                    ProductId = "PROD-402",
                    ProductCode = "PEN-001",
                    ProductName = "Apple Pencil 2",
                    ProductDescription = "Penna digitale per iPad Pro",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 129.00m,
                    DiscountPercentage = 0m,
                    DiscountAmount = 0m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 28.38m,
                    LineTotal = 157.38m
                },
                new OrderLine
                {
                    LineNumber = 3,
                    ProductId = "PROD-403",
                    ProductCode = "KEYBOARD-001",
                    ProductName = "Magic Keyboard per iPad Pro",
                    ProductDescription = "Tastiera con trackpad integrato",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 349.00m,
                    DiscountPercentage = 5.0m,
                    DiscountAmount = 17.45m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 72.96m,
                    LineTotal = 404.51m
                }
            },
            SubTotal = 1777.00m,
            TaxAmount = 387.12m,
            ShippingCost = 25.00m,
            DiscountAmount = 17.45m,
            TotalAmount = 2171.67m,
            Currency = "EUR",
            Notes = "Consegna in ufficio, piano 3",
            CreatedAt = new DateTime(2024, 4, 12, 16, 45, 0),
            CreatedBy = "user004"
        };
    }

    private static Order CreateOrder5()
    {
        return new Order
        {
            Version = 1,
            
            OrderNumber = "ORD-2024-005",
            OrderDate = new DateTime(2024, 5, 20, 11, 0, 0),
            DeliveryDate = null,
            CustomerId = "CUST-005",
            CustomerName = "Paolo Ferrari",
            CustomerEmail = "paolo.ferrari@example.com",
            ShippingAddress = new Address
            {
                Street = "Via Venezia 234",
                City = "Bologna",
                PostalCode = "40121",
                Province = "BO",
                Country = "IT"
            },
            BillingAddress = new Address
            {
                Street = "Via Venezia 234",
                City = "Bologna",
                PostalCode = "40121",
                Province = "BO",
                Country = "IT"
            },
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.Cash,
            OrderLines = new List<OrderLine>
            {
                new OrderLine
                {
                    LineNumber = 1,
                    ProductId = "PROD-501",
                    ProductCode = "HEADPHONE-001",
                    ProductName = "Cuffie Sony WH-1000XM5",
                    ProductDescription = "Cuffie wireless con cancellazione attiva del rumore",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 399.99m,
                    DiscountPercentage = 20.0m,
                    DiscountAmount = 80.00m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 70.40m,
                    LineTotal = 390.39m
                },
                new OrderLine
                {
                    LineNumber = 2,
                    ProductId = "PROD-502",
                    ProductCode = "SPEAKER-001",
                    ProductName = "Altoparlante Bluetooth JBL Charge 5",
                    ProductDescription = "Altoparlante portatile resistente all'acqua",
                    Quantity = 1,
                    UnitOfMeasure = "PZ",
                    UnitPrice = 179.99m,
                    DiscountPercentage = 10.0m,
                    DiscountAmount = 18.00m,
                    TaxPercentage = 22.0m,
                    TaxAmount = 35.64m,
                    LineTotal = 197.63m
                }
            },
            SubTotal = 579.98m,
            TaxAmount = 106.04m,
            ShippingCost = 10.00m,
            DiscountAmount = 98.00m,
            TotalAmount = 598.02m,
            Currency = "EUR",
            Notes = "Ritiro in negozio",
            CreatedAt = new DateTime(2024, 5, 20, 11, 0, 0),
            CreatedBy = "user005"
        };
    }
}
