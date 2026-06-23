using System.Drawing;
using System.Numerics;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class CheckBox : Control
{
    private const float MarkOffset = 2;
    private const float BoxWidthHeight = 20;
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label label;
    private RectShape box;
    private Line mark1;
    private Line mark2;
    private MouseState prevMouseState;
    private string text = "Check box";

    public CheckBox()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.label = new Label();
        this.label.Click += LabelOn_Click;
        this.label.Text = "Check box";
    }

    public bool IsChecked { get; set; }

    public string Text
    {
        get => this.text;
        set => this.text = value ?? string.Empty;
    }

    public override void Load()
    {
        this.label.Load();

        base.Load();
    }

    public override void Unload()
    {
        this.label.Click -= LabelOn_Click;
        this.label.Unload();

        base.Unload();
    }

    public override void Update()
    {
        var scrnPos = Position.ToScreen(BoxWidthHeight, BoxWidthHeight);

        this.box = new RectShape
        {
            Position = scrnPos,
            Width = BoxWidthHeight,
            Height = BoxWidthHeight,
            Color = Color.Yellow,// Color.FromArgb(255, 32, 49, 72),
            IsSolid = true,
        };

        var mark1StartX = Position.X + MarkOffset;
        var mark1StartY = Position.Y + MarkOffset;
        var mark1EndX = Position.X + (this.box.Width - MarkOffset);
        var mark1EndY = Position.Y + (this.box.Height - MarkOffset);

        this.mark1 = new Line
        {
            P1 = new Vector2(mark1StartX, mark1StartY),
            P2 = new Vector2(mark1EndX, mark1EndY),
            Color = Color.FromArgb(255, 89, 149, 224),
            Thickness = 3,
        };

        var mark2StartX = Position.X + (this.box.Width - MarkOffset);
        var mark2StartY = Position.Y + MarkOffset;
        var mark2EndX = Position.X + MarkOffset;
        var mark2EndY = Position.Y + (this.box.Height - MarkOffset);

        this.mark2 = new Line
        {
            P1 = new Vector2(mark2StartX, mark2StartY),
            P2 = new Vector2(mark2EndX, mark2EndY),
            Color = Color.FromArgb(255, 89, 149, 224),
            Thickness = 3,
        };

        this.label.Position = new Vector2(Position.X + this.box.Width + (this.label.TextSize.Width / 2f) + 5, Position.Y + (this.label.TextSize.Height / 2f));
        this.label.Update();

        ProcessCheckState();

        base.Update();
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.box);

        if (IsChecked)
        {
            var mark1Temp = this.mark1 with
            {
                P1 = this.mark1.P1.ToScreen(this.box.HalfWidth + MarkOffset, this.box.HalfHeight + MarkOffset),
                P2 = this.mark1.P2.ToScreen(this.box.HalfWidth - MarkOffset, this.box.HalfHeight - MarkOffset)
            };

            var mark2Temp = this.mark2 with
            {
                P1 = this.mark2.P1.ToScreen(this.box.HalfWidth - MarkOffset, this.box.HalfHeight + MarkOffset),
                P2 = this.mark2.P2.ToScreen(this.box.HalfWidth + MarkOffset, this.box.HalfHeight - MarkOffset)
            };

            this.lineRenderer.Render(this.mark1);
            this.lineRenderer.Render(this.mark2);
        }

        this.label.Render();

        base.Render();
    }

    private void ProcessCheckState()
    {
        var currentMouseState = this.mouse.GetState();

        if (currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            var mousePos = currentMouseState.GetPosition();
            var mousePosVector = new Vector2(mousePos.X, mousePos.Y);

            var isOverCheck = this.box.Contains(mousePosVector);

            // If the mouse is over the checkbox
            if (isOverCheck)
            {
                IsChecked = !IsChecked;
            }
        }

        this.prevMouseState = currentMouseState;
    }

    private void LabelOn_Click(object? sender, EventArgs e)
    {
        IsChecked = !IsChecked;
    }
}