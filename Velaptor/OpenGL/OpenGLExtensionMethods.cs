// <copyright file="OpenGLExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System.Numerics;

/// <summary>
/// Provides various helper methods for OpenGL-related operations.
/// </summary>
internal static class OpenGLExtensionMethods
{
    /// <summary>
    /// Converts the given <paramref name="pixelVector"/> to NDC units based on the given
    /// <paramref name="pixelScreenWidth"/> and <paramref name="pixelScreenHeight"/>.
    /// </summary>
    /// <param name="pixelVector">The vector in pixel units to convert.</param>
    /// <param name="pixelScreenWidth">The width of the screen in pixel units.</param>
    /// <param name="pixelScreenHeight">The height of the screen in pixel units.</param>
    /// <returns>The pixel vector in NDC units.</returns>
    /// <remarks>NDC is an acronym for Normal Device Coordinates.</remarks>
    public static Vector2 ToNDC(this Vector2 pixelVector, float pixelScreenWidth, float pixelScreenHeight)
    {
        var ndcX = pixelVector.X.MapValue(0, pixelScreenWidth, -1f, 1f);
        var ndcY = pixelVector.Y.MapValue(0, pixelScreenHeight, 1f, -1f);

        return new Vector2(ndcX, ndcY);
    }

    /// <summary>
    /// Converts the given value <paramref name="x"/> from pixel units to NDC units.
    /// </summary>
    /// <param name="x">The value to convert.</param>
    /// <param name="textureWidth">The horizontal width of the texture.</param>
    /// <returns>The value in NDC units.</returns>
    /// <remarks>
    /// <para>
    /// Texture coordinates are the coordinates that make up the local space (bounds) of a
    /// texture with the origin relative to the top right corner.
    /// </para>
    ///
    /// <para>
    /// NDC is an acronym for Normal Device Coordinates.
    /// </para>
    /// </remarks>
    public static float ToNDCTextureCoordX(this float x, float textureWidth)
        => x.MapValue(0f, textureWidth, 0f, 1f);

    /// <summary>
    /// Converts the given value <paramref name="y"/> from pixel units to NDC units.
    /// </summary>
    /// <param name="y">The value to convert.</param>
    /// <param name="textureHeight">The vertical height of the texture.</param>
    /// <returns>The value in NDC units.</returns>
    /// <remarks>
    /// <para>
    /// Texture coordinates are the coordinates that make up the local space (bounds) of a
    /// texture with the origin relative to the top right corner.
    /// </para>
    ///
    /// <para>
    /// NDC is an acronym for Normal Device Coordinates.
    /// </para>
    /// </remarks>
    public static float ToNDCTextureCoordY(this float y, float textureHeight)
        => y.MapValue(0f, textureHeight, 1f, 0f);

    /// <summary>
    /// Converts the given value <paramref name="coord"/> from pixel units to NDC units.
    /// </summary>
    /// <param name="coord">The value to convert.</param>
    /// <param name="textureWidth">The horizontal width of the texture.</param>
    /// <param name="textureHeight">The vertical height of the texture.</param>
    /// <returns>The value in NDC units.</returns>
    /// <remarks>
    /// <para>
    /// Texture coordinates are the coordinates that make up the local space (bounds) of a
    /// texture with the origin relative to the top right corner.
    /// </para>
    ///
    /// <para>
    /// NDC is an acronym for Normal Device Coordinates.
    /// </para>
    /// </remarks>
    public static Vector2 ToNDCTextureCoords(this Vector2 coord, float textureWidth, float textureHeight)
        => new (coord.X.ToNDCTextureCoordX(textureWidth), coord.Y.ToNDCTextureCoordY(textureHeight));
}
