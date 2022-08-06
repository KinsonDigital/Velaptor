## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.9.0</span>

### **New** ✨

1. Got the **Keyboard** functionality working.  Now the keyboard can be used!!
2. Got the **Mouse** functionality working.  Now the mouse can be used!!

### **Changes**

1. Removed old **SDL** related code from library/project/solution
2. Refactored code to prevent **OpenGL** related code from being exposed to the public API of the Velaptor library
3. Refactored code to use unsigned integers instead of signed integers where it makes sense
   * Example: You cannot have a texture with a negative width.  This has been converted to an unsigned integer
4. Created unit tests where applicable
5.  Implemented various analyzers for the purpose of better code.  This resulted in large amounts of refactoring of the code base to satisfy the analyzers

### **Nuget/Library Updates** 📦

1. Updated nuget package **coverlet.msbuild** from **v2.6.3** to **v2.9.0**
2. Updated nuget package **Microsoft.NET.Test.Sdk** from **v16.2.0** to **v16.6.1**
3. Updated nuget package **Moq** from **v4.12.0** to **v4.14.5**
4. Updated nuget package **xunit.runner.visualstudio** from **v2.4.1** to **v2.4.2**
5. Updated nuget package **SimpleInjector** from **v5.0.1** to **v5.0.2**
