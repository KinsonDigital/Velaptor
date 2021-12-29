// <copyright file="SpriteBatchSizeAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;

    /// <summary>
    /// Represents the size of a sprite batch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class SpriteBatchSizeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteBatchSizeAttribute"/> class.
        /// </summary>
        /// <param name="batchSize">The size of the batch to represent.</param>
        public SpriteBatchSizeAttribute(uint batchSize) => BatchSize = batchSize;

        /// <summary>
        /// Gets the size of the batch.
        /// </summary>
        public uint BatchSize { get; }
    }
}
