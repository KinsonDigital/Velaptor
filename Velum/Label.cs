namespace Velum;

using System.Drawing;
using System.Numerics;
using Carbonate;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public sealed class Label : Control
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly IDisposable subscription;
    private RectShape background;
    private MouseState prevMouseState;
    private IFont? font;
    private string text = string.Empty;
    private bool mouseClickDisabled;

    public EventHandler<LabelClickEventArgs>? Click;

    public Label()
    {
        var disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = disableMouseClickReactable.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) => this.mouseClickDisabled = data.IsExpanded,
            () => this.subscription?.Dispose()
        );

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
            this.text = value;

            if (this.font is null)
            {
                return;
            }

            TextSize = this.font.Measure(this.text);
            Width = (int)TextSize.Width;
            Height = (int)TextSize.Height;
        }
    }

    public override Vector2 Position
    {
        get => new Vector2(base.Position.X, base.Position.Y);
        set => base.Position = new Vector2(value.X, value.Y);
    }

    public override float Width
    {
        get => base.Width;
        set => base.Width = value >= TextSize.Width ? value : TextSize.Width;
    }

    public override float Height
    {
        get => base.Height + 1;
        set => base.Height = value >= TextSize.Height ? value : TextSize.Height;
    }

    public SizeF TextSize { get; private set; }

    public Color TextColor { get; set; } = Color.White;

    public Color BackgroundColor { get; set; } = Color.Transparent;

    public bool IsMouseOver { get; private set; }

    public override bool Enabled
    {
        get => base.Enabled;
        set
        {
            if (value)
            {

            }
            else
            {
            }

            base.Enabled = value;
        }
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);
        TextSize = this.font.Measure(this.text);
        Width = (int)TextSize.Width;
        Height = (int)TextSize.Height;

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
        if (!Visible)
        {
            return;
        }

        var screenPos = Position.ToWorld(Width, Height);

        var currentMouseState = this.mouse.GetState();

        this.background = new RectShape
        {
            Position = screenPos,
            Width = Width,
            Height = Height,
            Color = BackgroundColor,
            IsSolid = true,
        };

        var mousePos = currentMouseState.GetPosition().ToVector2();

        var labelRect = new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)TextSize.Width, (int)TextSize.Height);
        IsMouseOver = labelRect.Contains((int)mousePos.X, (int)mousePos.Y);
        var currentLeftBtnUp = currentMouseState.IsButtonUp(MouseButton.LeftButton);
        var prevLeftBtnDown = this.prevMouseState.IsButtonDown(MouseButton.LeftButton);

        if (IsMouseOver && !this.mouseClickDisabled && currentLeftBtnUp && prevLeftBtnDown)
        {
            this.Click?.Invoke(this, new LabelClickEventArgs(this));
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        if (this.font is null)
        {
            throw new InvalidOperationException($"The '{nameof(this.font)}' cannot be null. Could not render the '{nameof(Label)}' control.");
        }

        if (!Visible)
        {
            return;
        }

        if (!IsLoaded)
        {
            throw new InvalidOperationException($"The '{nameof(Label)}' must be loaded before it can be rendered.");
        }

        var screenPos = Position.ToWorld(TextSize.Width, TextSize.Height);
        screenPos.Y += 1; // Slightly offset the text to ensure the top of the text is not past the top of the label's rectangle area

        if (BackgroundColor != Color.Transparent)
        {
            this.shapeRenderer.Render(this.background, -10);
        }

        this.fontRenderer.Render(this.font, Text, screenPos, Enabled ? TextColor : DisabledColor, layer);

        base.Render(0);
    }
}
