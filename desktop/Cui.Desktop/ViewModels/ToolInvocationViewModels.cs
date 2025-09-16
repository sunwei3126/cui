using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cui.Desktop.Models;

namespace Cui.Desktop.ViewModels;

public sealed class ToolInvocationViewModel : ViewModelBase
{
    private readonly ToolInvocation _invocation;

    public ToolInvocationViewModel(ToolInvocation invocation)
    {
        _invocation = invocation;
        Content = ToolContentViewModelFactory.Create(invocation);
    }

    public string ToolName => _invocation.ToolName;
    public string DisplayName => string.IsNullOrWhiteSpace(_invocation.DisplayName) ? _invocation.ToolName : _invocation.DisplayName!;
    public IReadOnlyDictionary<string, object?> Arguments => new ReadOnlyDictionary<string, object?>(_invocation.Arguments);
    public ToolResultViewModel Result { get; } = new();
    public ToolContentViewModel Content { get; }

    public void Synchronize()
    {
        Result.Update(_invocation.Result);
        Content.Synchronize(_invocation);
    }
}

public sealed class ToolResultViewModel : ViewModelBase
{
    private ToolExecutionStatus _status;
    private bool _isError;
    private string? _output;
    private string? _errorMessage;
    private IReadOnlyList<ChatMessage> _childMessages = Array.Empty<ChatMessage>();

    public ToolExecutionStatus Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public bool IsError
    {
        get => _isError;
        private set => SetProperty(ref _isError, value);
    }

    public string? Output
    {
        get => _output;
        private set => SetProperty(ref _output, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public IReadOnlyList<ChatMessage> ChildMessages
    {
        get => _childMessages;
        private set => SetProperty(ref _childMessages, value);
    }

    public void Update(ToolResult result)
    {
        Status = result.Status;
        IsError = result.IsError;
        Output = result.Output;
        ErrorMessage = result.ErrorMessage;
        ChildMessages = result.ChildMessages;
        RaisePropertyChanged(nameof(IsPending));
    }

    public bool IsPending => Status == ToolExecutionStatus.Pending;
}

public abstract class ToolContentViewModel : ViewModelBase
{
    public abstract string IconGlyph { get; }
    public abstract string Title { get; }
    public abstract void Synchronize(ToolInvocation invocation);
}

public sealed class GenericToolContentViewModel : ToolContentViewModel
{
    private IReadOnlyDictionary<string, object?> _arguments = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());
    private string? _output;

    public override string IconGlyph => "\uE80F"; // default icon
    public override string Title => "工具输出";

    public IReadOnlyDictionary<string, object?> Arguments
    {
        get => _arguments;
        private set => SetProperty(ref _arguments, value);
    }

    public string? Output
    {
        get => _output;
        private set => SetProperty(ref _output, value);
    }

    public override void Synchronize(ToolInvocation invocation)
    {
        Arguments = new ReadOnlyDictionary<string, object?>(invocation.Arguments);
        Output = invocation.Result.Output;
    }
}

public sealed class ReadToolContentViewModel : ToolContentViewModel
{
    private string? _path;
    private string? _preview;

    public override string IconGlyph => "\uE8A5";
    public override string Title => "读取文件";

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }

    public string? Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public override void Synchronize(ToolInvocation invocation)
    {
        if (invocation.Arguments.TryGetValue("path", out var value) && value is string str)
        {
            Path = str;
        }

        Preview = invocation.Result.Output;
    }
}

public sealed class EditToolContentViewModel : ToolContentViewModel
{
    private string? _path;
    private string? _diff;

    public override string IconGlyph => "\uE104";
    public override string Title => "修改文件";

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }

    public string? Diff
    {
        get => _diff;
        private set => SetProperty(ref _diff, value);
    }

    public override void Synchronize(ToolInvocation invocation)
    {
        if (invocation.Arguments.TryGetValue("path", out var value) && value is string str)
        {
            Path = str;
        }

        Diff = invocation.Result.Output;
    }
}

public sealed class TaskToolContentViewModel : ToolContentViewModel
{
    private IReadOnlyList<TaskChildMessageViewModel> _children = Array.Empty<TaskChildMessageViewModel>();

    public override string IconGlyph => "\uE0F3";
    public override string Title => "子任务";

    public IReadOnlyList<TaskChildMessageViewModel> Children
    {
        get => _children;
        private set => SetProperty(ref _children, value);
    }

    public override void Synchronize(ToolInvocation invocation)
    {
        var list = new List<TaskChildMessageViewModel>();
        foreach (var message in invocation.Result.ChildMessages)
        {
            list.Add(new TaskChildMessageViewModel(message));
        }

        Children = list;
    }
}

public static class ToolContentViewModelFactory
{
    public static ToolContentViewModel Create(ToolInvocation invocation)
    {
        return invocation.ToolName switch
        {
            "read" => new ReadToolContentViewModel(),
            "edit" => new EditToolContentViewModel(),
            "task" => new TaskToolContentViewModel(),
            _ => new GenericToolContentViewModel()
        };
    }
}

public sealed class TaskChildMessageViewModel
{
    public TaskChildMessageViewModel(ChatMessage message)
    {
        Message = message;
        Summary = BuildSummary(message);
    }

    public ChatMessage Message { get; }
    public string Summary { get; }
    public DateTimeOffset Timestamp => Message.Timestamp;

    private static string BuildSummary(ChatMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            return message.Text!;
        }

        foreach (var block in message.Blocks)
        {
            if (block is TextContentBlock text)
            {
                return text.Markdown;
            }

            if (block is ThinkingContentBlock thinking)
            {
                return thinking.Markdown;
            }
        }

        return "(无内容)";
    }
}
