# WinAppTemplate — CT Mvvm Tuning

Una plantilla MVVM pragmática para **WinUI 3** (.NET 10) con **CommunityToolkit.Mvvm**, diseñada para desarrolladores independientes y equipos pequeños (1–3 personas).

> Originalmente un proyecto personal, adaptado para uso público.  
> Licencia MIT — libre de usar, modificar y compartir.

---

## Público objetivo

- **Desarrolladores independientes** creando apps de escritorio Windows
- **Equipos pequeños** que quieran una arquitectura predecible y de baja ceremonia
- **Principiantes** que encuentran el MVVM tradicional demasiado abstracto — esta plantilla te da convenciones concretas
- **Cualquiera** que quiera código compatible con AOT sin pelearse con reflexión

---

## Compatibilidad de plataformas

| Plataforma | Soporte AOT | Notas |
|-----------|-------------|-------|
| **WinUI 3** | Parcial | Destino principal. El código es AOT-friendly; WinUI framework tiene límites |
| **Avalonia** | Completo | Compatible — adapta el namespace y cambia la capa UI |
| **WPF** | No | Compatible (sigue siendo MVVM) pero AOT no está disponible |

> Técnicamente, las capas Core + Infrastructure son agnósticas a la plataforma.  
> Cambia `Features/Dashboard/DashboardPage.xaml` por el equivalente en tu framework.

---

## Inicio rápido

```bash
# 1. Copia la carpeta de la plantilla
xcopy /E WinAppTemplate_CT_Mvvm_Tuning MiNuevaApp

# 2. Renombra el namespace (elige UN método)
#    Opción A — Automática (recomendada)
cd MiNuevaApp
scripts\rename-template.bat MiNuevaApp

#    Opción B — Manual (VS Code)
#    Ctrl+Shift+H → reemplaza "MiPlantillaBase" por "MiNuevaApp"

# 3. Restaura y compila
dotnet restore
dotnet build
dotnet test
```

**Requisitos:**
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Windows SDK (incluido con Visual Studio o vía `dotnet workload install maui-windows`)

---

## Estructura de carpetas

```
src/
├── MiApp/
│   ├── Core/                    ← base compartida (ViewModelBase, interfaces, modelos)
│   ├── Features/                ← una carpeta por feature
│   │   ├── Dashboard/           ← ejemplo funcional (ViewModel + Page)
│   │   ├── Payments/            ← área recomendada (facturación, pagos)
│   │   ├── Trackers/            ← área recomendada (uso, métricas)
│   │   ├── Telemetry/           ← área recomendada (diagnósticos, logging)
│   │   └── Queries/             ← área recomendada (reportes, búsquedas)
│   └── Infrastructure/          ← configuración DI, servicios
└── MiApp.Tests/                 ← tests xUnit
```

Cada carpeta de feature debe contener:
- `XxxViewModel.cs` — lógica + estado
- `XxxPage.xaml` + `.xaml.cs` — UI con bindings
- Opcional: `XxxService.cs` — si la feature tiene su propia lógica de datos

---

## Por qué esta plantilla

| Problema | Solución |
|----------|----------|
| DI dispersa por todo el proyecto | Único `AppServices.cs` para todos los registros |
| MVVM tradicional = demasiado boilerplate | Source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| Features mezcladas entre sí | Cada feature autocontenida en `Features/Xxx/` |
| AOT falla con reflexión | Cero reflexión — código generado en compile-time |
| Sin tests desde el día 1 | Proyecto xUnit incluido con ejemplo funcional |
| "¿Dónde pongo este archivo?" | Convenciones claras: Core / Features / Infrastructure |

---

## Documentación

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — arquitectura completa, protocolo y notas para principiantes (EN)
- [`docs/ARCHITECTURE.es.md`](docs/ARCHITECTURE.es.md) — mismo contenido en español

---

## Stack tecnológico

| Capa | Tecnología |
|------|-----------|
| Framework | .NET 10 + Windows App SDK |
| UI | WinUI 3 (NavigationView + Frame) |
| MVVM | CommunityToolkit.Mvvm 8.4.2 |
| DI | Microsoft.Extensions.DependencyInjection |
| Tests | xUnit |

---

## Licencia

MIT — úsalo, fórcale, envíalo a producción.

Si te resulta útil, se agradece la atribución pero no es obligatoria.
