<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.25
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#710](https://github.com/KinsonDigital/Velaptor/issues/710) - Added the ability to get information about the system displays.
2. [#99](https://github.com/KinsonDigital/Velaptor/issues/99) - Added new half-width and height properties to UI controls.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#708](https://github.com/KinsonDigital/Velaptor/issues/708) - Fixed an issue with proper shutdown.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#710](https://github.com/KinsonDigital/Velaptor/issues/710) - Introduced the following breaking changes:
    - Refactored the `InputFactory` class name to `HardwareFactory`.
    - Refactored the `HardwareFactory.CreateKeyboard()` method name to `HardwareFactory.GetKeyboard()`.
    - Refactored the `HardwareFactory.CreateMouse()` method name to `HardwareFactory.GetMouse()`.
    - Refactored the `SystemMonitor` class name to a `readonly record struct`.
    - Refactored the `SystemMonitorException` class name to `SystemDisplayException`.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#717](https://github.com/KinsonDigital/Velaptor/pull/717) - Updated kinsondigital/infrastructure action to v13
2. [#712](https://github.com/KinsonDigital/Velaptor/pull/712) - Updated dependency sixlabors.imagesharp to v3.0.2
3. [#711](https://github.com/KinsonDigital/Velaptor/pull/711) - Updated dependency microsoft.net.test.sdk to v17.7.2
4. [#707](https://github.com/KinsonDigital/Velaptor/pull/707) - Updated dependency microsoft.codeanalysis.netanalyzers to v7.0.4
5. [#704](https://github.com/KinsonDigital/Velaptor/pull/704) - Updated dependency system.io.abstractions to v19.2.69

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#705](https://github.com/KinsonDigital/Velaptor/issues/705) - Adjusted workflow triggers.
2. [#366](https://github.com/KinsonDigital/Velaptor/issues/366) - Refactored unit test assertions.
