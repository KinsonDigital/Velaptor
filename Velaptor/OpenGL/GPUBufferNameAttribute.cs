// <copyright file="GPUBufferNameAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using Velaptor.Guards;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Represents the name of a buffer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class GPUBufferNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name to give a buffer.</param>
        public GPUBufferNameAttribute(string name)
        {
            EnsureThat.StringParamIsNotNullOrEmpty(name);
            Name = name;
        }

        /// <summary>
        /// Gets the name of a buffer.
        /// </summary>
        public string Name { get; }
    }
}
