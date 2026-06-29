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
    private Vector2 logicalPosition;
    private MouseState prevMouseState;
    private Vector2 lastMousePos;
    private Line leftLine;
    private Line rightLine;
    private Line topLine;
    private Line bottomLine;
    private bool isDragging;
    private float baseAreaTop;
    private int verticalSpacing = 10;
    private int horizontalSpacing = 10;

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

        this.baseAreaTop = this.logicalPosition.Y;
    }

    public string Title { get; set; }

    public override float Width
    {
        get => this.area.Width;
        set
        {
            if (AutoSize)
            {
                return;
            }

            this.area.Width = value;
        }
    }

    public override float Height
    {
        get
        {
            var borderHeight = BorderVisible ? BorderThickness : 0f;

            return TitleBarVisible
                ? TitleBarHeight + this.area.Height + borderHeight
                : this.area.Height + (borderHeight * 2f);
        }
        set
        {
            if (AutoSize)
            {
                return;
            }

            var borderHeight = BorderVisible ? BorderThickness : 0f;

            if (TitleBarVisible)
            {
                this.area.Height = value - TitleBarHeight - borderHeight;
            }
            else
            {
                this.area.Height = value - (borderHeight * 2f);
            }
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

    public bool AutoSize { get; set; } = false;

    public bool TitleBarVisible { get; set; } = true;

    public bool BorderVisible { get; set; } = true;

    public bool Draggable { get; set; }

    public int AreaPadding { get; set; } = 10;

    public int HorizontalSpacing
    {
        get => this.horizontalSpacing;
        set => this.horizontalSpacing = value;
    }

    public int VerticalSpacing
    {
        get => this.verticalSpacing;
        set => this.verticalSpacing = value;
    }

    public Layout Layout { get; set; } = new Layout { LayoutGroup = 0, StackDirection = StackDirection.Vertical };

    public bool Centered { get; set; } = false;

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
        // Process drag first — may change logicalPosition/baseAreaTop
        ProcessDragState();

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

            if (!TitleBarVisible)
            {
                this.lineRenderer.Render(this.topLine, -100);
            }
        }

        // Render all of the controls
        foreach (var control in this.controls)
        {
            control.Render();
        }
    }

    private void ProcessLayout()
    {
        var layoutOrigin = new Vector2(this.area.Left, this.area.Top);

        var maxHeight = this.controls.Max(c => c.Height);


        // Update all of the controls
        for (var i = 0; i < this.controls.Count; i++)
        {
            var isFirstItem = i == 0;
            var control = this.controls[i];
            var overlapOffset = control is Label ? 1 : 0;
            var centeredOffset = 0f;

            switch (Layout.StackDirection)
            {
                case StackDirection.Horizontal:
                    if (Centered)
                    {
                        centeredOffset = this.area.HalfHeight - (control.Height / 2f);
                    }
                    else
                    {
                        centeredOffset = 0f;
                    }

                    if (isFirstItem)
                    {
                        control.Position = new Vector2(AreaPadding + overlapOffset, AreaPadding);

                        // Take centering into account
                        control.Position = new Vector2(control.Position.X, centeredOffset);
                        control.Position += layoutOrigin;
                    }
                    else
                    {
                        var prevControl = this.controls[i - 1];
                        control.Position = new Vector2(
                            prevControl.Right + this.horizontalSpacing + overlapOffset,
                            layoutOrigin.Y + AreaPadding + centeredOffset);
                    }

                    break;
                case StackDirection.Vertical:
                    if (Centered)
                    {
                        centeredOffset = this.area.HalfWidth - (control.Width / 2f);
                    }
                    else
                    {
                        centeredOffset = 0f;
                    }

                    if (isFirstItem)
                    {
                        control.Position = new Vector2(AreaPadding + overlapOffset, AreaPadding);

                        // Take centering into account
                        control.Position = new Vector2(centeredOffset, control.Position.Y);
                        control.Position += layoutOrigin;
                    }
                    else
                    {
                        var prevControl = this.controls[i - 1];
                        control.Position = new Vector2(
                            layoutOrigin.X + AreaPadding + overlapOffset + centeredOffset,
                            prevControl.Bottom + this.verticalSpacing + overlapOffset);
                    }

                    break;
            }

            control.Update();
        }

        if (AutoSize)
        {
            AutoSize = false;

            switch (Layout.StackDirection)
            {
                case StackDirection.Horizontal:
                    var horizontalPaddingEachSide = AreaPadding * 2;
                    this.area.Width = this.controls.Sum(c => c.Width) + (horizontalPaddingEachSide + this.horizontalSpacing);
                    this.area.Height = this.controls.Max(c => c.Height) + horizontalPaddingEachSide;
                    break;
                case StackDirection.Vertical:
                    var verticalPaddingEachSide = AreaPadding * 2;
                    this.area.Width = this.controls.Max(c => c.Width) + verticalPaddingEachSide;
                    this.area.Height = this.controls.Sum(c => c.Height) + (verticalPaddingEachSide + this.verticalSpacing);
                    break;
            }

            AutoSize = true;
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
