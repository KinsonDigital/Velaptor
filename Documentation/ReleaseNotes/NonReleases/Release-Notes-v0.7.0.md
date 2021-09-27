## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.7.0</span>

### **Breaking Changes** 💣

1. Changed the visible scope of all **SDL** implementation classes from **public** to **internal**

### **Nuget/Library Updates** 📦

1. Updated nuget package **SDLCore** from **v0.1.0** to **v0.1.1**

### **Other** 👏

1. Changed how **SDL** files are dealt with in the build and nuget packaging process
   * Removes dependency on the native **SDL** libraries in **Velaptor** code base and relies on **SDLCore** nuget package native **SDL** libraries
2. Cleaned up **ExtensionMethodTests** file
3. Added a file to the code base for other developers to know how to contribute to the project
   * Refer to the **CONTRIBUTING.md** file
4. Improved **YAML** files for the **develop** and **production** build pipelines
   * This involved splitting various build tasks intro proper states and jobs
