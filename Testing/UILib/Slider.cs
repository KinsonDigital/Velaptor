using System.Drawing;
using System.Numerics;
using Carbonate.NonDirectional;
using Carbonate.Fluent;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Carbonate;
using Carbonate.OneWay;

public class Slider : Control
{
    private const int HandleWidth = 16;
    private const int HandleHalfWidth = HandleWidth / 2;
    private readonly IPushReactable<DisableMouseSubscriptionData> disableMouseClickReactable;
    private readonly IDisposable subscription;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label label;
    private RectShape sliderHandle;
    private RectShape sliderArea;
    private Vector2 handlePos;
    private bool isDragging;
    private bool mouseClickDisabled;
    private bool wasMouseDownLastFrame;

    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    public Slider()
    {
        this.disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = this.disableMouseClickReactable.CreateOneWayReceive(
            SubscriptionIds.DisableMouseClickId,
            nameof(SubscriptionIds.DisableMouseClickId),
            (data) => this.mouseClickDisabled = data.MouseDisabled,
            () => this.subscription.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.label = new Label();

        Width = 200;
        Height = 30;
    }

    public float Value { get; set; }

    public float Min { get; set; } = 0f;

    public float Max { get; set; } = 100f;

    public override void Load()
    {
        this.label.Load();

        base.Load();
    }

    public override void Unload()
    {
        this.label.Unload();

        base.Unload();
    }

    public override void Update()
    {
        if (!Visible)
        {
            return;
        }

        var scrnPos = Position.ToWorld(Width, Height);

        this.sliderArea = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 35, 48, 70),
            IsSolid = true,
        };

        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition().ToVector2();
        var mousePosVector = new Vector2(mousePos.X, mousePos.Y);
        var mouseIsDown = !this.mouseClickDisabled && currentMouseState.IsButtonDown(MouseButton.LeftButton);
        var isInsideSlider = this.sliderArea.Contains(mousePosVector);

        if (mouseIsDown)
        {
            // Only start dragging if the mouse was first pressed down inside the slider area
            if (!this.isDragging && !this.wasMouseDownLastFrame && isInsideSlider)
            {
                this.isDragging = true;
            }
        }

        if (mouseIsDown && this.isDragging && isInsideSlider)
        {
            this.handlePos = new Vector2(mousePos.X, scrnPos.Y);

            this.handlePos.X = this.handlePos.X < this.sliderArea.Left + HandleHalfWidth
                ? this.sliderArea.Left + HandleHalfWidth
                : this.handlePos.X;

            this.handlePos.X = this.handlePos.X > this.sliderArea.Right - HandleHalfWidth
                ? this.sliderArea.Right - HandleHalfWidth
                : this.handlePos.X;

            var oldValue = Value;

            var newValue = CalcNewValue(this.handlePos.X);
            Value = newValue < Min
                ? (float)Math.Round((float)Min, 2)
                : newValue > Max
                    ? (float)Math.Round((float)Max, 2)
                    : (float)Math.Round((float)newValue, 2);

            ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, Value));
        }
        else
        {
            var posX = CalcNewPosX(Value);
            var posY = scrnPos.Y;

            this.handlePos = new Vector2(posX, posY);
        }

        if (!mouseIsDown)
        {
            this.isDragging = false;
        }

        this.wasMouseDownLastFrame = mouseIsDown;

        this.sliderHandle = new RectShape
        {
            Position = this.handlePos,
            Width = HandleWidth,
            Height = Height,
            Color = Color.FromArgb(255, 45, 74, 117),
            IsSolid = true,
        };

        this.label.Position = new Vector2(
            Position.X + ((Width / 2f) - (this.label.TextSize.Width / 2f)),
            Position.Y + ((Height / 2f) - (this.label.TextSize.Height / 2f)));

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        if (!Visible)
        {
            return;
        }

        this.shapeRenderer.Render(this.sliderArea, -10);

        this.shapeRenderer.Render(this.sliderHandle);

        // TODO: Only update the text property if the value has changed since the last frame
        this.label.Text = $"{Value:0.00}";
        this.label.Render(10);

        base.Render(layer);
    }

    private float CalcNewValue(float value)
    {
        var minLeft = this.sliderArea.Left + HandleHalfWidth;
        var maxRight = this.sliderArea.Right - HandleHalfWidth;

        return value.MapValue(minLeft, maxRight, Min, Max);
    }

    private float CalcNewPosX(float value)
    {
        var minLeft = this.sliderArea.Left + HandleHalfWidth;
        var maxRight = this.sliderArea.Right - HandleHalfWidth;

        var posX = value.MapValue(Min, Max, minLeft, maxRight);
        posX = posX < minLeft ? minLeft : posX;

        return posX;
    }
}
