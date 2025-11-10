// <copyright file="ITextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Loads textures.
/// </summary>
internal interface ITextureLoader : IUnloader<ITexture>
{
    /// <summary>
    /// Gets the total number of cached textures.
    /// </summary>
    int TotalCachedItems { get; }

    /// <summary>
    /// Loads a texture with the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The full file path or name of the texture to load.</param>
    /// <returns>The loaded texture.</returns>
    /// textureLoader.Load("my-texture");
    /// // or
    /// textureLoader.Load("C:/content/my-texture.png");
    ITexture Load(string contentPathOrName);
}
