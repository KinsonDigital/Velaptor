using System;
using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class ArrowButton : Control
{
    private float PaddingRatio = 0.5f;
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IAppInput<MouseState> mouse;
    private RectShape face;
    private MouseState prevMouseState;

    public EventHandler<EventArgs>? Click;

    public ArrowButton()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.mouse = HardwareFactory.GetMouse();

        Width = 30;
        Height = 30;
    }

    public override void Update()
    {
        PaddingRatio = 0.50f;

        var scrnPos = Position.ToScreen(Width, Height);

        this.face = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = Color.FromArgb(255, 41, 72, 109),
            IsSolid = true,
        };

        var currentMouseState = this.mouse.GetState();
        var mousePos = currentMouseState.GetPosition();
        var mousePosVector = new Vector2(mousePos.X, mousePos.Y);
        var mouseIsOver = this.face.Contains(mousePosVector);

        if (mouseIsOver)
        {
            var mouseIsDown = currentMouseState.IsButtonDown(MouseButton.LeftButton);

            if (mouseIsDown)
            {
                this.face.Color = this.face.Color.IncreaseBrightness(0.4f);
            }

            this.face.Color = this.face.Color.IncreaseBrightness(0.2f);

            if (this.prevMouseState.IsButtonDown(MouseButton.LeftButton) && !mouseIsDown)
            {
                this.Click?.Invoke(this, EventArgs.Empty);
            }
        }

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.face, -10);

        var scrnPos = Position.ToScreen(Width, Height);
        var halfWidth = Width / 2f;
        var halfHeight = Height / 2f;

        var leftPadding = PaddingRatio <= 0 ? halfWidth : halfWidth * PaddingRatio;
        var topLeft = new Vector2(scrnPos.X - leftPadding, scrnPos.Y - leftPadding);
        var topRight = new Vector2(scrnPos.X + leftPadding, scrnPos.Y - leftPadding);
        var bottomCenter = new Vector2(scrnPos.X, scrnPos.Y + leftPadding);

        var arrowColor = Color.White;

        this.lineRenderer.RenderLine(topLeft, topRight, arrowColor, 2, -9);
        this.lineRenderer.RenderLine(topRight, bottomCenter, arrowColor, 2, -9);
        this.lineRenderer.RenderLine(bottomCenter, topLeft, arrowColor, 2, -9);

        base.Render();
    }
}