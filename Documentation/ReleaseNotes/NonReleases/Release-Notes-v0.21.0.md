## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.21.0</span>

### **Breaking Changes** 💣

1. Changed <span style='font-weight: bold; color: khaki'>Keyboard </span> class from static to non static
   * Implemented new <span style='font-weight: bold; color: khaki'>IKeyboard </span> interface that the <span style='font-weight: bold; color: khaki'>Keyboard </span> class inherits from.  This makes the keyboard functionality much more testable and able to be injected using a DI container
2. Changed <span style='font-weight: bold; color: khaki'>Mouse </span> class from static to non static
   * Implemented new <span style='font-weight: bold; color: khaki'>IMouse </span> interface that the <span style='font-weight: bold; color: khaki'>Mouse </span> class inherits from.  This makes the keyboard functionality much more testable and able to be injected using a DI container
3. Changed the <span style='font-weight: bold; color: khaki'>contentLoader </span> parameter type from a concrete <span style='font-weight: bold; color: khaki'>ContentLoader </span> type, to the <span style='font-weight: bold; color: khaki'>IContentLoader </span> interface type for the <span style='font-weight: bold; color: khaki'>LoadContent() </span> method in the <span style='font-weight: bold; color: khaki'>IContentLoadable </span> interface
   * This is to follow along with the design of making things flexible and testable and aligns with the rest of the library

### **Build/Release Pipelines** 🔁

1. Updated the YAML file for the build pipeline to use the new **Build-Release-Servers** agent pool
2. Added an additional task to the build, test, and publish artifact YAML template files to install **dotnet core 3.x sdk**
