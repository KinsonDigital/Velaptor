<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
   Velaptor Preview Release Notes - v1.0.0-preview.2
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. Created a new exception class that will be thrown if a keyboard was not detected in the system.
   - The name of the new exception class is _`NoKeyboardException`_.
2. Created a new exception class that will be thrown if a mouse was not detected in the system.
   - The name of the new exception class is _`NoMouseException`_.

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. Fixed an issue with the manual testing application crashing immediately after running the application.
   - This required an update of the **KinsonDigital.CASL** audio library.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. Updated the audio library **KinsonDigital.CASL** from version **_v1.0.0-preview.4_** to **_v1.0.0-preview.10_**.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. Implemented major performance improvements related to loading font content.
2. Improved PR templates for maintainers and contributors.
3. Improved and fixed various issues with the issue templates.
