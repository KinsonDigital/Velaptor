// <copyright file="FontMetaDataParseResult.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Holds information after the result of parsing font metadata from font file paths.
/// using the <see cref="FontMetaDataParser"/>.
/// </summary>
internal readonly record struct FontMetaDataParseResult
{
    /// <summary>
    /// Gets a value indicating whether or not a string contains metadata.
    /// </summary>
    public bool ContainsMetaData { get; init; }

    /// <summary>
    /// Gets a value indicating whether or not the metadata in the string is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the data before the metadata section.
    /// </summary>
    public string MetaDataPrefix { get; init; }

    /// <summary>
    /// Gets the metadata string.
    /// </summary>
    public string MetaData { get; init; }

    /// <summary>
    /// Gets the font size that was embedded in the metadata.
    /// </summary>
    public uint FontSize { get; init; }
}
