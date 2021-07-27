// <copyright file="FieldDataAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using OpenTK.Mathematics;

    /// <summary>
    /// Describes the data layout of a field for setting up OpenGL attribute pointers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class FieldDataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDataAttribute"/> class.
        /// </summary>
        /// <param name="bytesPerElement">
        ///     The total amount of bytes per element.
        ///     If the field is a float, then this would be 4.
        /// </param>
        /// <param name="totalElements">
        ///     The total number of elements for the field.
        ///     If the type is a <see cref="Vector2"/>, then this would be 2.
        /// </param>
        public FieldDataAttribute(uint bytesPerElement, uint totalElements)
        {
            BytesPerElement = bytesPerElement;
            TotalElements = totalElements;

            TotalBytes = bytesPerElement * totalElements;
        }

        /// <summary>
        /// Gets the total bytes for each element.
        /// If the field is a float, then this would be 4.
        /// </summary>
        public uint BytesPerElement { get; private set; }

        /// <summary>
        /// Gets the total number of elements.
        /// If the type is a <see cref="Vector2"/>, then this would be 2.
        /// </summary>
        public uint TotalElements { get; private set; }

        /// <summary>
        /// Gets the total number of bytes for the field.
        /// This would be the <see cref="BytesPerElement"/> multiplied by the <see cref="TotalElements"/>.
        /// </summary>
        /// <remarks>
        /// So if the bytes per element was 4 for a float and the total number of elements was
        /// 3 for a <see cref="Vector3"/> field, then the result would be 12 bytes.
        /// </remarks>
        public uint TotalBytes { get; private set; }
    }
}
