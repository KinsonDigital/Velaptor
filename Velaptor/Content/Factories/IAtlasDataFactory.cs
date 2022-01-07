// <copyright file="IAtlasDataFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.Generic;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates <see cref="IAtlasData"/> instances.
    /// </summary>
    internal interface IAtlasDataFactory
    {
        /// <summary>
        /// Creates a new <see cref="IAtlasData"/> instance using the given data.
        /// </summary>
        /// <param name="atlasSubTextureData">The atlas sub texture data.</param>
        /// <param name="dirPath">The directory path to the data file.</param>
        /// <param name="atlasName">The name of the atlas data content.</param>
        /// <returns>The atlas data.</returns>
        IAtlasData Create(IEnumerable<AtlasSubTextureData> atlasSubTextureData, string dirPath, string atlasName);
    }
}
