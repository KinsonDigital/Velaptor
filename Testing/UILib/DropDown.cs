using System.Drawing;
using System.Numerics;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;

public class DropDown : Control
{
    private const int ListDividerHeight = 5;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IContentManager contentManager;
    private readonly IAppInput<MouseState> mouse;
    private readonly List<DropDownItem> listItems = [];
    private readonly ArrowButton arrowBtn;
    private readonly Color listAreaBackgroundClr = Color.FromArgb(255, 17, 17, 17);
    private readonly Color selectedItemClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color hoverListItemClr = Color.FromArgb(255, 57, 124, 204);
    private RectShape selectedItemArea;
    private string selectedItemText;
    private Vector2 selectedItemTextPos;
    private RectShape listArea;
    private bool isExpanded;
    private RectShape listDividerRest;
    private IFont? font;
    private MouseState prevMouseState;

    public DropDown()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.mouse = HardwareFactory.GetMouse();
        this.arrowBtn = new ArrowButton();
        this.arrowBtn.Click += ArrowBtn_Click;

        Width = 200;
        Height = 30;

        this.listArea = new RectShape
        {
            Position = Position,
            Width = Width,
            Height = Height,
            Color = this.listAreaBackgroundClr,
            IsSolid = true,
        };
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
        this.arrowBtn.Load();
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
        this.arrowBtn.Click -= ArrowBtn_Click;
        this.arrowBtn.Unload();
        this.contentManager.Unload(this.font);

        foreach (var item in this.listItems)
        {
            item.Unload();
        }

        base.Unload();
    }

    public override void Update()
    {
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

        this.arrowBtn.Position = new Vector2(
            scrnPos.X + (this.selectedItemArea.HalfWidth - this.arrowBtn.Width),
            scrnPos.Y - this.selectedItemArea.HalfHeight);
        this.arrowBtn.Update();

        if (this.listItems.Count >= 1)
        {
            this.selectedItemTextPos = new Vector2(
                Position.X + ((Width / 2f) - (this.arrowBtn.Width / 2f)),
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

                var mousePos = currentMouseState.GetPosition().ToVector2();
                var itemArea = new RectangleF(item.Position.X, item.Position.Y, item.Width, item.Height);
                var isMouseOver = itemArea.Contains(mousePos.X, mousePos.Y);

                if (isMouseOver)
                {
                    item.BackgroundColor = this.hoverListItemClr;
                }

                item.Update();
                this.listItems[i] = item;
            }

            // TODO: Remove
            // this.listArea.Position = new Vector2(Position.X, scrnPos.Y + (Height * (i + 1)) + ListDividerHeight);

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
        this.shapeRenderer.Render(this.selectedItemArea, -10);
        this.arrowBtn.Render();
        
        // this.shapeRenderer.Render(this.listArea, -10);

        if (Items.Count >= 1)
        {
            this.fontRenderer.Render(this.font, this.selectedItemText, this.selectedItemTextPos, Color.White);

            if (this.isExpanded)
            {
                foreach (var item in this.listItems)
                {
                    item.Render();
                }

                // this.shapeRenderer.Render(this.listDividerRest, 10);
            }
        }

        base.Render(layer);
    }

    private void ArrowBtn_Click(object? sender, EventArgs e)
    {
        this.isExpanded = !this.isExpanded;
    }

    private void ItemOn_Click(object? sender, EventArgs e)
    {
        if (sender is DropDownItem item)
        {
            this.selectedItemText = item.Text;
        }

        this.isExpanded = false;
    }
}
