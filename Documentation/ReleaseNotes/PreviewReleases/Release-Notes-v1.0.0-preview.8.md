<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
   Velaptor Preview Release Notes - v1.0.0-preview.8
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added new constructor to `CornerRadius` struct for convenience.
    - This makes it easier to set all of the radius values to the same value.
2. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `HalfWidth` to the `RectShape` struct.
3. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `HalfHeight` to the `RectShape` struct.
4. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `Top` to the `RectShape` struct to get or set the location of the top of the rectangle.
   - This will automatically update the `Position` property of the rectangle.
5. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `Right` to the `RectShape` struct to get or set the location of the right side of the rectangle.
   - This will automatically update the `Position` property of the rectangle.
6. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `Bottom` to the `RectShape` struct to get or set the location of the bottom of the rectangle.
   - This will automatically update the `Position` property of the rectangle.
7. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new property named `Left` to the `RectShape` struct to get or set the location of the left side of the rectangle.
   - This will automatically update the `Position` property of the rectangle.
8. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added new `System.Drawing.Color` extension method named `IncreaseBrightness()` to increase the brightness of the color.
9. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added new `System.Drawing.Color` extension method named `DecreaseBrightness()` to decrease the brightness of the color.
10. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Changed the `ControlBase.Width` and `ControlBase.Height` property setters from `protected internal` to `public`.
11. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added `AutoSize` property to the `Label` control.
   - When set to `true`, the label's `Width` and `Height` will automatically be set based on the width and height of the rendered text.  When set to `false`, `Width` and `Height` are independent of the rendered text size.
12. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added new property named `AutoSize` to the `Button` control.
   - When set to `true`, the button's `Width` and `Height` will automatically be set based on the width and height of the button text.  When set to `false`, `Width` and `Height` are independent of the button text size.
13. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added a new method to the `IFont` interface and `Font` class that returns the bounds of each character of some given text relative to a position.
14. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added the ability to add a `Label` to a `Button`.  This can be done by sending in a `Label` instance into the `Button` constructor.
15. [#196](https://github.com/KinsonDigital/Velaptor/issues/196) - Added additional constructors to the `Button` class to improve the ease of use when creating new buttons.  List of new constructors listed below:
    - `Button(Label label)`
    - `Button(uint width, uint height)`
    - `Button(uint width, uint height, Label label)`
    - `Button(Point position, uint width, uint height)`
    - `Button(Point position, uint width, uint height, Label label)`

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#204](https://github.com/KinsonDigital/Velaptor/issues/204) - Fixed bug where animating and non-animating textures were not being rendered correctly.
2. [#189](https://github.com/KinsonDigital/Velaptor/issues/189) - Fixed an issue where a font size of 0 would crash the application.
3. [#192](https://github.com/KinsonDigital/Velaptor/issues/192) - Fixed issue with sound time not displaying correctly in the sound testing scene of the testing application.
4. [#71](https://github.com/KinsonDigital/Velaptor/issues/71) - Fixed an issue with the scroll mouse wheel not working.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. Changed the order of rendering from textures, text, then rectangle primitives to textures, rectangle primitives, then text.
2. Changed the behavior of the `RectShape.BorderThickness` and `RectShape.CornerRadius` properties.
   - Previously, both properties would check the incoming value and restrict them to a limit of the smallest value between the half width or half height.  This was done to prevent any unintended rendering artifacts when rendering rectangles.  This behavior was removed from the `RectShape` and moved to the internal `RectGPUBuffer` class.  These checks and restrictions now occur right before sending the data to the GPU.
3. Changed the `ControlBase.IsLoaded` property from `protected` to `private`.
   - The property was meant to be updated via the `ControlBase.LoadContent()` method.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. Created new extension method named `ToVector2()` that converts a `System.Drawing.Point` type to the `System.Numerics.Vector2` type.
2. Created new extension method named `ToPoint()` that converts a `System.Numerics.Vector2` type to the `System.Drawing.Point` type.

---

<h2 style="font-weight:bold" align="center">Nuget/Library Updates 📦</h2>

1. [#204](https://github.com/KinsonDigital/Velaptor/issues/204) - Update NuGet package **SixLabors.ImageSharp** library from **_v1.0.3_** to **_v2.0.0_**.
   - This is what fixed the texture rendering issue.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#197](https://github.com/KinsonDigital/Velaptor/issues/197) - Upgraded all projects in the solution to use C# language **_v10_**.
2. [#197](https://github.com/KinsonDigital/Velaptor/issues/197) - Upgraded all projects in the solution to use **_.NET 6.0_**.
   - Fixed an issue with the status check workflows where the status checks were not being run for preview branches.
   - This was done by adding an additional branch to the branch filter in the status check workflows.
