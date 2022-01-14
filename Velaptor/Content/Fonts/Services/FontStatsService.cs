// <copyright file="FontStatsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts.Services
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.NativeInterop.FreeType;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc cref="IFontStatsService"/>
    internal class FontStatsService : IFontStatsService
    {
        private readonly Dictionary<string, FontStats> contentFontStatsCache = new ();
        private readonly Dictionary<string, FontStats> systemFontStatsCache = new ();
        private readonly IFontService fontService;
        private readonly IPathResolver systemFontPathResolver;
        private readonly IPathResolver contentPathResolver;
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontStatsService"/> class.
        /// </summary>
        /// <param name="fontService">Provides extensions/helpers to free type library functionality.</param>
        /// <param name="contentPathResolver">Resolves paths to the application's content directory.</param>
        /// <param name="systemFontPathResolver">Resolves paths to the systems font directory.</param>
        /// <param name="directory">Performs directory operations.</param>
        public FontStatsService(
            IFontService fontService,
            IPathResolver contentPathResolver,
            IPathResolver systemFontPathResolver,
            IDirectory directory)
        {
            this.fontService = fontService;
            this.contentPathResolver = contentPathResolver;
            this.systemFontPathResolver = systemFontPathResolver;
            this.directory = directory;
        }

        /// <inheritdoc/>
        public FontStats[] GetContentStatsForFontFamily(string fontFamilyName)
        {
            // If any items already exist with the give font family, then it has already been added
            var foundFamilyItems = (from s in this.contentFontStatsCache.Values
                where s.FamilyName == fontFamilyName
                select s).ToArray();

            if (foundFamilyItems.Length > 0)
            {
                return foundFamilyItems;
            }

            var fontFiles = this.directory.GetFiles(this.contentPathResolver.ResolveDirPath(), "*.ttf");

            var results =
                (from filePath in fontFiles
                    where this.fontService.GetFamilyName(filePath, true) == fontFamilyName
                    select new FontStats()
                    {
                        FontFilePath = filePath,
                        FamilyName = fontFamilyName,
                        Style = this.fontService.GetFontStyle(filePath, true),
                    }).ToArray();

            foreach (var result in results)
            {
                this.contentFontStatsCache.Add(result.FontFilePath, result);
            }

            return (from s in this.contentFontStatsCache.Values
                where s.FamilyName == fontFamilyName
                select s).ToArray();
        }

        /// <inheritdoc/>
        public FontStats[] GetSystemStatsForFontFamily(string fontFamilyName)
        {
            // If any items already exist with the give font family, then it has already been added
            var foundFamilyItems = (from s in this.systemFontStatsCache.Values
                where s.FamilyName == fontFamilyName
                select s).ToArray();

            if (foundFamilyItems.Length > 0)
            {
                return foundFamilyItems;
            }

            var fontFiles = this.directory.GetFiles(this.systemFontPathResolver.ResolveDirPath(), "*.ttf");

            var results =
                (from filePath in fontFiles
                    where this.fontService.GetFamilyName(filePath, true) == fontFamilyName
                    select new FontStats()
                    {
                        FontFilePath = filePath,
                        FamilyName = fontFamilyName,
                        Style = this.fontService.GetFontStyle(filePath, true),
                    }).ToArray();

            foreach (var result in results)
            {
                this.systemFontStatsCache.Add(result.FontFilePath, result);
            }

            return (from s in this.systemFontStatsCache.Values
                where s.FamilyName == fontFamilyName
                select s).ToArray();
        }
    }
}
