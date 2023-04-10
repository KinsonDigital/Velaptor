<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.19
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#550](https://github.com/KinsonDigital/Velaptor/issues/550) - Added the ability to automatically create the content directories while the application during runtime.
2. [#551](https://github.com/KinsonDigital/Velaptor/issues/551) - Created new types `ImageLoader` class and `IImageLoader` interface to easily load images.
   > **Note** This gives the user the ability to do pre-processing on the pixels of an image before using it to create a `Texture`.
3. [#551](https://github.com/KinsonDigital/Velaptor/issues/551) - Added a new constructor to the `Texture` class.  The new constructor signature is `Texture(string name, ImageData imageData)`.
4. [#559](https://github.com/KinsonDigital/Velaptor/issues/559) - Added a new overload to the `ITextureRenderer` interface and `TextureRenderer` class.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#551](https://github.com/KinsonDigital/Velaptor/issues/551) - Added the following breaking changes.
   - Changed all of the public fields of the `ImageData` struct to `public readonly` properties.
   - Removed the `ImageData` parameter from the `Texture` class constructor.


---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#584](https://github.com/KinsonDigital/Velaptor/issues/584) - Updated **Silk.NET** from _**v2.16.0**_ to _**v2.17.0**_
2. [#554](https://github.com/KinsonDigital/Velaptor/pull/554) - Updated **System.IO.Abstractions** from _**v19.2.1**_ to _**v19.2.4**_
3. [#552](https://github.com/KinsonDigital/Velaptor/pull/552) - Updated **Microsoft.CodeAnalysis.NetAnalyzers** from _**v7.0.0**_ to _**v7.0.1**_

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#532](https://github.com/KinsonDigital/Velaptor/issues/532) - Integrated [SonarSource](https://www.sonarsource.com/products/sonarcloud/) into the project for code quality scans.  Also created a workflow to scan the code base upon a push for each pull request.
2. [#532](https://github.com/KinsonDigital/Velaptor/issues/532) - Added badges to the project _**README.md**_ file to show Sonar cloud statistics.
3. [#562](https://github.com/KinsonDigital/Velaptor/issues/562) - Updated the [CICD](https://github.com/KinsonDigital/CICD) tool from version _**v1.0.0-preview.17**_ to _**v1.0.0-preview.20**_
4. [#567](https://github.com/KinsonDigital/Velaptor/issues/567) - Removed unnecessary metadata from the Velaptor NuGet package.
5. [#567](https://github.com/KinsonDigital/Velaptor/issues/567) - Removed the Codacy code scanning tool.
6. [#574](https://github.com/KinsonDigital/Velaptor/issues/574) - Setup [Renovate](https://www.mend.io/renovate/) dependency management tool/bot.
7. [#572](https://github.com/KinsonDigital/Velaptor/issues/572) - Updated the contributor covenant badge to reflect the correct version of the contributor covenant used.
8. [#568](https://github.com/KinsonDigital/Velaptor/issues/568) - Updated various badges in the project _**README**_ file.
9. [#548](https://github.com/KinsonDigital/Velaptor/issues/548) - Made simple changes to the preview release workflow.
10. [#548](https://github.com/KinsonDigital/Velaptor/issues/548) - Removed the [dependabot](https://docs.github.com/en/code-security/dependabot) tool.
    > **Note** This is replaced with renovate.
11. [#549](https://github.com/KinsonDigital/Velaptor/issues/549) - Changed the code coverage requirement threshold from automatic to 85%.
12. [#555](https://github.com/KinsonDigital/Velaptor/issues/555) - Removed and cleaned up pre-processing comments from the project _**README**_ file.
13. [#553](https://github.com/KinsonDigital/Velaptor/issues/553) - Fixed issue with the workflow for running status checks for preview features.

