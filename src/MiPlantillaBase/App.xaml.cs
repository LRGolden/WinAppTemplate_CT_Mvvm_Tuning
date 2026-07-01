using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace MiPlantillaBase;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        Services = Infrastructure.AppServices.Configure();
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window? m_window;
}
