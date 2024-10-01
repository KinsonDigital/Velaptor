<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.38
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, so your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#957](https://github.com/KinsonDigital/Velaptor/issues/957) - Added the new overloads below to the texture renderer interface `ITextureRenderer`.
    - `void Render(ITexture texture, int x, int y, float angle, float size, int layer = 0);`
    - `void Render(ITexture texture, Vector2 pos, float angle, float size, int layer = 0);`
    - `void Render(ITexture texture, int x, int y, float angle, float size, Color color, int layer = 0);`
    - `void Render(ITexture texture, Vector2 pos, float angle, float size, Color color, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, Color color, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, Color color, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, Color color, int frameNumber, int layer = 0);`
    - `void Render(IAtlasData atlas, string subTextureName, Vector2 pos, float angle, float size, Color color, RenderEffects effects, int frameNumber = 0, int layer = 0);`
2. [#969](https://github.com/KinsonDigital/Velaptor/issues/969) - Added the ability to know if a circle shape contains a vector by adding a method named `Contains(Vector2)` to the `CircleShape` struct.
3. [#955](https://github.com/KinsonDigital/Velaptor/issues/955) - Added the ability to control how scenes wrap to first or last scene.  Added the following:
   - Added the property `CurrentSceneIndex` to the `ISceneManager` interface
   - Added the property `UsesNavigationWrapping` to the `ISceneManager` interface

<h2 align="center" style="font-weight: bold;">Enhancements 💎</h2>

1. [#965](https://github.com/KinsonDigital/Velaptor/issues/965) - Improved the `RectShape` default behavior to avoid confusion.

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#1019](https://github.com/KinsonDigital/Velaptor/issues/1019) - Fixed an issue where textures were not being unloaded from the content cache.
2. [#1009](https://github.com/KinsonDigital/Velaptor/issues/1009) - Fixed an issue where certain window sizes were preventing any rendering.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#967](https://github.com/KinsonDigital/Velaptor/issues/967) - Moved the `SceneAlreadyExistsException` from the `Velaptor.Exceptions` namespace to the `Velaptor.Scene.Exceptions` namespace
2. [#966](https://github.com/KinsonDigital/Velaptor/issues/966) - Changed the order of the constructor parameters of the `CornerRadius` struct from `CornerRadius(float topLeft, float bottomLeft, float bottomRight, float topRight)` to `CornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)`.  This is a change from counterclockwise order to clockwise order.
3. [#965](https://github.com/KinsonDigital/Velaptor/issues/965) - Changed default value of the `RectShape.CornerRadius` property to have a value of 0 for all corners.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#1032](https://github.com/KinsonDigital/Velaptor/pull/1032) - Updated dependency _**communitytoolkit.mvvm**_ to _**v8.3.1**_.
2. [#1031](https://github.com/KinsonDigital/Velaptor/pull/1031) - Updated dependency _**fluentassertions**_ to _**v6.12.1**_.
3. [#1030](https://github.com/KinsonDigital/Velaptor/pull/1030) - Updated dependency _**kinsondigital.kdgui**_ to _**v1.0.0-preview.4**_.
4. [#1022](https://github.com/KinsonDigital/Velaptor/pull/1022) - Updated dependency _**kinsondigital.casl**_ to _**v1.0.0-preview.20**_.
5. [#1015](https://github.com/KinsonDigital/Velaptor/pull/1015) - Updated dependency _**microsoft.net.test.sdk**_ to _**v17.11.1**_.
6. [#1004](https://github.com/KinsonDigital/Velaptor/pull/1004) - Updated dependency _**simpleinjector**_ to _**v5.5.0**_.
7. [#1002](https://github.com/KinsonDigital/Velaptor/pull/1002) - Updated dependency _**Avalonia**_ to _**v11.1.3**_.
9. [#1002](https://github.com/KinsonDigital/Velaptor/pull/1002) - Updated dependency _**Avalonia.Desktop**_ to _**v11.1.3**_.
9. [#1002](https://github.com/KinsonDigital/Velaptor/pull/1002) - Updated dependency _**Avalonia.Diagnostics**_ to _**v11.1.3**_.
9. [#1002](https://github.com/KinsonDigital/Velaptor/pull/1002) - Updated dependency _**Avalonia.Fonts.Inter**_ to _**v11.1.3**_.
9. [#1002](https://github.com/KinsonDigital/Velaptor/pull/1002) - Updated dependency _**Avalonia.Themes.Fluent**_ to _**v11.1.3**_.

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#1026](https://github.com/KinsonDigital/Velaptor/issues/1026) - Updated the [kdadmin](https://github.com/KinsonDigital/kd-admin) tool.
2. [#1008](https://github.com/KinsonDigital/Velaptor/issues/1008) - Replaced the testing app UI with [kdgui](https://github.com/KinsonDigital/KdGui).
3. [#1005](https://github.com/KinsonDigital/Velaptor/issues/1005) - Refactored moq code to [nsubstitute](https://nsubstitute.github.io/) code.
4. [#950](https://github.com/KinsonDigital/Velaptor/issues/950) - Improved the _**VelaptorTesting**_ app.
