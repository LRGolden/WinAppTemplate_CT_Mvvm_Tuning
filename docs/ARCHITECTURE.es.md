# Arquitectura — WinAppTemplate CT Mvvm Tuning

## Stack

| Capa | Tecnología |
|------|-----------|
| Framework | .NET 10 + Windows App SDK |
| UI | WinUI 3 (NavigationView + Frame) |
| MVVM | CommunityToolkit.Mvvm (source generators) |
| DI | Microsoft.Extensions.DependencyInjection |
| Tests | xUnit |

---

## Estructura de carpetas

```
src/
├── MiApp/
│   ├── Core/
│   │   ├── ViewModelBase.cs          ← hereda de ObservableObject
│   │   ├── Interfaces/               ← contratos (IDataService, …)
│   │   └── Models/                   ← modelos de dominio
│   ├── Features/
│   │   ├── Dashboard/                ← ejemplo funcional
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── DashboardPage.xaml
│   │   │   └── DashboardPage.xaml.cs
│   │   ├── Payments/                 ← recomendado: facturación, pagos
│   │   ├── Trackers/                 ← recomendado: uso, métricas
│   │   ├── Telemetry/                ← recomendado: diagnósticos, logging
│   │   └── Queries/                  ← recomendado: reportes, búsquedas
│   └── Infrastructure/
│       ├── AppServices.cs            ← ÚNICO punto de registro DI
│       └── Services/
│           └── DesignTimeDataService.cs
├── scripts/
│   └── rename-template.bat           ← renombra namespace automáticamente
├── docs/
│   ├── ARCHITECTURE.md               ← este archivo (EN)
│   └── ARCHITECTURE.es.md            ← mismo contenido (ES)
├── .gitignore
└── README.md
```

---

## Convenciones

### 1. Un solo ServiceProvider — nunca disperses la DI

Todas las dependencias se registran en **un solo archivo**: `Infrastructure/AppServices.cs`.

```csharp
services.AddSingleton<IDataService, DesignTimeDataService>();   // servicios
services.AddTransient<DashboardViewModel>();                    // ViewModels
services.AddTransient<DashboardPage>();                         // Pages
```

Nunca crees un segundo `BuildServiceProvider()` en toda la aplicación.

> **Para principiantes:** DI (Inyección de Dependencias) significa que no creas objetos manualmente con `new`.  
> Los listas aquí y el framework te los entrega donde los necesitas.  
> Esto hace tu código testeable y desacoplado.

### 2. Source generators — cero reflexión

CommunityToolkit.Mvvm genera código en tiempo de compilación:

```csharp
[ObservableProperty]       // → genera propiedad pública + notificación de cambio
private string _titulo;

[RelayCommand]             // → genera ICommand a partir de este método
private void HacerAlgo() { }
```

Sin boilerplate de `INotifyPropertyChanged`. Sin reflexión. Compatible con AOT.

> **Para principiantes:** `[ObservableProperty]` es un atributo mágico. Escribes el campo privado y el toolkit crea automáticamente una propiedad pública que la UI puede enlazar. `[RelayCommand]` convierte un método en un comando para botones.

### 3. Organización por features

Cada feature está **autocontenida** en su propia carpeta dentro de `Features/`:

```
Features/Pagos/
├── PagosViewModel.cs       ← lógica + estado
├── PagosPage.xaml          ← layout de la UI
├── PagosPage.xaml.cs       ← enlaza el ViewModel
└── PagosService.cs         ← opcional: lógica de datos específica
```

Esto mantiene los archivos relacionados cerca. Sin búsquedas entre carpetas.

### 4. Navegación — code-behind, sin abstracciones

La navegación usa **NavigationView + Frame** con manejadores de eventos simples en `MainWindow.xaml.cs`:

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

¿Por qué no un `INavigationService`? Para desarrolladores independientes y equipos pequeños, un manejador de eventos directo es más simple, fácil de depurar y una abstracción menos que mantener.

---

## Notas de plataforma y AOT

