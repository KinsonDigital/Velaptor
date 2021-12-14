// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System.Collections.Generic;
    using System.Numerics;

    // TODO: Code docs
    internal static class ExtensionMethods
    {
        public static Vector2 ToNDC(this Vector2 pixelVector, float pixelScreenWidth, float pixelScreenHeight)
        {
            var ndcX = pixelVector.X.MapValue(0, pixelScreenWidth, -1f, 1f);
            var ndcY = pixelVector.Y.MapValue(0, pixelScreenHeight, 1f, -1f);

            return new Vector2(ndcX, ndcY);
        }

        public static float ToNDCTextureCoordX(this float x, float textureWidth)
        {
            return x.MapValue(0f, textureWidth, 0f, 1f);
        }

        public static float ToNDCTextureCoordY(this float y, float textureHeight)
        {
            /* NOTE:
             * The toStart is 1 and the toStop is 0 inead of the other way around
             * because the Y axis in the OpenGL coordinate system is flipped opposite
             * compared to the CPU side. For example, the window.
             */
            return y.MapValue(0f, textureHeight, 1f, 0f);
        }

        public static Vector2 ToNDCTextureCoords(this Vector2 coord, float textureWidth, float textureHeight)
            => new (coord.X.ToNDCTextureCoordX(textureWidth), coord.Y.ToNDCTextureCoordY(textureHeight));

        public static float[] ToArray(this Vector2 vector)
        {
            float x;
            float y;

            unsafe
            {
                Vector2* myPtr = &vector;

                x = myPtr->X;
                y = myPtr->Y;
            }

            return new[] { x, y };
        }

        public static float[] ToArray(this System.Drawing.Color color)
            => new float[] { color.R, color.G, color.B, color.A };

        public static float[] ToArray(this TextureVertexData data)
        {
            var result = new List<float>();

            result.AddRange(data.VertexPos.ToArray());
            result.AddRange(data.TextureCoord.ToArray());
            result.AddRange(data.TintColor.ToArray());

            return result.ToArray();
        }

        public static float[] ToArray(this TextureQuadData data)
        {
            var result = new List<float>();

            result.AddRange(data.Vertex1.ToArray());
            result.AddRange(data.Vertex2.ToArray());
            result.AddRange(data.Vertex3.ToArray());
            result.AddRange(data.Vertex4.ToArray());

            return result.ToArray();
        }
    }
}
