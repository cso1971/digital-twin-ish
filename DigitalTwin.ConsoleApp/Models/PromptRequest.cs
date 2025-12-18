namespace DigitalTwin.ConsoleApp.Models;

// Request model for the endpoint
public record PromptRequest(string Prompt, string? Model = null);

