<h1 align="center" style='color:mediumseagreen;font-weight:bold'>
    Velaptor Preview Release Notes - v1.0.0-preview.13
</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

<div algn="center">

As with all software, there is always a chance for issues and bugs, especially for preview releases, which is why your input is greatly appreciated. 🙏🏼
</div>

---

<h2 style="font-weight:bold" align="center">New Features ✨</h2>

1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Added layered rendering to textures. This includes a new optional parameter added to all of the `IRenderer` method overloads below:
   > **💡**
   > The layer parameter represents the layer where the texture will be rendered.  Smaller layers will be rendered
   > behind layers with a higher layer value.  If two textures have the same layer, the order of rendering will be based
   > on the order of the `Render` method calls.  Negative layer values are allowed.
   - `IRender.Render(ITexture texture, int x, int y, int layer = 0)`
   - `IRender.Render(ITexture texture, int x, int y, RenderEffects effects, int layer = 0)`
   - `IRender.Render(ITexture texture, int x, int y, Color color, int layer = 0)`
   - `IRender.Render(ITexture texture, int x, int y, Color color, RenderEffects effects, int layer = 0)`
   - `IRender.Render(ITexture texture, NETRect srcRect, NETRect destRect, float size, float angle, Color color, RenderEffects effects, int layer = 0)`
1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Added layered rendering to text. This includes a new optional parameter added to all of the `IRenderer` method overloads below:
   > **💡**
   > The layer parameter represents the layer where the texture will be rendered.  Smaller layers will be rendered
   > behind layers with a higher layer value.  If two textures have the same layer, the order of rendering will be based
   > on the order of the `Render` method calls.  Negative layer values are allowed.
   - `Render(IFont font, string text, int x, int y, int layer = 0)`
   - `Render(IFont font, string text, Vector2 position, int layer = 0)`
   - `Render(IFont font, string text, int x, int y, float renderSize, float angle, int layer = 0)`
   - `Render(IFont font, string text, Vector2 position, float size, float angle, int layer = 0)`
   - `Render(IFont font, string text, int x, int y, Color color, int layer = 0)`
   - `Render(IFont font, string text, Vector2 position, Color color, int layer = 0)`
   - `Render(IFont font, string text, Vector2 position, float angle, Color color, int layer = 0)`
   - `Render(IFont font, string text, int x, int y, float angle, Color color, int layer = 0)`
   - `Render(IFont font, string text, int x, int y, float renderSize, float angle, Color color, int layer = 0)`
1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Added layered rendering to rendering of rectangle primitives. This includes a new optional parameter added to all of the `IRenderer` method overloads below:
      > **💡**
      > The layer parameter represents the layer where the texture will be rendered.  Smaller layers will be rendered
      > behind layers with a higher layer value.  If two textures have the same layer, the order of rendering will be based
      > on the order of the `Render` method calls.  Negative layer values are allowed.
     - `Render(RectShape rectangle, int layer = 0)`

---

<h2 style="font-weight:bold" align="center">Bug Fixes 🐛</h2>

1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Fixed a bug where the default size value of one of the render text overloads was incorrect.
    > **💡**
    > The method overload that was fixed was the `IRenderer.Render(IFont, string, Vector2, Color, int)` method.

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Moved the following methods from the `RectShape` struct to the `CornerRadius` struct.
   - `SetTopLeft`
   - `SetBottomLeft`
   - `SetBottomRight`
   - `SetTopRight`

---

<h2 style="font-weight:bold" align="center">Internal Changes ⚙️</h2>
<h5 align="center">(Changes that do not affect users.  Not breaking changes, new features, or bug fixes.)</h5>

1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Changed how various batch management service updates were managed.
2. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Made various improvements and refactored code to the internal `IReactable` message system.

---

<h2 style="font-weight:bold" align="center">Other 🪧</h2>
<h5 align="center">(Includes anything that does not fit into the categories above)</h5>

1. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Improved performance in rendering textures, text, and rectangle primitives.
2. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Added a new feature to the **_Velaptor Testing_** application debug console to display all of the loaded textures.  This includes font atlas textures, regular atlas textures and full textures.
   > **💡**
   > Use the command `--show loaded-textures` in the testing application debug console to view the loaded textures.
3. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Refactored and cleaned up various areas of the code base.
4. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Refactored, improved, and increased code coverage of unit tests across the code base.
5. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Created three new scenes in the **_Velaptor Testing_** application to test out layered rendering for textures, text, and rectangle primitives.
6. [#434](https://github.com/KinsonDigital/Velaptor/issues/434) - Made the following improvements in the rectangle scene in the _**Velaptor Testing**_ application.
   - Removed the buttons on the left side that move the rectangle around the screen.
   - Added instruction text to the top window.
   - Added the ability to move the rectangle around the window using the keyboard **_Arrow Keys_**.
   - Added the ability to change the size of the rectangle using the keyboard **_Shift_** + **_Arrow Keys_**.
11. [#438](https://github.com/KinsonDigital/Velaptor/issues/438) - Made grammar changes to the project README file.
12. [#250](https://github.com/KinsonDigital/Velaptor/issues/250) - Updated rectangle scene in the **_Velaptor Testing_** application to automatically adjust horizontal spacing for the buttons at the bottom of the window.
13. [#292](https://github.com/KinsonDigital/Velaptor/issues/292) - Updated demo section of the project README with a link to a YouTube [video](https://www.youtube.com/watch?v=nNeVKvkbXc4) of the features.
