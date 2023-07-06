<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.18
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#541](https://github.com/KinsonDigital/Velaptor/issues/541) - Fixed a bug where creating a new `Window` object would throw an exception related to fonts.
    > 💡 This was due to the system trying to create the default fonts
    > but instead trying to overwrite an OS font which would throw a permission exception.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#541](https://github.com/KinsonDigital/Velaptor/issues/541) - Implemented the following breaking changes:
   - Changed the name of the `IPathResolver` to `IContentPathResolver`.
   - Set the following exception classes to `sealed`:
     - `LoadAtlasException`
     - `CachingMetaDataException`
     - `CachingException`
     - `LoadContentException`
     - `LoadFontException`
     - `LoadSoundException`
     - `LoadTextureException`
     - `PushNotificationException`
     - `InvalidInputException`
     - `NoKeyboardException`
     - `NoMouseException`
     - `BufferNotInitializedException`
     - `ShaderNotInitializedException`
---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#540](https://github.com/KinsonDigital/Velaptor/pull/540) - Updated **Newtonsoft.Json** from _**v13.0.2**_ to _**v13.0.3**_
2. [#539](https://github.com/KinsonDigital/Velaptor/pull/539) - Updated **System.IO.Abstractions** from _**v19.1.5**_ to _**v19.2.1**_
3. [#537](https://github.com/KinsonDigital/Velaptor/pull/537) - Updated **SixLabors.ImageSharp** from _**v2.1.3**_ to _**v3.0.0**_
4. [#534](https://github.com/KinsonDigital/Velaptor/pull/534) - Updated **Microsoft.NET.Test.Sdk** from _**v17.4.1**_ to _**v17.5.0**_
5. [#528](https://github.com/KinsonDigital/Velaptor/pull/528) - Updated **FluentAssertions** from _**v6.9.0**_ to _**v6.10.0**_

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#535](https://github.com/KinsonDigital/Velaptor/issues/535) - Updated the Twitter links in the project's _**README.md**_ file.
2. [#530](https://github.com/KinsonDigital/Velaptor/issues/530) - Removed various markdown and image files from the project.  These have been moved to the organizational level.
3. [#526](https://github.com/KinsonDigital/Velaptor/issues/526) - Setup code coverage with CICD system through [codecov](https://about.codecov.io/).
4. [#122](https://github.com/KinsonDigital/Velaptor/issues/122) - Updated code documentation for constructors throughout the entire codebase.
