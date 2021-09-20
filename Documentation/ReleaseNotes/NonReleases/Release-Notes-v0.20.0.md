## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.20.0</span>

### **New** 🎉

1. Added a method named <span style='font-weight: bold; color: khaki'>ShowAsync() </span> to the <span style='font-weight: bold; color: khaki'>IWindow </span> interface and all associated <span style='font-weight: bold; color: khaki'>IWindow </span> implementations
   * This is to provide the ability for consumers of the library/framework to create <span style='font-weight: bold; color: khaki'>IWindow </span> implementations, that can show the window asynchronously instead of on the applications main thread
   * Example: Showing a **Velaptor** window inside of a GUI application like **WPF** or **WinForms**, and not block the UI thread
