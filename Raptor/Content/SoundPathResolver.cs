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
        /// <param name="contentName">The name of the content.</param>
        /// <returns>The path to the content item.</returns>
        /// <remarks>
        ///     The 2 types of sound formats supported are '.ogg' and '.mp3'.
        /// <para>
        ///     Precedence is taken with '.ogg' fles over '.mp3'.  What this means is that if
        ///     there are 2 files <br/> with the same name but with different extensions in the
        ///     same <see cref="ContentPathResolver.FileDirectoryName"/>, <br/> the '.ogg'
        ///     file will be loaded, not the '.mp3' file.
        /// </para>
        /// <para>
        ///     If no '.ogg' file exsits but a '.mp3' file does, then the '.mp3' file will be loaded.
        /// </para>
        /// </remarks>
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
