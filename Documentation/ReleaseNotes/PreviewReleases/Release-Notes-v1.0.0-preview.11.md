<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.11
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Created a logging system for logging various information.
   - Automatically creates 2 different types of logs in a **_logs_** directory in the same directory as the application.
   - Standard logs for logging misc. info, warnings, and errors are logged into a text file that is prefixed with **_logs-_** and ending with a date in the format _yyyyMMdd_.
   - Each day will have a log file.
   - An event log file for logging events only is logged into a text file that is prefixed with **_event-logs-_** and ends with a date with the format _yyyyMMdd_.
   - Logging settings are set in the app settings file named **_app-settings.json_** which is located in the same directory as the application.
   - Use the settings below to set up the type of logging behavior.  
     These can be set to `true` or `false` to enable and disable them.
     - LoggingEnabled
     - ConsoleLoggingEnabled
     - FileLoggingEnabled
2. [#248](https://github.com/KinsonDigital/Velaptor/issues/248) - Created an application settings system.
   - Added the ability to manage application settings.  This can also be expanded upon in the future.
   - Added the ability to set the default window width and height when the application is executed.
   - Added a new method overload named `CreateWindow()` with no parameters to the `App` class.  This method uses the app settings to determine the window width and height instead of 2 parameters.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#373](https://github.com/KinsonDigital/Velaptor/issues/373) - Refactored the classes below to `internal` that were not meant to be part of the public API.
   - `ReactorUnsubscriber`
   - `ImageService`
   - `JSONService`
   - `SystemMonitorService`
2. [#373](https://github.com/KinsonDigital/Velaptor/issues/373) - Refactored the interfaces below to `internal` that were not meant to be part of the public API.
   - `ITemplateProcessorService`
   - `IFontAtlasService`
   - `IImageService`
   - `IJSONService`
   - `ISystemMonitorService`
3. [#373](https://github.com/KinsonDigital/Velaptor/issues/373) - Removed the string extension methods below that were not meant to be part of the public API.
   - `HasValidFullFilePathSyntax()`
   - `HasInvalidFullFilePathSyntax()`
   - `HasValidDriveSyntax()`
   - `HasValidFullDirPathSyntax()`
   - `HasValidUNCPathSyntax()`

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Created the custom **_XUnit_** attributes below to create unit tests that will only run in debug or release builds.
   - `FactForDebugAttribute`
   - `FactForReleaseAttribute`
2. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Set up the **OpenGL** error callback system to take advantage of the new logging feature.
3. [#373](https://github.com/KinsonDigital/Velaptor/issues/373) - Set many internal classes to sealed to achieve a closed by default coding practice.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Added the NuGet package **Serilog** version **_v2.12.0_**
   - This is used for some of the new logging features added to the project.
2. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Added the NuGet package **Serilog.Sinks.Console** version **_v4.1.0_**
   - This is used for some of the new logging features added to the project.
3. [#247](https://github.com/KinsonDigital/Velaptor/issues/247) - Added the NuGet package **Serilog.Sinks.File** version **_v5.0.0_**
   - This is used for some of the new logging features added to the project.
4. [#264](https://github.com/KinsonDigital/Velaptor/issues/264) - Added the NuGet package **FluentAssertions** version **_v6.7.0_**
