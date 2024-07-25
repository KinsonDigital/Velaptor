// <copyright file="ContentPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO;
using System.Reflection;
using ExtensionMethods;

/// <summary>
/// Manages the content source.
/// </summary>
internal abstract class ContentPathResolver : IContentPathResolver
{
    private const char WinDirSeparatorChar = '\\';
    private const char CrossPlatDirSeparatorChar = '/';
    private static readonly string BaseDir = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);
    private string rootDirPath = $"{BaseDir}{CrossPlatDirSeparatorChar}Content";
    private string contentDirName = string.Empty;

    /// <inheritdoc/>
    public string RootDirectoryPath
    {
        get => this.rootDirPath;
        set
        {
            value = string.IsNullOrEmpty(value)
                ? string.Empty
                : value;

            var isNullOrEmpty = string.IsNullOrEmpty(value);

            value = value.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar).TrimDirSeparatorFromEnd();
            value = isNullOrEmpty ? BaseDir : value;

            if (isNullOrEmpty)
            {
                return;
            }

            this.rootDirPath = value;
        }
    }

    /// <inheritdoc/>
    public string ContentDirectoryName
    {
        get => this.contentDirName;
        set => this.contentDirName = value.GetLastDirName();
    }

    /// <inheritdoc/>
    public virtual string ResolveFilePath(string contentPathOrName)
    {
        if (string.IsNullOrEmpty(contentPathOrName))
        {
            throw new ArgumentNullException(nameof(contentPathOrName), "The string parameter must not be null or empty.");
        }

        if (contentPathOrName.EndsWith(WinDirSeparatorChar) || contentPathOrName.EndsWith(CrossPlatDirSeparatorChar))
        {
            throw new ArgumentException($"The '{contentPathOrName}' cannot end with a folder. It must end with a file name with or without the extension.", nameof(contentPathOrName));
        }

        return contentPathOrName;
    }

    /// <inheritdoc/>
    public string ResolveDirPath() => $"{this.rootDirPath}{CrossPlatDirSeparatorChar}{this.contentDirName}";

    /// <summary>
    /// Gets the directory path of the content.
    /// </summary>
    /// <returns>The full directory path to the content directory.</returns>
    protected string GetContentDirPath() => $"{this.rootDirPath}{CrossPlatDirSeparatorChar}{this.contentDirName}";
}
