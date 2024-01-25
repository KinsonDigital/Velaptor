<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.32
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">New Features ✨</h2>

1. [#859](https://github.com/KinsonDigital/Velaptor/issues/859) - Changed the following properties of the `CornerRadius` struct to `init` properties.
   - `TopLeft`
   - `TopRight`
   - `BottomRight`
   - `BottomLeft`

<h2 align="center" style="font-weight: bold;">Performance Improvements 🏎️</h2>

1. [#801](https://github.com/KinsonDigital/Velaptor/issues/801) - Refactored the `FontStats` struct to a `readonly record` struct.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**
3. [#481](https://github.com/KinsonDigital/Velaptor/issues/481) - Improved performance of OpenGL buffering when uploading GPU data.
    > **Thank you [@AndreBonda](https://github.com/AndreBonda)!!**

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#875](https://github.com/KinsonDigital/Velaptor/issues/875) - Fixed a bug where invoking the `Window.Show()` and `Window.ShowAsync()` methods after the `Window` has been disposed of throws incorrect exceptions.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#859](https://github.com/KinsonDigital/Velaptor/issues/859) - Set the font size to a maximum value of 100 when rendering text.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#863](https://github.com/KinsonDigital/Velaptor/pull/863) - Updated _**benchmarkdotnet**_ to _**v0.13.12**_
2. [#862](https://github.com/KinsonDigital/Velaptor/pull/862) - Updated _**simpleinjector**_ to _**v5.4.4**_
3. [#851](https://github.com/KinsonDigital/Velaptor/pull/851) - Updated _**sixlabors.imagesharp**_ to _**v3.1.2**_
4. [#850](https://github.com/KinsonDigital/Velaptor/pull/850) - Updated _**xunit**_ to _**v2.6.6**_
5. [#850](https://github.com/KinsonDigital/Velaptor/pull/850) - Updated _**xunit.runner.visualstudio**_ to _**v2.5.6**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#875](https://github.com/KinsonDigital/Velaptor/issues/875) - Refactored _**Moq**_ unit test code to  _**NSubstitute**_ in the internal `GLWindow` class.
2. [#859](https://github.com/KinsonDigital/Velaptor/issues/859) - Replaced all UI controls in the _**VelaptorTesting**_ application with [ImGui](https://github.com/ImGuiNET/ImGui.NET).
