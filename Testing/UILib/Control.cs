using System.Drawing;

public class Control : IControl
{
    public static readonly string DefaultBoldFontName = "TimesNewRoman-Regular.ttf";

    public virtual int Width { get; set; }

    public virtual int Height { get; set; }

    public virtual int HalfWidth => Width / 2;

    public virtual int HalfHeight => Height / 2;

    public virtual Point Position { get; set; }

    public int Left => Position.X;

    public int Top => Position.Y;

    public int Right => Position.X + Width;

    public int Bottom => Position.Y + Height;

    public virtual void Load()
    {
    }

    public virtual void Unload()
    {
    }

    public virtual void Render(int layer = 0)
    {
    }

    public virtual void Update()
    {
    }
}