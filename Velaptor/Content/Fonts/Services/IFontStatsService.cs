// <copyright file="IFontStatsService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts.Services;

/// <summary>
/// Collects data about fonts.
/// </summary>
internal interface IFontStatsService
{
    /// <summary>
    /// Gets the stats for all of the fonts for the given font family in the application's content font directory.
    /// </summary>
    /// <param name="fontFamilyName">The font family.</param>
    /// <returns>Information about all the different styles of the fonts that match the font family.</returns>
    /// <remarks>
    /// <para>
    ///     Default Content Dir Path: ~\App-Directory\Content\Fonts.
    /// </para>
    /// </remarks>
    FontStats[] GetContentStatsForFontFamily(string fontFamilyName);

    /// <summary>
    /// Gets the stats for all of the fonts for the given font family in the systems font directory.
    /// </summary>
    /// <param name="fontFamilyName">The font family.</param>
    /// <returns>Information about all the different styles of the fonts that match the font family.</returns>
    /// <remarks>
    /// <para>
    ///     Windows Font Dir Path: C:\Windows\Fonts\.
    /// </para>
    /// </remarks>
    FontStats[] GetSystemStatsForFontFamily(string fontFamilyName);
}
