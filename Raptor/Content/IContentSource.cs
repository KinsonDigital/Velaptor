// <copyright file="IContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using Raptor.Exceptions;

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
        ///     This directory is not a path. It is just a name and is always located
        ///     as a child directory of the <see cref="ContentRootDirectory"/>.
        /// </remarks>
        string GraphicsDirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the sounds content directory.
        /// </summary>
        /// <remarks>
        ///     This directory is not a path. It is just a name and is always located
        ///     as a child directory of the <see cref="ContentRootDirectory"/>.
        /// </remarks>
        string SoundsDirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the atlas data content directory.
        /// </summary>
        /// <remarks>
        ///     This directory is not a path. It is just a name and is always located
        ///     as a child directory of the <see cref="ContentRootDirectory"/>.
        /// </remarks>
        string AtlasDirectoryName { get; set; }

        /// <summary>
        /// Gets the full directory path to the graphics content.
        /// </summary>
        /// <returns>
        ///     This is the <see cref="ContentRootDirectory"/> and
        ///     <see cref="GraphicsDirectoryName"/> combined.
        /// </returns>
        string GetGraphicsPath();

        /// <summary>
        /// Gets the full directory path to the sound content.
        /// </summary>
        /// <returns>
        ///     This is the <see cref="ContentRootDirectory"/> and
        ///     <see cref="SoundsDirectoryName"/> combined.
        /// </returns>
        string GetSoundsPath();

        /// <summary>
        /// Gets the full directory path to the atlas data content.
        /// </summary>
        /// <returns>
        ///     This is the <see cref="ContentRootDirectory"/> and
        ///     <see cref="AtlasDirectoryName"/> combined.
        /// </returns>
        string GetAtlasPath();

        /// <summary>
        /// Returns the full path to a content item using the given content <paramref name="name"/>.
        /// </summary>
        /// <param name="contentType">The type of content.</param>
        /// <param name="name">The name of the content.</param>
        /// <returns>
        ///     The name of the content can include or exclude an extension but will not
        ///     be taken into account when searching for the content.
        ///     Duplicate names of content items in the same location will throw an exception.
        /// </returns>
        /// <exception cref="StringNullOrEmptyException">
        ///     Thrown when the content <paramref name="name"/> is null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the content <paramref name="name"/> ends with a '\' character.
        ///     The content name must not be treated like a directory or path.
        /// </exception>
        string GetContentPath(ContentType contentType, string name);
    }
}
