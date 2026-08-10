namespace Velum;

using System.Drawing;
using System.Numerics;

public class Control : IControl
{
    protected static readonly string DefaultBoldFontName = "TimesNewRoman-Regular.ttf";

    public string Name { get; set; } = string.Empty;

    public virtual float Width { get; set; }

    public virtual float Height { get; set; }

    public virtual float HalfWidth => Width / 2f;

    public virtual float HalfHeight => Height / 2f;

    public virtual Vector2 Position { get; set; }

    public float Left => Position.X;

    public float Top => Position.Y;

    public float Right => Position.X + Width;

    public float Bottom => Position.Y + Height;

    protected bool IsLoaded { get; private set; }

    public bool Visible { get; set; } = true;

    public virtual bool Enabled { get; set; } = true;

    public virtual Color DisabledColor { get; set; } = Color.FromArgb(76, 76, 76);

    public virtual void Load() => IsLoaded = true;

    public virtual void Unload() => IsLoaded = false;

    public virtual void Render(int layer)
    {
    }

    public virtual void Update()
    {
    }
}
