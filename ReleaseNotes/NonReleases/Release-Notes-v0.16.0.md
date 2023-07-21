## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.16.0</span>

### **New** ✨

1. Added the ability for the abstract <span style='font-weight: bold; color: khaki'>Window </span> class to allow for automatic buffer clearing
   * Setting the <span style='font-weight: bold; color: khaki'>AutoClearBuffer </span> property to true will automatically clear the buffer before rendering
   * If the <span style='font-weight: bold; color: khaki'>AutoClearBuffer </span> property is set to false, the buffer has to be cleared manually.  This can be done by using the <span style='font-weight: bold; color: khaki'>SpriteBatch.Clear() </span> method

### **Breaking Changes** 🧨

1. Moved the following types to to a new namespace with the name <span style='font-weight: bold; color: khaki'>Velaptor.Desktop`
   * <span style='font-weight: bold; color: khaki'>IWindow</span>
   * <span style='font-weight: bold; color: khaki'>Window</span>
2. Added 2 more new interfaces related to window properties for window state and actions
   * Added new interfaces called <span style='font-weight: bold; color: khaki'>IWindowProps </span> and <span style='font-weight: bold; color: khaki'>IWindowActions</span>
   * These interfaces were added to the <span style='font-weight: bold; color: khaki'>Velaptor.Desktop </span> namespace
3. Changed the name of the <span style='font-weight: bold; color: khaki'>Window.UpdateFreq </span> property to <span style='font-weight: bold; color: khaki'>Window.UpdateFrequency</span>

### **Bug Fixes** 🐛

1. Fixed a bug where the buffer was not being cleared before textures were being rendered
   * This was causing textures to be smeared on the render target during texture movement or animation
2. Fixed a bug where textures being rendered would be stretched horizontally and vertically when resizing the window
