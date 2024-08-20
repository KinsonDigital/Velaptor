// <copyright file="FontStatsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts.Services;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using NativeInterop.Services;

/// <inheritdoc cref="IFontStatsService"/>
internal sealed class FontStatsService : IFontStatsService
{
    private const string FontFileExtension = ".ttf";
    private readonly Dictionary<string, FontStats> contentFontStatsCache = new ();
    private readonly IFreeTypeService freeTypeService;
    private readonly IContentPathResolver contentPathResolver;
    private readonly IDirectory directory;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontStatsService"/> class.
    /// </summary>
    /// <param name="freeTypeService">Provides extensions/helpers to <c>FreeType</c> library functionality.</param>
    /// <param name="contentPathResolver">Resolves paths to the application's content directory.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="path">Processes directory and file paths.</param>
    public FontStatsService(
        IFreeTypeService freeTypeService,
        IContentPathResolver contentPathResolver,
        IDirectory directory,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(freeTypeService);
        ArgumentNullException.ThrowIfNull(contentPathResolver);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(path);

        this.freeTypeService = freeTypeService;
        this.contentPathResolver = contentPathResolver;
        this.directory = directory;
        this.path = path;
    }

    /// <inheritdoc/>
    public FontStats[] GetContentStatsForFontFamily(string fontFamilyName)
    {
        // If any items already exist with the given font family, then it has already been added
        var foundFamilyItems = (from s in this.contentFontStatsCache.Values
            where s.FamilyName == fontFamilyName
            select s).ToArray();

        if (foundFamilyItems.Length > 0)
        {
            return foundFamilyItems;
        }

        var fontFiles = this.directory.GetFiles(this.contentPathResolver.ResolveDirPath(), $"*{FontFileExtension}");

        var results =
            (from filePath in fontFiles
                where this.freeTypeService.GetFamilyName(filePath) == fontFamilyName
                select new FontStats
                {
                    FontFilePath = filePath,
                    FamilyName = fontFamilyName,
                    Style = this.freeTypeService.GetFontStyle(filePath),
                    Source = GetFontSource(filePath),
                }).ToArray();

        foreach (var result in results)
        {
            this.contentFontStatsCache.Add(result.FontFilePath, result);
        }

        return (from s in this.contentFontStatsCache.Values
            where s.FamilyName == fontFamilyName
            select s).ToArray();
    }

    /// <summary>
    /// Returns the source of the font using the given <paramref name="fileOrDirPath"/>.
    /// </summary>
    /// <param name="fileOrDirPath">The directory or file path of the font.</param>
    /// <returns>The source of the font.</returns>
    private FontSource GetFontSource(string fileOrDirPath)
    {
        var contentFontDirPath = this.contentPathResolver.ResolveDirPath().ToLower();

        fileOrDirPath = (this.path.GetDirectoryName(fileOrDirPath) ?? string.Empty).ToLower();

        if (fileOrDirPath != contentFontDirPath)
        {
            return FontSource.Unknown;
        }

        return fileOrDirPath == contentFontDirPath
            ? FontSource.AppContent
            : FontSource.System;
    }
}
