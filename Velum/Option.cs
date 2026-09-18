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

public sealed class Option : Control
{
    private const int BoxTextPadding = 5;
    private const float BoxWidthHeight = 20;
    private static readonly List<(int, Guid, bool)> CheckStates = new ();
    private readonly Guid id;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly Color optionColor = Color.FromArgb(255, 89, 149, 224);
    private readonly IDisposable subscription;
    private readonly string text = "Option";
    private IFont? font;
    private CircleShape circle;
    private Vector2 textPos;
    private MouseState prevMouseState;
    private bool mouseClickDisabled;

    public event EventHandler<CheckChangedEventArgs>? CheckChanged;

    public Option()
    {
        var disableMouseClickReactable1 = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = disableMouseClickReactable1.CreateOneWayReceive(
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
        init => this.text = value;
    }

    public int GroupNumber { get; init; }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        CheckStates.Add((GroupNumber, this.id, CheckStates.Count == 0));

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

        CheckStates.RemoveAll(x => x.Item2 == this.id);

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

        this.circle = new CircleShape
        {
            Position = screenPos,
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

        // If the mouse is over any part of the checkbox and the left mouse button was just released
        if (isMouseOver && !this.mouseClickDisabled && currentLeftBtnUp && prevLeftBtnDown)
        {
            // Set the state of each item in the group
            for (var i = 0; i < CheckStates.Count; i++)
            {
                var itemToUpdate = CheckStates[i];

                if (itemToUpdate.Item1 == GroupNumber)
                {
                    itemToUpdate.Item3 = itemToUpdate.Item2 == this.id;
                }

                CheckStates[i] = itemToUpdate;
            }

            this.CheckChanged?.Invoke(this, new CheckChangedEventArgs(IsChecked));
        }

        // Find the current item and set the check state.
        for (var i = 0; i < CheckStates.Count; i++)
        {
            if (!this.mouseClickDisabled && CheckStates[i].Item1 == GroupNumber && CheckStates[i].Item2 == this.id)
            {
                IsChecked = CheckStates[i].Item3;
            }
        }

        this.circle.IsSolid = IsChecked;
        this.circle.Color = this.optionColor;

        this.prevMouseState = currentMouseState;

        base.Update();
    }

    public override void Render(int layer)
    {
        if (this.font is null)
        {
            throw new InvalidOperationException($"The '{nameof(this.font)}' cannot be null. Could not render the '{nameof(Option)}' control.");
        }

        if (!Visible)
        {
            return;
        }

        if (!IsLoaded)
        {
            throw new InvalidOperationException($"The '{nameof(Option)}' must be loaded before it can be rendered.");
        }

        this.shapeRenderer.Render(this.circle);
        this.fontRenderer.Render(this.font, Text, this.textPos, Color.White);

        base.Render(0);
    }
}
