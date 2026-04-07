// <copyright file="IFontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

/// <summary>
/// Loads a specific font for rendering.
/// </summary>
internal interface IFontLoader : IUnloader<IFont>
{
    /// <summary>
    /// Gets the total number of cached textures.
    /// </summary>
    int TotalCachedItems { get; }

    /// <summary>
    /// Loads font content from the application's content directory or directly using a full file path.
    /// </summary>
    /// <param name="contentPathOrName">The name or full file path to the font with metadata.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>The loaded font.</returns>
    /// fontLoader.Load("my-font", 12);
    /// // or
    /// fontLoader.Load("C:/content/my-font.ttf", 24);
    IFont Load(string contentPathOrName, uint size);
}
