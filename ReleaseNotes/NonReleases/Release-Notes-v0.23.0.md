## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.23.0</span>

### **Breaking Changes** 🧨

1. Removed <span style='font-weight: bold; color: khaki'>InvalidReason </span> enum from library
2. Removed <span style='font-weight: bold; color: khaki'>AtlasRegionRectangle </span> from library
3. Removed <span style='font-weight: bold; color: khaki'>IAtlasRegionRectangle </span> from library

### **Changes**

1. Changed <span style='font-weight: bold; color: khaki'>AtlasData.Texture </span> property to no longer be readonly
2. Added caching ability to the <span style='font-weight: bold; color: khaki'>TextureLoader </span> class to internally cache textures to improve performance
   * Loading of a texture that has already been loaded will return the exact same texture
3. Added caching ability to the <span style='font-weight: bold; color: khaki'>AtlasLoader </span> class to internally cache atlas data to improve performance
   * Loading of a texture atlas and atlas data that has already been loaded will return the exact same data
4. Added caching ability to the <span style='font-weight: bold; color: khaki'>SoundLoader </span> class to internally cache sounds to improve performance
   * Loading of a sound that has already been loaded will return the exact same sound

### **New** ✨

1. Added new enumeration type named <span style='font-weight: bold; color: khaki'>TextureType </span> to represent the kind of texture
   * This can be represent the rendering of a whole texture or sub-texture
2. Added a new interface type named <span style='font-weight: bold; color: khaki'>IContentUnloadable</span>
   * This can be used to unload content
3. Added ability to throw a descriptive exception when attempting to render a texture with a width or height that is less than or equal to zero
4. Add feature to be able to unload texture, texture atlas, and sound data
   * This has been implemented into the <span style='font-weight: bold; color: khaki'>TextureLoader</span>, <span style='font-weight: bold; color: khaki'>AtlasLoader</span>, and <span style='font-weight: bold; color: khaki'>SoundLoader</span>

### **Tech Debt/Cleanup** 🧽

1. Refactor class field for <span style='font-weight: bold; color: khaki'>AtlasData </span> class
