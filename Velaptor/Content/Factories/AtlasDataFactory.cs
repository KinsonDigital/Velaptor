// <copyright file="AtlasDataFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Velaptor.Content.Caching;
using Velaptor.Graphics;

namespace Velaptor.Content.Factories;

/// <summary>
/// Generates <see cref="IAtlasData"/> instances.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class AtlasDataFactory : IAtlasDataFactory
{
    /// <inheritdoc/>
    public IAtlasData Create(IEnumerable<AtlasSubTextureData> atlasSubTextureData, string dirPath, string atlasName)
    {
        var textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var path = IoC.Container.GetInstance<IPath>();

        return new AtlasData(textureCache, path, atlasSubTextureData, dirPath, atlasName);
    }
}
