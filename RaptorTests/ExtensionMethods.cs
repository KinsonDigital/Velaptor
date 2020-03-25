using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace RaptorTests
{
    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExtensionMethods
    {
        #region Public Methods
        [ExcludeFromCodeCoverage]
        public static FieldInfo GetField(this object value, string name)
        {
            var privateFields = (from f in value.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                                 where f.Name == name
                                 select f).ToArray();

            //If the list is not found throw not found exception
            if (privateFields == null || privateFields.Length <= 0)
                throw new Exception($"Cannot find the field {name} on the given object of type {value.GetType()}");


            return privateFields.FirstOrDefault();
        }


        public static bool IsNullOrZeroField(this object fieldContainer, string name)
        {
            var foundField = fieldContainer.GetField(name);

            if (foundField.FieldType.IsPrimitive)
            {
                switch (foundField.FieldType)
                {
                    case Type intType when intType == typeof(int):
                        return (int)foundField.GetValue(null) == 0;
                    case Type uintType when uintType == typeof(uint):
                        return (int)foundField.GetValue(null) == 0;
                    case Type longType when longType == typeof(long):
                        return (long)foundField.GetValue(null) == 0;
                    case Type ulongType when ulongType == typeof(ulong):
                        return (ulong)foundField.GetValue(null) == 0;
                    case Type shortType when shortType == typeof(short):
                        return (short)foundField.GetValue(null) == 0;
                    case Type ushortType when ushortType == typeof(ushort):
                        return (ushort)foundField.GetValue(null) == 0;
                    case Type byteType when byteType == typeof(byte):
                        return (byte)foundField.GetValue(null) == 0;
                    case Type sbyteType when sbyteType == typeof(sbyte):
                        return (sbyte)foundField.GetValue(null) == 0;
                    case Type charType when charType == typeof(char):
                        return (char)foundField.GetValue(null) == 0;
                    case Type floatType when floatType == typeof(float):
                        return (float)foundField.GetValue(null) == 0.0f;
                    case Type decimalType when decimalType == typeof(decimal):
                        return (decimal)foundField.GetValue(null) == 0.0m;
                    case Type doubleType when doubleType == typeof(double):
                        return (double)foundField.GetValue(null) == 0.0;
                    default:
                        throw new Exception($"The field of type {foundField.FieldType.Name} is unknown.");
                }
            }
            else
            {
                return foundField.GetValue(fieldContainer) == null;
            }
        }
        #endregion
    }
}
