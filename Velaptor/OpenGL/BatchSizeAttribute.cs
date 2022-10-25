// <copyright file="BatchSizeAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;

    /// <summary>
    /// Represents the size of a batch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class BatchSizeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchSizeAttribute"/> class.
        /// </summary>
        /// <param name="batchSize">The size of the batch to represent.</param>
        public BatchSizeAttribute(uint batchSize) => BatchSize = batchSize;

        /// <summary>
        /// Gets the size of the batch.
        /// </summary>
        public uint BatchSize { get; }
    }
}
