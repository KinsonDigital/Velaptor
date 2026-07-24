namespace UILib;

using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Carbonate;

public sealed class Slider : Control
{
    private const int HandleWidth = 16;
    private const int HandleHalfWidth = HandleWidth / 2;
    private readonly IDisposable subscription;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label label;
    private readonly Color sliderAreaClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color sliderHandleEnabledClr = Color.FromArgb(255, 45, 74, 117);
    private readonly Color valueTextDisabledColor = Color.FromArgb(255, 175, 175, 175);
    private readonly Color sliderDisabledClr;
    private RectShape sliderHandle;
    private RectShape sliderArea;
    private Vector2 handlePos;
    private bool isDragging;
    private bool mouseClickDisabled;
    private bool wasMouseDownLastFrame;
    private float max = 100;
    private float value;

    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    public Slider()
    {
        var disableMouseClickReactable1 = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = disableMouseClickReactable1.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) => this.mouseClickDisabled = data.IsExpanded,
            () => this.subscription.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.label = new Label();

        Width = 200;
        Height = 30;
        this.sliderDisabledClr = DisabledColor.IncreaseBrightness(0.5f);
    }

    public float Value
    {
        get => this.value;
        set
        {
            var oldValue = this.value;

            if (value < Min)
            {
                this.value = Min;
            }
            else if (value > Max)
            {
                this.value = Max;
            }
            else
            {
                this.value = value;
            }

            if (Math.Abs(this.value - oldValue) > 0.00001f)
            {
                this.ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, this.value));
            }
        }
    }

    public float Min { get; set; }

    public float Max
    {
        get => this.max;
        set
        {
            var screenPos = Position.ToWorld(Width, Height);

            var minLeft = this.sliderArea.Left + HandleHalfWidth;
            var maxRight = this.sliderArea.Right - HandleHalfWidth;

            var posX = value.MapValue(Min, value, minLeft, maxRight);
            posX = posX < minLeft ? minLeft : posX;

            this.handlePos = new Vector2(posX, screenPos.Y);

            this.max = value;
        }
    }

    public Color TextColor { get; set; } = Color.White;

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

        var screenPos = Position.ToWorld(Width, Height);

        this.sliderArea = new RectShape
        {
            Position = screenPos,
            Width = Width,
            Height = Height,
            Color = Enabled ? this.sliderAreaClr : DisabledColor,
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

        if (Enabled && mouseIsDown && this.isDragging && isInsideSlider)
        {
            this.handlePos = new Vector2(mousePos.X, screenPos.Y);

            this.handlePos.X = this.handlePos.X < this.sliderArea.Left + HandleHalfWidth
                ? this.sliderArea.Left + HandleHalfWidth
                : this.handlePos.X;

            this.handlePos.X = this.handlePos.X > this.sliderArea.Right - HandleHalfWidth
                ? this.sliderArea.Right - HandleHalfWidth
                : this.handlePos.X;

            var oldValue = Value;

            var newValue = CalcNewValue(this.handlePos.X);
            Value = newValue < Min
                ? (float)Math.Round(Min, 2)
                : newValue > Max
                    ? (float)Math.Round(Max, 2)
                    : (float)Math.Round(newValue, 2);

            this.ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, Value));
        }
        else
        {
            var posX = CalcNewPosX(Value);
            var posY = screenPos.Y;

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
            Color = Enabled ? this.sliderHandleEnabledClr : this.sliderDisabledClr,
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

        this.label.TextColor = Enabled ? TextColor : this.valueTextDisabledColor;

        // TODO: Only update the text property if the value has changed since the last frame
        this.label.Text = $"{Value:0.00}";
        this.label.Render(10);

        base.Render(layer);
    }

    private float CalcNewValue(float oldValue)
    {
        var minLeft = this.sliderArea.Left + HandleHalfWidth;
        var maxRight = this.sliderArea.Right - HandleHalfWidth;

        return oldValue.MapValue(minLeft, maxRight, Min, Max);
    }

    private float CalcNewPosX(float oldValueX)
    {
        var minLeft = this.sliderArea.Left + HandleHalfWidth;
        var maxRight = this.sliderArea.Right - HandleHalfWidth;

        var posX = oldValueX.MapValue(Min, Max, minLeft, maxRight);
        posX = posX < minLeft ? minLeft : posX;

        return posX;
    }
}
