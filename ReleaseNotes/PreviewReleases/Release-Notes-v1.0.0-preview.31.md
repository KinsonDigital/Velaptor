<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.31
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#804](https://github.com/KinsonDigital/Velaptor/issues/804) - Fixed the following bugs:
    - Fixed a bug where the `KeyboardState.KeyToChar()` method would return a single apostrophe instead of a double apostrophe when a shift key was pressed.
    - Fixed a bug where the `KeyboardState.KeyToChar()` method would return nothing when the keycode is the grave accent key while the shift key was not being pressed down.

<h2 align="center" style="font-weight: bold;">Breaking Changes 🧨</h2>

1. [#805](https://github.com/KinsonDigital/Velaptor/issues/805) - Refactored the `MouseState` struct to a `readonly` struct.
2. [#804](https://github.com/KinsonDigital/Velaptor/issues/804) - Introduced the following breaking changes:
    - Removed the `GetKeyStates()` method from the `KeyboardState` struct.  This method was not needed due to the other methods available for users to get the state of the keyboard keys.
    - Removed the `AnyKeysDown(IEnumerable<KeyCode>)` from the `KeyboardState` struct.  This method was not needed due to the other methods available for users to get the state of the keyboard keys.
3. [#803](https://github.com/KinsonDigital/Velaptor/issues/803) - Refactored the `GlyphMetrics` to a `readonly record` struct.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#843](https://github.com/KinsonDigital/Velaptor/pull/843) - Updated all projects to dotnet _**v8**_
2. [#842](https://github.com/KinsonDigital/Velaptor/pull/842) - Updated reusable workflows/infrastructure to _**v13.6.0**_
4. [#827](https://github.com/KinsonDigital/Velaptor/pull/827) - Updated _**actions/setup-dotnet**_ action to v4
5. [#825](https://github.com/KinsonDigital/Velaptor/pull/825) - Updated _**actions/setup-java**_ action to v4
6. [#837](https://github.com/KinsonDigital/Velaptor/pull/837) - Updated _**xunit-dotnet monorepo**_
7. [#840](https://github.com/KinsonDigital/Velaptor/pull/840) - Updated dependency _**simpleinjector**_ to _**v5.4.3**_
8. [#839](https://github.com/KinsonDigital/Velaptor/pull/839) - Updated dependency _**silk.net**_ to _**v2.20.0**_
9. [#838](https://github.com/KinsonDigital/Velaptor/pull/838) - Updated dependency _**sixlabors.imagesharp**_ to _**v3.1.1**_
10. [#830](https://github.com/KinsonDigital/Velaptor/pull/830) - Updated dependency _**system.io.abstractions**_ to _**v20.0.0**_
11. [#822](https://github.com/KinsonDigital/Velaptor/pull/822) - Updated dependency _**serilog.sinks.console**_ to _**v5.0.1**_
12. [#799](https://github.com/KinsonDigital/Velaptor/pull/799) - Updated dependency _**serilog.sinks.console**_ to _**v5.0.0**_
13. [#813](https://github.com/KinsonDigital/Velaptor/pull/813) - Updated dependency _**microsoft.codeanalysis.netanalyzers**_ to _**v8.0.0**_
14. [#797](https://github.com/KinsonDigital/Velaptor/pull/797) - Updated dependency _**microsoft.net.test.sdk**_ to _**v17.8.0**_
15. [#798](https://github.com/KinsonDigital/Velaptor/pull/798) - Updated dependency _**serilog**_ to _**v3.1.1**_
16. [#836](https://github.com/KinsonDigital/Velaptor/pull/836) - Updated dependency _**benchmarkdotnet**_ to _**v0.13.11**_
17. [#831](https://github.com/KinsonDigital/Velaptor/pull/831) - Updated dependency _**avalonia**_ to _**v11.0.6**_
18. [#835](https://github.com/KinsonDigital/Velaptor/pull/835) - Updated dependency _**avalonia.themes.fluent**_ to _**v11.0.6**_
19. [#834](https://github.com/KinsonDigital/Velaptor/pull/834) - Updated dependency _**avalonia.fonts.inter**_ to _**v11.0.6**_
20. [#833](https://github.com/KinsonDigital/Velaptor/pull/833) - Updated dependency _**avalonia.diagnostics**_ to _**v11.0.6**_
21. [#832](https://github.com/KinsonDigital/Velaptor/pull/832) - Updated dependency _**avalonia.desktop**_ to _**v11.0.6**_
22. [#828](https://github.com/KinsonDigital/Velaptor/pull/828) - Updated dependency _**kinsondigital.casl**_ to _**v1.0.0-preview.17**_

<h2 align="center" style="font-weight: bold;">Other 🪧</h2>

1. [#845](https://github.com/KinsonDigital/Velaptor/issues/845) - Deprecated the UI controls API.
2. [#844](https://github.com/KinsonDigital/Velaptor/issues/844) - Processed all warnings.
3. [#823](https://github.com/KinsonDigital/Velaptor/issues/823) - Added permission to sync bot workflow.
4. [#818](https://github.com/KinsonDigital/Velaptor/issues/818) - Fixed deno permission issue.
5. [#806](https://github.com/KinsonDigital/Velaptor/issues/806) - Refactored `GlfwVideoMode` to `readonly` struct.
6. [#802](https://github.com/KinsonDigital/Velaptor/issues/802) - Refactored `FontAtlasMetrics` to `readonly` record.
7. [#786](https://github.com/KinsonDigital/Velaptor/issues/786), [#785](https://github.com/KinsonDigital/Velaptor/issues/785), [#784](https://github.com/KinsonDigital/Velaptor/issues/784) - Replaced custom guards.
8. [#764](https://github.com/KinsonDigital/Velaptor/issues/764) - Refactored `KeyCodeExtension` tests.
9. [#101](https://github.com/KinsonDigital/Velaptor/issues/101) - Replaced all equality checks with pattern matching.
