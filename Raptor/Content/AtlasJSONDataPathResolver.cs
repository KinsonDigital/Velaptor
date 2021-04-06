// <copyright file="AtlasJSONDataPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to atlas data content.
    /// </summary>
    public class AtlasJSONDataPathResolver : ContentPathResolver
    {
        private const string FileExtension = ".json";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasJSONDataPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public AtlasJSONDataPathResolver(IDirectory directory)
        {
            this.directory = directory;
            ContentDirectoryName = "Atlas";
        }

        /// <summary>
        /// Returns the path to the texture atlas data content.
        /// </summary>
        /// <param name="contentName">The name of the content item with or without the file extension.</param>
        /// <returns>The path to the content.</returns>
        public override string ResolveFilePath(string contentName)
        {
            // Performs other checks on the content name
            contentName = base.ResolveFilePath(contentName);

            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = (from f in this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
                         where f == $"{contentDirPath}{contentName}{FileExtension}"
                         select f).ToArray();

            if (files.Length <= 0)
            {
                throw new FileNotFoundException($"The texture atlas data file '{contentDirPath}{contentName}{FileExtension}' does not exist.");
            }

            return files[0];
        }
    }
}
