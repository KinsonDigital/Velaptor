## <span style='color:mediumseagreen;font-weight:bold'>Velaptor Preview Release Notes - v1.0.0-preview.2</span>

### **New** 🎉

1. Greatly improved the testing application to help improve the process of manual testing
2. Created scene management system with various scenes for the testing application
   * Added a scene to test text rendering
   * Added a scene to test mouse functionality
   * Added a scene to test keyboard functionality
   * Added a scene to test graphics rendering
   * Created a simple scene management system to help easily add new scenes to the testing application
3. Created new abstract control base class named `Velaptor.UI.ControlBase` for the purpose of creating custom controls
4. Created new `Velaptor.UI.Label` control
5. Created new `Velaptor.UI.Button` control
6. Created new `Velaptor.UI.Textbox` control
   * Warning!! - This control is not finished and needs some work

### **Bug Fixes** 🐛

1. Fixed a bug with the `MapValue` extension method
2. Fixed a bug with where clearing the screen to a set color was not working
   * This was a bug with the `ISpriteBatch.Clear()` method implementation
3. Fixed a bug where in rare cases, the mouse setup process would try to duplicate setup of the mouse buttons causing an exception
   * This issue might actually of never occurred but was discovered during unit testing

### **Nuget/Library Additions** 📦

1. Added nuget package **_Microsoft.CodeAnalysis.NetAnalyzers v5.0.3_**
   * This is for the purpose for code analysis during development
2. Added nuget package **_StyleCop.Analyzers v1.1.118_**
   * This is for the purpose for code analysis during development

### **Breaking Changes** 💣

1. Removed the following classes/types
   * `RenderText`
   * `UIText`
2. Changed the constructor of the `Velaptor.UI.Window` class from **_public_** to **_protected_**

### **Improvements** 🌟

1. 

### CI/CD 🚀

1. Created workflow to automatically add triage labels to new and reopened issues
   * Contribution made by [@teezzan](https://github.com/teezzan) - Thanks!! 🙏
2. Fixed an issue with the **_QA_** and **_Production_** workflow template
   * This was a syntax error preventing the workflow from running

### **Other** 👏

1. Increased code coverage
2. Miscellaneous cleanup and refactoring to meet coding standards
