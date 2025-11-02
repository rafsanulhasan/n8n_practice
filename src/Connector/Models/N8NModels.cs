using System.Diagnostics.CodeAnalysis;

namespace N8N.Connector.Models;

[ExcludeFromCodeCoverage]
public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class UserRegistrationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public User? User { get; set; }
    public List<string>? Errors { get; set; }
}

[ExcludeFromCodeCoverage]
public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class SimpleWebhookRequest
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object>? Data { get; set; }
}

[ExcludeFromCodeCoverage]
public class SimpleWebhookResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? ReceivedData { get; set; }
    public string ProcessedAt { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class DataProcessingRequest
{
    public List<DataItem>? Items { get; set; }
}

[ExcludeFromCodeCoverage]
public class DataItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class DataProcessingResponse
{
    public List<ProcessedDataItem>? Items { get; set; }
    public int TotalCount { get; set; }
    public string ProcessingTimestamp { get; set; } = string.Empty;
    public ProcessingStatistics? Statistics { get; set; }
}

[ExcludeFromCodeCoverage]
public class ProcessedDataItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ProcessedAt { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public class ProcessingStatistics
{
    public int TotalItems { get; set; }
    public string ProcessedAt { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
