namespace UILib;

using System.Drawing;
using System.Numerics;
using Carbonate;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public sealed class Button : Control
{
    private readonly IDisposable subscription;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label label;
    private RectShape face;
    private MouseState prevMouseState;
    private readonly Color faceClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color textDisabledColor = Color.FromArgb(255, 175, 175, 175);
    private readonly Color faceMouseDownClr;
    private readonly Color faceHoverClr;
    private bool mouseClickDisabled;

    public EventHandler<EventArgs>? Click;

    public Button()
    {
        var disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = disableMouseClickReactable.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) => this.mouseClickDisabled = data.IsExpanded,
            () => this.subscription.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();

        this.label = new Label();
        this.label.Text = "Button";

        Width = 200;
        Height = 30;

        this.faceMouseDownClr = this.faceClr.IncreaseBrightness(0.4f);
        this.faceHoverClr = this.faceClr.IncreaseBrightness(0.2f);
    }

    public string Text
    {
        get => this.label.Text;
        set => this.label.Text = value;
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.label.Load();

        base.Load();
    }

    public override void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.label.Unload();

        base.Unload();
    }

    public override void Update()
    {
        if (!Visible)
        {
            return;
        }

        var screenPos = Position.ToWorld(Width, Height);

        this.face = new RectShape
        {
            Position = screenPos,
            Width = Width,
            Height = Height,
            Color = Enabled ? this.faceClr : DisabledColor,
            IsSolid = true,
        };

        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition().ToVector2();
        var mousePosVector = new Vector2(mousePos.X, mousePos.Y);
        var mouseIsOver = this.face.Contains(mousePosVector);

        // If the mouse position is inside the slider area
        if (Enabled && mouseIsOver)
        {
            var mouseIsDown = currentMouseState.IsButtonDown(MouseButton.LeftButton);

            this.face.Color = mouseIsDown ? this.faceMouseDownClr : this.faceHoverClr;

            var currentLeftBtnUp = currentMouseState.IsButtonUp(MouseButton.LeftButton);
            var prevLeftBtnDown = this.prevMouseState.IsButtonDown(MouseButton.LeftButton);

            if (!this.mouseClickDisabled && prevLeftBtnDown && currentLeftBtnUp)
            {
                this.Click?.Invoke(this, EventArgs.Empty);
            }
        }

        this.label.TextColor = Enabled ? Color.White : this.textDisabledColor;
        this.label.Position = new Vector2(
            Position.X + ((Width / 2f) - (this.label.TextSize.Width / 2f)),
            Position.Y + ((Height / 2f) - (this.label.TextSize.Height / 2f)));

        this.label.Update();

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer)
    {
        if (!Visible)
        {
            return;
        }

        this.shapeRenderer.Render(this.face, -10);

        this.label.Text = Text;
        this.label.Render();

        base.Render(0);
    }
}
