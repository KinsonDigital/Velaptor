// <copyright file="IImGuiService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.Services;

/// <summary>
/// Provides extra <see cref="ImGui"/> functionality.
/// </summary>
internal interface IImGuiService
{
    /// <summary>
    /// Gets the pixel data, texture width, and texture height of the font atlas.
    /// </summary>
    /// <returns>The atlas related data.</returns>
    // ReSharper disable once InconsistentNaming
    (byte[] bytes, int textureWidth, int textureHeight) GetTexDataAsRGBA32();

    /// <summary>
    /// Sets the ID of the texture atlas of the font.
    /// </summary>
    /// <param name="textureId">The texture id.</param>
    // ReSharper disable once InconsistentNaming
    void SetTexID(uint textureId);

    /// <summary>
    /// Clears all of the currently loaded <see cref="ImGui"/> fonts.
    /// </summary>
    void ClearFonts();

    /// <summary>
    /// Clears the font texture atlas data from memory.
    /// </summary>
    void ClearTexData();

    /// <summary>
    /// Adds an embedded font to <see cref="ImGui"/> wiht the given <paramref name="fontSize"/>.
    /// </summary>
    /// <param name="fontSize">The size of the font.</param>
    void AddEmbeddedFont(uint fontSize);

    /// <summary>
    /// Enables the ini file for saving and loading window positions and sizes.
    /// </summary>
    void EnableIniFile();

    /// <summary>
    /// Disables the ini file for saving and loading window positions and sizes.
    /// </summary>
    void DisableIniFile();
}
