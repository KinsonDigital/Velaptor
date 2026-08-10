namespace Velum;

using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public sealed class DropDownItem : Control
{
    private const int TopLayer = 1000;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private IFont? font;
    private RectShape background;
    private MouseState prevMouseState;

    public EventHandler<EventArgs>? Click;

    public DropDownItem()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();
    }

    public string Text { get; init; } = string.Empty;

    public Color BackgroundColor { get; set; } = Color.FromArgb(255, 17, 17, 17);

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);

        base.Load();
    }

    public override void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.contentManager.Unload(this.font);

        base.Unload();
    }

    public override void Update()
    {
        var currentMouseState = this.mouse.GetState();

        var screenPos = Position.ToWorld(Width, Height);

        this.background = new RectShape
        {
            Position = screenPos,
            Color = BackgroundColor,
            Width = Width,
            Height = Height,
            IsSolid = true
        };

        var mousePos = this.mouse.GetState().GetPosition().ToVector2();
        var isMouseOver = this.background.Contains(mousePos);

        if (isMouseOver)
        {
            if (currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
            {
                this.Click?.Invoke(this, EventArgs.Empty);
            }
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer)
    {
        this.shapeRenderer.Render(this.background, 999);

        var renderPos = new Vector2(
            Position.X + (Width / 2f),
            Position.Y + HalfHeight);

        this.fontRenderer.Render(this.font, Text, renderPos, Color.White, TopLayer);

        base.Render(layer);
    }
}
