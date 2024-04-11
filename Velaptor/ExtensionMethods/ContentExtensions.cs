// <copyright file="ContentExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System;
using System.IO;
using Content;
using Content.Exceptions;
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
    /// Loads texture atlas data using the given <paramref name="atlasPathOrName"/>.
    /// </summary>
    /// <param name="loader">The loader that loads the atlas data.</param>
    /// <param name="atlasPathOrName">The content name or file path to the atlas data.</param>
    /// <returns>The loaded atlas data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlasPathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    /// <exception cref="IOException">The directory specified a file or the network name is not known.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="NotSupportedException">The path contains a colon character <c>:</c> that is not part of a drive label.</exception>
    /// <remarks>
    /// Valid Values:
    /// <list type="bullet">
    ///     <item>MyAtlas</item>
    ///     <item>C:/Atlas/MyAtlas.png</item>
    ///     <item>C:/Atlas/MyAtlas.json</item>
    /// </list>
    ///
    /// Invalid Values:
    /// <list type="bullet">
    ///     <item>C:/Atlas/MyAtlas</item>
    ///     <item>C:/Atlas/MyAtlas.txt</item>
    /// </list>
    /// </remarks>
    public static IAtlasData Load(this ILoader<IAtlasData> loader, string atlasPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(atlasPathOrName);

        return loader.Load(atlasPathOrName);
    }

    /// <summary>
    /// Loads the audio with the given name.
    /// </summary>
    /// <param name="loader">The loader that loads the texture.</param>
    /// <param name="audioPathOrName">The full file path or name of the audio to load.</param>
    /// <param name="bufferType">The type of buffer to use.</param>
    /// <returns>The loaded audio.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="audioPathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    /// <exception cref="IOException">The directory specified a file or the network name is not known.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="NotSupportedException">The path contains a colon character <c>:</c> that is not part of a drive label.</exception>
    public static IAudio Load(this ILoader<IAudio> loader, string audioPathOrName, AudioBuffer bufferType)
    {
        ArgumentException.ThrowIfNullOrEmpty(audioPathOrName);

        audioPathOrName = $"{audioPathOrName}|{bufferType}";

        return loader.Load(audioPathOrName);
    }

    /// <summary>
    /// Loads a texture with the given <paramref name="texturePathOrName"/>.
    /// </summary>
    /// <param name="loader">The loader that loads the texture.</param>
    /// <param name="texturePathOrName">The full file path or name of the texture to load.</param>
    /// <returns>The loaded texture.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="texturePathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    /// <exception cref="IOException">The directory specified a file or the network name is not known.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="NotSupportedException">The path contains a colon character <c>:</c> that is not part of a drive label.</exception>
    public static ITexture Load(this ILoader<ITexture> loader, string texturePathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(texturePathOrName);

        return loader.Load(texturePathOrName);
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
