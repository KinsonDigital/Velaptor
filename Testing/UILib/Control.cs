using System.Numerics;

public class Control : IControl
{
    public static readonly string DefaultBoldFontName = "TimesNewRoman-Regular.ttf";

    public virtual int Width { get; set; }

    public virtual int Height { get; set; }

    public virtual int HalfWidth => Width / 2;

    public virtual int HalfHeight => Height / 2;

    public Vector2 Position { get; set; }

    public virtual void Load()
    {
    }

    public virtual void Unload()
    {
    }

    public virtual void Render()
    {
    }

    public virtual void Update()
    {
    }
}