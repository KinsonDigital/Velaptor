// <copyright file="ITexture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;

    public interface ITexture : IDisposable
    {
        int ID { get; }

        string Name { get; }

        int Width { get; }

        int Height { get; }
    }
}