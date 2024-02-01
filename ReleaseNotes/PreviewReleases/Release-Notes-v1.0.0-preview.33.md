<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.33
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#903](https://github.com/KinsonDigital/Velaptor/issues/903) - Fixed a bug where the batching system would throw an exception when the draw calls exceed more than 1000.
   > **Thank you [@softwareantics](https://github.com/softwareantics)** for discovering and reporting this issue!!
2. [#902](https://github.com/KinsonDigital/Velaptor/issues/902) - Fixed a bug where the `WindowCenter` and `WindowSize` properties for scenes are not set after instantiation but before internal initialization.
    > **Thank you [@softwareantics](https://github.com/softwareantics)** for discovering and reporting this issue!!
3. [#902](https://github.com/KinsonDigital/Velaptor/issues/902) - Fixed rendering text bug.
    > **Thank you [@softwareantics](https://github.com/softwareantics)** for discovering and reporting this issue!!

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#895](https://github.com/KinsonDigital/Velaptor/pull/895) - Updated _**system.io.abstractions**_ to _**v20.0.15**_
2. [#894](https://github.com/KinsonDigital/Velaptor/pull/894) - Updated _**actions/cache**_ action to _**v4**_
3. [#893](https://github.com/KinsonDigital/Velaptor/pull/893) - Updated _**avalonia.themes.fluent**_ to _**v11.0.7**_
4. [#892](https://github.com/KinsonDigital/Velaptor/pull/892) - Updated _**avalonia.fonts.inter**_ to _**v11.0.7**_
5. [#891](https://github.com/KinsonDigital/Velaptor/pull/891) - Updated _**avalonia.diagnostics**_ to _**v11.0.7**_
6. [#889](https://github.com/KinsonDigital/Velaptor/pull/889) - Updated _**avalonia.desktop**_ to _**v11.0.7**_
7. [#888](https://github.com/KinsonDigital/Velaptor/pull/888) - Updated _**avalonia**_ to _**v11.0.7**_
8. [#887](https://github.com/KinsonDigital/Velaptor/pull/887) - Updated _**kinsondigital/infrastructure**_ action to _**v13.6.3**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#890](https://github.com/KinsonDigital/Velaptor/issues/890) - Updated readme.
2. [#883](https://github.com/KinsonDigital/Velaptor/issues/883) - Refactored moq code to nsubstitute code.
