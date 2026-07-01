using System.Drawing;
using System.Numerics;
using Carbonate;
using Carbonate.OneWay;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class Option : Control
{
    private const int BoxTextPadding = 5;
    private const float BoxWidthHeight = 20;
    private static readonly List<(int, Guid, bool)> checkStates = new ();
    private readonly Guid id;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly Color OptionColor = Color.FromArgb(255, 89, 149, 224);
    private readonly IDisposable subscription;
    private CircleShape circle;
    private Vector2 textPos;
    private MouseState prevMouseState;
    private string text = "Option";
    private IFont font;
    private IPushReactable<DisableMouseSubscriptionData> disableMouseClickReactable;
    private bool mouseClickDisabled;

    public event EventHandler<CheckChangedEventArgs>? CheckChanged;

    public Option()
    {
        this.disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = this.disableMouseClickReactable.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) => this.mouseClickDisabled = data.IsExpanded,
            () => this.subscription.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();

        Height = (int)BoxWidthHeight;
        this.id = Guid.NewGuid();
    }

    public bool IsChecked { get; set; }

    public string Text
    {
        get => this.text;
        set => this.text = value ?? string.Empty;
    }

    public int GroupNumber { get; set; }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        checkStates.Add((GroupNumber, this.id, checkStates.Count == 0));

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

        checkStates.RemoveAll(x => x.Item2 == this.id);

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

        var scrnPos = Position.ToWorld(BoxWidthHeight, BoxWidthHeight);

        this.circle = new CircleShape
        {
            Position = scrnPos,
            Diameter = BoxWidthHeight,
        };

        var textSize = this.font.Measure(this.text);
        this.textPos = new Vector2(
            Position.X + this.circle.Diameter + (textSize.Width / 2f) + BoxTextPadding,
            Position.Y + (textSize.Height / 2f) + 2);

        var mousePos = currentMouseState.GetPosition().ToVector2();
        var textRectPos = new Vector2(
            Position.X + this.circle.Diameter + BoxTextPadding,
            Position.Y);
        var textRect = new Rectangle((int)textRectPos.X, (int)textRectPos.Y, (int)textSize.Width, (int)textSize.Height);
        var isMouseOver = this.circle.Contains(mousePos) || textRect.Contains((int)mousePos.X, (int)mousePos.Y);
        var currentLeftBtnUp = currentMouseState.IsButtonUp(MouseButton.LeftButton);
        var prevLeftBtnDown = this.prevMouseState.IsButtonDown(MouseButton.LeftButton);

        // If the mouse if over any part of the checkbox and the left mouse button was just released
        if (isMouseOver && !this.mouseClickDisabled && currentLeftBtnUp && prevLeftBtnDown)
        {
            // Set the state of each item in the group
            for (var i = 0; i < checkStates.Count; i++)
            {
                var itemToUpdate = checkStates[i];

                if (itemToUpdate.Item1 == GroupNumber)
                {
                    itemToUpdate.Item3 = itemToUpdate.Item2 == this.id;
                }

                checkStates[i] = itemToUpdate;
            }

            CheckChanged?.Invoke(this, new CheckChangedEventArgs(IsChecked));
        }

        // Find the current item and set the check state.
        for (var i = 0; i < checkStates.Count; i++)
        {
            if (!this.mouseClickDisabled && checkStates[i].Item1 == GroupNumber && checkStates[i].Item2 == this.id)
            {
                IsChecked = checkStates[i].Item3;
            }
        }

        this.circle.IsSolid = IsChecked;
        this.circle.Color = Color.FromArgb(255, 89, 149, 224);

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        if (!Visible)
        {
            return;
        }

        this.shapeRenderer.Render(this.circle);
        this.fontRenderer.Render(this.font, Text, this.textPos, Color.White);

        base.Render();
    }

    private void LabelOn_Click(object? sender, EventArgs e)
    {
        IsChecked = !IsChecked;
    }
}