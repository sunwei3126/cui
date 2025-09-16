using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Cui.Desktop.Models;

namespace Cui.Desktop.ViewModels;

public sealed class MessageGroupViewModel : ViewModelBase
{
    private readonly List<ChatMessage> _messages = new();
    private readonly ObservableCollection<AssistantBlockViewModel> _assistantBlocks = new();
    private readonly RelayCommand _toggleExpandCommand;
    private bool _isExpanded;
    private string _combinedUserText = string.Empty;
    private string _previewText = string.Empty;
    private string _remainingText = string.Empty;

    private MessageGroupViewModel(ChatMessageType messageType)
    {
        MessageType = messageType;
        _toggleExpandCommand = new RelayCommand(() => IsExpanded = !IsExpanded, () => HasOverflowText);
    }

    public ChatMessageType MessageType { get; }
    public IReadOnlyList<ChatMessage> Messages => _messages;
    public ObservableCollection<AssistantBlockViewModel> AssistantBlocks => _assistantBlocks;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value))
            {
                RaisePropertyChanged(nameof(ExpandLabel));
            }
        }
    }

    public string CombinedUserText
    {
        get => _combinedUserText;
        private set => SetProperty(ref _combinedUserText, value);
    }

    public string PreviewText
    {
        get => _previewText;
        private set => SetProperty(ref _previewText, value);
    }

    public string RemainingText
    {
        get => _remainingText;
        private set
        {
            if (SetProperty(ref _remainingText, value))
            {
                RaisePropertyChanged(nameof(HasOverflowText));
                _toggleExpandCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasOverflowText => !string.IsNullOrWhiteSpace(RemainingText);
    public string ExpandLabel => IsExpanded ? "收起" : "展开剩余";
    public RelayCommand ToggleExpandCommand => _toggleExpandCommand;

    public bool HasPendingToolResult => AssistantBlocks.OfType<ToolUseBlockViewModel>().Any(block => block.Invocation.Result.IsPending);

    public static MessageGroupViewModel FromMessage(ChatMessage message)
    {
        var viewModel = new MessageGroupViewModel(message.Type);
        viewModel.Append(message);
        return viewModel;
    }

    public void Append(ChatMessage message)
    {
        _messages.Add(message);

        switch (MessageType)
        {
            case ChatMessageType.User:
                AppendUserMessage(message);
                break;
            case ChatMessageType.Assistant:
                AppendAssistantMessage(message);
                break;
            case ChatMessageType.Error:
                AppendErrorMessage(message);
                break;
        }
    }

    private void AppendUserMessage(ChatMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            if (CombinedUserText.Length > 0)
            {
                CombinedUserText += "\n\n";
            }

            CombinedUserText += message.Text;
        }

        CalculateUserPreview();
    }

    private void CalculateUserPreview()
    {
        var lines = CombinedUserText.Split(['\n'], StringSplitOptions.None);
        if (lines.Length <= 8)
        {
            PreviewText = CombinedUserText;
            RemainingText = string.Empty;
            return;
        }

        PreviewText = string.Join("\n", lines.Take(8));
        RemainingText = string.Join("\n", lines.Skip(8));
    }

    private void AppendAssistantMessage(ChatMessage message)
    {
        foreach (var block in message.Blocks)
        {
            switch (block)
            {
                case TextContentBlock text:
                    AssistantBlocks.Add(new TextBlockViewModel(text));
                    break;
                case ThinkingContentBlock thinking:
                    AssistantBlocks.Add(new ThinkingBlockViewModel(thinking));
                    break;
                case JsonContentBlock json:
                    AssistantBlocks.Add(new JsonBlockViewModel(json));
                    break;
                case ToolUseContentBlock toolUse:
                    var invocationVm = new ToolInvocationViewModel(toolUse.Invocation);
                    invocationVm.Synchronize();
                    invocationVm.Result.PropertyChanged += OnToolResultChanged;
                    AssistantBlocks.Add(new ToolUseBlockViewModel(toolUse, invocationVm));
                    RaisePropertyChanged(nameof(HasPendingToolResult));
                    break;
            }
        }
    }

    private void AppendErrorMessage(ChatMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            if (CombinedUserText.Length > 0)
            {
                CombinedUserText += "\n\n";
            }

            CombinedUserText += message.Text;
        }
    }

    private void OnToolResultChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ToolResultViewModel.IsPending))
        {
            RaisePropertyChanged(nameof(HasPendingToolResult));
        }
    }
}
