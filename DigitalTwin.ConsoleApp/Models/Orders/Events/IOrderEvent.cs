namespace DigitalTwin.ConsoleApp.Models.Orders.Events;

public interface IOrderEvent
{
    string OrderNumber { get; }
    int Version { get; }
    DateTime EventTimestamp { get; }
    string EventType { get; }
}

