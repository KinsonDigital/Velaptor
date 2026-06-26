namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class UIContainer : Control
{
    private const int TitleBarLeftTextPadding = 5;
    private const int ControlLeftPadding = 10;
    private const int ControlTopPadding = 10;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label titleBarText;
    private readonly List<IControl> controls = new();
    private RectShape background;
    private RectShape titleBar;
    private MouseState prevMouseState;
    private bool isDragging;
    private Vector2 lastMousePos;

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
        var scrnPos = Position.ToScreen(Width, Height);

        this.background = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 17, 17, 17),
            IsSolid = true,
        };

        var titleBarHeight = 30;
        var titleBarHalfHeight = titleBarHeight / 2f;
        this.titleBar = new RectShape
        {
            Position = new Vector2(scrnPos.X, scrnPos.Y - (this.background.HalfHeight - titleBarHalfHeight)),
            Width = Width,
            Height = titleBarHeight,
            Color = Color.FromArgb(255, 45, 74, 117),
            IsSolid = true,
        };

        this.titleBarText.Position = new Vector2(
            this.titleBar.Position.X - (Width / 2f) + TitleBarLeftTextPadding,
            this.titleBar.Position.Y - (this.titleBarText.TextSize.Height / 2f)).ToPoint();

        this.titleBarText.Update();

        var titleBarBottomLeftCorner = new Vector2(this.titleBar.Left, this.titleBar.Bottom);

        // Update all of the controls
        for (var i = 0; i < this.controls.Count; i++)
        {
            var control = this.controls[i];

            if (i == 0)
            {
                control.Position = (titleBarBottomLeftCorner + new Vector2(ControlLeftPadding, ControlTopPadding)).ToPoint();
            }
            else
            {
                var prevControl = this.controls[i - 1];
                control.Position = new Vector2(
                    titleBarBottomLeftCorner.X + ControlLeftPadding,
                    prevControl.Position.Y + prevControl.Height + (ControlTopPadding * (i + 0))).ToPoint();
            }

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

        // Start dragging if the left mouse button is pressed while in the title bar
        if (currentMouseState.IsButtonDown(MouseButton.LeftButton) &&
            this.prevMouseState.IsButtonUp(MouseButton.LeftButton) &&
            this.titleBar.Contains(mousePos))
        {
            this.isDragging = true;
            this.lastMousePos = mousePos;
        }

        // While dragging, move the container by the mouse delta each frame
        if (this.isDragging && currentMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            var delta = mousePos - this.lastMousePos;
            Position = (Position.ToVector2() + delta).ToPoint();
            this.lastMousePos = mousePos;
        }

        // Stop dragging when the mouse button is released
        if (this.prevMouseState.IsButtonDown(MouseButton.LeftButton) &&
            currentMouseState.IsButtonUp(MouseButton.LeftButton))
        {
            this.isDragging = false;
        }

        this.prevMouseState = currentMouseState;
    }
}
