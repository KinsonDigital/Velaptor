// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using OpenToolkit.Mathematics;

    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExtensionMethods
    {
        #region Public Methods
        [ExcludeFromCodeCoverage]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used inside method.")]
        public static FieldInfo GetField(this object value, string name)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "The parameter must not be null");

            var privateFields = (from f in value.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                                 where f.Name == name
                                 select f).ToArray();

            // If the list is not found throw not found exception
            if (privateFields == null || privateFields.Length <= 0)
                throw new Exception($"Cannot find the field {name} on the given object of type {value.GetType()}");

            return privateFields.FirstOrDefault();
        }

        public static bool IsNullOrZeroField(this object fieldContainer, string name)
        {
            var foundField = fieldContainer.GetField(name);

            if (foundField.FieldType.IsPrimitive)
            {
                var fieldValue = foundField.GetValue(fieldContainer);

                if (fieldValue is null)
                    return false;

                return foundField.FieldType switch
                {
                    Type intType when intType == typeof(int) => (int)fieldValue == 0,
                    Type uintType when uintType == typeof(uint) => (int)fieldValue == 0,
                    Type longType when longType == typeof(long) => (long)fieldValue == 0,
                    Type ulongType when ulongType == typeof(ulong) => (ulong)fieldValue == 0,
                    Type shortType when shortType == typeof(short) => (short)fieldValue == 0,
                    Type ushortType when ushortType == typeof(ushort) => (ushort)fieldValue == 0,
                    Type byteType when byteType == typeof(byte) => (byte)fieldValue == 0,
                    Type sbyteType when sbyteType == typeof(sbyte) => (sbyte)fieldValue == 0,
                    Type charType when charType == typeof(char) => (char)fieldValue == 0,
                    Type floatType when floatType == typeof(float) => (float)fieldValue == 0.0f,
                    Type decimalType when decimalType == typeof(decimal) => (decimal)fieldValue == 0.0m,
                    Type doubleType when doubleType == typeof(double) => (double)fieldValue == 0.0,
                    _ => throw new Exception($"The field of type {foundField.FieldType.Name} is unknown."),
                };
            }
            else
            {
                return foundField.GetValue(fieldContainer) == null;
            }
        }

        public static bool IsEmpty(this Vector4 vector) =>
            vector.X == 0 &&
            vector.Y == 0 &&
            vector.Z == 0 &&
            vector.W == 0;

        public static bool IsEmpty(this Matrix4 matrix) =>
            matrix.Column0.IsEmpty() &&
            matrix.Column1.IsEmpty() &&
            matrix.Column2.IsEmpty() &&
            matrix.Column3.IsEmpty();
        #endregion
    }
}
