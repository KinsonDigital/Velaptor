## <span style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.2</span>

### **New** 🎉

1. Created a new exception class that would be thrown if a keyboard was not detected in the system.
   * The name of the new exception class is `NoKeyboardException`
2. Created a new exception class that would be thrown if a mouse was not detected in the system.
   * The name of the new exception class is `NoMouseException`

### **Bug Fixes** 🐛

1. Fixed an issue with manual testing application crashing immediately after running the application
   * This required an update of the **KinsonDigital.CASL** audio library

### **Nuget/Library Updates** 📦

1. Updated the audio library **KinsonDigital.CASL** from version **_v1.0.0-preview.4_** to **_v1.0.0-preview.10_**

### **Improvements** 🌟

1. Implemented major performance improvements related to loading font content
2. Improved PR templates for maintainers and contributors
3. Improved and fixed various issues with the issue templates
