<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.3</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, there is always a chance for issues and bugs to exist with all releases.  It is also common to sometimes miss changes in the release notes when the amount of changes are large.  This is even more common in preview releases.

---

### **New** 🎉

1. Created new interface named ```ITemplateProcessorService``` for the purpose of processing template variables in shader code.
   * This is currently used for the embedded internal shaders for **Velaptor**.  Eventually this can be utilized for the user so they can write custom shaders.
2. Created new `SizeU` struct type to help provide easier support for unsigned integer data type changes to various areas of the API.
3. Added a new property to the `IFont` interface and `Font` class named `LineSpacing`.
   * This is the spacing between each line in pixels for multi-line text rendering.
4. Added a new method to the `IFont` interface and `Font` class named `Measure(string text)`.
   * This allows you to return a value of type `SizeF` that represents the width and height of any text.  This also measures multi-line text appropriately.
5. Added a new method to the `IFont` interface and `Font` class named `ToGlyphMetrics(string text)`
   * This adds the ability to return metric data for all of the glyphs that match the characters in the given text.
6. Added a new method to the `IFont` interface and `Font` class named `GetKerning(uint leftGlyphIndex, uint rightGlyphIndex)`
   * This adds the ability to return the kerning value between to glyphs. 
   * Refer to [Kerning](https://freetype.org/freetype2/docs/glyphs/glyphs-4.html) for more info about what kerning is all about
7. Added new extension method overloads for the types below.  This gives the user the ability to size up or down various values for game development purposes.
   * `float`
   * `uint`
   * `SizeF`
   * `RectangleF`
8.  Added the new methods to the `ISpriteBatch` interface for the purpose of rendering text with a particular font.
    * `void Render(IFont font, string text, int x, int y)`
    * `void Render(IFont font, string text, Vector2 position)`
    * `void Render(IFont font, string text, int x, int y, float size, float angle)`
    * `void Render(IFont font, string text, Vector2 position, float size, float angle)`
    * `void Render(IFont font, string text, int x, int y, Color color)`
    * `void Render(IFont font, string text, Vector2 position, Color color)`
    * `void Render(IFont font, string text, Vector2 position, float angle, Color color)`
    * `void Render(IFont font, string text, int x, int y, float angle, Color color)`
    * `void Render(IFont font, string text, int x, int y, float size, float angle, Color color)`
9.  Added ability to smoothly scale text rendering
10. Added ability to rotate rendered text
11. Added new testing scene to the **Velaptor Testing Application** for testing the operation of non animated graphics rendering.
12. Created new logo animation for animating graphics scene for **Velaptor Testing Application**

### **Bug Fixes** 🐛

1. asdf

### **Nuget/Library Additions** 📦

1. asdf

### **Breaking Changes** 💣

1. Changed the data type of the `Width` and `Height` properties from `int` to `uint` for both the `IAtlasData` interface and `AtlasData` class.
2. Changed the data type of the `Width` and `Height` properties from `int` to `uint` for both the `ITexture` interface and `Texture` class.
3. Changed the data type of the `Width` and `Height` properties from `int` to `uint` for the `ControlBase` class.
4. Changed the data type of the `RenderSurfaceWidth` and `RenderSurfaceHeight`  properties from `int` to `uint` for both the `ISpriteBatch` interface.
5. Changed the data type of the `Width` and `Height` properties from `int` to `uint` for the `ISizable` interface.
   * **_NOTE_**: This effects all user controls that have a width and height due to the inheritance chain below.
   * Inheritance Chain: `UIControl->ControlBase->IControl->ISizable`
1. Changed the data type of the `Width` and `Height` properties from `int` to `uint` for the `IWindowProps` interface.
   * **_NOTE_**: This effects the abstract class `Window` due to this class inheriting from `IWindowProps`.
2. Changed the data type of the `renderSurfaceWidth` and `renderSurfaceHeight` parameters from `int` to `uint` for the `SpriteBatchFactory.CreateSpriteBatch()` method.
3. Changed the data type of the `width` and `height` parameters from `int` to `uint` for the `WindowFactory.CreateWindow()` method.
4. Changed the positioning of the `Velaptor.UI.Button` and `Velaptor.UI.Label` controls to be relative of the top right corner to the center of the control.
5. Changed the positioning of rendering text to be based off of the center of the text instead of the top right corner.
6. Changed the `IWindowActions.WinResize` property from the type `Action?` to `Action<SizeU>` type.
7. Changed the data type of the properties below for the `GlyphMetrics` class.
    * `GlyphBounds` from `Rectangle` to `RectangleF`
    * `Ascender` from `int` to `float`
    * `Descender` from `int` to `float`
    * `HoriBearingX` from `int` to `float`
    * `HoriBearingY` from `int` to `float`
    * `HorizontalAdvance` from `int` to `float`
    * `GlyphWidth` from `int` to `float`
    * `GlyphHeight` from `int` to `float`
    * `XMin` from `int` to `float`
    * `XMax` from `int` to `float`
    * `YMin` from `int` to `float`
    * `YMax` from `int` to `float`
8. Changed the parameter named `tintColor` for all applicable `ISpriteBatch.Render()` methods for rendering texture to `color`.

### **Improvements** 🌟

1. Improved texture rendering performance
2. Improved text rendering performance
3. Changed the **Velaptor Testing Application** window size from **1020 x 800** to **1500 x 800**.
4. 

### **CI/CD** 🚀

1. asdf

### **Other** 👏

1. Moved embedded GLSL shader files from the **_~/Velaptor/OpenGL/_** project directory to the **_~/Velaptor/OpenGL/Shaders/_** project directory
2. Large code refactoring for the purpose of code style conventions and cleanup
3. Increased code coverage 99%
4. 


