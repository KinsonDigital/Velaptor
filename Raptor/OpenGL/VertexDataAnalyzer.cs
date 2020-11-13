// <copyright file="VertexDataAnalyzer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    /// <summary>
    /// Analyzes vertex data.
    /// </summary>
    internal static class VertexDataAnalyzer
    {
        private static readonly Dictionary<Type, uint> PrimitiveTypeSizes = new Dictionary<Type, uint>()
        {
            // In order from least to greatest bytes
            { typeof(byte), sizeof(byte) },
            { typeof(sbyte), sizeof(sbyte) },
            { typeof(short), sizeof(short) },
            { typeof(ushort), sizeof(ushort) },
            { typeof(int), sizeof(int) },
            { typeof(uint), sizeof(uint) },
            { typeof(float), sizeof(float) },
            { typeof(long), sizeof(long) },
            { typeof(ulong), sizeof(ulong) },
            { typeof(double), sizeof(double) },
        };

        private static readonly Dictionary<Type, uint> TotalItemsForTypes = new Dictionary<Type, uint>()
        {
            // In order from least to greatest bytes
            { typeof(byte), 1 },
            { typeof(sbyte), 1 },
            { typeof(short), 1 },
            { typeof(ushort), 1 },
            { typeof(int), 1 },
            { typeof(uint), 1 },
            { typeof(float), 1 },
            { typeof(long), 1 },
            { typeof(ulong), 1 },
            { typeof(double), 1 },
            { typeof(Vector2), 2 },
            { typeof(Vector2i), 2 },
            { typeof(Vector3), 3 },
            { typeof(Vector3i), 3 },
            { typeof(Vector2d), 2 },
            { typeof(Vector4), 4 },
            { typeof(Vector4i), 4 },
            { typeof(Vector3d), 3 },
            { typeof(Vector4d), 3 },
            { typeof(Matrix4), 16 },
        };

        private static readonly Dictionary<Type, VertexAttribPointerType> PointerTypeMappings = new Dictionary<Type, VertexAttribPointerType>()
        {
            { typeof(float), VertexAttribPointerType.Float },
            { typeof(Vector2), VertexAttribPointerType.Float },
            { typeof(Vector3), VertexAttribPointerType.Float },
            { typeof(Vector4), VertexAttribPointerType.Float },
        };

        /// <summary>
        /// Returns the total number of bytes to the given struct <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The struct type to check.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="type"/> param is null.</exception>
        /// <returns>The total bytes.</returns>
        public static uint GetTotalBytesForStruct(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type), "The argument must not be null");
            }

            var fields = type.GetFields();

            var result = 0u;

            foreach (var field in fields)
            {
                var otherAttrs = field.GetCustomAttribute<FieldDataAttribute>();

                if (!(otherAttrs is null))
                {
                    result += otherAttrs.TotalBytes;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the total bytes for the given primitive type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <exception cref="ArgumentException">Thrown if the given <paramref name="type"/> is not a primitive type.</exception>
        /// <returns>The number of bytes.</returns>
        public static uint GetPrimitiveByteSize(Type type)
        {
            if (!type.IsPrimitive)
            {
                throw new ArgumentException($"The param '{nameof(type)}' must be a primitive type.");
            }

            return PrimitiveTypeSizes[type];
        }

        /// <summary>
        /// Returns the total number of elements in the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Total elements for the type.  Example: Vector2 = 2 elements.</returns>
        public static uint TotalDataElementsForType(Type type) => TotalItemsForTypes[type];

        /// <summary>
        /// Returns the of <see cref="VertexAttribPointerType"/> based on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The data type to check.</param>
        /// <returns>The shader attribute pointer type.</returns>
        public static VertexAttribPointerType GetVertexPointerType(Type type) => PointerTypeMappings[type];
    }
}
