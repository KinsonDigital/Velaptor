# **Raptor Release Notes**

## **Version 0.11.0**

### **Misc**

1. Added rules to **editorconfig** files in solution to improve coding standards
2. Adjusted **editorconfig** solution setup
3. Updated **KinsonDigital.FileIO** nuget package from **v1.6.1** to **1.7.0**

---

## **Version 0.10.0**

### **Additions**

1. Added code analyzers to the solution to enforce coding standards and keep code clean
   * This required adding nuget packages to allow the analyzers to run
		1. Microsoft.CodeAnalysis.FxCopAnalyzers - v3.3.0
		2. StyleCop.Analyzers - v1.1.118
   * Added/setup required **editorconfig** files with appropriate coding analyzer rules
   * Added **stylecop.json** files for the stylecop analyzer
2. Refactored code to meet code analyzer requirements
   * This was a very large code refactor
3. Added unit tests to increase code coverage

### **Misc**

1. Fixed various failing unit tests

---

## **Version 0.9.0**

### **New**

1. Got the **Keyboard** functionality working.  Now the keyboard can be used!!
2. Got the **Mouse** functionality working.  Now the mouse can be used!!

### **Changes**

1. Updated nuget packages below:
   1. Unit Test Project:
		* coverlet.msbuild - v2.6.3 to v2.9.0
		* Microsoft.NET.Test.Sdk - v16.2.0 to v16.6.1
		* Moq - v4.12.0	to v4.14.5
		* xunit.runner.visualstudio - v2.4.1 to v2.4.2
	2. Raptor Project:
        * SimpleInjector - v5.0.1 to v5.0.2

2. Removed old **SDL** related code from library/project/solution
3. Refactored code to prevent **OpenGL** related code from being exposed to the public API of the Raptor library
4. Refactored code to use unsigned integers instead of signed integers where it makes sense.
   * Example: You cannot have a texture with a negative width.  This has been converted to an unsigned integer.
5. Created unit tests where applicable.
6. Implemented various analyzers for the purpose of better code.  This resulted in large amounts of refactoring of the code base to satisfy the analyzers.

## **Version 0.8.0**

### **New**

1. Setup library to use native **x86** **SDL** libraries

### **Changes**

1. Updated **SDLCore** library from **v0.1.1** to **v0.3.0**

### **Developer Related Items**

1. Updated **FxCopAnalyzers** library from version **v2.9.8** to **v3.0.0**.

---

## **Version 0.7.0**

### **Changes**

1. Changed the visible scope of all **SDL** implementation classes from **public** to **internal**.
   * This is a breaking change
2. Updated **SDLCore** nuget package library from **v0.1.0** to **v0.1.1**

### **Developer Related Items**

2. Changed how **SDL** files are dealt with in the build and nuget packaging process.
   * Removes dependency on the native **SDL** libraries in **Raptor** code base and relies on **SDLCore** nuget package native **SDL** libraries.
3. Added a file to the code base for other developers to know how to contribute to the project.
   * Refer to the **CONTRIBUTING.md** file
4. Improved **YAML** files for the **develop** and **production** build pipelines.
   * This involved splitting various build tasks intro proper states and jobs.
5. Cleaned up **ExtensionMethodTests** file.

---

## **Version 0.6.0**

### **Changes**
1. Updated **SDLCore** nuget package from **v0.0.1** to **v0.1.0**
2. Refactored code according to **Microsofts** FxCop analyzers as well as setting code base to use nullable references.
   * This greatly improves the code base to account for null reference exceptions as well as following better coding standards.

### **Developer Related Items**
1. Changed name of **Raptor.Tests** unit testing project to **RaptorTests**.
2. Added **runsettings** file to help facilitate better code coverage during development and during **CI/CD** operations.

---

## **Version 0.5.0**

### **Changes**

1. Set the **solution/project** to use nullable reference types.
2. Created **README.md** file.
3. Updated **solution/project** to use C# v8.0
4. Updated YAML files to improve build pipelines

### **Breaking Changes**
1. Replaced the custom **Vector** type with the dotnet core **System.Numerics.Vector2** type.
   * This reduces maintenance and upkeep of code.