| Plataforma | Soporte AOT | Detalles |
|-----------|-------------|----------|
| **WinUI 3** | Parcial | La capa de código es AOT-ready. El framework WinUI tiene algo de reflexión interna. Prueba tus features específicas. |
| **Avalonia** | Completo | Las capas Core + Infrastructure son agnósticas a la plataforma. Cambia `NavigationView` por la `Window` de Avalonia. |
| **WPF** | No | El patrón MVVM funciona idéntico, pero .NET WPF no soporta NativeAOT. |

**Para activar publicación AOT** (tras verificar compatibilidad con tus features):

```bash
# 1. Descomenta <PublishAot>true</PublishAot> en .csproj
# 2. Publica
dotnet publish -r win-x64 -c Release
```

---

## Protocolo de uso

### 1. Clonar la plantilla

Copia toda la carpeta y renómbrala con el nombre de tu proyecto.

### 2. Renombrar el namespace

**Opción A — Automática (recomendada)**

```bash
scripts\rename-template.bat TuProyecto
```

Esto reemplaza `MiPlantillaBase` en:
- Nombres de archivos y carpetas
- Contenido de archivos (código, XAML, csproj)

**Opción B — Manual**

En VS Code: `Ctrl+Shift+H` → reemplaza `MiPlantillaBase` por `TuProyecto`

Lista de archivos que contienen el namespace:
- Archivos `*.cs`
- Archivos `*.xaml` (App.xaml, MainWindow.xaml, páginas)
- `*.csproj`
- `*.slnx`
- `app.manifest`

### 3. Registrar nuevas dependencias

Abre `Infrastructure/AppServices.cs` y agrega tus servicios y ViewModels:

```csharp
services.AddTransient<PagosViewModel>();
services.AddSingleton<IPagoService, PagoService>();
services.AddTransient<PagosPage>();
```

### 4. Crear una nueva feature

Copia el patrón de `Dashboard/`:

```
Features/TuFeature/
├── TuFeatureViewModel.cs
├── TuFeaturePage.xaml
└── TuFeaturePage.xaml.cs
```

Patrón del ViewModel:
```csharp
public sealed partial class TuFeatureViewModel : ViewModelBase
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _title = "Tu Feature";

    public TuFeatureViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    [RelayCommand]
    private void HacerAlgo() { }
}
```

Patrón de la Page:
```csharp
public sealed partial class TuFeaturePage : Page
{
    public TuFeatureViewModel ViewModel { get; }

    public TuFeaturePage()
    {
        ViewModel = App.Services.GetRequiredService<TuFeatureViewModel>();
        InitializeComponent();
    }
}
```

### 5. Registrar navegación

- Agrega un `NavigationViewItem` en `MainWindow.xaml`
- Agrega un `case` en el switch de `OnNavItemInvoked`

### 6. Compilar y probar

```bash
dotnet build
dotnet test
```

---

## Resumen de principios

| Principio | Por qué |
|-----------|---------|
| Un solo `ServiceProvider` | No buscar registros DI. Un archivo para controlarlos a todos. |
| Source generators | Cero reflexión. Seguro para AOT. Menos código que escribir. |
| Features autocontenidas | Archivos relacionados juntos. Fácil de agregar/eliminar/refactorizar. |
| Tests desde el día 1 | xUnit incluido. Prueba tus ViewModels sin lanzar la UI. |
| Navegación en code-behind | Lo más simple que funciona. Sin sobreingeniería. |
| Infrastructure como utilidad | `AppServices.cs` es el único archivo "mágico". Todo lo demás es MVVM plano. |

---

## Áreas de feature recomendadas

Estas subcarpetas están pre-creadas en `Features/` como sugerencias:

| Carpeta | Uso típico |
|---------|------------|
| `Dashboard` | Página principal con datos de resumen |
| `Payments` | Facturación, cobros, suscripciones |
| `Trackers` | Métricas de uso, registros de actividad |
| `Telemetry` | Diagnósticos de la app, reporte de errores, rendimiento |
| `Queries` | Búsquedas, reportes con filtros, exportación de datos |
