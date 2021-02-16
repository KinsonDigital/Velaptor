# **Raptor Release Notes**

## <span style="color:mediumseagreen;font-weight:bold">Version 0.23.1</span> - <span style="color:indianred;font-weight:bold">(Hot Fix)</span>

### **Bug Fixes** 🐞

1. Fixed a bug where only the max of 2 textures could be rendered in a batch on the same texture
   * The max quad buffer data amount (batch size) of 2 was all that was getting alloted on the GPU.  This was throwing an exception when attempting to render more than 2 quads worth of buffer data

### **New** 🎉

1. Added the ability to the `SpriteBatch` class to be able to set a custom batch size amount
   * Doing this will dispose of the GPU buffers and shader programs and re-create them

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.23.0</span>

### **Breaking Changes** 💣

1. Removed `InvalidReason` enum from library
2. Removed `AtlasRegionRectangle` from library
3. Removed `IAtlasRegionRectangle` from library

### **Changes** ✨

1. Changed `AtlasData.Texture` property to no longer be readonly
2. Added caching ability to the `TextureLoader` class to internally cache textures to improve performance
   * Loading of a texture that has already been loaded will return the exact same texture
3. Added caching ability to the `AtlasLoader` class to internally cache atlas data to improve performance
   * Loading of a texture atlas and atlas data that has already been loaded will return the exact same data
4. Added caching ability to the `SoundLoader` class to internally cache sounds to improve performance
   * Loading of a sound that has already been loaded will return the exact same sound

### **New** 🎉

1. Added new enumeration type named `TextureType` to represent the kind of texture
   * This can be represent the rendering of a whole texture or sub-texture
2. Added a new interface type named `IContentUnloadable`
   * This can be used to unload content
3. Added ability to throw a descriptive exception when attempting to render a texture with a width or height that is less than or equal to zero
4. Add feature to be able to unload texture, texture atlas, and sound data
   * This has been implemented into the `TextureLoader`, `AtlasLoader`, and `SoundLoader`

### **Tech Debt/Cleanup** 🧹

1. Refactor class field for `AtlasData` class

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.22.0</span>

### **Breaking Changes** 💣

1. Changed the `Sound` class to take in a full file path to the sound content instead of just a sound name
   * Before this used a content source to resolve where the sound content was located
   * The new `SoundLoader` is now responsible for resolving the path to the sound content before loading the sound
2. The following types have changed their names
   * `AtlasDataLoader` class name has been changed to `AtlasLoader`
   * `IContentSource` class name has been changed to `IPathResolver`
   * `ContentSource` class name has been changed to `ContentPathResolver`
   * `AtlasContentSource` class has been changed to `AtlasJSONDataPathResolver`
   * `SoundContentSource` class has been changed to `SoundPathResolver`

### **New** 🎉

1. Added the following methods to the `ContentLoaderFactory` class to create different kinds of content loaders
   * `CreateTextureLoader()`
   * `CreateTextureAtlasLoader()`
   * `CreateSoundLoader()`
2. `ContentSource` types are now called `Resolvers` and have been improved.  The following resolvers have been created to help resolve paths to various pieces of content
   * `AtlasJSONDataPathResolver`
   * `AtlasTexturePathResolver`
   * `SoundPathResolver`
   * `TexturePathResolver`
5. Created a `PathResolverFactory` class to create instances of the different resolvers
6. Created new types for loading texture atlas data such as texture atlas images and atlas sub texture data
   * Created the following types to be used for loading of texture atlas's
      * `IAtlasData`
      * `AtlasData`
      * `AtlasRepository`
      * `AtlasSubTextureData`
9. Made `MapValue()` overloads public to the library API for library users to use
   * These extension methods can be used map a value from one range to another

### **Tech Debt/Cleanup** 🧹

1. Removed `VelcroPhysics.dll` library
2. Removed the following classes/types
   1. `PhysicsBody`
   2. `PhysicsWorld`
   3. `PhysicsBodySettings`
   4. `VelcroBody`
   5. `VelcroWorld`

### **Improvements** 🌟

1. Increase of code coverage with unit tests

### **Nuget/Library Updates** 📦

