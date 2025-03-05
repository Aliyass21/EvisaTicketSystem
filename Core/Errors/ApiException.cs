using System;

namespace EVisaTicketSystem.Core;

public class ApiException( int StatusCode , string message, string?details)
{
    public int StatusCode { get; set; } = StatusCode;
    public string Message { get; set; } = message;
    public string? Details { get; set; } = details;

}
