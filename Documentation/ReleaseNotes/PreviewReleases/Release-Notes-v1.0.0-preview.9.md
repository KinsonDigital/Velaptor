<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.9</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#209](https://github.com/KinsonDigital/Velaptor/issues/209) - Added a new method named `EndNotifications()` to `IReactable<TData>` interface and `Reactable<TData>` class to end notifications to subscribed `IReactor` implementations.
   - Before there was no way for the `Reactor.OnCompleted()` method implementation to be invoked.  When invoking the `IReactable<TData>.EndNotifications()` method, all subscribed `IReactor<T>` implementations will be completed.
   - Note #1: After `OnCompleted()` is invoked, `OnNext()` will never be invoked again.  This follows the observable pattern.
   - Note #2: No matter how many times `OnCompleted()` is invoked, the on completed `Action` will only be invoked the first time `OnCompleted()` is called.
2. Added a new method named `IsAnyAltKeyDown()`.

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#220](https://github.com/KinsonDigital/Velaptor/issues/220) - Fixed a bug where using mouse buttons other than the left, right, and middle buttons would cause **Velaptor** to crash.
2. [#224](https://github.com/KinsonDigital/Velaptor/issues/224) - Fix default value for solid fill color button in the scene used for testing rectangles for the **_VelaptorTesting_** application.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored `ISpriteBatch` interface to `IRenderer`.
    - This was done due to the class becoming more like a renderer and its batching operations being performed by other classes.  Batching is done by the types below.
      - `TextureBatchingService`
      - `RectBatchingService`
      - `FontGlyphBatchingService`
      - `BatchServiceManager`
2. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored the `IRenderer.BeginBatch()` method to `IRenderer.Begin()`.
   - This was done for the same reason as in item **_#1_** above.
   - NOTE: The `Renderer` class used to be named `ISpriteBatch`.
3. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored the `IRenderer.EndBatch()` method to `IRenderer.End()`.
   - This was done for the same reason as in item **_#1_** above.
   - NOTE: The `Renderer` class used to be named `ISpriteBatch`.
4. [#209](https://github.com/KinsonDigital/Velaptor/issues/209) - Refactored `afterUnloadAction` to `afterUnload` parameter for `IWindow.ShowAsync()` and `Window.ShowAsync()` methods.
   - This was done to simplify naming as well as internal naming of class field and to follow coding standards.
5. [#239](https://github.com/KinsonDigital/Velaptor/issues/239) - Removed method parameter from `IReactable` interface and the abstract `Reactable` class.
   - Removed the parameter named `unsubscribeAfterProcessing` from the `PushNotification()` method

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored `TextureBatchService` class to `TextureBatchingService`.
2. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored `RectBatchService` class to `RectBatchingService`.
3. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored `FontGlyphBatchService` class to `FontGlyphBatchingService`.
4. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored `SpriteBatchItem` struct to `TextureBatchItem`.
   - This is to help distinguish between different batch item types.
5. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Refactored names of various unit tests to improve clarity.
6. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Removed the `IBatchingService.AddRange()` method and uses throughout code base.
   - This method was not necessary and was done to simplify.
7. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Created new **_MOQ_** verify extension method named 'VerifyOnce()' to help reduce typing in the future.
8. [#226](https://github.com/KinsonDigital/Velaptor/issues/226) - Created new interface named `IBatchServiceManager` and class named `BatchServiceManager` to help simplify the `Renderer` class.
9. [#227](https://github.com/KinsonDigital/Velaptor/issues/227) - Created new internal `IShaderManager` interface and `ShaderManager` class for managing shaders.
10. [#227](https://github.com/KinsonDigital/Velaptor/issues/227) - Refactored internal `Renderer` class to use the new shader manager implementations.
11. [#228](https://github.com/KinsonDigital/Velaptor/issues/228) - Created new internal `IBufferManager` interface and `BufferManager` class for managing shaders.
12. [#228](https://github.com/KinsonDigital/Velaptor/issues/228) - Refactored internal `Renderer` class to use the new buffer manager implementations.
13. [#209](https://github.com/KinsonDigital/Velaptor/issues/209) - Deleted class named `GLWindowFacade`.
    - This class functionality was consolidated into `GLWindow`.
14. [#209](https://github.com/KinsonDigital/Velaptor/issues/209) - Deleted interface named `IGameWindowFacade`
    - This interface was not required due to the deletion of the `GLWindowFacade` class.
15. [#239](https://github.com/KinsonDigital/Velaptor/issues/239) - Improved how some of the `IReactable` subscriptions were being disposed of for some of the internal types.
16. [#246](https://github.com/KinsonDigital/Velaptor/issues/246) - Refactored **OpenGL** debug callback to print warnings and errors to the console.
   - Now if the solution is set to the solution configuration **_Debug-Console_**, the warnings and errors will be displayed in the console window that accompanies the **_VelaptorTesting_** testing application.  This of course works for **_Visual Studio_** as well as for **_JetBrains Rider_**

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#209](https://github.com/KinsonDigital/Velaptor/issues/209) - Updated nuget package **Microsoft.CodeAnalysis.NetAnalyzers** from **_v5.0.3_** to **_v6.0.0_**

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#254](https://github.com/KinsonDigital/Velaptor/issues/254) - Created new issue template for performing releases.
