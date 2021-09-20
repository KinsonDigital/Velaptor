## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.17.0</span>

### **Changes** ✨

1. Improved how the <span style='font-weight: bold; color: khaki'>Window </span> class shows the window
   * Previously, the window was displayed on window creation before the <span style='font-weight: bold; color: khaki'>Show() </span> method was invoked.  Now the window will only be displayed when invoking the <span style='font-weight: bold; color: khaki'>Show() </span> method.  This gives more control over how the window is displayed during window creation and also enables better management of when a window is shown on another thread other than the **main thread**
   * NOTE: Simply invoking the <span style='font-weight: bold; color: khaki'>Show() </span> method on a worker thread will run the window on that thread
