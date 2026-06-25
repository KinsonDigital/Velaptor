using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class CheckBox : Control
{
    private const int BoxTextPadding = 5;
    private const float MarkOffset = 2;
    private const float BoxWidthHeight = 20;
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private RectShape box;
    private Line mark1;
    private Line mark2;
    private Vector2 textPos;
    private MouseState prevMouseState;
    private string text = "Check box";
    private IFont font;

    public CheckBox()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();
    }

    public bool IsChecked { get; set; }

    public string Text
    {
        get => this.text;
        set => this.text = value ?? string.Empty;
    }

    public override void Load()
    {
        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);

        base.Load();
    }

    public override void Unload()
    {
        this.contentManager.Unload(this.font);

        base.Unload();
    }

    public override void Update()
    {
        var currentMouseState = this.mouse.GetState();

        var scrnPos = Position.ToScreen(BoxWidthHeight, BoxWidthHeight);

        this.box = new RectShape
        {
            Position = scrnPos,
            Width = BoxWidthHeight,
            Height = BoxWidthHeight,
            Color = Color.FromArgb(255, 32, 49, 72),
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

        var textSize = this.font.Measure(this.text);
        this.textPos = new Vector2(
            Position.X + this.box.Width + (textSize.Width / 2f) + BoxTextPadding,
            Position.Y + (textSize.Height / 2f) + 2);

        var mousePos = currentMouseState.GetPosition().ToVector2();
        var textRectPos = new Vector2(
            Position.X + this.box.Width + BoxTextPadding,
            Position.Y);
        var textRect = new Rectangle((int)textRectPos.X, (int)textRectPos.Y, (int)textSize.Width, (int)textSize.Height);
        var isMouseOver = this.box.Contains(mousePos) || textRect.Contains((int)mousePos.X, (int)mousePos.Y);

        // If the mouse if over any part of the checkbox and the left mouse button was just released
        if (isMouseOver && currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            IsChecked = !IsChecked;
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.box);

        if (IsChecked)
        {
            this.lineRenderer.Render(this.mark1);
            this.lineRenderer.Render(this.mark2);
        }

        this.fontRenderer.Render(this.font, Text, this.textPos, Color.White);

        base.Render();
    }

    private void LabelOn_Click(object? sender, EventArgs e)
    {
        IsChecked = !IsChecked;
    }
}
