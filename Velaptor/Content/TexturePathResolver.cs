// <copyright file="TexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Guards;

/// <summary>
/// Resolves paths to texture content.
/// </summary>
internal class TexturePathResolver : ContentPathResolver
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string FileExtension = ".png";
    private readonly IDirectory directory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturePathResolver"/> class.
    /// </summary>
    /// <param name="directory">Performs operations with directories.</param>
    public TexturePathResolver(IDirectory directory)
    {
        EnsureThat.ParamIsNotNull(directory);
        this.directory = directory;
        ContentDirectoryName = "Graphics";
    }

    /// <summary>
    /// Returns the path to the texture image content.
    /// </summary>
    /// <param name="contentName">The name of the content.</param>
    /// <returns>The path to the content item.</returns>
    public override string ResolveFilePath(string contentName)
    {
        // Performs other checks on the content name
        contentName = base.ResolveFilePath(contentName);

        contentName = Path.HasExtension(contentName)
            ? Path.GetFileNameWithoutExtension(contentName)
            : contentName;

        var contentDirPath = GetContentDirPath();

        var possibleFiles = this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
            .NormalizePaths();

        // Check if there are any files that match the name
        var files = (from f in possibleFiles
            where string.Compare(
                f,
                $"{contentDirPath}{CrossPlatDirSeparatorChar}{contentName}{FileExtension}",
                StringComparison.OrdinalIgnoreCase) == 0
            select f).ToArray();

        if (files.Length <= 0)
        {
            throw new FileNotFoundException($"The texture image file '{contentDirPath}{CrossPlatDirSeparatorChar}{contentName}{FileExtension}' does not exist.");
        }

        return files[0];
    }
}
