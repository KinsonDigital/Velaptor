// <copyright file="ContentPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
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
                var isNullOrEmpty = string.IsNullOrEmpty(value);

                value = isNullOrEmpty ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith(Path.DirectorySeparatorChar) ? value : $@"{value}{Path.DirectorySeparatorChar}";

                if (isNullOrEmpty)
                {
                    return;
                }

                this.contentRootDirectory = value;
            }
        }

        /// <inheritdoc/>
        public string ContentDirectoryName
        {
            get => this.contentDirName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception($"The '{nameof(ContentDirectoryName)}' must not be null or empty.");
                }

                this.contentDirName = value.GetLastDirName();
            }
        }

        /// <inheritdoc/>
        public virtual string ResolveFilePath(string contentName)
        {
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

        /// <inheritdoc/>
        public string ResolveDirPath() => $@"{this.contentRootDirectory}{this.contentDirName}\";

        /// <summary>
        /// Gets the directory path of the content.
        /// </summary>
        /// <returns>The full directory path to the content directory.</returns>
        protected string GetContentDirPath() => $@"{this.contentRootDirectory}{this.contentDirName}{Path.DirectorySeparatorChar}";
    }
}
