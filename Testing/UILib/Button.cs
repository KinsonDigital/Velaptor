using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Button : Control
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly IAppInput<MouseState> mouse;
    private readonly Label label;
    private RectShape face;
    private MouseState prevMouseState;
    public EventHandler<EventArgs>? Click;

    public Button()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.mouse = HardwareFactory.GetMouse();
        
        this.label = new Label();
        this.label.Text = "Button";

        Width = 200;
        Height = 30;
    }

    public string Text
    { 
        get => this.label.Text;
        set => this.label.Text = value;
    }

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

        this.face = new RectShape
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
        var mouseIsOver = this.face.Contains(mousePosVector);

        // If the mouse position is inside of the slider area
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
        else
        {
            this.face.Color = this.face.Color.DecreaseBrightness(0.0f);
        }

        this.label.Position = new Vector2(scrnPos.X, scrnPos.Y);
        this.label.Update();

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.face, -10);

        this.label.Text = Text;
        this.label.Render();

        base.Render();
    }
}
