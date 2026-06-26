using System.Drawing;
using System.Numerics;
using UILib;

public interface IControl : IRenderable, IUpdatable, ILoadable
{
    Point Position { get; set; }

    int Width { get; set; }

    int Height { get; set; }

    int Left { get; }

    int Top { get; }

    int Right { get; }

    int Bottom { get; }
}
