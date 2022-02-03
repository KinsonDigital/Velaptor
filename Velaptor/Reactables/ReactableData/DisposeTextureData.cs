// <copyright file="DisposeTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.ReactableData
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Reactables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds data for the <see cref="IReactable{T}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct DisposeTextureData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeTextureData"/> struct.
        /// </summary>
        /// <param name="textureId">The texture ID of the texture to dispose.</param>
        public DisposeTextureData(uint textureId) => TextureId = textureId;

        /// <summary>
        /// Gets the texture ID.
        /// </summary>
        public uint TextureId { get; }
    }
}
