<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.22
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div align="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#78](https://github.com/KinsonDigital/Velaptor/issues/78) - Added the following features:
   - Created a new text box control for entering single-line text.
   - Added a new property named `GlobalPos` to the event args for mouse move events.
     > **Note** This represents the global position of the mouse relative to the top left corner of the window.  
     > The class used to be named `MousePositionEventArgs` but is not named `MouseMoveEventArgs`.
   - 
   - Added 2 new events named `KeyDown` and `KeyUp` to the `ControlBase` class.
     > **Note** These are invoked when a key is pressed into the down position and released into the up position from the down position.
   - Added `protected` methods named `OnKeyDown()` and `OnKeyUp()` to the `ControlBase` class.
     > **Note** This is used when creating custom controls and is fired when the key up or down events occur and can also be used to manually invoke the `KeyDown` and `KeyUp` events.
   - Added the following `protected` members to the `ControlBase` class to make it easier to create custom controls.
      - `Keyboard` property
      - `OnMouseDown()` method
      - `OnMouseUp()` method
      - `OnMouseMove()` method
      - `OnKeyDown()` method
      - `OnKeyUp()` method
   - Added a new method named `Contains()` to the `RectShape` struct to check if a `Vector2` is contained by the shape.
   - Added a new method overload for the `GetCharacterBounds()` method of the `IFont` interface and `Font` class to take type `StringBuilder` as the text parameter.
   - Added a new method overload to the `IFontRenderer` interface for rendering text to the screen.
      > **Note** The new method signature is `Render(IFont font, Span<(GlyphMetrics metrics, Color clr)> charMetrics, int x, int y, float renderSize, float angle, int layer = 0)`
   - Added a new constructor to the `Label` control to take a text parameter.
   - Added the ability for controls to auto-unsubscribe all control events when unloading the control.

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#78](https://github.com/KinsonDigital/Velaptor/issues/78) - Fixed the following bugs:
    - Fixed an issue where the bounds width for text was not being calculated correctly with the `Font.GetCharacterBounds()` method.
      > **Note** This was occurring when the text being measured ended with whitespace.
    - Fixed an issue where measuring text with `Font.Measure()` was not measuring spaces at the end of the text.
    - Fixed an issue with UI control `MouseDown` and `MouseUp` events not firing.
    - Fixed an issue where if text ended with only spaces, they would not be rendered.
    - Fixed an issue where the internal text size cache of the `Font` class was not clearing the cache when changing font sizes.
    - Fixed an issue where Velaptor would crash when trying to measure the same piece of text content more than once.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#78](https://github.com/KinsonDigital/Velaptor/issues/78) - This release comes with the following breaking changes:
    - Renamed the `MousePositionEventArgs` struct to `MouseMoveEventArgs`.
    - Renamed the `MouseMoveEventArgs.MousePosition` property to `MouseMoveEventArgs.LocalPos`.
      > **Note** This position is relative to the top left corner of the control.
    - Changed the `MouseMoveEventArgs` from a class to a `readonly record struct`.
    - Refactored the name of the `protected` field `ControlBase.keyboard` to a `protected` property with the new name of `Keyboard`.
    - Removed the `IUIControlFactory` interface and `UIControlFactory` class from the public API by changing it to `internal`.
      > **Note** This was not meant to be part of the public API.
    - Refactored the name of the public extension methods class from `PublicExtensionMethods` to `GameHelpers`.

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#174](https://github.com/KinsonDigital/Velaptor/issues/174) - Added guards to various methods in the `GLInvoker` class to check if strings are null or empty.
   > **Note** Thank you [@AndreBonda](https://github.com/AndreBonda)!!

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#355](https://github.com/KinsonDigital/Velaptor/issues/355), [#354](https://github.com/KinsonDigital/Velaptor/issues/354), [#353](https://github.com/KinsonDigital/Velaptor/issues/353) - Refactored unit test code to use the [FluentAssertions](https://fluentassertions.com/) library.
   > **Note** Thank you [@AndreBonda](https://github.com/AndreBonda)!!
2. [#612](https://github.com/KinsonDigital/Velaptor/issues/612) - Updated CICD settings to prevent README pre-processing for NuGet packages from being skipped.
3. [#617](https://github.com/KinsonDigital/Velaptor/issues/617) - Set up renovate dependency management to use KinsonDigital organizational presets/settings.
