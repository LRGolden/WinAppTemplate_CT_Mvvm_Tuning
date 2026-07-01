# Architecture — WinAppTemplate CT Mvvm Tuning

## Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 10 + Windows App SDK |
| UI | WinUI 3 (NavigationView + Frame) |
| MVVM | CommunityToolkit.Mvvm (source generators) |
| DI | Microsoft.Extensions.DependencyInjection |
| Tests | xUnit |

---

## Folder Structure

```
src/
├── MyApp/
│   ├── Core/
│   │   ├── ViewModelBase.cs          ← inherits ObservableObject
│   │   ├── Interfaces/               ← contracts (IDataService, …)
│   │   └── Models/                   ← domain models
│   ├── Features/
│   │   ├── Dashboard/                ← working example
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── DashboardPage.xaml
│   │   │   └── DashboardPage.xaml.cs
│   │   ├── Payments/                 ← recommended: billing, invoicing
│   │   ├── Trackers/                 ← recommended: usage, metrics
│   │   ├── Telemetry/                ← recommended: diagnostics, logging
│   │   └── Queries/                  ← recommended: reports, search
│   └── Infrastructure/
│       ├── AppServices.cs            ← SINGLE DI registration point
│       └── Services/
│           └── DesignTimeDataService.cs
├── scripts/
│   └── rename-template.bat           ← auto-rename namespace
├── docs/
│   ├── ARCHITECTURE.md               ← this file (EN)
│   └── ARCHITECTURE.es.md            ← same content (ES)
├── .gitignore
└── README.md
```

---

## Conventions

### 1. Single ServiceProvider — never scatter DI

All dependencies are registered in **one file**: `Infrastructure/AppServices.cs`.

```csharp
services.AddSingleton<IDataService, DesignTimeDataService>();   // services
services.AddTransient<DashboardViewModel>();                    // ViewModels
services.AddTransient<DashboardPage>();                         // Pages
```

Never create a second `BuildServiceProvider()` anywhere in the app.

> **For beginners:** DI (Dependency Injection) means you don't create objects manually with `new`.  
> Instead, you list them here and the framework gives them to you where needed.  
> This makes your code testable and decoupled.

### 2. Source generators — zero reflection

CommunityToolkit.Mvvm generates code at compile time:

```csharp
[ObservableProperty]       // → generates public property + change notification
private string _title;

[RelayCommand]             // → generates ICommand from this method
private void DoSomething() { }
```

No `INotifyPropertyChanged` boilerplate. No reflection. AOT-friendly.

> **For beginners:** `[ObservableProperty]` is a magic attribute. Write the private field, and the toolkit automatically creates a public property that the UI can bind to. `[RelayCommand]` turns a method into a button command.

### 3. Feature-first organization

Each feature is **self-contained** in its own folder under `Features/`:

```
Features/Payments/
├── PaymentsViewModel.cs    ← logic + state
├── PaymentsPage.xaml       ← UI layout
├── PaymentsPage.xaml.cs    ← binds ViewModel
└── PaymentsService.cs      ← optional: feature-specific data logic
```

This keeps related files close. No hunting across folders.

### 4. Navigation — code-behind, no abstraction

Navigation uses **NavigationView + Frame** with simple event handlers in `MainWindow.xaml.cs`:

```csharp
private void OnNavItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
{
    ContentFrame.Navigate(args switch
    {
        "Dashboard" => typeof(DashboardPage),
        _           => typeof(DashboardPage)
    });
}
```

Why not an `INavigationService`? For solo devs and small teams, a direct event handler is simpler, easier to debug, and one less abstraction to maintain.

---

## Platform & AOT Notes

| Platform | AOT Support | Details |
|----------|-------------|---------|
| **WinUI 3** | Partial | Code layer is AOT-ready. WinUI framework itself has some runtime reflection. Test your specific features. |
| **Avalonia** | Full | The Core + Infrastructure layers are platform-agnostic. Swap `NavigationView` for Avalonia's `Window`. |
| **WPF** | No | MVVM pattern works identically, but .NET WPF doesn't support NativeAOT. |

**To enable AOT publishing** (after verifying compatibility with your features):

```bash
# 1. Uncomment <PublishAot>true</PublishAot> in .csproj
# 2. Publish
dotnet publish -r win-x64 -c Release
```

---

## Usage Protocol

### 1. Clone the template

Copy the entire folder and rename it to your project name.

### 2. Rename the namespace

**Option A — Automatic (recommended)**

```bash
scripts\rename-template.bat YourProjectName
```

This replaces `MiPlantillaBase` in:
- File and folder names
- File contents (code, XAML, csproj)

**Option B — Manual**

In VS Code: `Ctrl+Shift+H` → replace `MiPlantillaBase` with `YourProjectName`

Checklist of files that contain the namespace:
- `*.cs` files
- `*.xaml` files (App.xaml, MainWindow.xaml, pages)
- `*.csproj`
- `*.slnx`
- `app.manifest`

### 3. Register new dependencies

Open `Infrastructure/AppServices.cs` and add your services and ViewModels:

```csharp
services.AddTransient<PaymentsViewModel>();
services.AddSingleton<IPaymentService, PaymentService>();
services.AddTransient<PaymentsPage>();
```

### 4. Create a new feature

Copy the `Dashboard/` pattern:

```
Features/YourFeature/
├── YourFeatureViewModel.cs
├── YourFeaturePage.xaml
└── YourFeaturePage.xaml.cs
```

ViewModel pattern:
```csharp
public sealed partial class YourFeatureViewModel : ViewModelBase
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _title = "Your Feature";

    public YourFeatureViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    private void DoSomething() { }
}
```

Page pattern:
```csharp
public sealed partial class YourFeaturePage : Page
{
    public YourFeatureViewModel ViewModel { get; }

    public YourFeaturePage()
    {
        ViewModel = App.Services.GetRequiredService<YourFeatureViewModel>();
        InitializeComponent();
    }
}
```

### 5. Register navigation

- Add a `NavigationViewItem` in `MainWindow.xaml`
- Add a `case` in the `OnNavItemInvoked` switch

### 6. Build & test

```bash
dotnet build
dotnet test
```

---

## Principles Summary

| Principle | Why |
|-----------|-----|
| Single `ServiceProvider` | No hunting for DI registrations. One file to rule them all. |
| Source generators | Zero reflection. AOT-safe. Less code to write. |
| Self-contained features | Related files stay together. Easy to add / remove / refactor. |
| Tests from day one | xUnit included. Test your ViewModels without launching the UI. |
| Code-behind navigation | The simplest thing that works. No over-engineering. |
| Infrastructure as utility | `AppServices.cs` is the only "magic" file. Everything else is plain MVVM. |

---

## Recommended Feature Areas

These subfolders are pre-created in `Features/` as suggestions:

| Folder | Typical Use |
|--------|-------------|
| `Dashboard` | Main landing page with overview data |
| `Payments` | Billing, invoices, subscriptions |
| `Trackers` | Usage metrics, activity logs |
| `Telemetry` | App diagnostics, error reporting, performance |
| `Queries` | Search, filtered reports, data export |
