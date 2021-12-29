<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.2</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, there is always a chance for issues and bugs to exist with all releases.  It is also common to sometimes miss changes in the release notes when the amount of changes are large.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Created a new exception class that would be thrown if a keyboard was not detected in the system.
   - The name of the new exception class is _`NoKeyboardException`_
2. Created a new exception class that would be thrown if a mouse was not detected in the system.
   - The name of the new exception class is _`NoMouseException`_

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. Fixed an issue with manual testing application crashing immediately after running the application
   - This required an update of the **KinsonDigital.CASL** audio library

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. Updated the audio library **KinsonDigital.CASL** from version **_v1.0.0-preview.4_** to **_v1.0.0-preview.10_**

---

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Implemented major performance improvements related to loading font content
2. Improved PR templates for maintainers and contributors
3. Improved and fixed various issues with the issue templates
