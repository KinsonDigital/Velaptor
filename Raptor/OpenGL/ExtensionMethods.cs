using System.Drawing;
using OpenToolkit.Mathematics;
using Raptor.Graphics;

namespace Raptor.OpenGL
{
    public static class ExtensionMethods
    {
        public static float MapValue(this float value, float fromStart, float fromStop, float toStart, float toStop) =>
            toStart + (toStop - toStart) * ((value - fromStart) / (fromStop - fromStart));

        public static byte MapValue(this byte value, byte fromStart, byte fromStop, byte toStart, byte toStop) =>
            (byte)(toStart + (toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));
            

        public static Vector4 MapValues(this Vector4 value, float fromStart, float fromStop, float toStart, float toStop)
        {
            return new Vector4
            {
                X = value.X.MapValue(fromStart, fromStop, toStart, toStop),
                Y = value.Y.MapValue(fromStart, fromStop, toStart, toStop),
                Z = value.Z.MapValue(fromStart, fromStop, toStart, toStop),
                W = value.W.MapValue(fromStart, fromStop, toStart, toStop)
            };
        }


        public static Vector4 ToVector4(this System.Drawing.Color clr) => new Vector4(clr.R, clr.G, clr.B, clr.A);


        public static Vector4 ToVector4(this Graphics.Color clr) => new Vector4(clr.Red, clr.Green, clr.Blue, clr.Alpha);


        public static Vector4 ToGLColor(this System.Drawing.Color value)
        {
            var vec4 = value.ToVector4();
            return vec4.MapValues(0, 255, 0, 1);
        }


        public static Vector4 ToGLColor(this Graphics.Color value)
        {
            var vec4 = value.ToVector4();
            return vec4.MapValues(0, 255, 0, 1);
        }
    }
}
