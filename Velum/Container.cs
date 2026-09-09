namespace Velum;

using System.Drawing;
using System.Numerics;
using Carbonate;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Container : Control
{
    private const int TitleBarLeftTextPadding = 5;
    private const int TitleBarHeight = 30;
    private const int TitleBarHalfHeight = TitleBarHeight / 2;
    private const float BorderThickness = 3f;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label titleBarText;
    private Color titleBarClr = Color.FromArgb(255, 45, 74, 117);
    private readonly Color titleBarMouseNotOverClr = Color.FromArgb(255, 45, 74, 117);
    private readonly Color titleBarMouseOverClr = Color.FromArgb(255, 55, 91, 142);
    private readonly Color borderClr = Color.FromArgb(255, 45, 74, 117);
    private readonly IDisposable subscription;
    private Layout layout = new ();
    private RectShape area;
    private RectShape titleBar;
    private Vector2 logicalPosition;
    private MouseState prevMouseState;
    private Vector2 lastMousePos;
    private Line leftLine;
    private Line rightLine;
    private Line topLine;
    private Line bottomLine;
    private bool isDragging;
    private float baseAreaTop;
    private bool mouseClickDisabled;

    public Container()
    {
        var disableMouseClickReactable1 = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = disableMouseClickReactable1.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) => this.mouseClickDisabled = data.IsExpanded,
            () => this.subscription?.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.titleBarText = new Label();
        this.titleBarText.Text = "Container";

        this.area = new RectShape
        {
            Width = 300,
            Height = 270,
            Color = Color.FromArgb(255, 17, 17, 17),
            IsSolid = true,
        };

        this.baseAreaTop = this.logicalPosition.Y;
    }

    public string Title
    {
        get => this.titleBarText.Text;
        set => this.titleBarText.Text = value;
    }

    public override float Width
    {
        get
        {
            var totalBorderWidth = BorderVisible ? BorderThickness * 2f : 0f;
            var totalAreaPadding = AreaPadding * 2f;

            return this.area.Width + totalBorderWidth + totalAreaPadding;
        }
    }

    public override float Height
    {
        get
        {
            var totalBorderHeight = 0f;

            if (BorderVisible)
            {
                totalBorderHeight = TitleBarVisible ? BorderThickness : BorderThickness * 2f;
            }

            return TitleBarVisible
                ? TitleBarHeight + this.area.Height + totalBorderHeight
                : this.area.Height + totalBorderHeight;
        }
    }

    public override Vector2 Position
    {
        get => this.logicalPosition;
        set
        {
            this.logicalPosition = value;
            this.baseAreaTop = value.Y;

            // Resolve the area position immediately using current width/height.
            // This is correct if width/height were set before position.
            // If not, Update() will re-resolve with the current dimensions.
            this.area.Position = value.ToWorld(this.area.Width, this.area.Height);
        }
    }

    public Color BackgroundColor
    {
        get => this.area.Color;
        set => this.area.Color = value;
    }

    public bool TitleBarVisible { get; set; } = true;

    public bool BorderVisible { get; set; } = true;

    public bool Draggable { get; set; }

    public uint AreaPadding { get; set; } = 10;

    public int HorizontalSpacing { get; set; } = 10;

    public int VerticalSpacing { get; set; } = 10;

    public LayoutSettings Layout { get; set; } = new LayoutSettings { LayoutGroup = 0, StackDirection = StackDirection.Vertical };

    public bool Centered { get; set; } = false;

    public void AddLayoutControl(Layout control)
    {
        ArgumentNullException.ThrowIfNull(control);

        this.layout = control;
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.titleBarText.Load();

        this.layout.Load();

        base.Load();
    }

    public override void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.titleBarText.Unload();

        this.layout.Unload();

        base.Unload();
    }

    public override void Update()
    {
        // Process drag first — may change logicalPosition/baseAreaTop
        ProcessDragState();

        this.area.Width = this.layout.Width + (AreaPadding * 2f);
        this.area.Height = this.layout.Height + (AreaPadding * 2f);

        // Resolve area center position from logical (top-left) coordinates + current dimensions.
        // Deferred here so that Width/Height set after Position still produce correct results.
        this.area.Position = this.logicalPosition.ToWorld(this.area.Width, this.area.Height);

        // Position area based on title bar visibility (overrides Y from resolution above)
        if (TitleBarVisible)
        {
            // Stack: title bar at the top, area sits below it
            this.area.Top = this.baseAreaTop + TitleBarHeight;
        }
        else
        {
            // No title bar: area fills the entire container from the top
            this.area.Top = this.baseAreaTop;
        }

        // Create title bar at the container top (always created for bounds/border calcs)
        this.titleBar = new RectShape
        {
            Position = new Vector2(this.area.Position.X, this.baseAreaTop + TitleBarHalfHeight),
            Width = this.area.Width,
            Height = TitleBarHeight,
            Color = this.titleBarClr,
            IsSolid = true,
        };

        this.titleBarText.Position = new Vector2(
            this.titleBar.Position.X - (this.area.Width / 2f) + TitleBarLeftTextPadding,
            this.titleBar.Position.Y - (this.titleBarText.TextSize.Height / 2f));

        if (TitleBarVisible)
        {
            this.titleBarText.Update();
        }

        this.layout.Position = new Vector2(this.logicalPosition.X + AreaPadding, this.logicalPosition.Y + TitleBarHeight + AreaPadding);

        this.layout.Update();

        ProcessBorder();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.area, -1000);

        if (TitleBarVisible)
        {
            this.shapeRenderer.Render(this.titleBar, -100);
            this.titleBarText.Render(0);
        }

        if (BorderVisible)
        {
            this.shapeRenderer.Render(this.leftLine, -100);
            this.shapeRenderer.Render(this.bottomLine, -100);
            this.shapeRenderer.Render(this.rightLine, -100);

            if (!TitleBarVisible)
            {
                this.shapeRenderer.Render(this.topLine, -100);
            }
        }

        // Render all the controls
        this.layout.Render(0);
    }

    private void ProcessDragState()
    {
        if (!Draggable || !TitleBarVisible || this.mouseClickDisabled)
        {
            return;
        }

        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition().ToVector2();

        var isMouseOver = this.titleBar.Contains(mousePos);

        if (isMouseOver)
        {
            this.titleBarClr = this.titleBarMouseOverClr;
        }
        else
        {
            this.titleBarClr = this.titleBarMouseNotOverClr;
        }

        // Start dragging if the left mouse button is pressed while in the title bar
        if (isMouseOver && currentMouseState.IsButtonDown(MouseButton.LeftButton) &&
            this.prevMouseState.IsButtonUp(MouseButton.LeftButton))
        {
            this.isDragging = true;
            this.lastMousePos = mousePos;
        }

        // While dragging, move the container by the mouse delta each frame
        if (this.isDragging && currentMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            var delta = mousePos - this.lastMousePos;
            this.logicalPosition += delta;
            this.baseAreaTop = this.logicalPosition.Y;
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

        const float halfBorderThickness = BorderThickness / 2f;
        var titleBarOffset = TitleBarVisible ? TitleBarHeight : 0f;

        this.leftLine = new Line
        {
            P1 = new Vector2(this.area.Left - halfBorderThickness, this.area.Top - titleBarOffset),
            P2 = new Vector2(this.area.Left - halfBorderThickness, this.area.Bottom),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };

        this.bottomLine = new Line
        {
            P1 = new Vector2(this.area.Left - BorderThickness, this.area.Bottom + halfBorderThickness),
            P2 = new Vector2(this.area.Right + BorderThickness, this.area.Bottom + halfBorderThickness),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };

        this.rightLine = new Line
        {
            P1 = new Vector2(this.area.Right + halfBorderThickness, this.area.Bottom),
            P2 = new Vector2(this.area.Right + halfBorderThickness, this.area.Top - titleBarOffset),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };

        this.topLine = new Line
        {
            P1 = new Vector2(this.area.Right + BorderThickness, this.area.Top - halfBorderThickness),
            P2 = new Vector2(this.area.Left - BorderThickness, this.area.Top - halfBorderThickness),
            Color = this.borderClr,
            Thickness = BorderThickness,
        };
    }
}
