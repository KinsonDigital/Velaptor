## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.12.0</span>

### **Changes**

1. Changed the nuget package setup to properly include native **OpenAL** runtime libraries

### **Nuget/Library Updates** 📦

1. Updated **OpenTK** from **v4.0.0-pre9.1** to **v4.0.6**
2. Updated **SimpleInjector** from **v5.0.3** to **v5.0.4**
3. Updated **Microsoft.NET.Test.Sdk** from **v16.7.0** to **v16.7.1**
4. Updated **Moq** from **v4.14.5** to **v4.14.7**

### **Breaking Changes** 🧨

1. Refactored **KeyCode** enumeration to match closer to **OpenTK** version

### **Other** 🪧

1. Changed **OpenGL** shader source code files to embedded resources
   * This means that the source code is embedded into the assembly itself and loaded during runtime to be sent to the GPU
2. Refactored code to meet coding standards
3. Added ability to move the texture in the **Sandbox** project to test out keyboard input
4. Replaced the **KinsonDigital.FileIO** with **System.IO.Abstractions**
   * This was done to not have to maintain the **KinsonDigital.FileIO** library anymore and to use the better **System.IO.Abstractions** library
