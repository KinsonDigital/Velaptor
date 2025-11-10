// <copyright file="IContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using Fonts;

/// <summary>
/// Creates instances of a content loaders.
/// </summary>
internal interface IContentLoaderFactory
{
    /// <summary>
    /// Creates a loader that loads textures from disk.
    /// </summary>
    /// <returns>A loader for loading textures.</returns>
    ITextureLoader CreateTextureLoader();

    /// <summary>
    /// Creates a loader that loads textures from disk.
    /// </summary>
    /// <param name="pathResolver">Resolves paths to texture content.</param>
    /// <returns>A loader for loading textures.</returns>
    ITextureLoader CreateTextureLoader(IContentPathResolver pathResolver);

    /// <summary>
    /// Creates a loader for loading atlas data from disk.
    /// </summary>
    /// <returns>A loader for loading texture atlas data.</returns>
    IAtlasLoader CreateAtlasLoader();

    /// <summary>
    /// Creates a loader for loading atlas data from disk.
    /// </summary>
    /// <param name="pathResolver">Resolves paths to atlas content.</param>
    /// <returns>A loader for loading texture atlas data.</returns>
    IAtlasLoader CreateAtlasLoader(IContentPathResolver pathResolver);

    /// <summary>
    /// Creates a loader that loads audio from disk.
    /// </summary>
    /// <returns>A loader for loading audio data.</returns>
    IAudioLoader CreateAudioLoader();

    /// <summary>
    /// Creates a loader that loads audio from disk.
    /// </summary>
    /// <param name="pathResolver">Resolves paths to atlas content.</param>
    /// <returns>A loader for loading audio data.</returns>
    IAudioLoader CreateAudioLoader(IContentPathResolver pathResolver);

    /// <summary>
    /// Creates a loader that loads fonts from disk for rendering test.
    /// </summary>
    /// <returns>A loader for loading audio data.</returns>
    IFontLoader CreateFontLoader();

    /// <summary>
    /// Creates a loader that loads fonts from disk for rendering test.
    /// </summary>
    /// <param name="pathResolver">Resolves paths to atlas content.</param>
    /// <returns>A loader for loading audio data.</returns>
    IFontLoader CreateFontLoader(IContentPathResolver pathResolver);
}
