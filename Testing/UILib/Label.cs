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

        Width = 200;
        Height = 30;
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
            }
        }
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
        var currentMouseState = this.mouse.GetState();

        this.background = new RectShape
        {
            Position = Position,
            Width = Width,
            Height = Height,
            Color = BackgroundColor,
            IsSolid = true,
        };

        var mousePos = currentMouseState.GetPosition();
        var mousePosVector = new Vector2(mousePos.X, mousePos.Y);
        IsMouseOver = this.background.Contains(mousePosVector);

        if (IsMouseOver && currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
        {
            this.Click?.Invoke(this, new LabelClickEventArgs(this));
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render()
    {
        if (BackgroundColor != Color.Transparent)
        {
            this.shapeRenderer.Render(this.background, -10);
        }

        var textPos = new Vector2(Position.X, Position.Y);
        this.fontRenderer.Render(this.font, Text, textPos, Color.White);

        base.Render();
    }
}