1. The following packages were updated for the unit testing project
   * Removed nuget package **Microsoft.CodeAnalysis.FxCopAnalyers** *`v3.3.1`*
   * Added **Microsoft.CodeAnalysis.NetAnalyers** from *`v5.0.3`*
   * Updated nuget package **coverlet.msbuild** from *`v2.9.0`* to *`v3.0.2`*
   * Updated nuget package **Microsoft.NET.Test.Sdk** from *`v16.8.0`* to *`v16.8.3`*
   * Updated nuget package **Moq** from *`v4.15.1`* to *`v4.16.0`*
   * Updated nuget package **System.IO.Abstractions** from *`v12.2.24`* to *`v13.2.9`*

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.21.0</span>

### **Breaking Changes** 💣

1. Changed `Keyboard` class from static to non static
   * Implemented new `IKeyboard` interface that the `Keyboard` class inherits from.  This makes the keyboard functionality much more testable and able to be injected using a DI container.
2. Changed `Mouse` class from static to non static
   * Implemented new `IMouse` interface that the `Mouse` class inherits from.  This makes the keyboard functionality much more testable and able to be injected using a DI container.
3. Changed the `contentLoader` parameter type from a concrete `ContentLoader` type, to the `IContentLoader` interface type for the `LoadContent()` method in the `IContentLoadable` interface
   * This is to follow along with the design of making things flexible and testable and aligns with the rest of the library

### **Build/Release Pipelines** 🔁

1. Updated the YAML file for the build pipeline to use the new `Build-Release-Servers` agent pool
2. Added an additional task to the build, test, and publish artifact YAML template files to install `dotnet core 3.x sdk`

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.20.1</span> - <span style="color:indianred;font-weight:bold">(Hot Fix)</span>

### **New** 🎉

1. Changed the window to be displayed in the center of the screen horizontally and vertically by default

### **Bug Fixes** 🐞

1. Fixed an issue with the window position not working when using the `Window.Position` property
2. Fixed an issue with **GLFW** exceptions being thrown when using a width or height value of **'0'** when using the `CreateWindow()` method in the `WindowFactory` class

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.20.0</span>

### **New** 🎉

1. Added a method named `ShowAsync()` to the `IWindow` interface and all associated `IWindow` implementations
   * This is to provide the ability for consumers of the library/framework to create `IWindow` implementations, that can show the window asynchronously instead of on the applications main thread
   * Example: Showing a **Raptor** window inside of a GUI application like **WPF** or **WinForms**, and not block the UI thread

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.19.0</span>

### **Bug Fixes** 🐞

1. Fixed an issue with an **OpenGL** bindings exception being thrown when using the `SpriteBatchFactory.CreateSpriteBatch()` method
   * This was due to something that was missed when implementing the change in release **v0.18.0**

### **Nuget/Library Updates** 📦

1. Updated nuget package **OpenTK** from **v4.0.6** to **v4.2.0**
2. Updated nuget package **Microsoft.CodeAnalysis.FxCopAnalyzers** from **v3.3.0** to **v3.3.1**
3. Updated nuget package **SixLabors.ImageSharp** from **v1.1.1** to **v1.0.2**
4. Updated nuget package **System.IO.Abstractions** from **v12.2.7** to **v12.2.24**
5. Updated nuget package **Simplelnjector** from **v5.0.4** to **v5.1.0**
6. Updated nuget package **Moq** from **v4.14.7** to **v4.15.1**
7. Updated nuget package **Microsoft.NET.Test.Sdk** from **v16.7.1** to **v16.8.0**

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.18.0</span>

### **New** 🎉

1. Added a new method to the `Window` class named `ShowAsync()` to add the ability to run a **Raptor Window** on another thread
   * Currently `Show()` blocks the thread that it was invoked on due to the `Show()` method not returning until the window has been closed  This is not ideal due to how long the window needs to live.  Using `ShowAsync()` will allow the use of the window while still allowing execution after the invocation of the method
     * Use Case: Running a **Raptor Window** from a GUI application
   * Any implementation of the `Window` class will get this functionality

### **Bug Fixes** 🐞

1. Fixed an issue with the abstract `Window` class not being disposed of properly when closing the window
2. Fixed an issue with the `Sound` class not being disposed of properly when calling dispose explicitly or implicitly

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.17.0</span>

### **Improvements** 🌟

