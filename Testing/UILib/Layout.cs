namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;

public class Layout : Control
{
    private const float BorderThickness = 3f;
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly List<IControl> controls = new();
    private readonly Color borderClr = Color.FromArgb(255, 45, 74, 117);
    private RectShape area;
    private Vector2 logicalPosition;
    private Line leftLine;
    private Line rightLine;
    private Line topLine;
    private Line bottomLine;
    private float baseAreaTop;

    public Layout()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();

        this.area = new RectShape
        {
            Width = 300,
            Height = 270,
            Color = Color.FromArgb(255, 17, 17, 17),
            IsSolid = true,
        };

        this.baseAreaTop = this.logicalPosition.Y;
    }

    public string Title { get; set; }

    public override float Width => this.area.Width;

    public override float Height
    {
        get
        {
            var borderHeight = DebugBorderVisible ? BorderThickness : 0f;

            return this.area.Height + (borderHeight * 2f);
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

    public bool DebugBorderVisible { get; set; }

    public int AreaPadding { get; set; }

    public int HorizontalSpacing { get; set; } = 10;

    public int VerticalSpacing { get; set; } = 10;

    public StackDirection StackDirection { get; set; } = StackDirection.Vertical;

    public bool Centered { get; set; } = false;

    public override bool Enabled
    {
        get => base.Enabled;
        set
        {
            foreach (var ctrl in this.controls)
            {
                ctrl.Enabled = value;
            }

            base.Enabled = value;
        }
    }

    public void AddControl(IControl control)
    {
        ArgumentNullException.ThrowIfNull(control);

        // As long as the control is not a layout control, match the enabled and visible state
        if (control is not Layout)
        {
            control.Enabled = Enabled;
            control.Visible = Visible;
        }

        this.controls.Add(control);
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        foreach (var control in this.controls)
        {
            control.Load();
        }

        base.Load();
    }

    public override void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        foreach (var control in this.controls)
        {
            control.Unload();
        }

        base.Unload();
    }

    public override void Update()
    {
        if (!Visible)
        {
            return;
        }

        // Resolve area center position from logical (top-left) coordinates + current dimensions.
        // Deferred here so that Width/Height set after Position still produce correct results.
        this.area.Position = this.logicalPosition.ToWorld(this.area.Width, this.area.Height);
        this.area.Top = this.baseAreaTop;

        ProcessLayout();
        ProcessBorder();
    }

    public override void Render(int layer = 0)
    {
        if (!Visible)
        {
            return;
        }

        this.shapeRenderer.Render(this.area, -100);

        // TODO: Add debug preprocess directive to only be taken into account internally in this control if in debug mode
        if (DebugBorderVisible)
        {
            this.lineRenderer.Render(this.leftLine, -100);
            this.lineRenderer.Render(this.bottomLine, -100);
            this.lineRenderer.Render(this.rightLine, -100);
            this.lineRenderer.Render(this.topLine, -100);
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

        // Update all of the controls
        for (var i = 0; i < this.controls.Count; i++)
        {
            var isFirstItem = i == 0;
            var control = this.controls[i];

            if (!control.Visible)
            {
                continue;
            }

            var overlapOffset = control is Label ? 1 : 0;
            var centeredOffset = 0f;

            switch (StackDirection)
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
                        var posY = Centered ? centeredOffset : AreaPadding + overlapOffset;
                        control.Position = new Vector2(AreaPadding, posY);

                        // Take centering into account
                        control.Position += layoutOrigin;
                    }
                    else
                    {
                        var posY = Centered ? centeredOffset : AreaPadding + overlapOffset;

                        var prevCtrl = this.controls[i - 1];
                        control.Position = new Vector2(
                            prevCtrl.Right + HorizontalSpacing + overlapOffset,
                            layoutOrigin.Y + posY);
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
                        var posX = Centered ? centeredOffset : AreaPadding + overlapOffset;
                        control.Position = new Vector2(posX, AreaPadding);

                        // Take centering into account
                        control.Position += layoutOrigin;
                    }
                    else
                    {
                        var posX = Centered ? centeredOffset : AreaPadding + overlapOffset;

                        var prevCtrl = this.controls[i - 1];
                        control.Position = new Vector2(
                            layoutOrigin.X + posX,
                            prevCtrl.Bottom + VerticalSpacing + overlapOffset);
                    }

                    break;
            }

            control.Update();
        }

        if (this.controls.Count <= 0)
        {
            return;
        }

        switch (StackDirection)
        {
            case StackDirection.Horizontal:
                var horizontalPaddingEachSide = AreaPadding * 2;
                var totalHorizontalSpacing = (this.controls.Count - 1) * HorizontalSpacing;
                var totalWidth = this.controls.Count <= 0 ? 0 : this.controls.Sum(c => c.Width);
                var maxHeight = this.controls.Count <= 0 ? 0 : this.controls.Max(c => c.Height);

                this.area.Width = totalWidth + (horizontalPaddingEachSide + totalHorizontalSpacing);
                this.area.Height = maxHeight + horizontalPaddingEachSide;
                break;
            case StackDirection.Vertical:
                var verticalPaddingEachSide = AreaPadding * 2;
                var totalVerticalSpacing = (this.controls.Count - 1) * VerticalSpacing;

                var maxWidth = this.controls.Count <= 0 ? 0 : this.controls.Max(c => c.Width);
                var totalHeight = this.controls.Count <= 0 ? 0 : this.controls.Sum(c => c.Height);

                this.area.Width = maxWidth + verticalPaddingEachSide;
                this.area.Height = totalHeight + (verticalPaddingEachSide + totalVerticalSpacing);
                break;
        }
    }

    private void ProcessBorder()
    {
        if (!DebugBorderVisible)
        {
            return;
        }

        // TODO: Instead of having the border render internally, render it externally from the internal area.
        // This means that the width and height will have to be calculated by adding the half thickness of
        // the border as long as the border is set to visible.  This is to prevent half of the border being rendered
        // internally and overlapping any of the edges of controls when rendering

        const float halfBorderThickness = BorderThickness / 2f;

        this.leftLine = new Line
        {
            // P1 = new Vector2(this.area.Left - halfBorderThickness, this.area.Top),
            // P2 = new Vector2(this.area.Left - halfBorderThickness, this.area.Bottom),
            P1 = new Vector2(this.area.Left, this.area.Top),
            P2 = new Vector2(this.area.Left, this.area.Bottom),
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
            P2 = new Vector2(this.area.Right + halfBorderThickness, this.area.Top),
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
