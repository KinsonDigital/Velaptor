// <copyright file="ContentFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Guards;

/// <summary>
/// Resolves paths to font content.
/// </summary>
internal sealed class ContentFontPathResolver : ContentPathResolver
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string FileExtension = ".ttf";
    private readonly IDirectory directory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFontPathResolver"/> class.
    /// </summary>
    /// <param name="directory">Performs operations with directories.</param>
    public ContentFontPathResolver(IDirectory directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        this.directory = directory;
        ContentDirectoryName = "Fonts";
    }

    /// <inheritdoc/>
    public override string ResolveFilePath(string contentName)
    {
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

        return files.Length <= 0 ? string.Empty : files[0];
    }
}
