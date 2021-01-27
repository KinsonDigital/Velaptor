// <copyright file="AtlasTexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to atlas texture content.
    /// </summary>
    public class AtlasTexturePathResolver : ContentPathResolver
    {
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasTexturePathResolver"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public AtlasTexturePathResolver(IDirectory directory)
        {
            this.directory = directory;
            FileDirectoryName = "Atlas";
        }

        /// <summary>
        /// Returns the path to the texture atlas image content.
        /// </summary>
        /// <param name="name">The name of the content.</param>
        /// <returns>The path to the content.</returns>
        public override string ResolveFilePath(string name)
        {
            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(contentDirPath, "*.png");

            if (files.Length <= 0 || files.Any(f => f == $"{contentDirPath}{name}.png") is false)
            {
                throw new FileNotFoundException($"The texture atlas image file '{contentDirPath}{name}' does not exist.");
            }

            return files[0];
        }
    }
}
