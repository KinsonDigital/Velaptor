// <copyright file="SpriteBatchSizeAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    internal class SpriteBatchSizeAttribute : Attribute
    {
        public SpriteBatchSizeAttribute(uint batchSize)
        {
            BatchSize = batchSize;
        }

        public uint BatchSize { get; }
    }
}
