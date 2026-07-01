using Microsoft.Extensions.DependencyInjection;
using MiPlantillaBase.Core.Interfaces;
using MiPlantillaBase.Features.Dashboard;
using MiPlantillaBase.Infrastructure.Services;

namespace MiPlantillaBase.Infrastructure;

/// <summary>
/// ÚNICO punto de registro de dependencias.
/// No crees otro ServiceProvider en toda la aplicación.
/// </summary>
public static class AppServices
{
    private static IServiceProvider? _provider;

    public static IServiceProvider Configure()
    {
        if (_provider is not null)
            return _provider;

        ServiceCollection services = [];

        // ── Servicios de infraestructura ──────────────────────
        services.AddSingleton<IDataService, DesignTimeDataService>();

        // ── ViewModels ────────────────────────────────────────
        services.AddTransient<DashboardViewModel>();

        // ── Pages ─────────────────────────────────────────────
        services.AddTransient<DashboardPage>();

        _provider = services.BuildServiceProvider();
        return _provider;
    }
}
