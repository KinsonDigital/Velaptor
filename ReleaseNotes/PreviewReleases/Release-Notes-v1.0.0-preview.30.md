<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.30
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#791](https://github.com/KinsonDigital/Velaptor/issues/791) - Added the following extension methods so the user can pass the object to unload instead of the path.
    - `ILoader<ITexture>.Unload(ITexture?)`
    - `ILoader<IFont>.Unload(IFont?)`
    - `ILoader<ISound>.Unload(ISound?)`
    - `ILoader<IAtlasData>.Unload(IAtlasData?)`

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#791](https://github.com/KinsonDigital/Velaptor/issues/791) - Introduced the following breaking changes:
    - Removed the `ContentExtensions.Load(this FontLoader)` extension method.
    - Removed the `IContentLoader` interface.
    - Removed the `ContentLoader` class.
    - Removed the `ContentLoader` property from the `Window` class.
    - Removed the `ContentLoader` property from the `IWindow` interface.
    - Removed the `ContentLoader` property from the `IScene` interface.
    - Removed the `ContentLoader` property from the `SceneBase` class.
    - Removed the `CreateContentLoader` method from the `ContentLoaderFactory` class.
    - Converted the `RendererFactory` to a static class to follow the same pattern as `ContentLoaderFactory`
    - Removed the `IRendererFactory` interface.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#790](https://github.com/KinsonDigital/Velaptor/pull/790), [#788](https://github.com/KinsonDigital/Velaptor/pull/788) - Updated dependency _**xunit**_ to _**v2.6.1**_
2. [#789](https://github.com/KinsonDigital/Velaptor/pull/789) - Updated dependency _**benchmarkdotnet**_ to _**v0.13.10**_
4. [#779](https://github.com/KinsonDigital/Velaptor/pull/779) - Updated dependency _**communitytoolkit.mvvm**_ to _**v8.2.2**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#782](https://github.com/KinsonDigital/Velaptor/issues/782) - Replaced custom guards.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**
2. [#776](https://github.com/KinsonDigital/Velaptor/issues/776) - Updated sync workflow.
