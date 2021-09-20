## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.15.0</span>

### **Breaking Changes** 💣

1. Improved how content is used and managed
   * Removed <span style='font-weight: bold; color: khaki'>Window </span> class constructor dependency of the type <span style='font-weight: bold; color: khaki'>IContentLoader</span>
   * Created a <span style='font-weight: bold; color: khaki'>ILoader<T> </span> for **Velaptor** content types below
     1. Graphics
     2. Sound
     3. Atlas Data
   * Created new <span style='font-weight: bold; color: khaki'>IContent </span> type.  Every content type created must inherit from this interface to be able to be loaded and managed as **content**.  The list below are the current types that have been changed to use the new <span style='font-weight: bold; color: khaki'>IContent </span> type
     1. <span style='font-weight: bold; color: khaki'>ITexture</span>
     2. <span style='font-weight: bold; color: khaki'>ISound</span>
     3. <span style='font-weight: bold; color: khaki'>IAtlasRegionRectangle</span>

### **New** 🎉

1. Created new <span style='font-weight: bold; color: khaki'>IPlatform </span> type with associated implementation type <span style='font-weight: bold; color: khaki'>Platform </span> to detect which platform the code is running on
   * Implemented use of <span style='font-weight: bold; color: khaki'>IPlatform </span> type into the 
2. Added new exception type <span style='font-weight: bold; color: khaki'>UnknownContentException</span>
   * This exception is thrown when dealing with unsupported content
