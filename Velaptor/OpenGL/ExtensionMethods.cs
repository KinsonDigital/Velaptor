// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    // TODO: Code docs
    internal static class ExtensionMethods
    {
        public static Vector2 ToNDC(this Vector2 pixelVector, float pixelScreenWidth, float pixelScreenHeight)
        {
            var ndcX = pixelVector.X.MapValue(0, pixelScreenWidth, -1f, 1f);
            var ndcY = pixelVector.Y.MapValue(0, pixelScreenHeight, 1f, -1f);

            return new Vector2(ndcX, ndcY);
        }

        public static float ToNDCTextureCoord(this float y, float max) => y.MapValue(0f, max, 0f, 1f);

        public static Vector2 ToNDCTextureCoords(this Vector2 coord, float textureWidth, float textureHeight)
            => new (coord.X.ToNDCTextureCoord(textureWidth), coord.Y.ToNDCTextureCoord(textureHeight));

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
