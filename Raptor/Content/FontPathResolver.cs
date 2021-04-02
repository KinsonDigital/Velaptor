// <copyright file="FontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to font content.
    /// </summary>
    public class FontPathResolver : ContentPathResolver
    {
        private const string FileExtension = ".ttf";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Processes directories.</param>
        public FontPathResolver(IDirectory directory)
        {
            this.directory = directory;
            FileDirectoryName = "Fonts";
        }

        /// <inheritdoc/>
        public override string ResolveFilePath(string contentName)
        {
            contentName = base.ResolveFilePath(contentName);

            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = (from f in this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
                         where f == $"{contentDirPath}{contentName}{FileExtension}"
                         select f).ToArray();

            if (files.Length <= 0)
            {
                throw new FileNotFoundException($"The font file '{contentDirPath}{contentName}{FileExtension}' does not exist.");
            }

            return files[0];
        }
    }
}
