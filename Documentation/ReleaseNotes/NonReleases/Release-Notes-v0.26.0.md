## <span style='color:mediumseagreen;font-weight:bold'>Velaptor Release Notes - v0.26.0</span>

### **New** ✨

1. Exposed internal helper methods below for users to take advantage of
   * `RotateAround()`
     * This method lets you rotate a vector around a given origin at a given angle either clockwise or counterclockwise
   * ToVector4()
     * This method lets you convert a 4 component `Color` type to a `Vector4` type
2. Added new type called `ImageData` to hold pixel data as well as width and height of an image.
3. Added new property named `Unloaded` to the interface `IContent` to represent a value to indicating if content has been **unloaded**
    * This effects the current content types below as they as well have the new `Unloaded` property as well
        * `Sound`
        * `AtlasData`
        * `Texture`
4. Added new enum named `FontStyle` to represent the font styles **Regular**, **Bold**, **Italic**
5. Added new type called `FontSettings` that represent font settings JSON data that must exist on disk along with a font file when loading a font
6. Added new `ResolveDirPath()` method to the `IPathResolver` type to give the ability to get resolved directory paths to a content item
    * Other resolver types that are inheriting the abstract class `ContentPathResolver` will of course get this functionality for free
7. Added new `LoadContentException` type that will be thrown when there is an exception when loading content
8. Added new `LoadFontException` type that will be thrown when there is an exception loading font content
9. Added new `InvalidInputException` type that will be thrown when an input is used that is not part of the available `KeyCode` or `MouseButton` enums
10. Added new `SystemDisplayException` type that will be thrown when there is an exception with loading or setting system monitor settings
11. Added new method `CreateFontLoader()` to the `ContentLoaderFactory` class to allow users to create an instance of an `ILoader<IFont>` implementation for loading fonts
12. Added new method `CreateFontPathResolver()` to the `PathResolverFactory` class to allow users to create an instance of `IPathResolver` for resolving paths to font content
13. Added new types 'IFont' and `Font` to be used a new type of content that can be loaded for rendering text
14. Added new type `GlyphMetrics` that holds the various metrics of a single glyph for a particular font
    * This can be used to do custom text rendering
15. Added methods below to allow the user to render text to the screen using an `IFont` implementation
    * `void Render(IFont font, string text, int x, int y)`
    * `void Render(IFont font, string text, int x, int y, Color tintColor)`
16. Added new interface `IFontAtlasService`
    * An implementation of this interface is used internally in the library to create font atlas textures which is used to render texture during runtime.
    * This interface can be used to implement a custom font atlas texture creator
17. Added a `SetState(TInputs input, bool state)` method to the `IGameInput` interface that will allow the user to manually manipulate the state
    * The types below inherit this new functionality of the `IGameInput` interface
      * New interface `IKeyboardInput`
      * New interface `IMouseInput`
      * Class `Keyboard`
      * Class `Mouse`
1.  Added new `Initialized` property to the `IWindowProps` interface
    * This is to allow a users implementation to indicate when the window has finished being initialized
2.  Added new `Initialized` property to the `Window` class
    * This is new functionality that has added to indicate that the window has finished initialization
3.  Added observable pattern types below for the purpose of the library user to create messaging systems
    * `Observable`
    * `Observer`
    * `ObserverUnsubscriber`

### **Changes**

1. Incoming content name processing related to file extensions changed for abstract class `ContentPathResolver`
    * This class does not check for and remove file extensions anymore. This responsibility is now on the implementation of types of `ContentPathResolver`
    * The following already built resolvers have been updated appropriately to process file extensions
        * `AtlasJSONDataPathResolver`
        * `AtlasTexturePathResolver`
        * `SoundPathResolver`
        * `TexturePathResolver`

### **Nuget/Library Updates** 📦

1. Replaced nuget package **Microsoft.CodeAnalysis.FxCopAnalyzers** to **Microsoft.CodeAnalysis.NetCopAnalyzers** **v5.0.3**
   * The **Microsoft.CodeAnalysis.FxCopAnalyzers** nuget library is being deprecated and it should be replaced with the other nuget package mentioned above
2. Added nuget package **FreeTypeSharp** **v1.1.3**
   * This is a binding library to the native cross platform library **FreeType** for loading and dealing with fonts
3. Updated nuget package **Newtonsoft.Json** from **v12.0.3** to **v13.0.1**
4. Updated nuget package **SimpleInjector** from **v5.1.0** to **v5.3.0**
5. Updated nuget package **SixLabors.ImageSharp** from **v1.0.2** to **v1.0.3**
6. Updated nuget package **System.IO.Abstractions** from **v12.2.24** to **v13.2.28**

### **Breaking Changes** 🧨

1. Renamed interface `IImageFileService` to `IImageService`
    * Returns a new type named `ImageData` to represent the pixel data as well as width and height of an image
2. Renamed class `ImageFileService` to `ImageService`
    * Returns a new type named `ImageData` to represent the pixel data as well as width and height of an image
3. Changed `TextureLoader` constructor parameter to reflect name change to the `IImageFileService` interface
4. Changed `Texture` constructor signature from `Texture(string name, string path, byte[] pixelData, int width, int height)` to `Texture(string name, string path, ImageData imageData)`
5. Refactored the `FileDirectoryName` property name on the `IPathResolver` interface to the name `ContentDirectoryName`
    * This is to align more with the meaning and contextual purpose of the property
    * NOTE: All all other existing resolver types have picked this change up as well
6. Swapped parameters for the `AtlasData` class constructor to better align for the intended public facing API of the library
    * The position of the parameters `ITexture texture` and `AtlasSubTextureData[] atlasSubTextureData` were swapped
7. Removed the `None` enum value from the `MouseButton` enumeration

### **Bug Fixes** 🐛

1. Fixed a bug where content file items with extension with upper case letters were not being loaded
2. Fixed a bug where the default X position of a window was being calculated incorrectly
3. Fixed an issue where unit tests were failing with false positives related to unsafe code and testing the `FontAtlasService` class
   * This was causing unreliable builds with tests failing when they should not of
