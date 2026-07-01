using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiPlantillaBase.Core;
using MiPlantillaBase.Core.Interfaces;

namespace MiPlantillaBase.Features.Dashboard;

public sealed partial class DashboardViewModel : ViewModelBase
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _pageTitle = "Dashboard";

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private int _itemsLoaded;

    public DashboardViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    private void LoadData()
    {
        StatusMessage = "Loading...";
        ItemsLoaded = 42;
        StatusMessage = $"Loaded {ItemsLoaded} items";
    }

    [RelayCommand]
    private void Reset()
    {
        StatusMessage = "Ready";
        ItemsLoaded = 0;
    }
}
