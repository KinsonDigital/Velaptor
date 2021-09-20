## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.22.0</span>

### **Breaking Changes** 💣

1. Changed the <span style='font-weight: bold; color: khaki'>Sound </span> class to take in a full file path to the sound content instead of just a sound name
   * Before this used a content source to resolve where the sound content was located
   * The new <span style='font-weight: bold; color: khaki'>SoundLoader </span> is now responsible for resolving the path to the sound content before loading the sound
2. The following types have changed their names
   * <span style='font-weight: bold; color: khaki'>AtlasDataLoader </span> class name has been changed to <span style='font-weight: bold; color: khaki'>AtlasLoader</span>
   * <span style='font-weight: bold; color: khaki'>IContentSource </span> class name has been changed to <span style='font-weight: bold; color: khaki'>IPathResolver</span>
   * <span style='font-weight: bold; color: khaki'>ContentSource </span> class name has been changed to <span style='font-weight: bold; color: khaki'>ContentPathResolver</span>
   * <span style='font-weight: bold; color: khaki'>AtlasContentSource </span> class has been changed to <span style='font-weight: bold; color: khaki'>AtlasJSONDataPathResolver</span>
   * <span style='font-weight: bold; color: khaki'>SoundContentSource </span> class has been changed to <span style='font-weight: bold; color: khaki'>SoundPathResolver</span>

### **New** 🎉

1. Added the following methods to the <span style='font-weight: bold; color: khaki'>ContentLoaderFactory </span> class to create different kinds of content loaders
   * <span style='font-weight: bold; color: khaki'>CreateTextureLoader()</span>
   * <span style='font-weight: bold; color: khaki'>CreateTextureAtlasLoader()</span>
   * <span style='font-weight: bold; color: khaki'>CreateSoundLoader()</span>
2. <span style='font-weight: bold; color: khaki'>ContentSource </span> types are now called <span style='font-weight: bold; color: khaki'>Resolvers </span> and have been improved.  The following resolvers have been created to help resolve paths to various pieces of content
   * <span style='font-weight: bold; color: khaki'>AtlasJSONDataPathResolver</span>
   * <span style='font-weight: bold; color: khaki'>AtlasTexturePathResolver</span>
   * <span style='font-weight: bold; color: khaki'>SoundPathResolver</span>
   * <span style='font-weight: bold; color: khaki'>TexturePathResolver</span>
5. Created a <span style='font-weight: bold; color: khaki'>PathResolverFactory </span> class to create instances of the different resolvers
6. Created new types for loading texture atlas data such as texture atlas images and atlas sub texture data
   * Created the following types to be used for loading of texture atlas's
      * <span style='font-weight: bold; color: khaki'>IAtlasData</span>
      * <span style='font-weight: bold; color: khaki'>AtlasData</span>
      * <span style='font-weight: bold; color: khaki'>AtlasRepository</span>
      * <span style='font-weight: bold; color: khaki'>AtlasSubTextureData</span>
9. Made <span style='font-weight: bold; color: khaki'>MapValue() </span> overloads public to the library API for library users to use
   * These extension methods can be used map a value from one range to another

### **Tech Debt/Cleanup** 🧹

1. Removed **VelcroPhysics.dll** library
2. Removed the following classes/types
   1. <span style='font-weight: bold; color: khaki'>PhysicsBody</span>
   2. <span style='font-weight: bold; color: khaki'>PhysicsWorld</span>
   3. <span style='font-weight: bold; color: khaki'>PhysicsBodySettings</span>
   4. <span style='font-weight: bold; color: khaki'>VelcroBody</span>
   5. <span style='font-weight: bold; color: khaki'>VelcroWorld</span>

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
