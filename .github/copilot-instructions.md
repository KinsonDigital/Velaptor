# Velaptor – Copilot Instructions

Velaptor is a **cross-platform 2D game and multimedia application development framework** for C# (.NET 9, C# 12). It is published on NuGet as `KinsonDigital.Velaptor` and targets the x64 platform on Windows, Linux, and macOS.

Its purpose is to hide the complexity of low-level graphics, audio, and input APIs from game developers — similar in spirit to MonoGame or Love2D — while remaining extensible. Every design decision should be evaluated through that lens: does it help a game developer write a game more easily, and does it stay out of the way of performance?

Under the hood the framework is powered by:
- **[Silk.NET](https://github.com/dotnet/Silk.NET)** – OpenGL context, GLFW windowing, and native input
- **[CASL](https://github.com/KinsonDigital/CASL)** – cross-platform audio playback
- **[FreeType](https://freetype.org/)** (via FreeTypeSharp) – font rasterisation and glyph metrics
- **[Carbonate](https://github.com/KinsonDigital/Carbonate)** – internal pub/sub messaging
- **[SimpleInjector](https://simpleinjector.org/)** – dependency injection container
- **[Serilog](https://serilog.net/)** – structured logging

---

## Build & Test Commands

```bash
# Build the main library
dotnet build Velaptor/Velaptor.csproj -c Debug

# Run all unit tests
dotnet test Testing/VelaptorTests/VelaptorTests.csproj -c Debug

# Run a single test by name
dotnet test Testing/VelaptorTests/VelaptorTests.csproj -c Debug --filter "FullyQualifiedName~<TestMethodName>"

# Run tests by trait category
dotnet test Testing/VelaptorTests/VelaptorTests.csproj -c Debug --filter "Category=Ctor Tests"

# Build companion sub-projects
dotnet build Testing/VelaptorTesting/VelaptorTesting.csproj -c Debug
dotnet build Testing/AvaloniaTesting/AvaloniaTesting.csproj -c Debug
dotnet build Testing/PlaygroundApp/PlaygroundApp.csproj -c Debug
```

---

## Project Layout

| Path | Purpose |
|------|---------|
| `Velaptor/` | Main library — the NuGet package |
| `Testing/VelaptorTests/` | xUnit unit tests |
| `Testing/VelaptorTesting/` | Manual integration test app |
| `Testing/AvaloniaTesting/` | Avalonia-hosted render context test |
| `Testing/PlaygroundApp/` | Sandbox for manual exploration |
| `Performance/` | Micro-benchmark apps (keyboard, mouse, text measurement, font rendering) |

---

## Game Engine Architecture

### The Game Loop

The entry point for any Velaptor game is a class that extends the abstract `Window` (`Velaptor.UI`). The window drives a fixed-step game loop, calling three lifecycle delegates each frame:

```
Initialize → [Update(FrameTime) → Draw(FrameTime)] × N → Uninitialize
```

`FrameTime` carries `ElapsedTime` (delta for the current frame) and `TotalTime` (application uptime). All game logic that needs deterministic timing must use `FrameTime.ElapsedTime` rather than `DateTime.Now` or stopwatches.

`IUpdatable` and `IDrawable` are the interfaces that any game object can implement to hook into `Update`/`Draw` without direct coupling to the window.

### Scene Management

Games are structured around scenes. Each scene extends `SceneBase` (which implements `IScene`) and is registered with the `ISceneManager`. The scene manager handles:
- Activation / deactivation (only one scene is active at a time)
- Content load / unload when switching scenes
- Optional navigation wrapping (`UsesNavigationWrapping`)

`SceneBase` automatically subscribes to the window-size reactable so every scene always has access to `WindowSize` and `WindowCenter` without extra wiring.

### Rendering Pipeline

All rendering is **batch-based** to minimise GPU draw calls, which is critical for real-time game performance.

A frame's render flow:
1. `IBatcher.Begin()` — clears the screen, signals `BatchHasBegunId`
2. Game code calls renderer methods to **queue** items (nothing is drawn yet)
3. `IBatcher.End()` — signals each renderer to flush its batch to the GPU, signals `BatchHasEndedId`

The four renderers and their batch item types:

| Renderer | Batch Item | GPU Buffer | Shader |
|----------|-----------|------------|--------|
| `TextureRenderer` | `TextureBatchItem` | `TextureGpuBuffer` | `texture.vert/.frag` |
| `FontRenderer` | `FontGlyphBatchItem` | `FontGpuBuffer` | `font.vert/.frag` |
| `ShapeRenderer` | `ShapeBatchItem` | `ShapeGpuBuffer` | `shape.vert/.frag` |
| `LineRenderer` | `LineBatchItem` | `LineGpuBuffer` | `line.vert/.frag` |

GLSL shader sources are embedded resources under `OpenGL/ShaderCode/`. GPU buffer base class is `GpuBufferBase<TData>` (`OpenGL/Buffers/`). Each renderer and buffer subscribes to reactable notifications for GL initialisation (`GLInitializedId`), viewport resize (`ViewPortSizeChangedId`), and shutdown (`SystemShuttingDownId`).

The default batch capacity starts at **1 000 items** per renderer. Avoid calling `Begin`/`End` more than once per frame.

### Content System (Asset Loading)

`IContentManager` (obtained via `ContentManager.Create()`) is the game developer's API for loading assets. It is a singleton and is safe to call multiple times with the same path — content is cached after the first load.

Supported content types:

| Type | Interface | Loader |
|------|-----------|--------|
| Textures (PNG/etc.) | `ITexture` | `TextureLoader` |
| Fonts (TTF) | `IFont` | `FontLoader` |
| Texture atlas | `IAtlasData` | `AtlasLoader` |
| Audio (OGG/MP3) | `IAudio` | `AudioLoader` |

Path resolution is cross-platform. Each loader has a corresponding `IContentPathResolver` that maps a simple name (e.g., `"player"`) to an absolute path on the current OS. Content path resolvers are used internally; game developers use only `IContentManager.Load<T>(name)`.

Disposing a content object (e.g., `texture.Dispose()`) evicts it from the cache and releases GPU/audio resources. Loaded content should be disposed in `SceneBase.UnloadContent()`.

Four embedded TTF fonts (Times New Roman variants) are bundled in the assembly as fallbacks under `Content/Fonts/EmbeddedResources/`.

### Input System

Input is polled, not event-driven, to align with the game-loop model:

```csharp
var keyboard = IoC.Container.GetInstance<IAppInput<KeyboardState>>();
var state = keyboard.GetState();
if (state.IsKeyDown(KeyCode.Space)) { /* fire */ }
```

Similarly for mouse (`IAppInput<MouseState>`). Raw state changes are broadcast internally via `KeyboardStateChangedId` and `MouseStateChangedId` reactables.

### Cross-Platform Considerations

- The library targets **x64 only** (`<Platforms>x64</Platforms>`). ARM builds are not currently supported.
- CI builds and tests run on `windows-latest`, `ubuntu-latest`, and `macos-latest` (M1+) in GitHub Actions.
- Platform-specific test variants use `[FactForWindows]` / `[FactForLinux]` (and Theory equivalents) to conditionally skip on other OSes.
- Native library interop (OpenGL, GLFW, FreeType, ImGui) is wrapped behind interfaces (`IGLInvoker`, `IGlfwInvoker`, `IFreeTypeInvoker`, `IImGuiInvoker`) so platform differences are contained in `NativeInterop/`.
- File system access always goes through `System.IO.Abstractions` (`IFile`, `IDirectory`, `IPath`, `IFileStreamFactory`) to remain testable and OS-neutral.

### Performance Patterns

- **`CachedValue<T>`** (`Velaptor.CachedValue<T>`) – defers reads/writes to an expensive backing store (e.g., the GPU or native window) until caching is disabled. Used heavily in `GLWindow` for window properties (width, height, position) that would otherwise round-trip through GLFW on every access.
- **Reactable over events** – the Carbonate pub/sub system avoids hard references between subsystems and lets systems self-unsubscribe, which prevents cross-frame memory retention caused by lingering event handlers.
- **Struct batch items** – all batch items (`TextureBatchItem`, `FontGlyphBatchItem`, etc.) are `struct`s to avoid heap allocation per render call.
- **Logging disabled in Release** – `AppSettings.LoggingEnabled` is `true` only in `DEBUG`/`DEBUG_CONSOLE` builds. Never add `Console.Write` or `Serilog.Log.*` calls directly; use `ILoggingService`.
- **Performance projects** in `Performance/` (KeyboardPerf, MousePerf, MeasureTextPerf, FontRendererPerf) are standalone apps for manual benchmarking of hot paths.

---

## Internal Wiring Conventions

### Reactable Messaging (Carbonate)

All internal engine events are communicated through typed reactables registered in `IoC.cs`. Notification IDs are static `Guid` properties on `PushNotifications` and `PullNotifications` — **never inline a GUID**. Always use these constants when subscribing or pushing.

Key notification IDs:

| Constant | Meaning |
|----------|---------|
| `GLContextCreatedId` | OpenGL context handed to the renderer |
| `GLInitializedId` | OpenGL fully initialised; buffers/shaders may now upload |
| `BatchHasBegunId` / `BatchHasEndedId` | Frame render boundaries |
| `RenderTexturesId` / `RenderFontsId` / `RenderShapesId` / `RenderLinesId` | Renderer flush signals |
| `ViewPortSizeChangedId` / `WindowSizeChangedId` | Window/viewport resize |
| `SystemShuttingDownId` | Engine tear-down; unsubscribe and release GPU resources |
| `TextureDisposedId` / `AudioDisposedId` | Cache eviction signals |

Subscribers must store the returned `IDisposable` and dispose it (or unsubscribe in the `onUnsubscribe` callback) to avoid stale handlers.

### Dependency Injection (SimpleInjector)

`IoC.cs` is the single place where the object graph is wired. The container **throws `InvalidOperationException`** if accessed during a unit test (detected by `UnitTestDetector`, which inspects loaded assemblies for xUnit). This prevents tests from accidentally exercising the real container.

**Dual-constructor pattern** — every injectable type that needs to be unit-testable has two constructors:
1. A `public` or `protected` no-arg constructor that pulls dependencies from `IoC.Container` (annotated with `[ExcludeFromCodeCoverage]`).
2. A `private protected` or `internal` constructor that takes explicit dependencies — this is what tests use via NSubstitute mocks.

### File Headers

Every `.cs` file starts with:
```csharp
// <copyright file="FileName.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>
```

### Nullable & Unsafe

Both `<Nullable>enable</Nullable>` and `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` are active project-wide. Treat all reference types as non-nullable by default. Unsafe blocks are used in GPU data marshalling.

### Guard Clauses

- Public API null checks: `ArgumentNullException.ThrowIfNull(param)`
- Low-level native pointer checks: `EnsureThat.PointerIsNotNull(ptr)` (`Velaptor.Guards`)

### `[ExcludeFromCodeCoverage]` Usage

Always include a `Justification` string:
- IoC-touching constructors: `"Cannot test due to interaction with 'IoC' container."`
- Static pass-through classes: `"This is a static class that doesn't need to be tested."`

### `InternalsVisibleTo`

`Velaptor.csproj` grants internal access to `VelaptorTests`, `DynamicProxyGenAssembly2` (NSubstitute), and the performance projects. Add a matching entry when creating a new companion project that needs internal access.

---

## Test Conventions

- Framework: **xUnit** + **NSubstitute** (mocking) + **Shouldly** (assertions)
- `AssertExtensions.ThrowsWithMessage<T>(action, message)` — verifies exception type and message together.
- `TestsBase` defines trait category constants: `Ctor`, `Method`, `Prop`, `Subscription`. Decorate tests with `[Trait("Category", Ctor)]` etc.
- Platform-conditional attributes: `[FactForWindows]`, `[FactForLinux]`, `[TheoryForWindows]`, `[TheoryForLinux]`
- Build-configuration-conditional attributes: `[FactForDebug]`, `[FactForProduction]`, `[TheoryForDebug]`, `[TheoryForProduction]`
- Complex GPU output assertions load expected vertex/batch data from JSON files in `Testing/VelaptorTests/SampleTestData/` (all marked `CopyToOutputDirectory: Always`).
