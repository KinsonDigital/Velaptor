// <copyright file="TexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to texture content.
    /// </summary>
    public class TexturePathResolver : ContentPathResolver
    {
        private const string FileExtension = ".png";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TexturePathResolver"/> class.
        /// </summary>
        /// /// <param name="directory">Manages directories.</param>
        public TexturePathResolver(IDirectory directory)
        {
            this.directory = directory;
            FileDirectoryName = "Graphics";
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

            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = (from f in this.directory.GetFiles(contentDirPath, FileExtension)
                         where f == $"{contentDirPath}{contentName}{FileExtension}"
                         select f).ToArray();

            if (files.Length <= 0)
            {
                throw new FileNotFoundException($"The texture image file '{contentDirPath}{contentName}{FileExtension}' does not exist.");
            }

            return files[0];
        }
    }
}
