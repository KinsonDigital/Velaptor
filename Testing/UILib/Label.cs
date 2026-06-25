using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Label : Control
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private RectShape background;
    private MouseState prevMouseState;
    private IFont? font;
    private string text = string.Empty;
    private bool isLoaded;
    private bool deferLoadingItems = true;

    public EventHandler<LabelClickEventArgs>? Click;

    public Label()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();
    }

    public string Text
    {
        get => this.text;
        set
        {
            this.text = value ?? string.Empty;

            if (this.font is not null)
            {
                TextSize = this.font.Measure(this.text);
                Width = (int)TextSize.Width;
                Height = (int)TextSize.Height;
            }
        }
    }

    public override int Width
    {
        get => base.Width;
        set => base.Width = value >= TextSize.Width ? value : (int)TextSize.Width;
    }

    public override int Height
    {
        get => base.Height;
        set => base.Height = value >= TextSize.Height ? value : (int)TextSize.Height;
    }

    public SizeF TextSize { get; private set; }

    public Color BackgroundColor { get; set; } = Color.Transparent;

    public bool IsMouseOver { get; private set; }

    public override void Load()
    {
        if (this.isLoaded)
        {
            return;
        }

        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);
        TextSize = this.font.Measure(this.text);
        Width = (int)TextSize.Width;
        Height = (int)TextSize.Height;

        this.isLoaded = true;

        base.Load();
    }

    public override void Unload()
    {
        this.contentManager.Unload(this.font);

        base.Unload();
    }

    public override void Update()
    {
        var scrnPos = Position.ToScreen(Width, Height);

        var currentMouseState = this.mouse.GetState();

        this.background = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = BackgroundColor,
            IsSolid = true,
        };

        var mousePos = currentMouseState.GetPosition().ToVector2();

        var labelRect = new Rectangle((int)scrnPos.X, (int)scrnPos.Y, (int)TextSize.Width, (int)TextSize.Height);
        IsMouseOver = labelRect.Contains((int)mousePos.X, (int)mousePos.Y);

        if (IsMouseOver && currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            // TODO: DEBUG - REMOVE THIS
            if (BackgroundColor == Color.Transparent)
            {
                BackgroundColor = Color.CornflowerBlue;
            }
            else if (BackgroundColor == Color.CornflowerBlue)
            {
                BackgroundColor = Color.Transparent;
            }

            this.Click?.Invoke(this, new LabelClickEventArgs(this));
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        var scrnPos = Position.ToScreen(TextSize.Width, TextSize.Height);
        scrnPos.Y += 1; // Slightly offset the text to ensure the top of the text is not past the top of the label's rectangle area

        if (BackgroundColor != Color.Transparent)
        {
            this.shapeRenderer.Render(this.background, -10);
        }

        this.fontRenderer.Render(this.font, Text, scrnPos, Color.White, layer);

        base.Render();
    }
}
