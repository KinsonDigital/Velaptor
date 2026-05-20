<h1 align="center" style="color: mediumseagreen;font-weight: bold;">
Velaptor Preview Release Notes - v1.0.0-preview.45
</h1>

<h2 align="center" style="font-weight: bold;">Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, so your input is greatly appreciated. 🙏🏼
</div>

<h2 align="center" style="font-weight: bold;">Enhancements 💎</h2>

1. [#1214](https://github.com/KinsonDigital/Velaptor/issues/1214) - Added opt-in only usage and hardware instrumentation.
2. [#1179](https://github.com/KinsonDigital/Velaptor/issues/1179) - Improved disposable implementations for atomic operations.
3. [#1164](https://github.com/KinsonDigital/Velaptor/issues/1164) - Added path normalization to content loading system.

<h2 align="center" style="font-weight: bold;">Bug Fixes 🐛</h2>

1. [#1062](https://github.com/KinsonDigital/Velaptor/issues/1062) - Fixed a bug where using non-keyboard keys such as volume knobs and buttons was causing **Velaptor** to crash. Updating _**silk.net**_ to _**v2.23.0**_ fixed this.

<h2 align="center" style="font-weight: bold;">Tech Debt 📉</h2>

1. [#1197](https://github.com/KinsonDigital/Velaptor/issues/1197) - Fixed failing tests and flaky CICD workflows by moving preprocessor directives from the `OpenGLService` to `GLInvoker`.

<h2 align="center" style="font-weight: bold;">Documentation 📄</h2>

1. [#1201](https://github.com/KinsonDigital/Velaptor/issues/1201) - Updated the contribution/funding section in the README file.
2. [#1193](https://github.com/KinsonDigital/Velaptor/issues/1193) - Added a Bluesky link to README file.
3. [#1191](https://github.com/KinsonDigital/Velaptor/issues/1191) - Improved code docs for the `ISceneManager` interface.

<h2 align="center" style="font-weight: bold;">Configuration 🛠️</h2>

1. [#1221](https://github.com/KinsonDigital/Velaptor/issues/1221) - Updated the production and preview release templates.
2. [#1204](https://github.com/KinsonDigital/Velaptor/issues/1204) - Created custom agents.

<h2 align="center" style="font-weight: bold;">CICD ⚙️</h2>

1. [#1182](https://github.com/KinsonDigital/Velaptor/issues/1182) - Updated the build, SDK, and unit test status checks workflows and release workflow to build for Windows, Linux, and macOS.

<h2 align="center" style="font-weight: bold;">Dependency Updates 📦</h2>

1. [#1226](https://github.com/KinsonDigital/Velaptor/pull/1226) - Updated the dependency _**hardware.info**_ to _**v110.0.0**_.
2. [#1224](https://github.com/KinsonDigital/Velaptor/pull/1224) - Updated the dependency _**avalonia.diagnostics**_ to _**v11.3.16**_.
3. [#1222](https://github.com/KinsonDigital/Velaptor/pull/1222) - Updated the dependency _**simpleinjector**_ to _**v5.5.2**_.
4. [#1218](https://github.com/KinsonDigital/Velaptor/pull/1218) - Updated the dependency _**microsoft.net.test.sdk**_ to _**v18.5.1**_.
5. [#1217](https://github.com/KinsonDigital/Velaptor/pull/1217) - Updated the dependency _**microsoft.codeanalysis.netanalyzers**_ to _**v10.0.300**_.
6. [#1212](https://github.com/KinsonDigital/Velaptor/pull/1212) - Updated the dependency _**coverlet.collector**_ to _**v10.0.0**_.
7. [#1200](https://github.com/KinsonDigital/Velaptor/pull/1200) - Updated the dependency _**avalonia**_ to _**v12.0.0**_.
8. [#1202](https://github.com/KinsonDigital/Velaptor/pull/1202) - Updated the dependency _**system.io.abstractions**_ to _**v22.1.1**_.
9. [#1192](https://github.com/KinsonDigital/Velaptor/pull/1192) - Updated the dependency _**communitytoolkit.mvvm**_ to _**v8.4.2**_.
10. [#1185](https://github.com/KinsonDigital/Velaptor/pull/1185) - Updated the dependency _**serilog**_ to _**v4.3.1**_.
11. [#1180](https://github.com/KinsonDigital/Velaptor/pull/1180) - Updated the dependency _**freetypesharp**_ to _**v3.1.0**_.
12. [#1177](https://github.com/KinsonDigital/Velaptor/pull/1177) - Updated the dependency _**silk.net**_ to _**v2.23.0**_.
13. [#1178](https://github.com/KinsonDigital/Velaptor/pull/1178) - Updated the dependency _**silk.net.opengl.extensions.imgui**_ to _**v2.23.0**_.
