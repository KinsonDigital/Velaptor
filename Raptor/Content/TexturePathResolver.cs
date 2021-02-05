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
        /// <param name="name">The name of the content.</param>
        /// <returns>The path to the content.</returns>
        public override string ResolveFilePath(string name)
        {
            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(contentDirPath);

            var foundTexture = (from f in files
                                where f == $"{contentDirPath}{name}.png"
                                select f).FirstOrDefault();

            if (files.Length <= 0 || foundTexture is null || foundTexture.Length <= 0)
            {
                throw new FileNotFoundException($"The texture image file '{contentDirPath}{name}' does not exist.");
            }

            return foundTexture;
        }
    }
}
