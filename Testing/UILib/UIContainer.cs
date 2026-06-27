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
    private const int TitleBarHeight = 30;
    private const int TitleBarHalfHeight = TitleBarHeight / 2;
    private const int ControlLeftPadding = 10;
    private const int ControlTopPadding = 10;
    private const float BorderThickness = 3f;
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label titleBarText;
    private readonly List<IControl> controls = new();
    private readonly Color titleBarClr = Color.FromArgb(255, 45, 74, 117);
    private readonly Color borderClr = Color.FromArgb(255, 45, 74, 117);
    private readonly Color areaClr = Color.FromArgb(255, 17, 17, 17);
    private RectShape area;
    private RectShape titleBar;
    private MouseState prevMouseState;
    private Vector2 lastMousePos;
    private Line leftLine;
    private Line rightLine;
    private Line bottomLine;
    private bool isDragging;

    public UIContainer()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.titleBarText = new Label();
        this.titleBarText.Text = "Container";

        this.area = new RectShape
        {
            Width = 300,
            Height = 270,
            Color = this.areaClr,
            IsSolid = true,
        };
    }

    public string Title { get; set; }

    public override float Width
    {
        get => this.area.Width;
        set => this.area.Width = value;
    }

    public override float Height
    {
        get => TitleBarVisible ? this.titleBar.Height + this.area.Height : this.area.Height;
        set
        {
            if (TitleBarVisible)
            {
                this.area.Height = value - this.titleBar.Height;
            }
            else
            {
                this.area.Height = value;
            }
        }
    }

    public override Vector2 Position
    {
        get => this.area.Position;
        set => this.area.Position = value.ToWorld(this.area.Width, this.area.Height);
    }

    public bool AutoSize { get; set; } = false;

    public bool TitleBarVisible { get; set; } = true;

    public bool BorderVisible { get; set; } = true;

    public bool Draggable { get; set; }

    public Layout Layout { get; set; } = new Layout { LayoutGroup = 0, StackDirection = StackDirection.Vertical };

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
        this.titleBar = new RectShape
        {
            Position = new Vector2(Position.X, Position.Y - (this.area.HalfHeight - TitleBarHalfHeight)),
            Width = Width,
            Height = TitleBarHeight,
            Color = this.titleBarClr,
            IsSolid = true,
        };

        this.titleBarText.Position = new Vector2(
            this.titleBar.Position.X - (Width / 2f) + TitleBarLeftTextPadding,
            this.titleBar.Position.Y - (this.titleBarText.TextSize.Height / 2f));

        if (TitleBarVisible)
        {
            this.titleBarText.Update();
        }

        ProcessLayout();
        ProcessBorder();
        ProcessDragState();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.area, -100);

        if (TitleBarVisible)
        {
            this.shapeRenderer.Render(this.titleBar, -100);
            this.titleBarText.Render();
        }

        if (BorderVisible)
        {
            this.lineRenderer.Render(this.leftLine, -100);
            this.lineRenderer.Render(this.bottomLine, -100);
            this.lineRenderer.Render(this.rightLine, -100);
        }

        // Render all of the controls
        foreach (var control in this.controls)
        {
            control.Render();
        }
    }

    private void ProcessLayout()
    {
        var titleBarBottomLeftCorner = new Vector2(this.titleBar.Left, this.titleBar.Bottom);
        var maxRight = 0f;
        var maxBottom = 0f;

        // Update all of the controls
        for (var i = 0; i < this.controls.Count; i++)
        {
            var control = this.controls[i];

            switch (Layout.StackDirection)
            {
                case StackDirection.Horizontal:
                    if (i == 0)
                    {
                        control.Position = titleBarBottomLeftCorner + new Vector2(ControlLeftPadding, ControlTopPadding);
                    }
                    else
                    {
                        var prevControl = this.controls[i - 1];
                        control.Position = new Vector2(
                            prevControl.Right + ControlLeftPadding,
                            titleBarBottomLeftCorner.Y + ControlTopPadding);
                    }

                    break;
                case StackDirection.Vertical:
                    if (i == 0)
                    {
                        control.Position = titleBarBottomLeftCorner + new Vector2(ControlLeftPadding, ControlTopPadding);
                    }
                    else
                    {
                        var prevControl = this.controls[i - 1];
                        control.Position = new Vector2(
                            titleBarBottomLeftCorner.X + ControlLeftPadding,
                            prevControl.Bottom + ControlTopPadding);
                    }

                    break;
            }

            control.Update();

            maxRight = Math.Max(maxRight, control.Right);
            maxBottom = Math.Max(maxBottom, control.Bottom);
        }

        if (AutoSize)
        {
            Width = maxRight - (float)Math.Round(Position.X, 0) + ControlLeftPadding;
            Height = maxBottom - (float)Math.Round(Position.Y, 0) + ControlTopPadding;
        }
    }

    private void ProcessDragState()
    {
        if (!Draggable || !TitleBarVisible)
        {
            return;
        }

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
            Position += delta;
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

    private void ProcessBorder()
    {
        if (!BorderVisible)
        {
            return;
        }

        // TODO: Instead of having the border render internally, render it externally from the internal area.
        // This means that the width and height will have to be calculated by adding the half thickness of
        // the border as long as the border is set to visible.  This is to prevent half of the border being rendered
        // internally and overlapping any of the edges of controls when rendering

        this.leftLine = new Line
        {
            P1 = new Vector2(this.titleBar.Left + (BorderThickness / 2f), this.titleBar.Bottom),
            P2 = new Vector2(this.titleBar.Left + (BorderThickness / 2f), this.area.Bottom),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };

        this.bottomLine = new Line
        {
            P1 = new Vector2(this.titleBar.Left, this.area.Bottom - (BorderThickness / 2f)),
            P2 = new Vector2(this.titleBar.Right, this.area.Bottom - (BorderThickness / 2f)),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };

        this.rightLine = new Line
        {
            P1 = new Vector2(this.titleBar.Right - (BorderThickness / 2f), this.area.Bottom),
            P2 = new Vector2(this.titleBar.Right - (BorderThickness / 2f), this.titleBar.Bottom),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };
    }
}
