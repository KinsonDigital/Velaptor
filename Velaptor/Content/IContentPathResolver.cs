// <copyright file="IContentPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Resolves file paths.
/// </summary>
public interface IContentPathResolver
{
    /// <summary>
    /// Gets the root directory of the content.
    /// </summary>
    string RootDirectoryPath { get; }

    /// <summary>
    /// Gets the name of the content directory.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This directory is not a path. It is just a name and is always located
    ///     as a child directory of the <see cref="RootDirectoryPath"/>.
    /// </para>
    /// <para>
    ///     If the value is a file path, the file name will be stripped and the
    ///     deepest child directory name will be used.
    /// </para>
    /// </remarks>
    string ContentDirectoryName { get; }

    /// <summary>
    /// Resolves the full file path to a content item that matches the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content item with or without the file extension.</param>
    /// <returns>
    ///     The <see cref="RootDirectoryPath"/>, content file name, and the <see cref="ContentDirectoryName"/> combined.
    /// </returns>
    string ResolveFilePath(string contentPathOrName);

    /// <summary>
    /// Resolves the full directory path to some content.
    /// </summary>
    /// <returns>The directory only path to some content.</returns>
    string ResolveDirPath();
}
