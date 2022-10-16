// <copyright file="WindowsFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Guards;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Resolves paths to Windows system fonts.
    /// </summary>
    internal class WindowsFontPathResolver : IPathResolver
    {
        private const char CrossPlatDirSeparatorChar = '/';
        private const string FileExtension = ".ttf";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFontPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Processes directories.</param>
        public WindowsFontPathResolver(IDirectory directory)
        {
            EnsureThat.ParamIsNotNull(directory);
            this.directory = directory;
        }

        /// <inheritdoc/>
        public string RootDirectoryPath => $"C:{CrossPlatDirSeparatorChar}Windows";

        /// <inheritdoc/>
        public string ContentDirectoryName => "Fonts";

        /// <inheritdoc/>
        public string ResolveFilePath(string contentName)
        {
            if (string.IsNullOrEmpty(contentName))
            {
                throw new ArgumentNullException(nameof(contentName), "The string parameter must not be null or empty.");
            }

            if (contentName.EndsWith(CrossPlatDirSeparatorChar))
            {
                throw new ArgumentException($"The '{contentName}' cannot end with a folder.  It must end with a file name with or without the extension.", nameof(contentName));
            }

            var contentDirPath = $@"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
            var fullContentPath = $"{contentDirPath}{CrossPlatDirSeparatorChar}{contentName}{FileExtension}";
            var files = (from f in this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
                              where string.Compare(
                                  f,
                                  fullContentPath,
                                  StringComparison.OrdinalIgnoreCase) == 0
                select f).ToArray();

            return files.Length <= 0 ? string.Empty : files[0];
        }

        /// <inheritdoc/>
        public string ResolveDirPath() => $@"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
    }
}
