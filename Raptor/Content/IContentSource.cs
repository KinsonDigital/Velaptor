// <copyright file="IContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Manages the content source.
    /// </summary>
    public interface IContentSource
    {
        /// <summary>
        /// Gets or sets the root directory of the content.
        /// </summary>
        string ContentRootDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the graphics content directory.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     This directory is not a path. It is just a name and is always located
        ///     as a child directory of the <see cref="ContentRootDirectory"/>.
        /// </para>
        /// <para>
        ///     If the value is a file path, the file name will be stripped and the deepest
        ///     directory name will be used.
        /// </para>
        /// </remarks>
        string ContentDirectoryName { get; set; }

        /// <summary>
        /// Gets the full directory path to the content.
        /// </summary>
        /// <param name="name">The name of the content item.</param>
        /// <returns>
        ///     This is the <see cref="ContentRootDirectory"/> and
        ///     <see cref="ContentDirectoryName"/> combined.
        /// </returns>
        string GetContentPath(string name);
    }
}
