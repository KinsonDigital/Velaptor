<h1 align="center" style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.5</h1>

<h2 align="center" style='font-weight:bold'>Quick Reminder</h2>

As with all software, there is always a chance for issues and bugs to exist with all releases.  It is also common to sometimes miss changes in the release notes when the amount of changes are large.  This is even more common in preview releases.

---

<h2 style="font-weight:bold" align="center">New 🎉</h2>

1. Added new exception named `LoadTextureException` that is thrown when something goes wrong with attempting to load textures.
2. Added new exception named `LoadAtlasException` that is thrown when something goes wrong with attempting to load texture atlas data.
3. Added new exception named `CachingException` that will be thrown if something goes wrong with caching items during the content loading process.~~~~
4. Added new exception named `LoadEmbeddedResourceException` that will be thrown when something goes wrong with loading embedded resources during the content loading process.
5. Added new exception named `CachingMetaDataException` that will be thrown if something goes wrong with processing metadata during the font loading process.
6. Added new property named `AtlasDataFilePath` to the `AtlasData` class and `IAtlasData` interface.
   - This is the fully qualified file path to a atlas texture **JSON** data file.
7. Added ability to automatically create default white pixels for the `ImageData` struct constructor when using `null` for the `pixels` parameter.
8. Improved caching system to improve performance and code maintainability and testability.
9. Added new `IItemCache` interface that can be used to build caching systems for personal application use.
    - Refer to [System.Collections.Concurrent](https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent?view=net-6.0) and  
      [System.Runtime.Caching](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.caching?view=dotnet-plat-ext-6.0) for more information on how caching can be done.
10. Added new `IDisposableItemCache` interface that inherits the new `IItemCache` interface to provide a way to manage caching items that are disposable.
11. Added the ability to use a default font when creating UI controls which prevents the user from having to manage content items when dealing with `Label`  
    or controls or controls that use any kind of label for textual content.
12. Added the ability to load system fonts when loading an `IFont` content type.
    - Use the name of the content file with or without the extension to load this on purpose.
13. Added ability to automatically search and load a system font that matches a request font to be used if it does not exist in the  
    application's content directory.
14. Added new enumeration named `FontSource` in the `Velaptor.Content.Fonts` namespace that is used to signify if a font was  
    loaded from the application's content directory or from the current platform system fonts.
15. Added the properties below to the `IPlatform` interface and `Platform` class to check if the current platform is running as a 64 or 32 bit process.
    - `Is64BitProcess`
    - `Is32BitProcess`
16. Added extension methods below to help develop applications.
    - `IsLetter()` - Checks to see if a character is a letter.
    - `IsNotLetter()` - Checks to see if a character is not a letter.
    - `HasValidFullFilePathSyntax()` - Checks to see if a string is a valid fully qualified file path from a syntax perspective.
    - `HasInvalidFullFilePathSyntax()` - Checks to see if a string is an invalid fully qualified file path from a syntax perspective.
    - `HasValidDriveSytax()` - Checks to see if a string contains valid syntax for a drive from a syntax perspective.
    - `HasValidFullDirPathSyntax()` - Checks to see if a string is a fully qualified directory path from a syntax perspective.
    - `HasValidUNCPathSyntax()` - Checks to see if a string is a valid UNC path from a syntax perspective.
17. Created an interface and class implementation to abstract the process of serializing JSON data.
    - This is to help provide a easy testable way to manage JSON data used in games and multi-media applications. 
    - `IJSONService`
    - `JSONService`

---

<h2 style="font-weight:bold" align="center">Breaking Changes 💣</h2>

1. Changed the data type of the _`Width`_ and _`Height`_ properties from _`int`_ to _`uint`_ for both the _`IAtlasData`_ interface and _`AtlasData`_ class.
2. Moved the `FontStyle` enumeration to a new namespace named `Velaptor/Content/Fonts`.
3. Moved the `Font` class to a new namespace named `Velaptor/Content/Fonts`.
4. Moved the `IFont` class to a new namespace named `Velaptor/Content/Fonts`.
5. Moved the `Font` class to a new namespace named `Velaptor/Content/Fonts`.
6. Moved the `FontLoader` class to a new namespace named `Velaptor/Content/Fonts`.
7. Moved the `FontPathResolver` class to a new namespace named `Velaptor/Content/Fonts`.
8. Removed the `UnknownContentException` class.
9. Removed the `StringNullOrEmptyException` class.
10. The enumeration `ContentType` has been removed.
11. The enumeration `TextureType` has been removed.
12. The enumeration `TextureCreationSource` has been removed.
13. Refactored `IContent` interface property `Path` to `FilePath` to better describe what kind of path it is.
    - This effects the `IContent` types below.
      - `AtlasData`
      - `Texture`
      - `Font`
      - `Sound`
14. API changes to the `IContentLoader` and `ContentLoader` types.  This was done due to extra font features needing more control over the font loading process.  This lead to using generics causing some issues with needing that finer control. 
    - The methods `Load<T>()` and `Unload<T>` were removed and replaced with respective methods to load each type of content.
    - Added the methods below to load the respective types of content.
      - `LoadTexture()`
      - `LoadSound()`
      - `LoadAtlas()`
      - `LoadFont`
    - Added the methods below to unload the respective types of content.
      - `UnloadTexture()`
      - `UnloadSound()`
      - `UnloadAtlas()`
      - `UnloadFont`
15. Refactored the parameter name of the `ILoader.Load()` and `ILoader.Unload()` methods from `name` to `contentPathOrName`.
16. Changed the `ShaderProgram` type from `public` to `internal`.
    - These types are not meant to be part of the public facing API.
17. Changed the `GPUBufferBase<TData>` type from `public` to `internal`.
    - These types are not meant to be part of the public facing API.
18. Change all of the resolve types below from `public` to `internal`.
    - These types are not meant to be part of the public facing API.
      - `ContentPathResolver`
      - `FontPathResolver`
      - `ContentFontPathResolver`
      - `WindowsFontPathResolver`
      - `SoundPathResolver`
      - `TexturePathResolver`
      - `AtlasJSONDataPathResolver`
      - `AtlasTexturePathResolver`
19. Changed the type `ISoundFactory` interface and `SoundFactory` class from `public` to `internal`.
    - These types were not meant to be part of the public facing API.
20. Changed readonly struct `ImageData` constructor parameter `pixels` from non-nullable to nullable. 
21. Changed the scope of the `Window.Dispose(bool disposing)` method from `protected` to `private`.

---

<h2 style="font-weight:bold" align="center">Improvements 🌟</h2>

1. Changed the `Label` class to use the newly implemented default font system as a default font.
2. Improved how content is managed increasing testability, stability and performance.

---

<h2 style="font-weight:bold" align="center">Other 👏</h2>

1. Lots of improvements to documentation related to grammar spelling and clarity.
    - This is all due to the hard work of my beautiful wife [@kselena](https://github.com/kselena/kselena)!!  Thanks babe!! 😚
