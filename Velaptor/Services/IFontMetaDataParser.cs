// <copyright file="IFontMetaDataParser.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

/// <summary>
/// Parses metadata from a string.
/// </summary>
internal interface IFontMetaDataParser
{
    /// <summary>
    /// Parse the given <paramref name="stringToParse"/> to extract the contained metadata.
    /// </summary>
    /// <param name="stringToParse">The string to parse the metadata from.</param>
    /// <returns>The metadata that was contained in the given <paramref name="stringToParse"/>.</returns>
    FontMetaDataParseResult Parse(string stringToParse);
}
