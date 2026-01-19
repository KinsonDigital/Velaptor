<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.44
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, so your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Enhancement</h2>

- [#1174](https://github.com/KinsonDigital/Velaptor/issues/1174) - Improved OpenGL debugging by removing reflection for buffer names and only including debugging with debug builds.
- [#1153](https://github.com/KinsonDigital/Velaptor/issues/1153) - Revamped and designed the content loading API.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

- [#1153](https://github.com/KinsonDigital/Velaptor/issues/1153) - Introduced the following breaking changes due to content loading API changes:
  - The `IAudio` interface does not inherit from `IDisposable` anymore.
  - The `Audio.Dispose()` method has been removed from the `Audio` class.
    - This should not have been part of the public API.  Unloading audio content is now done using the `ContentManager.Unload<T>()` method.
  - The `ContentLoaderFactory` class has been removed.
    - Loading content is now done using the `ContentManager` class.
  - The `Size` property setter of the `Font` class has been removed.
    - Setting the size of a font is no longer done on an already loaded font. This is now done using the following `ContentManager` methods:
      - To load a font using the default font size of 12, use `ContentManager.Load<T>()`.
      - To load a font with a desired size, use the `ContentManager.LoadFont()` method.
      - This means if you want to use a font of a different size, instead of changing the size of a single already loaded font, you would load a font object for each size of font you want to use.
  - The `Style` property setter of the `Font` class has been removed.
    - Setting the style of a font is no longer done on an already loaded font. This is now done using the following `ContentManager` methods:
      - To load a font with a particular style, use the `ContentManager.LoadFont()` method.
  - Renamed the `Font.MaxCacheSize` class and `IFont.MaxCacheSize` interface property names to `MaxMeasureCacheSize`.
  - Renamed the `Font.CurrentCacheSize` class and `IFont.CurrentCacheSize` interface property names to `CurrentMeasureCacheSize`.
  - The `ILoader<T>` interface has been removed.
    - Content is now loaded using the `ContentManager` class.
  - Removed the `IItemCache<T, T>` interface.
  - Removed the following load content extension methods:
    - `ILoader<IFont>.Load(string fontName, uint size)`
    - `ILoader<IAtlasData>.Load(string atlasPathOrName)`
    - `ILoader<IAudio>.Load(string audioPathOrName, AudioBuffer bufferType)`
    - `ILoader<ITexture>.Load(string texturePathOrName)`
    - `ILoader<ITexture>.Unload(ITexture? texture)`
    - `ILoader<IFont>.Unload(IFont? font)`
    - `ILoader<IAudio>.Unload(IAudio? audio)`
    - `ILoader<IAtlasData>.Unload(IAtlasData? atlas)`
  - Removed the `Width` and `Height` properties from the `IAtlasData` interface and `AtlasData` class.
    - These properties did not make sense.  To get the width and height of the atlas texture, reference the `Texture` property of the `IAtlasData` interface and/or `AtlasData` class instead.
  - The `PathResolverFactory` class has been changed from static to non-static.
  - Added a new interface `IPathResolverFactory` that is used by the `PathResolverFactory` class.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#1173](https://github.com/KinsonDigital/Velaptor/pull/1173) - Updated _**actions/cache**_ action to _**v5.0.0**_.
2. [#1169](https://github.com/KinsonDigital/Velaptor/pull/1169) - Updated _**system.io.abstractions**_ to _**v22.1.0**_.
3. [#1168](https://github.com/KinsonDigital/Velaptor/pull/1168) - Updated _**actions/checkout**_ action to _**v6.0.0**_.
4. [#1167](https://github.com/KinsonDigital/Velaptor/pull/1167) - Updated _**avalonia**_ to _**v11.3.11**_.
5. [#1162](https://github.com/KinsonDigital/Velaptor/pull/1162) - Updated _**microsoft.codeanalysis.netanalyzers**_ to _**v10.0.0**_.
6. [#1161](https://github.com/KinsonDigital/Velaptor/pull/1161) - Updated _**microsoft.net.test.sdk**_ to _**v18.0.1**_.
7. [#1157](https://github.com/KinsonDigital/Velaptor/pull/1157) - Updated _**serilog.sinks.console**_ to _**v6.1.1**_.
8. [#1156](https://github.com/KinsonDigital/Velaptor/pull/1156) - Updated _**benchmarkdotnet**_ to _**v0.15.8**_.
9. [#1152](https://github.com/KinsonDigital/Velaptor/pull/1152) - Updated _**sixlabors.imagesharp**_ to _**v3.1.12**_.
