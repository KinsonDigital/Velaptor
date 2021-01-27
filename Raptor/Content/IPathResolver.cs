// <copyright file="IPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;

    /// <summary>
    /// Resolves file paths.
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Gets or sets the root directory of the content.
        /// </summary>
        string RootDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the content directory.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     This directory is not a path. It is just a name and is always located
        ///     as a child directory of the <see cref="RootDirectory"/>.
        /// </para>
        /// <para>
        ///     If the value is a file path, the file name will be stripped and the
        ///     deepest child directory name will be used.
        /// </para>
        /// </remarks>
        string FileDirectoryName { get; set; }

        /// <summary>
        /// Resolves the full file path.
        /// </summary>
        /// <param name="name">The name of the file with or without the extension.</param>
        /// <returns>
        ///     The <see cref="RootDirectory"/>, <see cref="FileDirectoryName"/> and file directory name combined.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Occurs when the given <paramref name="name"/> is null or emtpy.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Occurs when the given <paramref name="name"/> ends with a directory separator.
        /// </exception>"
        string ResolveFilePath(string name);
    }
}
