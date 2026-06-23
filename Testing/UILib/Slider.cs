using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Slider : Control
{
    private const int HandleWidth = 16;
    private const int HandleHalfWidth = HandleWidth / 2;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly Label label;
    private RectShape sliderHandle;
    private RectShape sliderArea;
    private Vector2 handlePos;

    public Slider()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.mouse = HardwareFactory.GetMouse();
        this.keyboard = HardwareFactory.GetKeyboard();
        this.label = new Label();

        Width = 200;
        Height = 30;
    }

    public float Value { get; set; } = 50f;

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
        var scrnPos = Position.ToScreen(Width, Height);

        this.sliderArea = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 35, 48, 70),
            IsSolid = true,
        };

        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition();
        var mousePosVector = new Vector2(mousePos.X, mousePos.Y);
        var mouseIsDown = currentMouseState.IsButtonDown(MouseButton.LeftButton);

        // If the mouse position is inside of the slider area
        if (mouseIsDown && this.sliderArea.Contains(mousePosVector))
        {
            this.handlePos = new Vector2(mousePos.X, scrnPos.Y);

            this.handlePos.X = this.handlePos.X < this.sliderArea.Left + HandleHalfWidth
                ? this.sliderArea.Left + HandleHalfWidth
                : this.handlePos.X;

            this.handlePos.X = this.handlePos.X > this.sliderArea.Right - HandleHalfWidth
                ? this.sliderArea.Right - HandleHalfWidth
                : this.handlePos.X;

            var newValue = CalcNewValue(this.handlePos.X);
            Value = newValue < Min
                ? (float)Math.Round((float)Min, 2)
                : newValue > Max
                    ? (float)Math.Round((float)Max, 2)
                    : (float)Math.Round((float)newValue, 2);
        }
        else
        {
            var posX = CalcNewPosX(Value);
            var posY = scrnPos.Y;

            this.handlePos = new Vector2(posX, posY);
        }

        this.sliderHandle = new RectShape
        {
            Position = this.handlePos,
            Width = HandleWidth,
            Height = Height,
            Color = Color.FromArgb(255, 45, 74, 117),
            IsSolid = true,
        };

        this.label.Position = new Vector2(scrnPos.X, scrnPos.Y);

        base.Update();
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.sliderArea, -10);

        this.label.Text = $"{Value:0.00}";
        this.label.Render();

        this.shapeRenderer.Render(this.sliderHandle);

        base.Render();
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
