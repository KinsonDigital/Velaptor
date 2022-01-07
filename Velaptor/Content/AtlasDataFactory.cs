// <copyright file="AtlasDataFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates <see cref="IAtlasData"/> instances.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class AtlasDataFactory : IAtlasDataFactory
    {
        /// <inheritdoc/>
        public IAtlasData Create(IEnumerable<AtlasSubTextureData> atlasSubTextureData, string dirPath, string atlasName)
        {
            var textureCache = IoC.Container.GetInstance<IDisposableItemCache<string, ITexture>>();
            var path = IoC.Container.GetInstance<IPath>();

            return new AtlasData(textureCache, path, atlasSubTextureData, dirPath, atlasName) { IsPooled = false };
        }
    }
}
