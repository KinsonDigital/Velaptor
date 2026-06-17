namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;

public class UIContainer : IRenderable
{
    private readonly IShapeRenderer shapeRenderer;

    public UIContainer()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
    }

    public string Title { get; set; }

    public Vector2 Position { get; set; } = new Vector2(300, 300);

    public int Width { get; set; } = 300;

    public int Height { get; set; } = 300;

    public void Render()
    {
        var backgroundRect = new RectShape
        {
            Position = Position,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 30, 30, 30),
            IsSolid = true,
        };

        this.shapeRenderer.Render(backgroundRect);
    }
}
