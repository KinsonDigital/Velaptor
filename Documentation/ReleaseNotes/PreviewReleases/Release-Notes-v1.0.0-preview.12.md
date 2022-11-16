<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.12
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#375](https://github.com/KinsonDigital/Velaptor/issues/375) - Refactored unit test code to use the [FluentAssertions](https://fluentassertions.com/) library.
   - This was done by the outside contributor [@SyedMSawaid](https://twitter.com/SyedMSawaid) during [Hacktoberfest 2022](https://hacktoberfest.com/)!! Thanks!! 😉
2. [#285](https://github.com/KinsonDigital/Velaptor/issues/285) - Refactored code docs and test object implementations.
3. [#285](https://github.com/KinsonDigital/Velaptor/issues/285) - Refactored the entire code base to use [file-scoped namespacing](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#file-scoped-namespace-declaration) instead of using [block-scoped namespacing](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/namespace).
4. [#171](https://github.com/KinsonDigital/Velaptor/issues/171) - Changed the data type of constructor parameters and properties from `int` to `uint`.
   - This is not a breaking change and is only an internal change.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#426](https://github.com/KinsonDigital/Velaptor/issues/426) - Improved project **_README_** and **_CONTRIBUTING_** documents by fixing broken links, adding **_Note_** and **_Warning_** blocks, and fixing the indentation of some list items.
2. [#423](https://github.com/KinsonDigital/Velaptor/issues/423) - Added the project **_README_** and **_LICENSE_** documents to the NuGet package.
    - This includes setting up the project README with comment and uncomment blocks of content for pre-processing by the GitHub organization's [CICD](https://github.com/KinsonDigital/CICD) system.  Refer to issue [#121](https://github.com/KinsonDigital/CICD/issues/121) for more information.
3. [#408](https://github.com/KinsonDigital/Velaptor/issues/408) - Improved the project's outside **_CONTRIBUTING_** document.
4. [#404](https://github.com/KinsonDigital/Velaptor/issues/404) - Set up the project to detect and set issues as stale using the stale bot [here](https://github.com/probot/stale).
5. [#395](https://github.com/KinsonDigital/Velaptor/issues/395) - Updated the [CICD](https://github.com/KinsonDigital/CICD) system for the project from version **_v1.0.0-preview.12_** to **_v1.0.0-preview.13_**.
6. [#395](https://github.com/KinsonDigital/Velaptor/issues/395) - Added a release tweet template to the project for the [CICD](https://github.com/KinsonDigital/CICD) system to use when performing releases.
   - This template is what is used when doing a preview or production release to send out a tweet about the release.
7. [#393](https://github.com/KinsonDigital/Velaptor/issues/393) - Removed the ability for the project to copy the native library **_soft_oal.dll_** to the build target directory.
   - This library is used by **_OpenAL_** for sound.  This is not required anymore due to using the [CASL](https://github.com/KinsonDigital/CASL) project instead for sound.
8. [#389](https://github.com/KinsonDigital/Velaptor/issues/389) - Performed simple project file cleanup.
9. [#388](https://github.com/KinsonDigital/Velaptor/issues/388) - Changed the label for the discord badge in the **_README_** file from `discord` to `chat on discord`.
10. [#376](https://github.com/KinsonDigital/Velaptor/issues/376) - Updated the project's logo to a new and improved logo.
11. [#347](https://github.com/KinsonDigital/Velaptor/issues/347) - Added badge to the project's **_README_** file that shows the total number of NuGet package downloads.
