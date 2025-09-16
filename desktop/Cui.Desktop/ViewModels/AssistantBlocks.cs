using Cui.Desktop.Models;

namespace Cui.Desktop.ViewModels;

public abstract class AssistantBlockViewModel : ViewModelBase
{
    protected AssistantBlockViewModel(MessageContentBlock source)
    {
        Source = source;
    }

    public MessageContentBlock Source { get; }
}

public sealed class TextBlockViewModel : AssistantBlockViewModel
{
    public TextBlockViewModel(TextContentBlock block)
        : base(block)
    {
        Markdown = block.Markdown;
    }

    public string Markdown { get; }
}

public sealed class ThinkingBlockViewModel : AssistantBlockViewModel
{
    public ThinkingBlockViewModel(ThinkingContentBlock block)
        : base(block)
    {
        Markdown = block.Markdown;
    }

    public string Markdown { get; }
}

public sealed class JsonBlockViewModel : AssistantBlockViewModel
{
    public JsonBlockViewModel(JsonContentBlock block)
        : base(block)
    {
        Json = block.Json;
    }

    public string Json { get; }
}

public sealed class ToolUseBlockViewModel : AssistantBlockViewModel
{
    public ToolUseBlockViewModel(ToolUseContentBlock block, ToolInvocationViewModel invocation)
        : base(block)
    {
        Invocation = invocation;
    }

    public ToolInvocationViewModel Invocation { get; }
}
