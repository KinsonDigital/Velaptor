<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.37
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, so your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#954](https://github.com/KinsonDigital/Velaptor/issues/954) - Added the ability to detect size changes with the game window.  This makes it much easier to update your game objects that are based on the window size. Added the following methods to various types.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**
   - `ISceneManager.Resize(SizeU)`
   - `SceneBase.Resize(SizeU size)`
   - `IScene.Resize(SizeU size)`

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#985](https://github.com/KinsonDigital/Velaptor/issues/985) - Fixed a bug where relative content paths were not working.
2. [#985](https://github.com/KinsonDigital/Velaptor/issues/985) - Fixed a bug where text measurement caching was not working when using the `Font.Measure()` method.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#985](https://github.com/KinsonDigital/Velaptor/issues/985) - Changed the name of the `IContentPathResolver.ResolveFilePath()` method parameter named `contentName` to `contentPathOrName`.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#989](https://github.com/KinsonDigital/Velaptor/pull/989) - Updated dependency _**benchmarkdotnet**_ to _**v0.14.0**_
2. [#988](https://github.com/KinsonDigital/Velaptor/pull/988) - Updated dependency _**system.io.abstractions**_ to _**v21.0.29**_
3. [#984](https://github.com/KinsonDigital/Velaptor/pull/984) - Updated dependency _**sixlabors.imagesharp**_ to _**v3.1.5**_
   - This update was a fix for [CVE-2024-41131](https://github.com/SixLabors/ImageSharp/security/advisories/GHSA-63p8-c4ww-9cg7)
4. [#979](https://github.com/KinsonDigital/Velaptor/pull/979) - Updated dependency _**avalonia.themes.fluent**_ to _**v11.1.1**_
5. [#978](https://github.com/KinsonDigital/Velaptor/pull/978) - Updated dependency _**avalonia.fonts.inter**_ to _**v11.1.1**_
6. [#977](https://github.com/KinsonDigital/Velaptor/pull/977) - Updated dependency _**avalonia.diagnostics**_ to _**v11.1.1**_
7. [#976](https://github.com/KinsonDigital/Velaptor/pull/976) - Updated dependency _**avalonia.desktop**_ to _**v11.1.1**_
8. [#975](https://github.com/KinsonDigital/Velaptor/pull/975) - Updated dependency _**avalonia**_ to _**v11.1.1**_
9.  [#974](https://github.com/KinsonDigital/Velaptor/pull/974) - Updated dependency _**serilog.sinks.console**_ to _**v6**_
10. [#973](https://github.com/KinsonDigital/Velaptor/pull/973) - Updated dependency _**serilog**_ to _**v4**_
11. [#972](https://github.com/KinsonDigital/Velaptor/pull/972) - Updated dependency _**xunit**_ to _**v2.9.0**_
12. [#972](https://github.com/KinsonDigital/Velaptor/pull/972) - Updated dependency _**xunit.runner.visualstudio**_ to _**v2.8.2**_
13. [#971](https://github.com/KinsonDigital/Velaptor/pull/971) - Updated dependency _**simpleinjector**_ to _**v5.4.6**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#980](https://github.com/KinsonDigital/Velaptor/issues/980) - Set up KD-Admin tool for development purposes.
