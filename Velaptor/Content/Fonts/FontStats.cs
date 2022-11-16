// <copyright file="FontStats.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Velaptor.Content.Fonts;

/// <summary>
/// Holds information about fonts.
/// </summary>
internal struct FontStats : IEquatable<FontStats>
{
    /// <summary>
    /// The file path to the font that the font stats represent.
    /// </summary>
    public string FontFilePath;

    /// <summary>
    /// The font family.
    /// </summary>
    public string FamilyName;

    /// <summary>
    /// The style of font.
    /// </summary>
    /// <remarks>
    /// <list type="number">
    ///     <item><see cref="FontStyle.Regular"/></item>
    ///     <item><see cref="FontStyle.Bold"/></item>
    ///     <item><see cref="FontStyle.Italic"/></item>
    ///     <item><see cref="FontStyle.Bold"/><see cref="FontStyle.Italic"/></item>
    /// </list>
    /// </remarks>
    public FontStyle Style;

    /// <summary>
    /// The source of where the font was loaded.
    /// </summary>
    public FontSource Source;

    /// <inheritdoc/>
    public bool Equals(FontStats other) =>
        this.FontFilePath == other.FontFilePath &&
        this.FamilyName == other.FamilyName &&
        this.Style == other.Style &&
        this.Source == other.Source;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is FontStats other && Equals(other);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => HashCode.Combine(this.FontFilePath, this.FamilyName, (int)this.Style, this.Source);
}
