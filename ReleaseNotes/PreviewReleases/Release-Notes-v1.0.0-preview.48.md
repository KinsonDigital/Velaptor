<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.48
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, so your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Features ✨</h2>

1. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Migrated rendering backend to WebGPU.
2. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added a new 2-D camera system (`ICamera2D` / `Camera2D`) with pan and zoom support.  The camera works in screen-pixel coordinates with a top-left origin, exposes `Position` (pan), `Zoom` (with `ZoomMin` / `ZoomMax` limits), and the `TransformPosition(...)` and `TransformSize(...)` helper methods for converting world coordinates into screen positions.
3. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added the capability to track the total size in KB of a batch, which is the all batch item data sent to the GPU (texture, font, shape, and line data combined).
   - NOTE: This is not part of the public API yet. This lays the groundwork for surfacing memory-usage statistics in a future release.
4. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added a new `TextRenderer` render overload for rendering text with a `Vector2` position, render size, angle, and color in one call: `Render(IFont font, string text, Vector2 position, float renderSize, float angle, Color color, int layer = 0)`.

<h2 align="center" style="font-weight: bold;">Enhancements 💎</h2>

1. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added the ability to configure the color of the built-in FPS counter by adding a `FpsDisplayColor` property to the `Window` class.
   - The FPS value is now rendered in the bottom-left corner.
   - The key combo to toggle the FPS stat is still **Ctrl + Alt + Shift + S**.
2. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added more accurate FPS calculation via a new `FrameMetricsTracker` that uses a ring buffer of frame times instead of the old simple calculation.  The tracker also computes advanced metrics such as average FPS, 1% low FPS, and 0.1% low FPS for spotting stutters.
3. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Improved font and atlas unloading with internal reference counting.  Fonts and atlases are only unloaded and disposed once nothing references them anymore, preventing premature disposal and improving memory reuse.
4. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Added a build-targets file (`KinsonDigital.Velaptor.targets`) that is distributed to projects consuming Velaptor.  Native libraries (e.g. `wgpu-native`) are now automatically copied next to your running application on all operating systems, so you don't have to manage them manually.
5. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Improved render coordination by 4.6%.

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#1287](https://github.com/KinsonDigital/Velaptor/issues/1287) - Disposing of already disposed atlas data causes a crash.
2. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a crash that occurred when closing the window on Linux.
3. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a crash that occurred when minimizing the window.
4. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a crash that occurred when switching between scenes due to improper disposal.
5. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a text rendering glitch so text now renders correctly after switching scenes.
6. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed an issue where rendering more than one batch per frame did not draw all batches correctly.
7. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed an issue where GPU buffer handles were disposed between rendering layers.
8. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a shape rendering issue on macOS so shapes now render correctly.
9. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed resizing batches across render layers when dispatching renders.
10. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed a major allocation issue in font rendering that was causing garbage-collection pressure.
11. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed shutdown cleanup being skipped due to exceptions.  The user `Uninitialize` callback is now exception-guarded, and GPU resource cleanup is driven by the window's closing handler, so shutdown cleanup always runs.
12. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed events not being logged to file when the logs directory did not exist yet; log files are now written even if the logs directory doesn't exist.
13. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed incorrect handling of texture, font, atlas, and audio unloading; unloading of each content type now works correctly under all conditions.
14. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed apps with no scenes failing to render; an app with no scenes now runs instead of failing to render.
15. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed an issue with the animated graphics test scene so it now works correctly.
16. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed incorrect batching and rendering statistics.
17. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed an issue where mp3 audio content with a name that has no file extension was not loading.
18. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Fixed an issue where rendering was not taking into account monitor scaling.

<h2 align="center" style="font-weight: bold;">Configuration 🛠️</h2>

