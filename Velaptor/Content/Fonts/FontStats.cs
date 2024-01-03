// <copyright file="FontStats.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

/// <summary>
/// Holds information about fonts.
/// </summary>
internal readonly record struct FontStats
{
    /// <summary>
    /// Gets the file path to the font that the font stats represent.
    /// </summary>
    public string FontFilePath { get; init; }

    /// <summary>
    /// Gets the font family.
    /// </summary>
    public string FamilyName { get; init; }

    /// <summary>
    /// Gets the style of font.
    /// </summary>
    /// <remarks>
    /// <list type="number">
    ///     <item><see cref="FontStyle.Regular"/></item>
    ///     <item><see cref="FontStyle.Bold"/></item>
    ///     <item><see cref="FontStyle.Italic"/></item>
    ///     <item><see cref="FontStyle.Bold"/><see cref="FontStyle.Italic"/></item>
    /// </list>
    /// </remarks>
    public FontStyle Style { get; init; }

    /// <summary>
    /// Gets the source of where the font was loaded.
    /// </summary>
    public FontSource Source { get; init; }
}
