namespace UILib;

using System.Drawing;
using System.Numerics;
using UILib;

public interface IControl : IRenderable, IUpdatable, ILoadable
{
    string Name { get; set; }

    Vector2 Position { get; set; }

    float Width { get; set; }

    float Height { get; set; }

    float Left { get; }

    float Top { get; }

    float Right { get; }

    float Bottom { get; }

    bool Visible { get; set; }

    bool Enabled { get; set; }

    Color DisabledColor { get; set; }
}
