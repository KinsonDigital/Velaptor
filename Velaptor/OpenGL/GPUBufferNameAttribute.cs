// <copyright file="GPUBufferNameAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    internal class GPUBufferNameAttribute : Attribute
    {
        public GPUBufferNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
