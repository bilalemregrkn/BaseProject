# BaseProject

A production-ready Unity game architecture boilerplate featuring a service-oriented design with dependency injection, async operations, and a suite of reusable backend systems.

## Tech Stack

- **Unity:** 6000.3.10f1
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Dependency Injection:** [Reflex](https://github.com/gustavopsantos/Reflex) v14.1.0
- **Async/Await:** UniTask (Cysharp.Threading.Tasks)
- **Animation/Tweening:** PrimeTween
- **Input:** Unity Input System

## Architecture

The project follows a **service-oriented architecture** where each system is an isolated service injected via Reflex DI. Services are registered in `RootInstaller.cs` and exposed through well-defined interfaces.

```
Assets/!_Project/
├── Backend/          # Core service layer
│   ├── AudioService
│   ├── CurrencyService
│   ├── EventBusService
│   ├── HapticService
│   ├── MusicService
│   ├── PanelService
│   ├── SaveService
│   ├── SceneService
│   ├── UpdateService
│   ├── ComponentService
│   └── Installer
├── Game/
│   └── Example/      # Example gameplay code
└── Tool/
    ├── FavTool
    └── PlayerPrefsTool
```

## Services

| Service | Description |
|---|---|
| **PanelService** | UI panel management — show/hide with async fade animations |
| **SceneService** | Async scene loading/unloading (single & additive) with events |
| **AudioService** | Sound effects playback with volume/mute and AudioMixer support |
| **MusicService** | Background music with crossfade, loop, and volume control |
| **EventBusService** | Type-safe, decoupled publish/subscribe event system with scopes |
| **CurrencyService** | Multi-currency management (add, spend, balance checks) |
| **SaveService** | Key-based generic data persistence (serialize/deserialize) |
| **UpdateService** | Centralized update loop — register IUpdatable / ILateUpdatable / IFixedUpdatable |
| **HapticService** | Device haptic feedback (light, medium, heavy) |
| **ComponentService** | Cached GetComponent wrapper to avoid repeated reflection calls |

## Editor Tools

- **FavTool** — Bookmark frequently used assets with named profiles
- **PlayerPrefsTool** — View, edit, and debug PlayerPrefs keys at runtime

## Getting Started

1. Clone the repository
2. Open in **Unity 6000.3.10f1** or later
3. Open `Assets/!_Project/Game/GameScene.unity`
4. Press Play — the `RootInstaller` bootstraps all services automatically

## Key Patterns

- **Dependency Injection** — Use `[Inject]` attributes from Reflex on any MonoBehaviour or plain C# class
- **Events** — Publish and subscribe via `IEventBusService` to decouple systems
- **Async** — All long-running operations (scene loads, panel transitions) use `async/await` with UniTask
- **Settings** — Service configuration lives in ScriptableObject `.asset` files, editable in the Inspector
- **SmartComponent** — Extend `SmartComponent` instead of `MonoBehaviour` for cached component lookups
