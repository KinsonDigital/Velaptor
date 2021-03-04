// <copyright file="SoundPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to sound content.
    /// </summary>
    public class SoundPathResolver : ContentPathResolver
    {
        private readonly string[] AllowedExtensions = new[] { ".ogg", ".mp3" };
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public SoundPathResolver(IDirectory directory)
        {
            this.directory = directory;
            FileDirectoryName = "Sounds";
        }

        /// <summary>
        /// Returns the path to the sound content.
        /// </summary>
        /// <param name="contentName">The name of the content.</param>
        /// <returns>The path to the content item.</returns>
        public override string ResolveFilePath(string contentName)
        {
            // Performs other checks on the content name
            contentName = base.ResolveFilePath(contentName);

            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(contentDirPath)
                        .Where(f =>
                        {
                            var fileNameNoExt = Path.GetFileNameWithoutExtension(f);
                            var allowedExtensions = new[] { ".ogg", ".mp3" };
                            var currentExtension = Path.GetExtension(f);

                            return fileNameNoExt == contentName && allowedExtensions.Contains(currentExtension);
                        }).ToArray();

            var oggFiles = files.Where(f => Path.GetExtension(f) == ".ogg").ToArray();

            // If therre are any ogg files, choose this first
            if (oggFiles.Length > 0)
            {
                return oggFiles[0];
            }

            var mp3Files = files.Where(f => Path.GetExtension(f) == ".mp3").ToArray();

            // If therre are any ogg files, choose this first
            if (mp3Files.Length > 0)
            {
                return mp3Files[0];
            }

            throw new FileNotFoundException($"The sound file '{contentDirPath}{contentName}' does not exist.");
        }
    }
}
