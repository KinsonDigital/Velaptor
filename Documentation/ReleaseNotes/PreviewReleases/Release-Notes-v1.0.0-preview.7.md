<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.7</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, including releases, there is always a chance for issues and bugs.  It is also common to miss changes in the release notes when there are many.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#75](https://github.com/KinsonDigital/Velaptor/issues/75) - Add ability to render rectangles to the scene with the various attributes below:
   - Solid/filled rectangles
   - Empty rectangles
   - Ability to change the fill color of a filled rectangle
   - Ability to change the border color of an empty rectangle
   - Ability to have a horizontal color gradient with a start and end color
     - This can be applied to filled and empty rectangles
   - Ability to have a vertical color gradient with a start and end color
     - This can be applied to filled and empty rectangles
   - Ability to change the border thickness of an empty rectangle
   - Ability to set radius of each corner independently
     - This can be applied to filled and empty rectangles
2. [#131](https://github.com/KinsonDigital/Velaptor/issues/131) - Loading sounds cannot be cached to greatly improve sound loading performance.
3. [#128](https://github.com/KinsonDigital/Velaptor/issues/128) - Loading sounds can now be loaded via a fully qualified file path.  This means you do not have to only load sounds from the content system!!

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#153](https://github.com/KinsonDigital/Velaptor/issues/153) - The class `SystemMonitor` constructor no longer requires an `IPlatform` type argument.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit in any of the categories above)</h5>

1. [#153](https://github.com/KinsonDigital/Velaptor/issues/153) - Improved constructor argument null checks by implementing a guarding pattern.
   - This resulted in null checks that were not being performed previously.
2. [#150](https://github.com/KinsonDigital/Velaptor/issues/150), [#158](https://github.com/KinsonDigital/Velaptor/issues/158) - Improvements to PR templates such as the code review checklist, grammar, and other miscellaneous items.  This was done on all pull request templates.
3. [#159](https://github.com/KinsonDigital/Velaptor/issues/159) - Simple graphical fix with light and dark mode versions of the branching diagram.
4. [#162](https://github.com/KinsonDigital/Velaptor/issues/162) - Added a **_BACKERS.md_** file to the root of the solution to hold a list of any backers that sponsor projects.
5. [#159](https://github.com/KinsonDigital/Velaptor/issues/159) - Made various improvements to all of the issue templates.
