using System.Windows;
using System.Windows.Controls;
using Cui.Desktop.ViewModels;

namespace Cui.Desktop.Selectors;

public sealed class MessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? UserTemplate { get; set; }
    public DataTemplate? AssistantTemplate { get; set; }
    public DataTemplate? ErrorTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not MessageGroupViewModel message)
        {
            return base.SelectTemplate(item, container);
        }

        return message.MessageType switch
        {
            Models.ChatMessageType.User => UserTemplate,
            Models.ChatMessageType.Assistant => AssistantTemplate,
            Models.ChatMessageType.Error => ErrorTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
