using System.Drawing;

namespace Velaptor.UI
{
    using System.Numerics;
    using Velaptor.Graphics;

    public abstract class ControlBase : IControl
    {
        public string Name { get; set; }

        public virtual Point Position { get; set; }

        public virtual int Left
        {
            get => Position.X;
            set => Position = new Point(value, Position.Y);
        }

        public virtual int Right
        {
            get => Position.X + Width;
            set => Position = new Point(value - Width, Position.Y);
        }

        public virtual int Top
        {
            get => Position.Y;
            set => Position = new Point(Position.X, value);
        }

        public virtual int Bottom
        {
            get => Position.Y + Height;
            set => Position = new Point(Position.X, value - Height);
        }

        public virtual int Width { get; set; }

        public virtual int Height { get; set; }

        public virtual bool Visible { get; set; } = true;

        public virtual bool Enabled { get; set; } = true;

        public bool IsLoaded { get; private set; }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
            if (IsLoaded)
            {
                return;
            }

            IsLoaded = true;
        }

        public virtual void Update(FrameTime frameTime)
        {
            if (IsLoaded is false)
            {
                return;
            }
        }

        public virtual void Render(ISpriteBatch spriteBatch)
        {
        }

        public virtual void Dispose()
        {

        }
    }
}
