using System.Drawing;
using System.Numerics;
using Carbonate.OneWay;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

/* TODO:
    1. Need to add the ability to disable mouse clicks for other controls
    as long as a dropdown is expanded. This will prevent other controls from
    being clicked when choosing an item from the dropdown list.
        - This can be achieved using Carbonate to send a notification to all
        controls that mouse clicking is disabled or enabled based on the dropdown's expanded state.
*/

public class DropDown : Control
{
    private const int ListDividerHeight = 5;
    private const float PaddingRatio = 0.5f;
    private const float ArrowButtonWidthHeight = 30f;
    private const float ArrowButtonHalfWidthHeight = ArrowButtonWidthHeight / 2f;
    private readonly IPushReactable<DisableMouseSubscriptionData> disableMouseClickReactable;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly List<DropDownItem> listItems = [];
    private readonly Color listAreaBackgroundClr = Color.FromArgb(255, 17, 17, 17);
    private readonly Color hoverListItemClr = Color.FromArgb(255, 57, 124, 204);
    private readonly Color selectedItemClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color arrowDefaultClr  = Color.FromArgb(255, 41, 72, 109);
    private RectShape selectedItemArea;
    private RectShape arrowFace;
    private string selectedItemText;
    private Vector2 selectedItemTextPos;
    private bool isExpanded;
    private RectShape listDividerRest;
    private IFont? font;
    private MouseState prevMouseState;

    public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;

    public DropDown()
    {
        this.disableMouseClickReactable = ReactableFactory.CreateDisableMouseClickReactable();
        
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();

        Width = 200;
        Height = 30;
    }

    public List<string> Items => [.. this.listItems.Select(x => x.Text)];

    public string SelectedItem => this.selectedItemText;

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
            this.selectedItemText = text;
        }
    }

    public void RemoveItem(string text)
    {

    }

    public override void Load()
    {
        // this.arrowBtn.Load();
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
        this.contentManager.Unload(this.font);

        foreach (var item in this.listItems)
        {
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
            Color = this.arrowDefaultClr,
            IsSolid = true,
        };

        // this.arrowBtn.Update();

        var mousePos = currentMouseState.GetPosition().ToVector2();
        var isMouseOver = this.selectedItemArea.Contains(mousePos) || this.arrowFace.Contains(mousePos);

        if (isMouseOver)
        {
            this.selectedItemArea.Color = this.selectedItemClr.IncreaseBrightness(0.4f);
            this.arrowFace.Color = this.arrowFace.Color.IncreaseBrightness(0.4f);

            if (currentMouseState.IsButtonUp(MouseButton.LeftButton) && this.prevMouseState.IsButtonDown(MouseButton.LeftButton))
            {
                this.isExpanded = !this.isExpanded;
                this.disableMouseClickReactable.Push(
                    SubscriptionIds.DisableMouseClickId,
                    new DisableMouseSubscriptionData { MouseDisabled = true });
            }
        }
        else
        {
            this.selectedItemArea.Color = this.selectedItemClr;
            this.arrowFace.Color = this.arrowDefaultClr;
        }

        if (this.listItems.Count >= 1)
        {
            this.selectedItemTextPos = new Vector2(
                Position.X + ((Width / 2f) - (this.arrowFace.Width / 2f)),
                Position.Y + (Height / 2f));
        }

        if (this.isExpanded)
        {
            for (var i = 0; i < this.listItems.Count; i++)
            {
                var item = this.listItems[i];
                item.Position = new Vector2(
                        Position.X,
                        Position.Y + (Height * (i + 1)) + ListDividerHeight);

                if (this.selectedItemText == item.Text)
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

            this.listDividerRest = new RectShape
            {
                Position = new Vector2(scrnPos.X, scrnPos.Y + this.selectedItemArea.HalfHeight + (ListDividerHeight / 2f)),
                Width = Width,
                Height = ListDividerHeight,
                Color = this.listAreaBackgroundClr,
                IsSolid = true,
            };
        }

        this.prevMouseState = currentMouseState;

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
            this.fontRenderer.Render(this.font, this.selectedItemText, this.selectedItemTextPos, Color.White);

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

    private void ArrowBtn_Click(object? sender, EventArgs e)
    {
        this.isExpanded = !this.isExpanded;
    }

    private void ItemOn_Click(object? sender, EventArgs e)
    {
        if (sender is DropDownItem item)
        {
            var oldItem = this.selectedItemText;
            this.selectedItemText = item.Text;

            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(oldItem, this.selectedItemText));
        }

        this.isExpanded = false;
    }
}
