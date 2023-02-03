<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.16
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#485](https://github.com/KinsonDigital/Velaptor/issues/485) - Improved layered rendering in the following ways:
   - Different types of rendering now follow the numerical layer assigned.
     >💡Example: If a texture is rendered on layer 3 and a rectangle is rendered on layer 2, the texture will be rendered on top of the rectangle regardless of the order of render calls in code.
   - Multiple items on the same layer respect layering based on the time of render in the code relative to the rest of the items on the same layer.
   - UI controls are now always rendered on top of textures, text, lines, and rectangles.
     >💡There are plans in the future to add control to this behavior.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#485](https://github.com/KinsonDigital/Velaptor/issues/485) - Added the breaking changes listed below:
   - Changed the numerical value of the `RenderEffects` enum.
     >💡This was not a change to the enum names.  This was required to facilitate empty checks for internal render items.
   - Changed the exception thrown by the `PathResolverFactory.CreateSystemFontPathResolver()` method when executed on non-windows platforms.
     >💡This is temporary until other platforms can be implemented.
2. [#451](https://github.com/KinsonDigital/Velaptor/issues/451) - All of the breaking changes are provided below:
   - Removed the method `CreateRenderer()` from the `RendererFactory` class.
     >💡This has been replaced with multiple methods to create different render types.
   - Changed the `RendererFactory` to `sealed`.
   - There is not a single renderer that is used to render everything anymore.  This has been removed and replaced by the different rendering types below:
     >💡These are created using the `RendererFactory` class.
     - `ITextureRenderer`
     - `IFontRenderer`
     - `IRectangleRenderer`
     - `ILineRenderer`
   - The process for beginning and ending a batch has changed. This is now done by using the `Begin()` and `End()` methods in the `IRenderer` interface.
   - The process of clearing the screen has changed. This is now done by using the `Clear()` method in the `IRenderer` interface.
   - Removed the `IRenderer` parameter from the `IDrawable.Render()` interface method.
   - Removed the `GetFrame()` method from the `IAtlasData` interface.
     >💡This same functionality cane accomplished by using the `GetFrames()` method.
   - Changed the name of the property `FontTextureAtlas` to `Atlas` in the `IFont` interface.
   - Changed the name of the `CreateTextureAtlasLoader` method to `CreateAtlasLoader()` in the `ContentLoaderFactory` class.
   - Changed the name of the `CreateTextureAtlasPathResolver()` method to `CreateAtlasPathResolver()` in the `PathResolverFactory` class.
   - Changed the name of the `CreateFontAtlas()` method to `CreateAtlas()` in the `IFontAtlasService` interface.
   - Changed the name of the `textureAtlas` parameter to `atlasTexture` in the `Create()` method in the `IFontFactory` interface.
   - Changed the class `AtlasSubTextureData` to a `readonly struct`.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#485](https://github.com/KinsonDigital/Velaptor/issues/485) - Improved internal batching of render items which has greatly improved performance.
2. [#497](https://github.com/KinsonDigital/Velaptor/issues/497) - Improved performance by changing how buffering on the GPU is handled.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#485](https://github.com/KinsonDigital/Velaptor/issues/485) - Updated **KinsonDigital.Carbonate** from _**v1.0.0-preview.12**_ to _**v1.0.0-preview.14**_
   >💡This required significant internal changes to the codebase.
2. [#455](https://github.com/KinsonDigital/Velaptor/issues/455) - Updated the nuget packages below:
   - Updated **System.IO.Abstractions** from _**v17.1.1**_ to _**v19.1.5**_
   - Updated **KinsonDigital.CASL** from _**v1.0.0-preview.10**_ to _**v1.0.0-preview.11**_

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#451](https://github.com/KinsonDigital/Velaptor/issues/451) - Fixed badges in project **README**.
2. [#474](https://github.com/KinsonDigital/Velaptor/issues/474) - Updated the **CICD** dotnet tool from version _**v1.0.0-preview.14**_ to _**v1.0.0-preview.15**_
3. [#479](https://github.com/KinsonDigital/Velaptor/issues/479) - Updated all uses of the GitHub [checkout](https://github.com/marketplace/actions/checkout) action from _**v2**_ to _**v3**_ in all of the workflows.
4. [#492](https://github.com/KinsonDigital/Velaptor/issues/492) - Added API documentation to the nuget package.
   >💡This will provide API documentation in intellisense in your preferred IDE.
5. [#477](https://github.com/KinsonDigital/Velaptor/issues/477) - Improved grammar and spelling issues in previous preview release notes.
6. [#377](https://github.com/KinsonDigital/Velaptor/issues/377) - Moved the setup to expose the visibility of `internal`` types to the unit test project to the **Velaptor** project file.
7. [#361](https://github.com/KinsonDigital/Velaptor/issues/361) - Improved unit tests by refactoring assertion code to use [Fluent Assertions](https://fluentassertions.com/).
8. [#152](https://github.com/KinsonDigital/Velaptor/issues/152) - Setup the project to automatically publish unit test code coverage results to [codecov](https://about.codecov.io/).
