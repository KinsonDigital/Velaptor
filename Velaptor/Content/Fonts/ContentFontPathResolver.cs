// <copyright file="ContentFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to font content.
    /// </summary>
    public class ContentFontPathResolver : ContentPathResolver
    {
        private const string FileExtension = ".ttf";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFontPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Processes directories.</param>
        public ContentFontPathResolver(IDirectory directory)
        {
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

            // Check if there are any files that match the name
            var files = (from f in this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
                         where string.Compare(
                             f,
                             $"{contentDirPath}{contentName}{FileExtension}",
                             StringComparison.OrdinalIgnoreCase) == 0
                         select f).ToArray();

            return files.Length <= 0 ? string.Empty : files[0];
        }
    }
}
