using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Option : Control
{
    private const int BoxTextPadding = 5;
    private const float BoxWidthHeight = 20;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private CircleShape circle;
    private Vector2 textPos;
    private MouseState prevMouseState;
    private readonly Color OptionColor = Color.FromArgb(255, 89, 149, 224);
    private string text = "Check box";
    private IFont font;

    public Option()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();

        Height = (int)BoxWidthHeight;
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
        var textWidth = this.font.Measure(this.text).Width;
        Width = (int)(BoxWidthHeight + BoxTextPadding + textWidth);

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

        this.circle = new CircleShape
        {
            Position = scrnPos,
            Diameter = BoxWidthHeight,
        };

        var textSize = this.font.Measure(this.text);
        this.textPos = new Vector2(
            Position.X + this.circle.Diameter + (textSize.Width / 2f) + BoxTextPadding,
            Position.Y + (textSize.Height / 2f) + 2);

        var mousePos = currentMouseState.GetPosition().ToVector2();
        var textRectPos = new Vector2(
            Position.X + this.circle.Diameter + BoxTextPadding,
            Position.Y);
        var textRect = new Rectangle((int)textRectPos.X, (int)textRectPos.Y, (int)textSize.Width, (int)textSize.Height);
        var isMouseOver = this.circle.Contains(mousePos) || textRect.Contains((int)mousePos.X, (int)mousePos.Y);

        // If the mouse if over any part of the checkbox and the left mouse button was just released
        if (isMouseOver && currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            IsChecked = !IsChecked;
        }

        this.circle.IsSolid = IsChecked;
        this.circle.Color = Color.FromArgb(255, 89, 149, 224);

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.circle);
        this.fontRenderer.Render(this.font, Text, this.textPos, Color.White);

        base.Render();
    }

    private void LabelOn_Click(object? sender, EventArgs e)
    {
        IsChecked = !IsChecked;
    }
}