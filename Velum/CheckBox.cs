namespace Velum;

using System.Drawing;
using System.Numerics;
using Carbonate;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public sealed class CheckBox : Control
{
    private const int BoxTextPadding = 5;
    private const float MarkOffset = 2;
    private const float BoxWidthHeight = 20;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly IDisposable subscription;
    private readonly Color markEnabledClr = Color.FromArgb(255, 89, 149, 224);
    private readonly Color markDisabledClr;
    private RectShape mainArea;
    private Line mark1;
    private Line mark2;
    private Vector2 textPos;
    private MouseState prevMouseState;
    private string text = "Check box";
    private IFont? font;
    private bool isChecked;
    private bool mouseClickDisabled;

    public event EventHandler<CheckChangedEventArgs>? CheckedChanged;

    public CheckBox()
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

        Height = (int)BoxWidthHeight;
        this.markDisabledClr = DisabledColor.IncreaseBrightness(0.5f);
    }

    public bool IsChecked
    {
        get => this.isChecked;
        set
        {
            this.isChecked = value;
            this.CheckedChanged?.Invoke(this, new CheckChangedEventArgs(value));
        }
    }

    public string Text
    {
        get => this.text;
        set => this.text = value;
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);
        var textWidth = this.font.Measure(this.text).Width;
        Width = (int)(BoxWidthHeight + BoxTextPadding + textWidth);

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

        var currentMouseState = this.mouse.GetState();

        var screenPos = Position.ToWorld(BoxWidthHeight, BoxWidthHeight);

        this.mainArea = new RectShape
        {
            Position = screenPos,
            Width = BoxWidthHeight,
            Height = BoxWidthHeight,
            Color = Color.FromArgb(255, 32, 49, 72),
            IsSolid = true,
        };

        var mark1StartX = Position.X + MarkOffset;
        var mark1StartY = Position.Y + MarkOffset;
        var mark1EndX = Position.X + (this.mainArea.Width - MarkOffset);
        var mark1EndY = Position.Y + (this.mainArea.Height - MarkOffset);

        this.mark1 = new Line
        {
            P1 = new Vector2(mark1StartX, mark1StartY),
            P2 = new Vector2(mark1EndX, mark1EndY),
            Color = Enabled ? this.markEnabledClr : this.markDisabledClr,
            Thickness = 3,
        };

        var mark2StartX = Position.X + (this.mainArea.Width - MarkOffset);
        var mark2StartY = Position.Y + MarkOffset;
        var mark2EndX = Position.X + MarkOffset;
        var mark2EndY = Position.Y + (this.mainArea.Height - MarkOffset);

        this.mark2 = new Line
        {
            P1 = new Vector2(mark2StartX, mark2StartY),
            P2 = new Vector2(mark2EndX, mark2EndY),
            Color = Enabled ? this.markEnabledClr : this.markDisabledClr,
            Thickness = 3,
        };

        var textSize = this.font.Measure(this.text);
        this.textPos = new Vector2(
            Position.X + this.mainArea.Width + (textSize.Width / 2f) + BoxTextPadding,
            Position.Y + (textSize.Height / 2f) + 2);

        var mousePos = currentMouseState.GetPosition().ToVector2();
        var textRectPos = new Vector2(
            Position.X + this.mainArea.Width + BoxTextPadding,
            Position.Y);
        var textRect = new Rectangle((int)textRectPos.X, (int)textRectPos.Y, (int)textSize.Width, (int)textSize.Height);
        var isMouseOver = this.mainArea.Contains(mousePos) || textRect.Contains((int)mousePos.X, (int)mousePos.Y);
        var currentLeftBtnUp = currentMouseState.IsButtonUp(MouseButton.LeftButton);
        var prevLeftBtnDown = this.prevMouseState.IsButtonDown(MouseButton.LeftButton);

        // If the mouse is over any part of the checkbox and the left mouse button was just released
        if (Enabled && isMouseOver && !this.mouseClickDisabled && currentLeftBtnUp && prevLeftBtnDown)
        {
            this.isChecked = !this.isChecked;
            this.CheckedChanged?.Invoke(this, new CheckChangedEventArgs(this.isChecked));
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        if (this.font is null)
        {
            throw new InvalidOperationException($"The font object cannot be null. Could not render the '{nameof(CheckBox)}'.");
        }

        if (!Visible)
        {
            return;
        }

        if (!IsLoaded)
        {
            throw new InvalidOperationException($"The '{nameof(CheckBox)}' must be loaded before it can be rendered.");
        }

        this.shapeRenderer.Render(this.mainArea);

        if (this.isChecked)
        {
            this.shapeRenderer.Render(this.mark1);
            this.shapeRenderer.Render(this.mark2);
        }

        this.fontRenderer.Render(this.font, Text, this.textPos, Enabled ? Color.White : DisabledColor);

        base.Render(0);
    }
}
