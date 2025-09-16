using System;
using System.Collections.Generic;

namespace Cui.Desktop.Models;

public sealed class ToolInvocation
{
    public string ToolName { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public IDictionary<string, object?> Arguments { get; init; } = new Dictionary<string, object?>();
    public ToolResult Result { get; init; } = ToolResult.Pending();
}

public enum ToolExecutionStatus
{
    Pending,
    Completed
}

public sealed class ToolResult
{
    private ToolResult(ToolExecutionStatus status)
    {
        Status = status;
    }

    public ToolExecutionStatus Status { get; }
    public bool IsError { get; init; }
    public string? Output { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<ChatMessage> ChildMessages { get; init; } = Array.Empty<ChatMessage>();
    public DateTimeOffset? CompletedAt { get; init; }

    public static ToolResult Pending() => new(ToolExecutionStatus.Pending);

    public static ToolResult Completed(string? output = null, bool isError = false, string? errorMessage = null, IEnumerable<ChatMessage>? children = null)
    {
        return new ToolResult(ToolExecutionStatus.Completed)
        {
            Output = output,
            IsError = isError,
            ErrorMessage = errorMessage,
            ChildMessages = children is null ? Array.Empty<ChatMessage>() : new List<ChatMessage>(children),
            CompletedAt = DateTimeOffset.UtcNow
        };
    }
}