1. Improved how the `Window` class shows the window
   * Previously, the window was displayed on window creation before the `Show()` method was invoked.  Now the window will only be displayed when invoking the `Show()` method.  This gives more control over how the window is displayed during window creation and also enables better management of when a window is shown on another thread other than the **main thread**
   * NOTE: Simply invoking the `Show()` method on a worker thread will run the window on that thread

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.16.1</span> - <span style="color:indianred;font-weight:bold">(Hot Fix)</span>

### **Bug Fixes** 🐞

1. Fixed an issue with failing unit tests related to unsafe pointer setup with the unit tests and implementation details related to **GLFW**

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.16.0</span>

### **New** 🎉

1. Added the ability for the abstract `Window` class to allow for automatic buffer clearing
   * Setting the `AutoClearBuffer` property to true will automatically clear the buffer before rendering
   * If the `AutoClearBuffer` property is set to false, the buffer has to be cleared manually.  This can be done by using the `SpriteBatch.Clear()` method

### **Breaking Changes** 💣

1. Moved the following types to to a new namespace with the name `Raptor.Desktop`
   * `IWindow`
   * `Window`
2. Added 2 more new interfaces related to window properties for window state and actions
   * Added new interfaces called `IWindowProps` and `IWindowActions`
   * These interfaces were added to the `Raptor.Desktop` namespace
3. Changed the name of the `Window.UpdateFreq` property to `Window.UpdateFrequency`

### **Bug Fixes** 🐞

1. Fixed a bug where the buffer was not being cleared before textures were being rendered
   * This was causing textures to be smeared on the render target during texture movement or animation
2. Fixed a bug where textures being rendered would be stretched horizontally and vertically when resizing the window

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.15.0</span>

### **Breaking Changes** 💣

1. Improved how content is used and managed
   * Removed `Window` class constructor dependency of the type `IContentLoader`
   * Created a `ILoader<T>` for **Raptor** content types below
     1. Graphics
     2. Sound
     3. Atlas Data
   * Created new `IContent` type.  Every content type created must inherit from this interface to be able to be loaded and managed as **content**.  The list below are the current types that have been changed to use the new `IContent` type
     1. `ITexture`
     2. `ISound`
     3. `IAtlasRegionRectangle`

### **New** 🎉

1. Created new `IPlatform` type with associated implementation type `Platform` to detect which platform the code is running on
   * Implemented use of `IPlatform` type into the 
2. Added new exception type `UnknownContentException`
   * This exception is thrown when dealing with unsupported content

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.14.0</span>

### **New** 🎉

1. Added the ability to retrieve information about monitors that might be hooked up to the system
   * This includes width, height, DPI, scaling and bit depth

### **Other** 👏

1. Simple cleanup and refactoring

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.13.0</span>

### **New** 🎉

1. Added the ability to change the position of the window programmatically
2. Added the ability to enable and disable the maximize button and the resizing of the window border
   * Both the maximize button and the disabling the resizing of the window border are both set using the `WindowBorder.Fixed` border state value
3. Added the ability to set the mouse cursor to be shown or hidden when hovering over the window
4. Added the ability to set the state of the window
   * Available states:
     1. Minimized
     2. Normal
     3. Maximized
     4. Full Screen

### **Other** 👏

1. Simple cleanup and refactoring

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.12.0</span>

### **Changes** ✨

1. Changed the nuget package setup to properly include native **OpenAL** runtime libraries

### **Nuget/Library Updates** 📦

1. Updated **OpenTK** from **v4.0.0-pre9.1** to **v4.0.6**
2. Updated **SimpleInjector** from **v5.0.3** to **v5.0.4**
3. Updated **Microsoft.NET.Test.Sdk** from **v16.7.0** to **v16.7.1**
4. Updated **Moq** from **v4.14.5** to **v4.14.7**

### **Breaking Changes** 💣

1. Refactored **KeyCode** enumeration to match closer to **OpenTK** version

### **Other** 👏

1. Changed **OpenGL** shader source code files to embedded resources
   * This means that the source code is embedded into the assembly itself and loaded during runtime to be sent to the GPU
2. Refactored code to meet coding standards
3. Added ability to move the texture in the **Sandbox** project to test out keyboard input
4. Replaced the **KinsonDigital.FileIO** with **System.IO.Abstractions**
   * This was done to not have to maintain the **KinsonDigital.FileIO** library anymore and to use the better **System.IO.Abstractions** library.

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.11.0</span>

