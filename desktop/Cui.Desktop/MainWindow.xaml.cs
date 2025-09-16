using System.Windows;
using Cui.Desktop.ViewModels;
using Telerik.Windows.Controls;

namespace Cui.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.LoadSampleConversation();
        }
    }

    private void RadListView_SelectionChanged(object sender, SelectionChangeEventArgs e)
    {
        if (sender is RadListView listView)
        {
            listView.SelectedItems.Clear();
        }
    }
}
