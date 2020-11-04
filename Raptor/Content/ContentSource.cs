// <copyright file="ContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Content
{
    using System;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Raptor.Exceptions;

    /// <summary>
    /// Manages the content source.
    /// </summary>
    public abstract class ContentSource : IContentSource
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly IDirectory directory;
        private string contentRootDirectory = @$"{BaseDir}Content\";
        private string contentDirName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSource"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public ContentSource(IDirectory directory) => this.directory = directory;

        /// <inheritdoc/>
        public string ContentRootDirectory
        {
            get => this.contentRootDirectory;
            set
            {
                value = string.IsNullOrEmpty(value) ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith(Path.DirectorySeparatorChar) ? value : $@"{value}\";

                this.contentRootDirectory = $@"{value}Content\";
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
        public string GetContentPath(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new StringNullOrEmptyException();

            // If the name ends with a '\', throw and exception
            if (name.EndsWith(Path.DirectorySeparatorChar))
                throw new ArgumentException($"The '{name}' cannot end with folder.  It must end with a file name with or without the extension.");

            // If the name has an extension, remove it
            if (Path.HasExtension(name))
            {
                // NOTE: No localization required
                name = $@"{Path.GetDirectoryName(name)}\{Path.GetFileNameWithoutExtension(name)}".Replace(@"\", string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            var filePath = string.Empty;

            this.contentRootDirectory = this.contentRootDirectory.EndsWith(Path.DirectorySeparatorChar)
                ? this.contentRootDirectory.TrimEnd(Path.DirectorySeparatorChar)
                : this.contentRootDirectory;

            filePath = $@"{this.contentRootDirectory}\{this.contentDirName}\{name}";

            var directory = Path.GetDirectoryName(filePath);
            var fileNameNoExt = Path.GetFileNameWithoutExtension(filePath).ToUpperInvariant();

            // Check if there are any files that match the name
            var files = this.directory.GetFiles(directory)
                .Where(f => Path.GetFileNameWithoutExtension(f).ToUpperInvariant() == fileNameNoExt).ToArray();

            if (files.Length <= 0)
                throw new Exception($"The content item '{Path.GetFileNameWithoutExtension(filePath)}' does not exist.");

            if (files.Length > 1)
            {
                var exceptionMsg = new StringBuilder();
                exceptionMsg.AppendLine("Multiple items match the content item name.");
                exceptionMsg.AppendLine("The content item name must be unique and the file extension is not taken into account.");

                // Add the items to the exception message
                foreach (var file in files)
                {
                    exceptionMsg.AppendLine($"\t{Path.GetFileName(file)}");
                }

                throw new Exception(exceptionMsg.ToString());
            }

            return files[0];
        }
    }
}
