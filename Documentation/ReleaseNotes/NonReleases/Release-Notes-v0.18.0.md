## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.18.0</span>

### **New** ✨

1. Added a new method to the <span style='font-weight: bold; color: khaki'>Window </span> class named <span style='font-weight: bold; color: khaki'>ShowAsync() </span> to add the ability to run a **Velaptor Window** on another thread
   * Currently <span style='font-weight: bold; color: khaki'>Show() </span> blocks the thread that it was invoked on due to the <span style='font-weight: bold; color: khaki'>Show() </span> method not returning until the window has been closed.  This is not ideal due to how long the window needs to live.  Using <span style='font-weight: bold; color: khaki'>ShowAsync() </span> will allow the use of the window while still allowing execution after the invocation of the method
     * Use Case: Running a **Velaptor Window** from a GUI application
   * Any implementation of the <span style='font-weight: bold; color: khaki'>Window </span> class will get this functionality

### **Bug Fixes** 🐛

1. Fixed an issue with the abstract <span style='font-weight: bold; color: khaki'>Window </span> class not being disposed of properly when closing the window
2. Fixed an issue with the <span style='font-weight: bold; color: khaki'>Sound </span> class not being disposed of properly when calling dispose explicitly or implicitly
