using System;

namespace Raptor
{
    public struct TextureData : IComparable
    {
        #region Props
        public int ID { get; set; }

        public int Layer { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Angle { get; set; }

        public float Size { get; set; }

        public byte TintColorAlpha { get; set; }

        public byte TintColorRed { get; set; }

        public byte TintColorGreen { get; set; }

        public byte TintColorBlue { get; set; }

        public int CompareTo(object? obj)
        {
            if (!(obj is TextureData data))
                return 0;

            if (Layer < data.Layer)
                return -1;
            else if (Layer > data.Layer)
                return 1;


            return 0;
        }
        #endregion
    }
}
