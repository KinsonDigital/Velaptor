using System.Numerics;
using UILib;

public interface IControl : IRenderable, IUpdatable, ILoadable
{
    Vector2 Position { get; set; }

    int Width { get; set; }

    int Height { get; set; }
}