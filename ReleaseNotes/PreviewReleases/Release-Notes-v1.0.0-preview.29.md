<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.29
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Enhancements 💎</h2>

1. [#755](https://github.com/KinsonDigital/Velaptor/issues/755) - Removed the font size syntax requirements when loading fonts.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#755](https://github.com/KinsonDigital/Velaptor/issues/755) - Thrown exception changes implemented.
   - Previously, when no font size meta-data syntax was used, an exception would be thrown.  Instead, a default font size of 12 is used instead of throwing an exception.  This is technically a breaking change.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. Velaptor dependencies:
   - [#772](https://github.com/KinsonDigital/Velaptor/pull/772) - Updated dependency _**silk.net**_ to _**v2.18.0**_
1. Performance testing application dependencies: 
   - [#757](https://github.com/KinsonDigital/Velaptor/pull/757) - Updated dependency _**benchmarkdotnet**_ to _**v0.13.9**_
1. Avalonia testing application dependencies:
   - [#771](https://github.com/KinsonDigital/Velaptor/pull/771) - Updated dependency _**avalonia.themes.fluent**_ to _**v11.0.5**_
   - [#770](https://github.com/KinsonDigital/Velaptor/pull/770) - Updated dependency _**avalonia.fonts.inter**_ to _**v11.0.5**_
   - [#769](https://github.com/KinsonDigital/Velaptor/pull/769) - Updated dependency _**avalonia.diagnostics**_ to _**v11.0.5**_
   - [#768](https://github.com/KinsonDigital/Velaptor/pull/768) - Updated dependency _**avalonia.desktop**_ to _**v11.0.5**_
   - [#767](https://github.com/KinsonDigital/Velaptor/pull/767) - Updated dependency _**avalonia**_ to _**v11.0.5**_
1. Unit testing dependencies:
   - [#773](https://github.com/KinsonDigital/Velaptor/pull/773) - Updated dependency _**xunit**_ to _**v2.5.3**_
   - [#762](https://github.com/KinsonDigital/Velaptor/pull/762) - Updated _**xunit.runner.visualstudio**_ to _**v2.5.3**_
1. CICD related dependencies:
   - [#766](https://github.com/KinsonDigital/Velaptor/pull/766) - Updated _**actions/checkout**_ action to _**v4.1.1**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#751](https://github.com/KinsonDigital/Velaptor/issues/751) - Created unit tests for extension methods.
2. [#359](https://github.com/KinsonDigital/Velaptor/issues/359) - Refactored unit test assertions.
3. [#237](https://github.com/KinsonDigital/Velaptor/issues/237) - Refactored switch statements and out of range exceptions.
