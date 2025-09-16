using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Cui.Desktop.Models;
using Cui.Desktop.Services;

namespace Cui.Desktop.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IConversationDataSource _dataSource;
    private bool _isStreaming;
    private bool _hasPendingToolResults;

    public MainViewModel()
        : this(new SampleDataService())
    {
    }

    public MainViewModel(IConversationDataSource dataSource)
    {
        _dataSource = dataSource;
        LoadSampleConversationCommand = new RelayCommand(LoadSampleConversation);
        Messages.CollectionChanged += OnMessagesCollectionChanged;
    }

    public ObservableCollection<MessageGroupViewModel> Messages { get; } = new();

    public bool IsStreaming
    {
        get => _isStreaming;
        private set
        {
            if (SetProperty(ref _isStreaming, value))
            {
                RaisePropertyChanged(nameof(ShowStreamingIndicator));
            }
        }
    }

    public bool HasPendingToolResults
    {
        get => _hasPendingToolResults;
        private set
        {
            if (SetProperty(ref _hasPendingToolResults, value))
            {
                RaisePropertyChanged(nameof(ShowStreamingIndicator));
            }
        }
    }

    public bool ShowStreamingIndicator => IsStreaming && !HasPendingToolResults;

    public RelayCommand LoadSampleConversationCommand { get; }

    public async void LoadSampleConversation()
    {
        IsStreaming = true;
        try
        {
            var conversation = await _dataSource.GetConversationAsync();
            LoadConversation(conversation);
        }
        finally
        {
            IsStreaming = false;
        }
    }

    public void LoadConversation(IEnumerable<ChatMessage> rawMessages)
    {
        foreach (var group in Messages.ToList())
        {
            group.PropertyChanged -= OnMessageGroupPropertyChanged;
        }

        Messages.Clear();
        MessageGroupViewModel? current = null;

        foreach (var message in FilterMessages(rawMessages))
        {
            if (current is null || current.MessageType != message.Type)
            {
                current = MessageGroupViewModel.FromMessage(message);
                Messages.Add(current);
            }
            else
            {
                current.Append(message);
            }
        }

        UpdatePendingToolFlag();
    }

    public void UpdateStreamingState(bool isStreaming)
    {
        IsStreaming = isStreaming;
        UpdatePendingToolFlag();
    }

    private IEnumerable<ChatMessage> FilterMessages(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            if (message.Type == ChatMessageType.User && message.Blocks.Count > 0 && message.Blocks.All(block => block is ToolUseContentBlock))
            {
                continue;
            }

            yield return message;
        }
    }

    private void UpdatePendingToolFlag()
    {
        HasPendingToolResults = Messages.Any(group => group.HasPendingToolResult);
    }

    private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is MessageGroupViewModel group)
                {
                    group.PropertyChanged += OnMessageGroupPropertyChanged;
                }
            }
        }

        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is MessageGroupViewModel group)
                {
                    group.PropertyChanged -= OnMessageGroupPropertyChanged;
                }
            }
        }

        UpdatePendingToolFlag();
    }

    private void OnMessageGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MessageGroupViewModel.HasPendingToolResult))
        {
            UpdatePendingToolFlag();
        }
    }
}
