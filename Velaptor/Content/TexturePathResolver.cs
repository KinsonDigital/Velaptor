// <copyright file="TexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

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
        ArgumentNullException.ThrowIfNull(directory);
        this.directory = directory;
        ContentDirectoryName = "Graphics";
    }

    /// <summary>
    /// Returns the path to the texture image content.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content.</param>
    /// <returns>The path to the content item.</returns>
    public override string ResolveFilePath(string contentPathOrName)
    {
        // Performs other checks on the content name
        contentPathOrName = base.ResolveFilePath(contentPathOrName);

        contentPathOrName = Path.HasExtension(contentPathOrName)
            ? Path.GetFileNameWithoutExtension(contentPathOrName)
            : contentPathOrName;

        var contentDirPath = GetContentDirPath();

        var possibleFiles = this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
            .NormalizePaths();

        // Check if there are any files that match the name
        var files = (from f in possibleFiles
            where string.Compare(
                f,
                $"{contentDirPath}{CrossPlatDirSeparatorChar}{contentPathOrName}{FileExtension}",
                StringComparison.OrdinalIgnoreCase) == 0
            select f).ToArray();

        if (files.Length <= 0)
        {
            throw new FileNotFoundException($"The texture image file '{contentDirPath}{CrossPlatDirSeparatorChar}{contentPathOrName}{FileExtension}' does not exist.");
        }

        return files[0];
    }
}
