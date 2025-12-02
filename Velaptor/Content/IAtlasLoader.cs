// <copyright file="IAtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Loads texture atlas data.
/// </summary>
internal interface IAtlasLoader : IUnloader<IAtlasData>
{
    /// <summary>
    /// Gets the total number of cached atlas data items.
    /// </summary>
    int TotalCachedItems { get; }

#pragma warning disable SA1629 // Documentation text should end with a period.
    /// <summary>
    /// Loads texture atlas data using the given <paramref name="atlasPathOrName"/>.
    /// </summary>
    /// <param name="atlasPathOrName">The content name or file path to the atlas data.</param>
    /// <returns>The loaded atlas data.</returns>
    /// <code>
    /// atlasLoader.Load("my-atlas");
    /// // or
    /// atlasLoader.Load("C:/content/my-atlas.png");
    /// </code>
    IAtlasData Load(string atlasPathOrName);
#pragma warning restore SA1629 // Documentation text should end with a period.
}
