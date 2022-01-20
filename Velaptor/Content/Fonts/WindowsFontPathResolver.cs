// <copyright file="WindowsFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to Windows system fonts.
    /// </summary>
    internal class WindowsFontPathResolver : IPathResolver
    {
        private const string FileExtension = ".ttf";
        private readonly IDirectory directory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFontPathResolver"/> class.
        /// </summary>
        /// <param name="directory">Processes directories.</param>
        public WindowsFontPathResolver(IDirectory directory) => this.directory = directory;

        /// <inheritdoc/>
        public string RootDirectoryPath => @"C:\Windows\";

        /// <inheritdoc/>
        public string ContentDirectoryName => "Fonts";

        /// <inheritdoc/>
        public string ResolveFilePath(string contentName)
        {
            if (string.IsNullOrEmpty(contentName))
            {
                throw new ArgumentNullException(nameof(contentName), $"The parameter must not be null or empty.");
            }

            if (contentName.EndsWith(Path.DirectorySeparatorChar))
            {
                throw new ArgumentException($"The '{contentName}' cannot end with a folder.  It must end with a file name with or without the extension.", nameof(contentName));
            }

            var contentDirPath = $@"{RootDirectoryPath}{ContentDirectoryName}\";
            var fullContentPath = $"{contentDirPath}{contentName}{FileExtension}";
            var files = (from f in this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
                              where string.Compare(
                                  f,
                                  fullContentPath,
                                  StringComparison.OrdinalIgnoreCase) == 0
                select f).ToArray();

            return files.Length <= 0 ? string.Empty : files[0];
        }

        /// <inheritdoc/>
        public string ResolveDirPath() => $@"{RootDirectoryPath}{ContentDirectoryName}\";
    }
}
