# **Raptor Release Notes**


# **Version 0.5.0**

## **Changes**

1. Set the **solution/project** to use nullable reference types.
2. Created **README.md** file.
3. Updated **solution/project** to use C# v8.0
4. Updated YAML files to improve build pipelines

### **Breaking Changes**
1. Replaced the custom **Vector** type with the dotnet core **System.Numerics.Vector2** type.
   * This reduces maintenance and upkeep of code.
