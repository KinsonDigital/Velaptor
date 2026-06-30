using System.Drawing;
using System.Numerics;
using Carbonate.OneWay;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class DropDownItem : Control
{
    private readonly IPushReactable<DisableMouseSubscriptionData> disableMouseClickReactable;

    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private IFont? font;
    private RectShape background;

    public EventHandler<EventArgs>? Click;
    private MouseState prevMouseState;


    public DropDownItem()
    {
        this.disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();
        
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();
    }

    public string Text { get; set; }

    public Color BackgroundColor { get; set; } = Color.FromArgb(255, 17, 17, 17);

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

        var scrnPos = Position.ToWorld(Width, Height);

        this.background = new RectShape
        {
            Position = scrnPos,
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
                this.disableMouseClickReactable.Push(
                    SubscriptionIds.DisableMouseClickId,
                    new DisableMouseSubscriptionData { MouseDisabled = false });
            }
        }

        this.prevMouseState = currentMouseState; 

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        this.shapeRenderer.Render(this.background, 999);

        var renderPos = new Vector2(
            Position.X + (Width / 2f),
            Position.Y + (Height / 2f));

        this.fontRenderer.Render(this.font, Text, renderPos, Color.White, 1000);

        base.Render(layer);
    }
}