<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.7</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, including releases, there is always a chance for issues and bugs.  It is also common to miss changes in the release notes when there are many.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Add ability to render rectangles to the scene with the various attributes below:  
   GitHub Issues: [#75](https://github.com/KinsonDigital/Velaptor/issues/75)
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
2. Loading sounds cannot be cached to greatly improve sound loading performance.  
   GitHub Issues: [#131](https://github.com/KinsonDigital/Velaptor/issues/131)
3. Loading sounds can now be loaded via a fully qualified file path.  This means you do not have to only load sounds from the content system!!.  
   GitHub Issues: [#128](https://github.com/KinsonDigital/Velaptor/issues/128)

---

<h2 style="font-weight:bold" align="center">Breaking Changes 💣</h2>

1. The class `SystemMonitor` constructor no longer requires an `IPlatform` type argument.  
   GitHub Issues: [#153](https://github.com/KinsonDigital/Velaptor/issues/153)

---

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Upgraded all projects in the solution to use C# language **_v10_**  
   GitHub Issues: [#97](https://github.com/KinsonDigital/Velaptor/issues/97), [#184](https://github.com/KinsonDigital/Velaptor/issues/184)
2. Upgraded all projects in the solution to use **_NET 6.0_**  
   GitHub Issues: [#97](https://github.com/KinsonDigital/Velaptor/issues/97), [#184](https://github.com/KinsonDigital/Velaptor/issues/184)
1. Improved constructor argument null checks by implementing a guarding pattern.  
   GitHub Issues: [#153](https://github.com/KinsonDigital/Velaptor/issues/153)
   - This resulted in null checks that were not being performed previously

---

<h2 style="font-weight:bold" align="center">Other 👏</h2>

1. Improvements to PR templates such as the code review checklist, grammar, and other miscellaneous items.  This was done on all pull request templates.  
   GitHub Issues: [#150](https://github.com/KinsonDigital/Velaptor/issues/150), [#158](https://github.com/KinsonDigital/Velaptor/issues/158)
2. Simple graphical fix with light and dark mode versions of the branching diagram.  
   GitHub Issues: [#159](https://github.com/KinsonDigital/Velaptor/issues/159)
3. Added a **_BACKERS.md_** file to the root of the solution to hold a list of any backers that sponsor projects  
   GitHub Issues: [#162](https://github.com/KinsonDigital/Velaptor/issues/162)
4. Made various improvements to all of the issue templates.  
   GitHub Issues: [#159](https://github.com/KinsonDigital/Velaptor/issues/159)
