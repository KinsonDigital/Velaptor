namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class UIContainer : Control
{
    private const int TitleBarPaddingLeft = 5;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label titleBarText;
    private readonly List<IControl> controls = new();
    private RectShape background;
    private RectShape titleBar;
    private MouseState prevMouseState;
    private Vector2 dragStartPos = new(float.MinValue, float.MinValue);

    public UIContainer()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.titleBarText = new Label();
        this.titleBarText.Text = "UI Container";
    }

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
        this.titleBarText.Load();

        foreach (var control in this.controls)
        {
            control.Load();
        }

        base.Load();
    }

    public override void Unload()
    {
        this.titleBarText.Unload();

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

        this.titleBarText.Position = new Vector2(
            this.titleBar.Position.X - (Width / 2f) + TitleBarPaddingLeft,
            this.titleBar.Position.Y - (this.titleBarText.TextSize.Height / 2f));
        this.titleBarText.Update();

        // Update all of the controls
        foreach (var control in this.controls)
        {
            control.Position = Position;
            control.Update();
        }

        ProcessDragState();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.background, -100);
        this.shapeRenderer.Render(this.titleBar, -100);
        this.titleBarText.Render();

        // Render all of the controls
        foreach (var control in this.controls)
        {
            control.Render();
        }
    }

    private void ProcessDragState()
    {
        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition().ToVector2();

        // If the mouse is in the title bar
        if (this.titleBar.Contains(mousePos))
        {
            // If the mouse is in the down position, move the container based on the delta movement of the mouse
            if (currentMouseState.IsButtonDown(MouseButton.LeftButton) && this.prevMouseState.IsButtonUp(MouseButton.LeftButton))
            {
                this.dragStartPos = mousePos;
                var delta = mousePos - this.dragStartPos;
                Position += delta;
            }
        }

        // If the mouse is button has been lifted
        if (this.prevMouseState.IsButtonDown(MouseButton.LeftButton) && currentMouseState.IsButtonUp(MouseButton.LeftButton))
        {
            this.dragStartPos = new Vector2(float.MinValue, float.MinValue);
        }

        this.prevMouseState = currentMouseState;
    }
}