1. [#1269](https://github.com/KinsonDigital/Velaptor/issues/1269) - Updated the logo.

<h2 align="center" style="font-weight: bold;">CICD ⚙️</h2>

1. [#1195](https://github.com/KinsonDigital/Velaptor/issues/1195) - Added CI sample project checks.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#1251](https://github.com/KinsonDigital/Velaptor/issues/1251) - Rename the `IFontRenderer` interface to `ITextRenderer`.
2. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Replaced the entire OpenGL rendering backend with a WebGPU implementation.
   - All drawing code (`IBatcher`, renderers, `IContentManager`) keeps the same shape, but rendering now requires WebGPU's native libraries (`wgpu-native`).
3. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - The `IScene` interface and `SceneBase` class no longer implement `IDisposable`.
   - Scene cleanup is now handled automatically by the engine when a scene is unloaded, so remove any `Dispose()` overrides from your custom scenes.
4. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the `ILineRenderer` interface and `LineRenderer` class.
   - Line rendering has been merged into the `IShapeRenderer` / `ShapeRenderer` types, which now include `Render(Line line, ...)` and the `RenderLine(...)` overloads.
5. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the `ITextRenderer.Render(...)`, (used to be `IFontRenderer.Render(...)`) overload that accepted a `Span<(GlyphMetrics metrics, Color clr)>` glyph array.
   - Use the remaining string-based `Render(IFont font, string text, ...)` overloads instead.
6. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the `ShowAsync(Action? afterStart, Action? afterUnload)` method from `IWindow` and `Window`.
   - Windows are now shown synchronously, so replace `ShowAsync(...)` calls with `Show()`.
7. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the `Dispose()` method from `IWindow` and `Window`.
   - `IWindow` no longer inherits `IDisposable`, and calling `Close()` is enough because closing the window now triggers the full cleanup/shutdown process internally.
8. [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the `AutoClearBuffer` property from `IWindow` and `Window`.
   - The back buffer is now always cleared automatically by the renderer.
9.  [#1256](https://github.com/KinsonDigital/Velaptor/issues/1256) - Removed the Avalonia integration, which includes the `Testing/AvaloniaTesting` sample application and the internal `AvaloniaRenderContext` class.
   - It depended on OpenGL, so it cannot work with the new WebGPU backend.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#1296](https://github.com/KinsonDigital/Velaptor/pull/1296) - Updated dependency _**System.IO.Abstractions**_ to _**v22.3.0**_.
2. [#1295](https://github.com/KinsonDigital/Velaptor/pull/1295) - Updated dependency _**Serilog**_ to _**v4.4.0**_.
3. [#1294](https://github.com/KinsonDigital/Velaptor/pull/1294) - Updated dependency _**coverlet.msbuild**_ to _**v10.1.0**_.
4. [#1293](https://github.com/KinsonDigital/Velaptor/pull/1293) - Updated dependency _**coverlet.collector**_ to _**v10.1.0**_.
5. [#1290](https://github.com/KinsonDigital/Velaptor/pull/1290) - Updated dependency _**@cliffy/prompt**_ to _**v1.3.1**_.
6. [#1283](https://github.com/KinsonDigital/Velaptor/pull/1283) - Updated dependency _**Microsoft.CodeAnalysis.NetAnalyzers**_ to _**v10.0.401**_.
7. [#1282](https://github.com/KinsonDigital/Velaptor/pull/1282) - Updated dependency _**KinsonDigital.Carbonate**_ to _**v1.0.0-preview.19**_.
8. [#1267](https://github.com/KinsonDigital/Velaptor/pull/1267) - Updated dependency _**NSubstitute**_ to _**v6.0.0**_.
9. [#1258](https://github.com/KinsonDigital/Velaptor/pull/1258) - Updated dependency _**Microsoft.NET.Test.Sdk**_ to _**v18.10.1**_.
10. [#1285](https://github.com/KinsonDigital/Velaptor/pull/1285) - Updated _**KinsonDigital/Infrastructure**_ action to _**v19.0.1**_.
11. [#1279](https://github.com/KinsonDigital/Velaptor/pull/1279) - Updated _**actions/checkout**_ action to _**v7.0.1**_.
12. [#1273](https://github.com/KinsonDigital/Velaptor/pull/1273) - Updated _**denoland/setup-deno**_ digest to _**4606d5cc6fb3f673efd4f594850e3f4b3e9d29cd**_.
13. [#1261](https://github.com/KinsonDigital/Velaptor/pull/1261) - Updated _**actions/setup-dotnet**_ action to _**v5.4.0**_.
