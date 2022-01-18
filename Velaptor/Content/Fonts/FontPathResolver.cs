// <copyright file="FontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    using System;
    using System.IO.Abstractions;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Resolves paths to font content to be used for rendering text.
    /// </summary>
    /// <remarks>
    ///     The location of fonts will first be checked in the default content location
    ///     '&lt;app-dir&gt;/Content/Fonts' directory.  If the font exists in this directory, it will be loaded.  If the font
    ///     does not exist in that location, then the path will be resolved to the current operating systems system font
    ///     location.
    /// <para/>
    /// <para>
    ///     NOTE: Only windows system fonts are currently supported.
    ///     Other systems will be supported in a future release.
    /// </para>
    /// </remarks>
    internal class FontPathResolver : IPathResolver
    {
        private const string OnlyWindowsSupportMessage = "Currently loading system fonts is only supported on Windows.";
        private readonly IPlatform platform;
        private readonly IPathResolver windowsFontPathResolver;
        private readonly IPathResolver contentFontPathResolver;
        private readonly IDirectory directory;
        private readonly IFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontPathResolver"/> class.
        /// </summary>
        /// <param name="contentFontPathResolver">
        /// Resolves the path to the font path in the currently set content directory.
        /// </param>
        /// <param name="windowsFontPathResolver">
        /// Resolves the path to the windows system fonts directory.
        /// </param>
        /// <param name="file">Performs file type operations.</param>
        /// <param name="directory">Performs directory type operations.</param>
        /// <param name="platform">Gets information about the current platform.</param>
        public FontPathResolver(
            IPathResolver contentFontPathResolver,
            IPathResolver windowsFontPathResolver,
            IFile file,
            IDirectory directory,
            IPlatform platform)
        {
            this.contentFontPathResolver = contentFontPathResolver;
            this.windowsFontPathResolver = windowsFontPathResolver;
            this.file = file;
            this.directory = directory;
            this.platform = platform;
        }

        /// <summary>
        /// Gets the root directory of the content.
        /// This will depend if the default content directory exists and what the current platform for
        /// the root directory of the system fonts if the default content directory does not exist.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
        public string RootDirectoryPath
        {
            get
            {
                if (this.directory.Exists(this.contentFontPathResolver.RootDirectoryPath))
                {
                    return this.contentFontPathResolver.RootDirectoryPath;
                }

                if (this.platform.CurrentPlatform != OSPlatform.Windows)
                {
                    throw new NotImplementedException(OnlyWindowsSupportMessage);
                }

                return this.windowsFontPathResolver.RootDirectoryPath;
            }
        }

        /// <summary>
        /// Gets the name of the directory that contains the content that is located in the <see cref="RootDirectoryPath"/>.
        /// This will depend if the default content directory exists and what the current platform for
        /// the root directory of the system fonts if the default content directory does not exist.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
        public string ContentDirectoryName
        {
            get
            {
                if (this.directory.Exists(this.contentFontPathResolver.RootDirectoryPath))
                {
                    return this.contentFontPathResolver.ContentDirectoryName;
                }

                if (this.platform.CurrentPlatform != OSPlatform.Windows)
                {
                    throw new NotImplementedException(OnlyWindowsSupportMessage);
                }

                return this.windowsFontPathResolver.ContentDirectoryName;
            }
        }

        /// <summary>
        /// Resolves the full file path to a content item that matches the given <paramref name="contentName"/>.
        /// </summary>
        /// <param name="contentName">The name of the content item with or without the file extension.</param>
        /// <returns>
        ///     The <see cref="RootDirectoryPath"/>, content file name, and the <see cref="ContentDirectoryName"/> combined.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
        public string ResolveFilePath(string contentName)
        {
            if (this.platform.CurrentPlatform != OSPlatform.Windows)
            {
                throw new NotImplementedException(OnlyWindowsSupportMessage);
            }

            var contentFilePath = this.contentFontPathResolver.ResolveFilePath(contentName);

            return this.file.Exists(contentFilePath)
                ? contentFilePath
                : this.windowsFontPathResolver.ResolveFilePath(contentName);
        }

        /// <summary>
        /// Resolves the full directory path to font content.
        /// </summary>
        /// <returns>The directory only path to font content.</returns>
        /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
        public string ResolveDirPath()
        {
            if (this.platform.CurrentPlatform != OSPlatform.Windows)
            {
                throw new NotImplementedException(OnlyWindowsSupportMessage);
            }

            var contentDirPath = this.contentFontPathResolver.ResolveDirPath();

            return this.directory.Exists(contentDirPath)
                ? contentDirPath
                : this.windowsFontPathResolver.ResolveDirPath();
        }
    }
}
