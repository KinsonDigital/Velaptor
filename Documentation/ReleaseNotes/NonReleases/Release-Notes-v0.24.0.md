## <span style="color:mediumseagreen;font-weight:bold">Velaptor Release Notes - v0.24.0</span>

### **Breaking Changes** 🧨

1. Moved the following types from the <span style='font-weight: bold; color: khaki'>Velaptor.Desktop</span> namespace to the <span style='font-weight: bold; color: khaki'>Velaptor.UI</span> namespace
   * NOTE: The types below were all of the types in the <span style='font-weight: bold; color: khaki'>Velaptor.Desktop</span> namespace.  The namespace has been removed entirely
   * <span style='font-weight: bold; color: khaki'>IWindow</span>
   * <span style='font-weight: bold; color: khaki'>IWindowAction</span>
   * <span style='font-weight: bold; color: khaki'>IWindowProps</span>
   * <span style='font-weight: bold; color: khaki'>Window</span>
2. Replaced the <span style='font-weight: bold; color: khaki'>IKeyboard</span> and <span style='font-weight: bold; color: khaki'>IMouse</span> types with a new type named <span style='font-weight: bold; color: khaki'>IGameInput<TInputs, TInputState></span>
   * The types <span style='font-weight: bold; color: khaki'>IKeyboard</span> and <span style='font-weight: bold; color: khaki'>IMouse</span> have been removed
3. Removed the <span style='font-weight: bold; color: khaki'>PointerContainer</span> class from the library

### **Improvements** 🌟

1. Improved audio device management and how audio is played

### **Tech Debt/Cleanup** 🧽

1. Refactored code to meet coding standards
2. Cleaned up <span style='font-weight: bold; color: dodgerblue'>YAML</span> files for build pipeline
   * No functional changes were done.  Just cleanup
3. Refactored unit test method names for consistency and clarity
4. Increased code coverage of various areas of the code base
5. Made correct to PR templates used when merging hot fix branches to develop branches
