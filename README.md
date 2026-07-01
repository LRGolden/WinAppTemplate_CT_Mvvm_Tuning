# WinAppTemplate — CT Mvvm Tuning

A pragmatic MVVM template for **WinUI 3** (.NET 10) with **CommunityToolkit.Mvvm**, designed for solo developers and small teams (1–3 people).

> Originally a personal project, adapted for public use.  
> MIT Licensed — free to use, modify, and share.

---

## Target Audience

- **Indie developers** building Windows desktop apps
- **Small teams** wanting predictable, low-ceremony architecture
- **Beginners** who find vanilla MVVM too abstract — this template gives you concrete conventions to follow
- **Anyone** who wants AOT-friendly code without fighting reflections

---

## Platform Compatibility

| Platform | AOT Support | Notes |
|----------|-------------|-------|
| **WinUI 3** | Partial | Primary target. Code is AOT-friendly; WinUI framework has some AOT limits |
| **Avalonia** | Full | Fully compatible — adapt the namespace and swap the UI layer |
| **WPF** | No | Compatible (it's just MVVM) but AOT is not available on .NET Framework / WPF |

> Technically, the Core + Infrastructure layers are platform-agnostic.  
> Swap `Features/Dashboard/DashboardPage.xaml` for the equivalent in your target framework.

---

## Quick Start

```bash
# 1. Copy the template folder
xcopy /E WinAppTemplate_CT_Mvvm_Tuning MyNewApp

# 2. Rename the namespace (pick ONE method)
#    Option A — Automatic (recommended)
cd MyNewApp
scripts\rename-template.bat MyNewApp

#    Option B — Manual (VS Code)
#    Ctrl+Shift+H → replace "MiPlantillaBase" with "MyNewApp"

# 3. Restore & build
dotnet restore
dotnet build
dotnet test
```

**Prerequisites:**
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Windows SDK (included with Visual Studio or via `dotnet workload install maui-windows`)

---

## Folder Structure

```
src/
├── MyApp/
│   ├── Core/                    ← shared base (ViewModelBase, interfaces, models)
│   ├── Features/                ← one folder per feature
│   │   ├── Dashboard/           ← working example (ViewModel + Page)
│   │   ├── Payments/            ← recommended area (invoicing, billing)
│   │   ├── Trackers/            ← recommended area (usage, metrics)
│   │   ├── Telemetry/           ← recommended area (diagnostics, logging)
│   │   └── Queries/             ← recommended area (reports, search)
│   └── Infrastructure/          ← DI setup, services
└── MyApp.Tests/                 ← xUnit tests
```

Each feature folder should contain:
- `XxxViewModel.cs` — logic + state
- `XxxPage.xaml` + `.xaml.cs` — UI bindings
- Optionally: `XxxService.cs` — if the feature has its own data logic

---

## Why This Template?

| Pain Point | Solution |
|------------|----------|
| DI scattered across the project | Single `AppServices.cs` for all registrations |
| Vanilla MVVM = too much boilerplate | Source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| Features mixed together | Each feature is self-contained in `Features/Xxx/` |
| AOT breaks with reflection | Zero reflection — code gen at compile time |
| No tests from day one | xUnit project included with a working example |
| "Where do I put this file?" | Conventions for Core / Features / Infrastructure |

---

## Documentation

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — full architecture, protocol, and beginner notes (EN)
- [`docs/ARCHITECTURE.es.md`](docs/ARCHITECTURE.es.md) — mismo contenido en español

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 10 + Windows App SDK |
| UI | WinUI 3 (NavigationView + Frame) |
| MVVM | CommunityToolkit.Mvvm 8.4.2 |
| DI | Microsoft.Extensions.DependencyInjection |
| Tests | xUnit |

---

## License

MIT — see [LICENSE](LICENSE) (or simply: use it, fork it, ship it).

If you find it useful, attribution is appreciated but not required.
