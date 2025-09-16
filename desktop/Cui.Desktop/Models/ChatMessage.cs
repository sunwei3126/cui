using System;
using System.Collections.Generic;

namespace Cui.Desktop.Models;

public enum ChatMessageType
{
    User,
    Assistant,
    Error,
    System
}

public sealed class ChatMessage
{
    public ChatMessageType Type { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public string? WorkingDirectory { get; init; }
    public string? Text { get; init; }
    public IList<MessageContentBlock> Blocks { get; init; } = new List<MessageContentBlock>();
}

public abstract class MessageContentBlock
{
    protected MessageContentBlock(ContentBlockType blockType)
    {
        BlockType = blockType;
    }

    public ContentBlockType BlockType { get; }
}

public enum ContentBlockType
{
    Text,
    Thinking,
    ToolUse,
    Json
}

public sealed class TextContentBlock : MessageContentBlock
{
    public TextContentBlock(string markdown)
        : base(ContentBlockType.Text)
    {
        Markdown = markdown;
    }

    public string Markdown { get; }
}

public sealed class ThinkingContentBlock : MessageContentBlock
{
    public ThinkingContentBlock(string markdown)
        : base(ContentBlockType.Thinking)
    {
        Markdown = markdown;
    }

    public string Markdown { get; }
}

public sealed class JsonContentBlock : MessageContentBlock
{
    public JsonContentBlock(string json)
        : base(ContentBlockType.Json)
    {
        Json = json;
    }

    public string Json { get; }
}

public sealed class ToolUseContentBlock : MessageContentBlock
{
    public ToolUseContentBlock(ToolInvocation invocation)
        : base(ContentBlockType.ToolUse)
    {
        Invocation = invocation;
    }

    public ToolInvocation Invocation { get; }
}
