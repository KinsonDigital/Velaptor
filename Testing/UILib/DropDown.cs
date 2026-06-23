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
    private readonly IAppInput<MouseState> mouse;
    private readonly List<Label> listItems = [];
    private readonly ArrowButton arrowBtn;
    private readonly Color listAreaBackgroundClr = Color.FromArgb(255, 17, 17, 17);
    private readonly Color selectedItemClr = Color.FromArgb(255, 35, 48, 70);
    private readonly Color hoverListItemClr = Color.FromArgb(255, 57, 124, 204);
    private Label selectedItem;
    private RectShape face;
    private RectShape listArea;
    private bool isExpanded;
    private RectShape listDividerRest;

    public DropDown()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
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

    public string SelectedItem => this.selectedItem.Text;

    public void AddItem(string text)
    {
        var label = new Label();
        label.Text = text;
        label.Click += LabelOn_Click;

        this.listItems.Add(label);

        // If the total number of items is 1, set the first item and only
        // item as the dropdown's selected item. This is done to ensure
        // that the dropdown has a default selection.
        if (this.listItems.Count == 1)
        {
            this.selectedItem = new Label();
            this.selectedItem.Text = text;
        }
    }

    public void RemoveItem(string text)
    {

    }

    public override void Load()
    {
        this.arrowBtn.Load();
        this.selectedItem.Load();

        foreach (var label in this.listItems)
        {
            label.Load();
        }

        base.Load();
    }

    public override void Unload()
    {
        this.arrowBtn.Click -= ArrowBtn_Click;
        this.arrowBtn.Unload();
        this.selectedItem.Unload();

        foreach (var label in this.listItems)
        {
            label.Click -= LabelOn_Click;
            label.Unload();
        }

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
            Color = this.selectedItemClr,
            IsSolid = true,
        };

        foreach (var label in this.listItems)
        {
            label.Width = Width;
            label.Height = Height;
        }

        this.arrowBtn.Position = new Vector2(scrnPos.X + (this.face.HalfWidth - this.arrowBtn.Width), scrnPos.Y - this.face.HalfHeight);
        this.arrowBtn.Update();
        this.selectedItem.Update();

        if (this.listItems.Count >= 1)
        {
            this.selectedItem.Position = new Vector2(scrnPos.X, scrnPos.Y);
        }

        if (this.isExpanded)
        {
            for (var i = 0; i < this.listItems.Count; i++)
            {
                var label = this.listItems[i];

                label.Position = new Vector2(scrnPos.X, scrnPos.Y + (Height * (i + 1)) + ListDividerHeight);
                if (label.Text == this.selectedItem.Text)
                {
                    label.BackgroundColor = this.selectedItemClr;
                }
                else
                {
                    label.BackgroundColor = Color.Transparent;
                }

                if (label.IsMouseOver)
                {
                    label.BackgroundColor = this.hoverListItemClr;
                }

                label.Update();

                this.listArea.Position = new Vector2(scrnPos.X, scrnPos.Y + (Height * (i + 1)) + ListDividerHeight);

                this.listDividerRest = new RectShape
                {
                    Position = new Vector2(scrnPos.X, scrnPos.Y + this.face.HalfHeight + (ListDividerHeight / 2f)),
                    Width = Width,
                    Height = ListDividerHeight,
                    Color = this.listAreaBackgroundClr,
                    IsSolid = true,
                };
            }
        }

        base.Update();
    }

    public override void Render()
    {
        this.shapeRenderer.Render(this.face, -10);
        this.arrowBtn.Render();

        if (Items.Count >= 1)
        {
            this.selectedItem.Render();

            if (this.isExpanded)
            {
                for (var i = 0; i < this.listItems.Count; i++)
                {
                    this.shapeRenderer.Render(this.listArea, -10);
                    this.listItems[i].Render();
                }

                this.shapeRenderer.Render(this.listDividerRest, 10);
            }
        }

        base.Render();
    }

    private void ArrowBtn_Click(object? sender, EventArgs e)
    {
        this.isExpanded = !this.isExpanded;
    }

    private void LabelOn_Click(object? sender, LabelClickEventArgs e)
    {
        this.selectedItem.Text = e.Label.Text;
        this.isExpanded = false;
    }
}
