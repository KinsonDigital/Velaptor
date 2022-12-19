<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.15
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#447](https://github.com/KinsonDigital/Velaptor/issues/447) - Fixed a bug when rendering text where if the text was only carriage return and/or line feed characters, the application would crash.
   > **💡**Carriage returns characters are _**\r**_ and line feed characters are _**\n**_.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#370](https://github.com/KinsonDigital/Velaptor/issues/370) - Refactored the unit test assertion code for the following classes to [Fluent Assertions](https://fluentassertions.com/).
   - `TextureBatchingService`
   - `Button`
   - `ControlBase`
   - `Label`
   - `MousePositionEventArgs`

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#437](https://github.com/KinsonDigital/Velaptor/issues/437) - Updated NuGet package **Microsoft.CodeAnalysis.NetAnalyzers** from version _**v6.0.0**_ to _**v7.0.0**_
   > **💡** This does not directly affect the project and is only for development purposes for analyzing code.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#437](https://github.com/KinsonDigital/Velaptor/issues/437) - Updated the project to [dotnet 7.0](https://devblogs.microsoft.com/dotnet/announcing-dotnet-7/) and version of C# language version from _**v10.0**_ to _**v11.0**_!!
   > **💡**This includes the other various projects within the application such as the unit testing and _**Velaptor Testing**_ projects.
2. [#447](https://github.com/KinsonDigital/Velaptor/issues/447) - Improved the performance of the `Font` class.
   > **💡**These performance improvements affect the render text process as well as UI controls that contain text.
   - Improved the performance of the `Font.Measure()` method which is used to measure the size of the text.
   - Improved the performance of the `Font.ToGlyphMetrics()` method which is used to get the metrics for all of the glyphs in a piece of text.
   - Improved the performance of the `Font.GetCharacterBounds()` method which is for getting a list of all the individual characters and the associated rectangular bounds of each character for a piece of text.
   - Improved the internal performance of the font glyph texture atlas process.
3. [#461](https://github.com/KinsonDigital/Velaptor/issues/461) - Fixed a broken link for the logo in the project README file.
    > **💡**This was only for the readme file for the NuGet package which  also fixed various links in the readme.
