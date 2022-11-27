// <copyright file="AtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using Caching;
using Graphics;
using Guards;

/// <summary>
/// Holds data relating to a texture atlas.
/// </summary>
public sealed class AtlasData : IAtlasData
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string AtlasDataExtension = ".json";
    private const string TextureExtension = ".png";
    private readonly AtlasSubTextureData[] subTexturesData;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasData"/> class.
    /// </summary>
    /// <param name="textureCache">Caches textures for later use to improve performance.</param>
    /// <param name="path">Performs path related operations.</param>
    /// <param name="atlasSubTextureData">The sub texture data of all sub textures in the atlas.</param>
    /// <param name="dirPath">The path to the content.</param>
    /// <param name="atlasName">The name of the atlas.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if any of the constructor parameters are null.
    /// </exception>
    public AtlasData(
        IItemCache<string, ITexture> textureCache,
        IPath path,
        IEnumerable<AtlasSubTextureData> atlasSubTextureData,
        string dirPath,
        string atlasName)
    {
        // ReSharper disable PossibleMultipleEnumeration
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.ParamIsNotNull(path);
        EnsureThat.ParamIsNotNull(atlasSubTextureData);
        EnsureThat.StringParamIsNotNullOrEmpty(dirPath);
        EnsureThat.StringParamIsNotNullOrEmpty(atlasName);

        this.subTexturesData = atlasSubTextureData.OrderBy(data => data.FrameIndex).ToArray();

        // ReSharper restore PossibleMultipleEnumeration
        atlasName = path.GetFileNameWithoutExtension(atlasName);

        dirPath = dirPath.TrimDirSeparatorFromEnd();

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
    public ReadOnlyCollection<string> SubTextureNames
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

            return result.ToReadOnlyCollection();
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
    public AtlasSubTextureData GetFrame(string subTextureId)
    {
        var foundFrame = (from s in this.subTexturesData
            where s.Name == subTextureId
            select s).FirstOrDefault();

        if (foundFrame is null)
        {
            // TODO: Create a custom exception named TextureAtlasException and implement here
            throw new Exception($"The frame '{subTextureId}' was not found in the atlas '{Name}'.");
        }

        return foundFrame;
    }

    /// <inheritdoc/>
    public AtlasSubTextureData[] GetFrames(string subTextureId)
        => (from s in this.subTexturesData
            where s.Name == subTextureId
            select s).ToArray();
}
