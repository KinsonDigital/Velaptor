# **Raptor Release Notes**


## **Version 0.6.0**

### **Changes**
1. Updated **SDLCore** nuget package from **v0.0.1** to **v0.1.0**
2. Refactored code according to **Microsofts** FxCop analyzers as well as to adhere to nullable references.
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
