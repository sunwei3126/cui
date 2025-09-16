using System.Windows;
using System.Windows.Controls;
using Cui.Desktop.ViewModels;

namespace Cui.Desktop.Selectors;

public sealed class ToolContentTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ReadTemplate { get; set; }
    public DataTemplate? EditTemplate { get; set; }
    public DataTemplate? TaskTemplate { get; set; }
    public DataTemplate? GenericTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not ToolContentViewModel content)
        {
            return base.SelectTemplate(item, container);
        }

        return content switch
        {
            ReadToolContentViewModel => ReadTemplate,
            EditToolContentViewModel => EditTemplate,
            TaskToolContentViewModel => TaskTemplate,
            _ => GenericTemplate
        };
    }
}
