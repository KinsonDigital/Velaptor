using System.Drawing;

namespace Raptor.OpenGL
{
    internal struct SpriteBatchItem
    {
        public int TextureID;

        public Rectangle SrcRect;

        public Rectangle DestRect;

        public float Size;

        public float Angle;

        public Color TintColor;

        public static SpriteBatchItem Empty => new SpriteBatchItem() { TextureID = -1 };

        public bool IsEmpty => this.TextureID == -1 &&
                    this.SrcRect.IsEmpty &&
                    this.DestRect.IsEmpty &&
                    this.Size == 0f &&
                    this.Angle == 0f &&
                    this.TintColor.IsEmpty;
    }
}
