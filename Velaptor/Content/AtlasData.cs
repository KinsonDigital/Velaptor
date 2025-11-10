// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Graphics;
using Velaptor.Exceptions;

/// <summary>
/// Holds data relating to a texture atlas.
/// </summary>
public sealed class AtlasData : IAtlasData
{
    private const string AtlasDataExtension = ".json";
    private const string TextureExtension = ".png";
    private readonly AtlasSubTextureData[] subTexturesData;
    private readonly Dictionary<string, AtlasSubTextureData[]> dataGroups = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasData"/> class.
    /// </summary>
    /// <param name="texture">The texture atlas.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="atlasSubTextureData">The sub texture data of all sub textures in the atlas.</param>
    /// <param name="dirPath">The path to the content.</param>
    /// <param name="atlasName">The name of the atlas.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if any of the constructor parameters are null.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">Thrown if the <paramref name="dirPath"/> does not exist.</exception>
    internal AtlasData(
        ITexture texture,
        IDirectory directory,
        IPath path,
        IList<AtlasSubTextureData> atlasSubTextureData,
        string dirPath,
        string atlasName)
    {
        ArgumentNullException.ThrowIfNull(texture);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(atlasSubTextureData);
        ArgumentException.ThrowIfNullOrEmpty(dirPath);
        ArgumentException.ThrowIfNullOrEmpty(atlasName);

        var groups = atlasSubTextureData.GroupBy(x => x.Name).ToArray();

        foreach (var group in groups)
        {
            this.dataGroups.Add(group.Key, [.. group]);
        }

        this.subTexturesData = [.. atlasSubTextureData.OrderBy(data => data.FrameIndex)];

        atlasName = path.GetFileNameWithoutExtension(atlasName);

        if (!directory.Exists(dirPath))
        {
            throw new DirectoryNotFoundException($"The directory '{dirPath}' does not exist.");
        }

        Name = atlasName;
        FilePath = path.Combine(dirPath, atlasName + TextureExtension);
        AtlasDataFilePath = path.Combine(dirPath, atlasName + AtlasDataExtension);

        Texture = texture;
    }

    /// <summary>
    /// Gets a list of unique sub texture names.
    /// </summary>
    /// <remarks>
    ///     Will not return duplicate names of animating sub textures.
    ///     Animating sub textures will have identical names.
    /// </remarks>
    public IReadOnlyCollection<string> SubTextureNames
    {
        get
        {
            var result = new List<string>();
            var allNames = this.subTexturesData.Select(item => item.Name).ToArray();

            foreach (var name in allNames)
            {
                if (!result.Contains(name))
                {
                    result.Add(name);
                }
            }

            return result.AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the name of the atlas.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path to the texture.
    /// </summary>
    public string FilePath { get; }

    /// <inheritdoc/>
    public string AtlasDataFilePath { get; }

    /// <inheritdoc/>
    public ITexture Texture { get; }

    /// <inheritdoc/>
    public uint Width => Texture.Width;

    /// <inheritdoc/>
    public uint Height => Texture.Height;

    /// <inheritdoc/>
    public AtlasSubTextureData this[int index] => this.subTexturesData[index];

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">
    ///     Thrown if the <paramref name="subTextureId"/> is null or empty.
    /// </exception>
    /// <exception cref="AtlasException">Thrown if the given <paramref name="subTextureId"/> does not exist in the atlas.</exception>
    public AtlasSubTextureData[] GetFrames(string subTextureId)
    {
        ArgumentException.ThrowIfNullOrEmpty(subTextureId);

        return !this.dataGroups.TryGetValue(subTextureId, out AtlasSubTextureData[]? frames)
            ? throw new AtlasException($"The sub-texture id '{subTextureId}' does not exist in the atlas.")
            : frames;
    }
}
