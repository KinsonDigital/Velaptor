// <copyright file="AtlasDataFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Graphics;

/// <summary>
/// Generates <see cref="IAtlasData"/> instances.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal sealed class AtlasDataFactory : IAtlasDataFactory
{
    /// <inheritdoc/>
    public IAtlasData Create(ITexture texture, IList<AtlasSubTextureData> atlasSubTextureData, string dirPath, string atlasName)
    {
        var directory = IoC.Container.GetInstance<IDirectory>();
        var path = IoC.Container.GetInstance<IPath>();

        return new AtlasData(texture, directory, path, atlasSubTextureData, dirPath, atlasName);
    }
}
