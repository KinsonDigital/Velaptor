using System.Drawing;
using System.Numerics;

public class Control : IControl
{
    public static readonly string DefaultBoldFontName = "TimesNewRoman-Regular.ttf";

    public virtual float Width { get; set; }

    public virtual float Height { get; set; }

    public virtual float HalfWidth => Width / 2f;

    public virtual float HalfHeight => Height / 2f;

    public virtual Vector2 Position { get; set; }

    public float Left => Position.X;

    public float Top => Position.Y;

    public float Right => Position.X + Width;

    public float Bottom => Position.Y + Height;

    protected bool IsLoaded { get; set; }

    public virtual void Load()
    {
        IsLoaded = true;
    }

    public virtual void Unload()
    {
        IsLoaded = false;
    }

    public virtual void Render(int layer = 0)
    {
    }

    public virtual void Update()
    {
    }
}