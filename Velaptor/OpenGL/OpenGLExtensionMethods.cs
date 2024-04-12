// <copyright file="OpenGLExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using GpuData;

/// <summary>
/// Provides various helper methods for OpenGL related operations.
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
    /// <remarks>NDC is an acronym for (N)ormal (D)evice (C)oordinates.</remarks>
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
    /// NDC is an acronym for (N)ormal (D)evice (C)oordinates.
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
    /// NDC is an acronym for (N)ormal (D)evice (C)oordinates.
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
    /// NDC is an acronym for (N)ormal (D)evice (C)oordinates.
    /// </para>
    /// </remarks>
    public static Vector2 ToNDCTextureCoords(this Vector2 coord, float textureWidth, float textureHeight)
        => new (coord.X.ToNDCTextureCoordX(textureWidth), coord.Y.ToNDCTextureCoordY(textureHeight));

    /// <summary>
    /// Converts the given <paramref name="vector"/> to an array of <see cref="float"/>[] values.
    /// </summary>
    /// <param name="vector">The value to convert.</param>
    /// <returns>The components of the value as a <see cref="float"/>[] array.</returns>
    /// <remarks>
    /// Value To Array Order:
    /// <list type="number">
    ///     <item><see cref="Vector"/>.<see cref="Vector2.X"/></item>
    ///     <item><see cref="Vector"/>.<see cref="Vector2.Y"/></item>
    /// </list>
    /// </remarks>
    public static float[] ToArray(this Vector2 vector) => new[] { vector.X, vector.Y };

    /// <summary>
    /// Returns all the <see cref="Vector4"/> components as a <see cref="float"/> array.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>The components in a <c>X</c> <c>Y</c> <c>Z</c> <c>W</c> order.</returns>
    public static float[] ToArray(this Vector4 vector) => new[] { vector.X, vector.Y, vector.Z, vector.W };

    /// <summary>
    /// Converts the given <paramref name="color"/> to an array of <see cref="float"/>[] values.
    /// </summary>
    /// <param name="color">The value to convert.</param>
    /// <returns>The components of the value as a <see cref="float"/>[] array.</returns>
    /// <remarks>
    /// The order of color components R,G,B,A is an OpenGL specific requirement.
    /// Value To Array Order:
    /// <list type="number">
    ///     <item><see cref="Color"/>.<see cref="Color.R"/></item>
    ///     <item><see cref="Color"/>.<see cref="Color.G"/></item>
    ///     <item><see cref="Color"/>.<see cref="Color.B"/></item>
    ///     <item><see cref="Color"/>.<see cref="Color.A"/></item>
    /// </list>
    /// </remarks>
    public static float[] ToArray(this Color color)
        => new float[] { color.R, color.G, color.B, color.A };

    /// <summary>
    /// Converts the given <paramref name="vertexData"/> components to an array of floats.
    /// </summary>
    /// <param name="vertexData">The data to convert.</param>
    /// <returns>An array of float values.</returns>
    public static float[] ToArray(this TextureVertexData vertexData)
    {
        // NOTE: The order of the array elements are extremely important.
        // They determine the layout of each stride of vertex data and the layout
        // here has to match the layout told to OpenGL
        var result = new List<float>();

        result.AddRange(vertexData.VertexPos.ToArray());
        result.AddRange(vertexData.TextureCoord.ToArray());
        result.AddRange(vertexData.TintColor.ToArray());

        return result.ToArray();
    }

    /// <summary>
    /// Converts the given <paramref name="data"/> components to an array of floats.
    /// </summary>
    /// <param name="data">The quad to convert.</param>
    /// <returns>An array of float values.</returns>
    public static float[] ToArray(this TextureGpuData data)
    {
        var result = new List<float>();

        result.AddRange(data.Vertex1.ToArray());
        result.AddRange(data.Vertex2.ToArray());
        result.AddRange(data.Vertex3.ToArray());
        result.AddRange(data.Vertex4.ToArray());

        return result.ToArray();
    }

    /// <summary>
    /// Converts the given list of <paramref name="data"/> to an array of floats.
    /// </summary>
    /// <param name="data">The quads to convert.</param>
    /// <returns>An array of float values.</returns>
    public static float[] ToArray(this IEnumerable<TextureGpuData> data)
    {
        var result = new List<float>();

        foreach (var quad in data)
        {
            result.AddRange(quad.ToArray());
        }

        return result.ToArray();
    }
}
