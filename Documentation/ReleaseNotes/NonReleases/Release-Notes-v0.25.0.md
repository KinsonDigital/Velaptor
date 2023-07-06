## <span style='color:mediumseagreen;font-weight:bold'>Velaptor Release Notes - v0.25.0</span>

### **New** ✨

1. Added the ability to apply render effects to a texture being rendered
   * Added a render effect to flip the texture horizontally
   * Added a render effect to flip the texture vertically
   * Added a render effect to flip the texture horizontally and vertically in one render call
2. Added the ability to set the screen clear color during runtime

### **Improvements** 🌟

1. Added simple improvements to internal value caching
   * This is related to caching <span style='font-weight: bold; color: dodgerblue'>OpenGL</span> values before <span style='font-weight: bold; color: dodgerblue'>OpenGL</span> has been initialized
