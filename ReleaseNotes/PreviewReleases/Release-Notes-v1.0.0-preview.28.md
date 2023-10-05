<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.28
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#752](https://github.com/KinsonDigital/Velaptor/issues/752) - Improved the loading of fonts by adding the extension method `Load(string fontName, uint size)` to the `FontLoader` class and `IFontLoader<IFont>` interface.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#752](https://github.com/KinsonDigital/Velaptor/issues/752) - Implemented the following breaking changes:
   - Renamed the parameter name of the `FontLoader.Load()` from `contentWithMetaData` to `contentPathOrName`.
   - Renamed the parameter name of the `FontLoader.Unload()` from `contentWithMetaData` to `contentPathOrName`.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#747](https://github.com/KinsonDigital/Velaptor/pull/747) - Updated the dependency _**kinsondigital/infrastructure action**_ to _**v13.1.0**_
2. [#745](https://github.com/KinsonDigital/Velaptor/pull/745) - Updated the dependency _**actions/checkout action**_ to _**v4.1.0**_
3. [#741](https://github.com/KinsonDigital/Velaptor/pull/741) - Updated the dependency _**xunit-dotnet monorepo**_ to _**v2.5.1**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#748](https://github.com/KinsonDigital/Velaptor/issues/748) - Added an additional funding options using the [OpenCollective](https://opencollective.com/) platform.
2. [#743](https://github.com/KinsonDigital/Velaptor/issues/743) - Improved build and status check workflows.
3. [#356](https://github.com/KinsonDigital/Velaptor/issues/356) - Refactored unit test assertions.
