<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.14
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#154](https://github.com/KinsonDigital/Velaptor/issues/154) - Added the ability to render 2D line primitives with the following features.
   > **💡**
   > To render a line, use the new render methods added to `IRenderer` interface.
   - Simple 2D line primitive rendering.
   - Line colors.
   - Line thickness.

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#154](https://github.com/KinsonDigital/Velaptor/issues/154) - Fixed a small rendering bug introduced in the last release.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#154](https://github.com/KinsonDigital/Velaptor/issues/154) - Removed public constant named `BatchSize` from the `IRenderer` interface.
   > **💡**
   > This breaking change is very low impact.  This constant was only used internally within the library and did not hold any value for outside users.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#351](https://github.com/KinsonDigital/Velaptor/issues/351) - Refactored unit test code assertions to use [Fluent Assertions](https://fluentassertions.com/).
2. [#172](https://github.com/KinsonDigital/Velaptor/issues/172) - Created a new [guard](https://maximegel.medium.com/what-are-guard-clauses-and-how-to-use-them-350c8f1b6fd2) to check for zero/null pointers.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#351](https://github.com/KinsonDigital/Velaptor/issues/351) - Updated the NuGet packages below:
   - Updated **SimpleInjector** from _**v5.4.0 to v5.4.1**_
   - Updated **Newtonsoft.Json** from _**v13.0.1 to v13.0.2**_
   - Updated **Moq** from version _**v4.18.2 to v4.18.3**_
   - Updated **Microsoft.NET.Test.Sdk** from version _**v17.3.0 to v17.4.0**_
   - Updated **coverlet.msbuild** from version _**v3.1.2 to v3.2.0**_

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#154](https://github.com/KinsonDigital/Velaptor/issues/154) - Added two new scenes to the _**VelaptorTesting**_ application.
2. [#433](https://github.com/KinsonDigital/Velaptor/issues/433) - Fixed a broken link to the logo in the project README file that was used for the NuGet package.
