# **Raptor Release Notes**

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
