<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.4</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, there is always a chance for issues and bugs to exist with all releases.  It is also common to sometimes miss changes in the release notes when the amount of changes are large.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Created new interface named _`ITemplateProcessorService`_ for the purpose of processing template variables in shader code.
   - This is currently used for the embedded internal shaders for **Velaptor**.  Eventually this can be utilized for the user so they can write custom shaders.
2. Created new _`SizeU`_ struct type to help provide easier support for unsigned integer data type changes to various areas of the API.
3. Added a new property to the _`IFont`_ interface and _`Font`_ class named _`LineSpacing`_.
   - This is the spacing between each line in pixels for multi-line text rendering.
4. Added a new method to the _`IFont`_ interface and _`Font`_ class named _`Measure(string text)`_.
   - This allows you to return a value of type _`SizeF`_ that represents the width and height of any text.  This also measures multi-line text appropriately.
5. Added a new method to the _`IFont`_ interface and _`Font`_ class named _`ToGlyphMetrics(string text)`_
   - This adds the ability to return metric data for all of the glyphs that match the characters in the given text.
6. Added a new method to the _`IFont`_ interface and _`Font`_ class named _`GetKerning(uint leftGlyphIndex, uint rightGlyphIndex)`_
   - This adds the ability to return the kerning value between to glyphs. 
   - Refer to [Kerning](https://freetype.org/freetype2/docs/glyphs/glyphs-4.html) for more info about what kerning is all about
7. Added new extension method overloads for the types below.  This gives the user the ability to size up or down various values for game development purposes.
   - _`float`_
   - _`uint`_
   - _`SizeF`_
   - _`RectangleF`_
8.  Added the new methods to the _`ISpriteBatch`_ interface for the purpose of rendering text with a particular font.
    - _`void Render(IFont font, string text, int x, int y)`_
    - _`void Render(IFont font, string text, Vector2 position)`_
    - _`void Render(IFont font, string text, int x, int y, float size, float angle)`_
    - _`void Render(IFont font, string text, Vector2 position, float size, float angle)`_
    - _`void Render(IFont font, string text, int x, int y, Color color)`_
    - _`void Render(IFont font, string text, Vector2 position, Color color)`_
    - _`void Render(IFont font, string text, Vector2 position, float angle, Color color)`_
    - _`void Render(IFont font, string text, int x, int y, float angle, Color color)`_
    - _`void Render(IFont font, string text, int x, int y, float size, float angle, Color color)`_
9.  Added ability to smoothly scale text rendering
10. Added ability to rotate rendered text
11. Added new testing scene to the **Velaptor Testing Application** for testing the operation of non animated graphics rendering.
12. Created new logo animation for animating graphics scene for **Velaptor Testing Application**

---

<h2 style="font-weight:bold" align="center">Breaking Changes 🧨</h2>

1. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for both the _`IAtlasData`_ interface and _`AtlasData`_ class.
2. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for both the _`ITexture`_ interface and _`Texture`_ class.
3. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for the _`ControlBase`_ class.
4. Changed the data type of the _`RenderSurfaceWidth`_ and _`RenderSurfaceHeight`_  properties from _`int`_ to _`uint`_ for both the _`ISpriteBatch`_ interface.
5. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for the _`ISizable`_ interface.
   - **_NOTE_**: This effects all user controls that have a width and height due to the inheritance chain below.
   - Inheritance Chain: _`UIControl->ControlBase->IControl->ISizable`_
6. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for the _`IWindowProps`_ interface.
   - **_NOTE_**: This effects the abstract class _`Window`_ due to this class inheriting from _`IWindowProps`_.
7. Changed the data type of the _`renderSurfaceWidth`_ and _`renderSurfaceHeight`_ parameters from _`int`_ to _`uint`_ for the _`SpriteBatchFactory.CreateSpriteBatch()`_ method.
8. Changed the data type of the _`width`_ and _`height`_ parameters from _`int`_ to _`uint`_ for the _`WindowFactory.CreateWindow()`_ method.
9. Changed the positioning of the _`Velaptor.UI.Button`_ and _`Velaptor.UI.Label`_ controls to be relative of the top right corner to the center of the control.
10. Changed the positioning of rendering text to be based off of the center of the text instead of the top right corner.
11. Changed the _`IWindowActions.WinResize`_ property from the type _`Action?`_ to _`Action<SizeU>`_ type.
12. Changed the data type of the properties below for the _`GlyphMetrics`_ class.
    - _`GlyphBounds`_ from _`Rectangle`_ to _`RectangleF`_
    - _`Ascender`_ from _`int`_ to _`float`_
    - _`Descender`_ from _`int`_ to _`float`_
    - _`HoriBearingX`_ from _`int`_ to _`float`_
    - _`HoriBearingY`_ from _`int`_ to _`float`_
    - _`HorizontalAdvance`_ from _`int`_ to _`float`_
    - _`GlyphWidth`_ from _`int`_ to _`float`_
    - _`GlyphHeight`_ from _`int`_ to _`float`_
    - _`XMin`_ from _`int`_ to _`float`_
    - _`XMax`_ from _`int`_ to _`float`_
    - _`YMin`_ from _`int`_ to _`float`_
    - _`YMax`_ from _`int`_ to _`float`_
13. Changed the parameter named _`tintColor`_ for all applicable _`ISpriteBatch.Render()`_ methods for rendering texture to _`color`_.

---

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Improved texture rendering performance
2. Improved text rendering performance
3. Changed the **Velaptor Testing Application** window size from **1020 x 800** to **1500 x 800**.
4. 

---

<h2 style="font-weight:bold" align="center">Other 👏</h2>

1. Moved embedded GLSL shader files from the **_~/Velaptor/OpenGL/_** project directory to the **_~/Velaptor/OpenGL/Shaders/_** project directory
2. Large code refactoring for the purpose of code style conventions and cleanup
3. Increased code coverage 99%
