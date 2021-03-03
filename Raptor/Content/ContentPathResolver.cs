// <copyright file="ContentPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Manages the content source.
    /// </summary>
    public abstract class ContentPathResolver : IPathResolver
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}";
        private string contentRootDirectory = @$"{BaseDir}Content{Path.DirectorySeparatorChar}";
        private string contentDirName = string.Empty;

        /// <inheritdoc/>
        public string RootDirectory
        {
            get => this.contentRootDirectory;
            set
            {
                value = string.IsNullOrEmpty(value) ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith(Path.DirectorySeparatorChar) ? value : $@"{value}{Path.DirectorySeparatorChar}";

                this.contentRootDirectory = $@"{value}Content{Path.DirectorySeparatorChar}";
            }
        }

        /// <inheritdoc/>
        public string FileDirectoryName
        {
            get => this.contentDirName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception($"The '{nameof(FileDirectoryName)}' must not be null or empty.");
                }

                this.contentDirName = value.GetLastDirName();
            }
        }

        /// <summary>
        /// Resolves the full file path to a content item that matches the given <paramref name="contentName"/>.
        /// </summary>
        /// <param name="contentName">The name of the content item.</param>
        /// <returns>The resolved file path to the content item.</returns>
        /// <remarks>
        ///     If the <paramref name="contentName"/> contains a file extension, it will be ignored
        ///     and the file extension '.json' will be used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Occurs when the given <paramref name="contentName"/> is null or emtpy.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Occurs when the given <paramref name="contentName"/> ends with a directory separator.
        /// </exception>"
        public virtual string ResolveFilePath(string contentName)
        {
            contentName = Path.HasExtension(contentName)
                ? Path.GetFileNameWithoutExtension(contentName)
                : contentName;

            if (string.IsNullOrEmpty(contentName))
            {
                throw new ArgumentNullException(nameof(contentName), $"The parameter must not be null or empty.");
            }

            if (contentName.EndsWith(Path.DirectorySeparatorChar))
            {
                throw new ArgumentException($"The '{contentName}' cannot end with a folder.  It must end with a file name with or without the extension.", nameof(contentName));
            }

            return contentName;
        }

        /// <summary>
        /// Gets the directory path of the content.
        /// </summary>
        /// <returns>The full directory path to the content directory.</returns>
        protected string GetContentDirPath() => $@"{this.contentRootDirectory}{this.contentDirName}{Path.DirectorySeparatorChar}";
    }
}
