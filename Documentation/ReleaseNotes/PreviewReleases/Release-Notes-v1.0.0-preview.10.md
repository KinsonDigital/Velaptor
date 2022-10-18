<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.10
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#382](https://github.com/KinsonDigital/Velaptor/issues/382) - Fixed a bug where various file and directory path processing and new line characters were only working when running on **Windows**.
   - This change made the entire application more cross-platform.
2. [#385](https://github.com/KinsonDigital/Velaptor/issues/385) - Fixed a bug with resolving paths when loading any type of content.
   - This bug was introduced when implementing issue [#382](https://github.com/KinsonDigital/Velaptor/issues/382).

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#344](https://github.com/KinsonDigital/Velaptor/issues/344) - Updated the projects to build and run for **_x64_(64 bit)** bit machines.
   - This was done due to the number of people out there running **_x86(32bit)_** machines.  Running this kind of architecture is getting rare and the work involved with supporting both is simply not worth the effort.
   - Updated all workflows to build and execute unit tests for the **_x64_(64 bit)** architecture.
   - Created publish profiles for **Windows** and **Linux** for each build configuration to assist with future cross-platform testing.
   - Configured the debugging console to display both **Debug**** and **Release** modes when using the **Velaptor Testing** application.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#339](https://github.com/KinsonDigital/Velaptor/issues/339) - Fixed an issue in the **_triage-issue_** workflow where the incorrect label was added to the issue.
2. [#342](https://github.com/KinsonDigital/Velaptor/issues/342) - Upgraded to the new build system for the organization named [CICD](https://github.com/CICD).
   - This is the new build system for all of the C#/dotnet projects in the [KinsonDigital](https://github.com/KinsonDigital) organization.
   - This upgrade/change of the build system made the work for issue [#259](https://github.com/KinsonDigital/Velaptor/issues/259) pointless due to this issue being implemented before the new build system was implemented.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#293](https://github.com/KinsonDigital/Velaptor/issues/293) - Updated the following NuGet packages throughout the entire solution below:
   - Upgraded **coverlet.msbuild** from version **_v3.1.0_** to **_v3.1.2_**.
   - Upgraded **Microsoft.NET.Test.Sdk** from version **_v16.10.0_** to **_v17.3.0_**.
   - Upgraded **Moq** from version **_v4.16.1_** to **_v4.18.2_**.
   - Upgraded **System.IO.Abstractions** from version **_v13.2.43_** to **_v17.1.1_**.
   - Upgraded **xunit** from version **_v2.4.1_** to **_v2.4.2_**.
   - Upgraded **xunit.runner.visualstudio** from version **_v2.4.3_** to **_v2.4.5_**.
   - Upgraded **Silk.NET** from version **_v2.6.0_** to **_v2.16.0_**.
   - Upgraded **SimpleInjector** from version **_v5.3.2_** to **_v5.4.0_**.
   - Upgraded **SixLabors.ImageSharp** from version **_v2.0.0_** to **_v2.1.3_**.
   - Upgraded **System.IO.Abstractions** from version **_v13.2.43_** to **_v17.1.1_**.
   - Downgraded **NVorbis** from version **_v0.10.4_** to **_v0.10.3_**.
     - The reasoning for the downgrade is not known and was unintentional.  The downgrade is safe due to this only being a patch change and testing was performed.  An upgrade is planned using version [#389](https://github.com/KinsonDigital/Velaptor/issues/389).

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#310](https://github.com/KinsonDigital/Velaptor/issues/310) - Made simple grammar change to the project's readme document.
2. [#309](https://github.com/KinsonDigital/Velaptor/issues/309) - Added contact email to the project's code of conduct.
3. [#302](https://github.com/KinsonDigital/Velaptor/issues/302) - Made various grammar and style changes to the project branching documentation.
   - This can be found at the location `./Documentation/Branching.md`.
4. [#283](https://github.com/KinsonDigital/Velaptor/issues/283) - Improved badges on the project's readme document.
5. [#147](https://github.com/KinsonDigital/Velaptor/issues/147) - Updated branching documentation.
   - Added clarification about preview feature branches, added a section about hotfix branches as well as improved grammar.
6. [#89](https://github.com/KinsonDigital/Velaptor/issues/89) - Set the application icon and NuGet package to the project logo.
7. [#89](https://github.com/KinsonDigital/Velaptor/issues/89) - Added the project license to the NuGet package.
8. The following issues were all improvements to all of the issues and pull request templates before they were all moved to the organization level.  This project does not manage its templates anymore.
   - [#338](https://github.com/KinsonDigital/Velaptor/issues/338), [#335](https://github.com/KinsonDigital/Velaptor/issues/335), [#333](https://github.com/KinsonDigital/Velaptor/issues/333), [#327](https://github.com/KinsonDigital/Velaptor/issues/327), [#325](https://github.com/KinsonDigital/Velaptor/issues/325), [#316](https://github.com/KinsonDigital/Velaptor/issues/316), [#312](https://github.com/KinsonDigital/Velaptor/issues/312), [#296](https://github.com/KinsonDigital/Velaptor/issues/296), [#295](https://github.com/KinsonDigital/Velaptor/issues/295), [#282](https://github.com/KinsonDigital/Velaptor/issues/282), [#267](https://github.com/KinsonDigital/Velaptor/issues/267), [#261](https://github.com/KinsonDigital/Velaptor/issues/261), [#260](https://github.com/KinsonDigital/Velaptor/issues/260), [#253](https://github.com/KinsonDigital/Velaptor/issues/253), [#219](https://github.com/KinsonDigital/Velaptor/issues/219), [#215](https://github.com/KinsonDigital/Velaptor/issues/215)
9. [#188](https://github.com/KinsonDigital/Velaptor/issues/188) - Added templates for preview and production release notes.
10. [#286](https://github.com/KinsonDigital/Velaptor/issues/286) - Updated the style rule for private read-only fields to not start with an underscore.
    - Added this as a **Style Cop** rule for rule number **SA1309**.
    - Updated the **JetBrains Rider** IDE tool to prevent a conflict with the **Style Cop** rule.
11. [#240](https://github.com/KinsonDigital/Velaptor/issues/240) - Added a CODEOWNERS file to the repository for the project maintainers [@CalvinWilkinson](https://github.com/CalvinWilkinson/CalvinWilkinson) and [@kselena](https://github.com/kselena/kselena).
12. [#222](https://github.com/KinsonDigital/Velaptor/issues/222) - Removed the **Mouse Scroll Speed** label from the mouse testing screen in the **Velaptor Testing** application.
    - The mouse scroll event behavior has been changed which does not fit within the context of the label.
13. [#280](https://github.com/KinsonDigital/Velaptor/issues/280) - Updated the **_Velaptor.runsettings_** file to exclude the **_VelaptorTesting_** used for manual QA testing from unit test code coverage results.
