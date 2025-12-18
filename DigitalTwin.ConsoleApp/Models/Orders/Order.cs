namespace DigitalTwin.ConsoleApp.Models.Orders;

public record Order
{
    private static int _orderCounter;
    private static readonly Random Random = new();
    private static readonly Lock Lock = new();

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

    public static Order Create()
    {
        lock (Lock)
        {
            _orderCounter++;
            var orderNumber = $"ORD-{DateTime.UtcNow.Year}-{_orderCounter:D6}";
            var customerNumber = _orderCounter;
            var customerId = $"CUST-{customerNumber:D6}";

            var firstNames = new[] { "Mario", "Giulia", "Luca", "Anna", "Paolo", "Sara", "Marco", "Elena", "Andrea", "Chiara", "Francesco", "Valentina", "Alessandro", "Martina", "Davide" };
            var lastNames = new[] { "Rossi", "Bianchi", "Verdi", "Neri", "Ferrari", "Romano", "Colombo", "Ricci", "Marino", "Greco", "Bruno", "Gallo", "Conti", "Costa", "Fontana" };
            var cities = new[] { "Milano", "Roma", "Torino", "Firenze", "Bologna", "Napoli", "Palermo", "Genova", "Venezia", "Verona", "Padova", "Brescia", "Parma", "Modena", "Reggio Emilia" };
            var streets = new[] { "Via Roma", "Corso Garibaldi", "Via Firenze", "Piazza Duomo", "Via Venezia", "Via Verdi", "Corso Vittorio Emanuele", "Via Dante", "Piazza Navona", "Via Mazzini" };
            var provinces = new[] { "MI", "RM", "TO", "FI", "BO", "NA", "PA", "GE", "VE", "VR", "PD", "BS", "PR", "MO", "RE" };
            var postalCodes = new[] { "20100", "00100", "10121", "50122", "40121", "80121", "90121", "16121", "30121", "37121", "35121", "25121", "43121", "41121", "42121" };

            var productCategories = new[] { "Laptop", "Smartphone", "Tablet", "Monitor", "Tastiera", "Mouse", "Cuffie", "Altoparlante", "Webcam", "Stampante" };
            var productBrands = new[] { "Dell", "Samsung", "Apple", "HP", "Lenovo", "Logitech", "Sony", "JBL", "Canon", "Epson" };
            var productModels = new[] { "XPS 15", "Galaxy S24", "iPad Pro", "UltraSharp", "MX Keys", "MX Master", "WH-1000XM5", "Charge 5", "EOS R5", "EcoTank" };

            var firstName = firstNames[Random.Next(firstNames.Length)];
            var lastName = lastNames[Random.Next(lastNames.Length)];
            var customerName = $"{firstName} {lastName}";
            var customerEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";

            var cityIndex = Random.Next(cities.Length);
            var city = cities[cityIndex];
            var province = provinces[cityIndex];
            var postalCode = postalCodes[cityIndex];
            var streetNumber = Random.Next(1, 500);
            var street = $"{streets[Random.Next(streets.Length)]} {streetNumber}";

            var shippingAddress = new Address
            {
                Street = street,
                City = city,
                PostalCode = postalCode,
                Province = province,
                Country = "IT"
            };

            var useDifferentBilling = Random.Next(100) < 30;
            var billingAddress = useDifferentBilling
                ? new Address
                {
                    Street = $"{streets[Random.Next(streets.Length)]} {Random.Next(1, 500)}",
                    City = cities[Random.Next(cities.Length)],
                    PostalCode = postalCodes[Random.Next(postalCodes.Length)],
                    Province = provinces[Random.Next(provinces.Length)],
                    Country = "IT"
                }
                : shippingAddress;

            var status = (OrderStatus)Random.Next(Enum.GetValues<OrderStatus>().Length);
            var paymentMethod = (PaymentMethod)Random.Next(Enum.GetValues<PaymentMethod>().Length);

            var numLines = Random.Next(1, 5);
            var orderLines = new List<OrderLine>();
            decimal subTotal = 0;

            for (int i = 0; i < numLines; i++)
            {
                var category = productCategories[Random.Next(productCategories.Length)];
                var brand = productBrands[Random.Next(productBrands.Length)];
                var model = productModels[Random.Next(productModels.Length)];
                var productName = $"{category} {brand} {model}";
                var productId = $"PROD-{_orderCounter * 100 + i + 1:D3}";
                var productCode = $"{category.ToUpper()}-{Random.Next(1000, 9999)}";
                var productDescription = $"{category} {brand} {model}, caratteristiche premium e design moderno";
                var quantity = Random.Next(1, 4);
                var unitPrice = Math.Round((decimal)(Random.NextDouble() * 1500 + 50), 2);
                var discountPercentage = Random.Next(100) < 40 ? Math.Round((decimal)(Random.NextDouble() * 20), 2) : 0;
                var discountAmount = Math.Round(unitPrice * quantity * discountPercentage / 100, 2);
                var taxPercentage = category == "Laptop" || category == "Smartphone" || category == "Tablet" ? 22.0m : 4.0m;
                var lineSubtotal = (unitPrice * quantity) - discountAmount;
                var taxAmount = Math.Round(lineSubtotal * taxPercentage / 100, 2);
                var lineTotal = lineSubtotal + taxAmount;

                orderLines.Add(new OrderLine
                {
                    LineNumber = i + 1,
                    ProductId = productId,
                    ProductCode = productCode,
                    ProductName = productName,
                    ProductDescription = productDescription,
                    Quantity = quantity,
                    UnitOfMeasure = "PZ",
                    UnitPrice = unitPrice,
                    DiscountPercentage = discountPercentage,
                    DiscountAmount = discountAmount,
                    TaxPercentage = taxPercentage,
                    TaxAmount = taxAmount,
                    LineTotal = lineTotal
                });

                subTotal += lineSubtotal;
            }

            var orderDiscount = Random.Next(100) < 30 ? Math.Round((decimal)(Random.NextDouble() * 100), 2) : 0;
            var shippingCost = Random.Next(100) < 70 ? Math.Round((decimal)(Random.NextDouble() * 30), 2) : 0;
            var taxAmount1 = orderLines.Sum(l => l.TaxAmount);
            var totalAmount = subTotal - orderDiscount + shippingCost + taxAmount1;

            var notes = Random.Next(100) < 40
                ? new[] { "Consegna urgente richiesta", "Pagamento anticipato ricevuto", "Regalo per team di sviluppo", "Consegna in ufficio, piano 3", "Ritiro in negozio", "Consegna al piano terra" }[Random.Next(6)]
                : null;

            var createdAt = DateTime.UtcNow.AddDays(-Random.Next(0, 90)).AddHours(-Random.Next(0, 24));
            var updatedAt = Random.Next(100) < 60 ? createdAt.AddHours(Random.Next(1, 48)) : (DateTime?)null;
            var createdBy = $"user{Random.Next(1, 100):D3}";
            var updatedBy = updatedAt.HasValue ? $"user{Random.Next(1, 100):D3}" : null;
            var deliveryDate = Random.Next(100) < 80 ? createdAt.AddDays(Random.Next(3, 14)) : (DateTime?)null;

            return new Order
            {
                Version = 1,
                OrderNumber = orderNumber,
                OrderDate = createdAt,
                DeliveryDate = deliveryDate,
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                Status = status,
                PaymentMethod = paymentMethod,
                OrderLines = orderLines,
                SubTotal = Math.Round(subTotal, 2),
                TaxAmount = Math.Round(taxAmount1, 2),
                ShippingCost = shippingCost,
                DiscountAmount = orderDiscount,
                TotalAmount = Math.Round(totalAmount, 2),
                Currency = "EUR",
                Notes = notes,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy
            };
        }
    }
}

public record OrderLine
{
    public int LineNumber { get; init; }
    public string ProductId { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string? ProductDescription { get; init; }
    public int Quantity { get; init; }
    public string UnitOfMeasure { get; init; } = "PZ";
    public decimal UnitPrice { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxPercentage { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal LineTotal { get; init; }
    public string? Notes { get; init; }
}

public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = "IT";
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Returned
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    BankTransfer,
    PayPal,
    Other
}
