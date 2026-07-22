namespace UILib;

using System.Drawing;
using System.Numerics;
using Carbonate;
using Carbonate.OneWay;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class DropDown : Control
{
    private const int ListDividerHeight = 5;
    private const float PaddingRatio = 0.5f;
    private const float ArrowButtonWidthHeight = 30f;
    private const float ArrowButtonHalfWidthHeight = ArrowButtonWidthHeight / 2f;
    private static int nextId = 1;
    private readonly int id;
    private readonly IPushReactable<DisableMouseSubscriptionData> dropDownReactable;
    private readonly IDisposable subscription;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly List<DropDownItem> listItems = [];
    private readonly Color listAreaBackgroundClr = Color.FromArgb(255, 17, 17, 17);
    private readonly Color hoverListItemClr = Color.FromArgb(255, 57, 124, 204);
    private readonly Color selectedItemClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color arrowFaceClr = Color.FromArgb(255, 41, 72, 109);
    private readonly Color arrowFaceDisabledClr;
    private readonly Color selectedItemAreaHoverClr;
    private readonly Color arrowFaceHoverClr;
    private readonly Color itemTextDisabledClr = Color.FromArgb(255, 175, 175, 175);
    private RectShape selectedItemArea;
    private RectShape arrowFace;
    private Vector2 selectedItemTextPos;
    private IFont? font;
    private MouseState prevMouseState;
    private bool isExpanded;
    private bool mouseClickDisabled;
    private bool clickConsumedThisFrame;

    public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;

    public DropDown()
    {
        this.id = nextId++;

        this.dropDownReactable = ReactableFactory.CreateDisableMouseClickReactable();

        this.subscription = this.dropDownReactable.CreateOneWayReceive(
            SubscriptionIds.OverDropDownItemId,
            nameof(SubscriptionIds.OverDropDownItemId),
            (data) =>
            {
                this.mouseClickDisabled = data.IsExpanded && data.ExpandedDropDownId != this.id;
                this.clickConsumedThisFrame = data.ConsumedClick;
            },
            () => this.subscription.Dispose()
        );

        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();

        Width = 200;
        Height = 30;
        this.selectedItemAreaHoverClr = this.selectedItemClr.IncreaseBrightness(0.4f);
        this.arrowFaceHoverClr = this.arrowFaceClr.IncreaseBrightness(0.4f);
        this.arrowFaceDisabledClr = DisabledColor.IncreaseBrightness(0.4f);
    }

    public List<string> Items => [.. this.listItems.Select(x => x.Text)];

    public string SelectedItem { get; private set; } = string.Empty;

    public void AddItem(string text)
    {
        var newItem = new DropDownItem
        {
            Text = text,
            Width = Width,
            Height = Height,
        };
        this.listItems.Add(newItem);

        // If the total number of items is 1, set the first item and only
        // item as the dropdown's selected item. This is done to ensure
        // that the dropdown has a default selection.
        if (this.listItems.Count == 1)
        {
            SelectedItem = text;
        }
    }

    public void RemoveItem(string text)
    {

    }

    public void SelectItem(string text)
    {
        if (!this.listItems.Any((i) => i.Text == text))
        {
            throw new Exception($"The item '{text}' does not exist.");
        }

        SelectedItem = text;
    }

    public override void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        this.font = this.contentManager.LoadFont(DefaultBoldFontName, 12);

        foreach (var item in this.listItems)
        {
            item.Click += ItemOn_Click;
            item.Load();
        }

        base.Load();
    }

    public override void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        this.contentManager.Unload(this.font);

        foreach (var item in this.listItems)
        {
            item.Click -= ItemOn_Click;
            item.Unload();
        }

        base.Unload();
    }

    public override void Update()
    {
        if (!Visible)
        {
            return;
        }

        var currentMouseState = this.mouse.GetState();

        var scrnPos = Position.ToWorld(Width, Height);

        this.selectedItemArea = new RectShape
        {
            Position = scrnPos,
            Width = Width,
            Height = Height,
            Color = this.selectedItemClr,
            IsSolid = true,
        };

        this.arrowFace = new RectShape
        {
            Position = new Vector2(this.selectedItemArea.Right - ArrowButtonHalfWidthHeight, this.selectedItemArea.Top + ArrowButtonHalfWidthHeight),
            Width = ArrowButtonWidthHeight,
            Height = ArrowButtonWidthHeight,
            Color = this.arrowFaceClr,
            IsSolid = true,
        };

        var mousePos = currentMouseState.GetPosition().ToVector2();

        if (this.isExpanded)
        {
            for (var i = 0; i < this.listItems.Count; i++)
            {
                var item = this.listItems[i];
                item.Width = Width;
                item.Height = Height;
                item.Position = new Vector2(
                        Position.X,
                        Position.Y + (Height * (i + 1)) + ListDividerHeight);

                if (SelectedItem == item.Text)
                {
                    item.BackgroundColor = this.selectedItemClr;
                }
                else
                {
                    item.BackgroundColor = this.listAreaBackgroundClr;
                }

                var itemArea = new RectangleF(item.Position.X, item.Position.Y, item.Width, item.Height);
                var isMouseOverItem = itemArea.Contains(mousePos.X, mousePos.Y);

                if (isMouseOverItem)
                {
                    item.BackgroundColor = this.hoverListItemClr;
                }

                item.Update();

                this.listItems[i] = item;
            }
        }

        var isMouseOver = this.selectedItemArea.Contains(mousePos) || this.arrowFace.Contains(mousePos);

        if (Enabled && isMouseOver && !this.mouseClickDisabled && !this.clickConsumedThisFrame)
        {
            this.selectedItemArea.Color = this.selectedItemAreaHoverClr;
            this.arrowFace.Color = this.arrowFaceHoverClr;

            if (currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
            {
                this.isExpanded = !this.isExpanded;
                this.dropDownReactable.Push(
                    SubscriptionIds.OverDropDownItemId,
                    new DisableMouseSubscriptionData
                    {
                        IsExpanded = this.isExpanded,
                        ExpandedDropDownId = this.isExpanded ? this.id : 0,
                    });
            }
        }
        else
        {
            this.selectedItemArea.Color = Enabled ? this.selectedItemClr : DisabledColor;
            this.arrowFace.Color = Enabled ? this.arrowFaceClr : this.arrowFaceDisabledClr;
        }

        if (this.listItems.Count >= 1)
        {
            this.selectedItemTextPos = new Vector2(
                Position.X + ((Width / 2f) - (this.arrowFace.Width / 2f)),
                Position.Y + HalfHeight);//(this.selectedItemTextSize.Height / 2f));
        }

        this.prevMouseState = currentMouseState;
        this.clickConsumedThisFrame = false;

        base.Update();
    }

    public override void Render(int layer = 0)
    {
        if (!Visible)
        {
            return;
        }

        this.shapeRenderer.Render(this.selectedItemArea, -10);
        this.shapeRenderer.Render(this.arrowFace, -10);

        RenderArrowButton();

        if (Items.Count >= 1)
        {
            this.fontRenderer.Render(this.font, SelectedItem, this.selectedItemTextPos, Enabled ? Color.White : this.itemTextDisabledClr);

            if (this.isExpanded)
            {
                foreach (var item in this.listItems)
                {
                    item.Render();
                }
            }
        }

        base.Render(layer);
    }

    private void RenderArrowButton()
    {
        this.shapeRenderer.Render(this.arrowFace, -10);

        var scrnPos = this.arrowFace.Position;//.ToWorld(Width, Height);
        var halfWidth = this.arrowFace.Width / 2f;

        var leftPadding = PaddingRatio <= 0 ? halfWidth : halfWidth * PaddingRatio;
        var topLeft = new Vector2(scrnPos.X - leftPadding, scrnPos.Y - leftPadding);
        var topRight = new Vector2(scrnPos.X + leftPadding, scrnPos.Y - leftPadding);
        var bottomCenter = new Vector2(scrnPos.X, scrnPos.Y + leftPadding);

        var arrowColor = Color.White;

        this.lineRenderer.RenderLine(topLeft, topRight, arrowColor, 2, -9);
        this.lineRenderer.RenderLine(topRight, bottomCenter, arrowColor, 2, -9);
        this.lineRenderer.RenderLine(bottomCenter, topLeft, arrowColor, 2, -9);
    }

    private void ItemOn_Click(object? sender, EventArgs e)
    {
        if (sender is DropDownItem item)
        {
            var oldItem = SelectedItem;
            SelectedItem = item.Text;

            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(oldItem, SelectedItem));
        }

        this.isExpanded = false;
        this.dropDownReactable.Push(
            SubscriptionIds.OverDropDownItemId,
            new DisableMouseSubscriptionData { IsExpanded = false, ExpandedDropDownId = 0, ConsumedClick = true });
    }
}
