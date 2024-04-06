// <copyright file="ContentExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System;
using System.IO;
using Content;
using Content.Fonts;

/// <summary>
/// Provides content related extension methods.
/// </summary>
public static class ContentExtensions
{
    /// <summary>
    /// Loads font content from the application's content directory or directly using a full file path.
    /// </summary>
    /// <param name="loader">The font loader.</param>
    /// <param name="fontName">The name or full file path to the font with metadata.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>The loaded font.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Occurs when the <paramref name="fontName"/> argument is null or empty.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Occurs if the font file does not exist.
    /// </exception>
    /// <remarks>
    ///     If a path is used, it must be a fully qualified file path.
    ///     <para>Directory paths are not valid.</para>
    /// </remarks>
    public static IFont Load(this ILoader<IFont> loader, string fontName, uint size)
    {
        ArgumentException.ThrowIfNullOrEmpty(fontName);

        fontName = Path.HasExtension(fontName) ? Path.GetFileNameWithoutExtension(fontName) : fontName;
        fontName = $"{fontName}.ttf|size:{size}";

        return loader.Load(fontName);
    }

    /// <summary>
    /// Unloads the given <paramref name="texture"/>.
    /// </summary>
    /// <param name="loader">The loader.</param>
    /// <param name="texture">The content to unload.</param>
    public static void Unload(this ILoader<ITexture> loader, ITexture? texture)
    {
        if (texture is null)
        {
            return;
        }

        loader.Unload(texture.FilePath);
    }

    /// <summary>
    /// Unloads the given <paramref name="font"/>.
    /// </summary>
    /// <param name="loader">The loader.</param>
    /// <param name="font">The content to unload.</param>
    public static void Unload(this ILoader<IFont> loader, IFont? font)
    {
        if (font is null)
        {
            return;
        }

        loader.Unload($"{font.FilePath}|size:{font.Size}");
    }

    /// <summary>
    /// Unloads the given <paramref name="audio"/>.
    /// </summary>
    /// <param name="loader">The loader.</param>
    /// <param name="audio">The content to unload.</param>
    public static void Unload(this ILoader<IAudio> loader, IAudio? audio)
    {
        if (audio is null)
        {
            return;
        }

        loader.Unload(audio.FilePath);
    }

    /// <summary>
    /// Unloads the given <paramref name="atlas"/>.
    /// </summary>
    /// <param name="loader">The loader.</param>
    /// <param name="atlas">The content to unload.</param>
    public static void Unload(this ILoader<IAtlasData> loader, IAtlasData? atlas)
    {
        if (atlas is null)
        {
            return;
        }

        loader.Unload(atlas.AtlasDataFilePath);
    }
}
