using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MiPlantillaBase;

namespace MiPlantillaBase.Features.Dashboard;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage()
    {
        ViewModel = App.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();
    }
}
