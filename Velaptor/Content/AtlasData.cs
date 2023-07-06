// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Caching;
using ExtensionMethods;
using Graphics;
using Guards;
using Velaptor.Exceptions;

/// <summary>
/// Holds data relating to a texture atlas.
/// </summary>
public sealed class AtlasData : IAtlasData
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string AtlasDataExtension = ".json";
    private const string TextureExtension = ".png";
    private readonly AtlasSubTextureData[] subTexturesData;
    private readonly Dictionary<string, AtlasSubTextureData[]> dataGroups = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasData"/> class.
    /// </summary>
    /// <param name="textureCache">Caches textures for later use to improve performance.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="atlasSubTextureData">The sub texture data of all sub textures in the atlas.</param>
    /// <param name="dirPath">The path to the content.</param>
    /// <param name="atlasName">The name of the atlas.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if any of the constructor parameters are null.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">Thrown if the <paramref name="dirPath"/> does not exist.</exception>
    public AtlasData(
        IItemCache<string, ITexture> textureCache,
        IDirectory directory,
        IPath path,
        IEnumerable<AtlasSubTextureData> atlasSubTextureData,
        string dirPath,
        string atlasName)
    {
        // ReSharper disable PossibleMultipleEnumeration
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(path);
        EnsureThat.ParamIsNotNull(atlasSubTextureData);
        EnsureThat.StringParamIsNotNullOrEmpty(dirPath);
        EnsureThat.StringParamIsNotNullOrEmpty(atlasName);

        var groups = atlasSubTextureData.GroupBy(x => x.Name).ToArray();

        foreach (var group in groups)
        {
            this.dataGroups.Add(group.Key, group.ToArray());
        }

        this.subTexturesData = atlasSubTextureData.OrderBy(data => data.FrameIndex).ToArray();

        // ReSharper restore PossibleMultipleEnumeration
        atlasName = path.GetFileNameWithoutExtension(atlasName);

        dirPath = dirPath.ToCrossPlatPath().TrimDirSeparatorFromEnd();

        if (directory.Exists(dirPath) is false)
        {
            throw new DirectoryNotFoundException($"The directory '{dirPath}' does not exist.");
        }

        Name = atlasName;
        FilePath = $"{dirPath}{CrossPlatDirSeparatorChar}{atlasName}{TextureExtension}";
        AtlasDataFilePath = $"{dirPath}{CrossPlatDirSeparatorChar}{atlasName}{AtlasDataExtension}";
        Texture = textureCache.GetItem(FilePath);
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
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="subTextureId"/> is null or empty.
    /// </exception>
    public AtlasSubTextureData[] GetFrames(string subTextureId)
    {
        EnsureThat.StringParamIsNotNullOrEmpty(subTextureId);

        if (this.dataGroups.ContainsKey(subTextureId) is false)
        {
            throw new AtlasException($"The sub-texture id '{subTextureId}' does not exist in the atlas.");
        }

        return this.dataGroups[subTextureId];
    }
}
