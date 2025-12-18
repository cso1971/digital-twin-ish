namespace DigitalTwin.ConsoleApp.Models;

public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultModel { get; set; } = "llama3.2";
}

public class QdrantSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6334;
    public int GrpcPort { get; set; } = 6333;
    public bool UseHttps { get; set; } = false;
}

