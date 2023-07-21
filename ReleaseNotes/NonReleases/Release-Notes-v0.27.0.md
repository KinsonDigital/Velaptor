## <span style='color:mediumseagreen;font-weight:bold'>Velaptor Release Notes - v0.27.0</span>

### **New** ✨

1. Exposed internal struct type `SpriteBatchItem`
   * This was exposed just in case a library user wanted to use this
2. Added a new extension method named `MapValue` to the `byte` data type to allow the user to map a `byte` value to another range where the start and stop values of the range are of the type `float`

### **Changes**

1. Updated the **feature to develop** PR template to not include the card type in the PR title
2. Update code documentation throughout the library related to explanations of true and false values
   * This is about making the true and false values show as key words in the documentation
3. Fixed spelling issue in `AtlasSubTextureData` constructor code documentation

### **Improvements** 🌟

1. Added a finalizer to the `Texture` class to make sure disposal of unmanaged resources is performed.
2. Fixed code documentation on following properties
   * `IWindowProps.Initialized`
   * `ISpriteBatch.BatchSize`

### **Nuget/Library Updates** 📦

1. Updated the nuget package **System.IO.Abstractions** from **v13.2.28** to **v13.2.29**

### **Breaking Changes** 🧨

1. Updated all solution projects to **NET 5.0**!!
2. Added `IDisposable` interface to the `IContent` interface to enforce disposable of content types
3. Removed the `KeyEventArgs` type
   * This was not being used in the library and was an old type that used to used but was never removed
4. Changed the `ToGLColor` extension method on the `System.Drawing.Color` type from public to internal
   * This was unintentionally exposing a type that was meant to be hidden from the library user
5. Renamed constructor parameter for `AtlasData` class to fix spelling mistake
   * Renamed parameter from `atlasSubTexutureData` to `atlasSubTextureData`
6. Changed the `FreeTypeErrorEventArgs` class from public to internal
   * This was not meant to be exposed to the library user
7. Changed the `ITaskService` interface from public to internal
   * This was not meant to be exposed to the library user
8. Changed the `TaskService` class from public to internal
   * This was not meant to be exposed to the library user
9. Changed the name of the `SystemDisplayException` class to `SystemDisplayException`
   * This was due to keeping the API language synonymous with the rest of the library API.  For example, the `ISystemMonitorService` and `SystemMonitorService` types
10. Changed the `OpenGLObservable` class from public to internal
   * This was not meant to be exposed to the library user
11. Changed the `ImageData` struct to a readonly structure
   * This will enforce the use of the constructor to be used to set the `ImageData.Color`, `ImageData.Width`, and `ImageData.Height` values
12. Changed the `ImageData` `Width`, `Height`, and `Color` properties to public fields
