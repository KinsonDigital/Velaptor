namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;

public class UIContainer : Control
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly List<IControl> controls = new();
    private RectShape background;
    private RectShape titleBar;

    public UIContainer() => this.shapeRenderer = RendererFactory.CreateShapeRenderer();

    public string Title { get; set; }

    public int Width { get; set; } = 300;

    public int Height { get; set; } = 300;

    public void AddControl(IControl control)
    {
        ArgumentNullException.ThrowIfNull(control);

        this.controls.Add(control);
    }

    public override void Load()
    {
        foreach (var control in this.controls)
        {
            control.Load();
        }

        base.Load();
    }

    public override void Unload()
    {
        foreach (var control in this.controls)
        {
            control.Unload();
        }

        base.Unload();
    }

    public override void Update()
    {
        this.background = new RectShape
        {
            Position = Position,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 17, 17, 17),
            IsSolid = true,
        };

        var titleBarHeight = 30;
        var titleBarHalfHeight = titleBarHeight / 2f;
        this.titleBar = new RectShape
        {
            Position = new Vector2(Position.X, Position.Y - (this.background.HalfHeight - titleBarHalfHeight)),
            Width = Width,
            Height = titleBarHeight,
            Color = Color.FromArgb(255, 45, 74, 117),
            IsSolid = true,
        };

        // Update all of the controls
        foreach (var control in this.controls)
        {
            control.Update();
        }
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.background, -100);
        this.shapeRenderer.Render(this.titleBar, -100);

        // Render all of the controls
        foreach (var control in this.controls)
        {
            var ctrlPos = control.Position;

            control.Position = new Vector2(Position.X + ctrlPos.X, Position.Y + ctrlPos.Y);
            control.Render();
            control.Position = ctrlPos;
        }
    }
}
