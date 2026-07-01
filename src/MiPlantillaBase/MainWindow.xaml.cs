using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MiPlantillaBase.Features.Dashboard;

namespace MiPlantillaBase;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = "MiPlantillaBase";
    }

    private void OnNavLoaded(object sender, RoutedEventArgs e)
    {
        NavView.SelectedItem = NavView.MenuItems[0];
        NavigateToPage(typeof(DashboardPage));
    }

    private void OnNavItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer?.Tag is string tag)
        {
            NavigateToPage(tag switch
            {
                "Dashboard" => typeof(DashboardPage),
                _ => typeof(DashboardPage)
            });
        }
    }

    private void NavigateToPage(Type pageType)
    {
        ContentFrame.Navigate(pageType);
    }
}
