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
        /// <param name="name">The name of the content.</param>
        /// <returns>The path to the content.</returns>
        public override string ResolveFilePath(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);

            var contentDirPath = GetContentDirPath();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(contentDirPath)
                        .Where(f =>
                        {
                            var fileNameNoExt = Path.GetFileNameWithoutExtension(f);
                            var allowedExtensions = new[] { ".ogg", ".mp3" };
                            var currentExtension = Path.GetExtension(f);

                            return fileNameNoExt == name && allowedExtensions.Contains(currentExtension);
                        }).ToArray();

            if (files.Length <= 0 || files.Any(f => f == $"{contentDirPath}{name}.ogg" || f == $"{contentDirPath}{name}.mp3") is false)
            {
                throw new FileNotFoundException($"The sound file '{contentDirPath}{name}' does not exist.");
            }

            return files[0];
        }
    }
}