### **Other** 👏

1. Added rules to **editorconfig** files in solution to improve coding standards
2. Adjusted **editorconfig** solution setup

### **Nuget/Library Updates** 📦

1. Updated **KinsonDigital.FileIO** nuget package from **v1.6.1** to **1.7.0**

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.10.0</span>

### **Additions**

1. Added code analyzers to the solution to enforce coding standards and keep code clean
   * This required adding nuget packages to allow the analyzers to run
		1. Microsoft.CodeAnalysis.FxCopAnalyzers - v3.3.0
		2. StyleCop.Analyzers - v1.1.118
   * Added/setup required **editorconfig** files with appropriate coding analyzer rules
   * Added **stylecop.json** files for the stylecop analyzer
2. Refactored code to meet code analyzer requirements
   * This was a very large code refactor
3. Added unit tests to increase code coverage

### **Other** 👏

1. Fixed various failing unit tests

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.9.0</span>

### **New** 🎉

1. Got the **Keyboard** functionality working.  Now the keyboard can be used!!
2. Got the **Mouse** functionality working.  Now the mouse can be used!!

### **Changes** ✨

1. Removed old **SDL** related code from library/project/solution
2. Refactored code to prevent **OpenGL** related code from being exposed to the public API of the Raptor library
3. Refactored code to use unsigned integers instead of signed integers where it makes sense
   * Example: You cannot have a texture with a negative width.  This has been converted to an unsigned integer
4. Created unit tests where applicable
5.  Implemented various analyzers for the purpose of better code.  This resulted in large amounts of refactoring of the code base to satisfy the analyzers

### **Nuget/Library Updates** 📦

1. Updated nuget package **coverlet.msbuild** from **v2.6.3** to **v2.9.0**
2. Updated nuget package **Microsoft.NET.Test.Sdk** from **v16.2.0** to **v16.6.1**
3. Updated nuget package **Moq** from **v4.12.0** to **v4.14.5**
4. Updated nuget package **xunit.runner.visualstudio** from **v2.4.1** to **v2.4.2**
5. Updated nuget package **SimpleInjector** from **v5.0.1** to **v5.0.2**

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.8.0</span>

### **New** 🎉

1. Setup library to use native **x86** **SDL** libraries

### **Changes** ✨

1. Updated **SDLCore** library from **v0.1.1** to **v0.3.0**

### **Other** 👏

1. Updated **FxCopAnalyzers** library from version **v2.9.8** to **v3.0.0**

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.7.0</span>

### **Breaking Changes** 💣

1. Changed the visible scope of all **SDL** implementation classes from **public** to **internal**

### **Nuget/Library Updates** 📦

1. Updated nuget package **SDLCore** from **v0.1.0** to **v0.1.1**

### **Other** 👏

1. Changed how **SDL** files are dealt with in the build and nuget packaging process
   * Removes dependency on the native **SDL** libraries in **Raptor** code base and relies on **SDLCore** nuget package native **SDL** libraries
2. Cleaned up **ExtensionMethodTests** file
3. Added a file to the code base for other developers to know how to contribute to the project
   * Refer to the **CONTRIBUTING.md** file
4. Improved **YAML** files for the **develop** and **production** build pipelines
   * This involved splitting various build tasks intro proper states and jobs

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.6.0</span>

### **Nuget/Library Updates** 📦

1. Updated **SDLCore** nuget package from **v0.0.1** to **v0.1.0**

### **Changes** ✨

2. Refactored code according to **Microsofts** FxCop analyzers as well as setting code base to use nullable references
   * This greatly improves the code base to account for null reference exceptions as well as following better coding standards

### **Other** 👏

1. Changed name of **Raptor.Tests** unit testing project to **RaptorTests**
2. Added **runsettings** file to help facilitate better code coverage during development and during **CI/CD** operations

---

## <span style="color:mediumseagreen;font-weight:bold">Version 0.5.0</span>

### **Changes** ✨

1. Set the **solution/project** to use nullable reference types
2. Created **README.md** file
3. Updated **solution/project** to use C# v8.0
4. Updated YAML files to improve build pipelines

### **Breaking Changes** 💣
1. Replaced the custom **Vector** type with the dotnet core **System.Numerics.Vector2** type
   * This reduces maintenance and upkeep of code
