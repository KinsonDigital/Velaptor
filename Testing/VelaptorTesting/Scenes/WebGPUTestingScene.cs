// <copyright file="WebGPUTestingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// TODO: Remove the scene entirely.

using System.Drawing;
using System.Numerics;
using UILib;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;
using Velaptor.WebGPU.Batching;

public class WebGPUTestingScene : SceneBase
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly Container container;
    private readonly IBatcher batcher;
    private readonly CheckBox checkbox;
    private readonly Slider slider;
    private readonly Button btnIncreaseSliderValue;
    private readonly Button bntDecreaseSliderValue;
    private readonly Button button;
    private readonly Label label;
    private readonly ArrowButton arrowButton;
    private readonly DropDown dropdown;
    private readonly CheckBox otherCheckbox;
    private readonly Option option1;
    private readonly Option option4;
    private readonly Option option3;
    private readonly Option option2;

    public WebGPUTestingScene()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.batcher = RendererFactory.CreateBatcher();

        this.container = new Container();
        this.container.Position = new(0, 0);

        this.checkbox = new CheckBox
        {
            Position = new(50, 50),
            IsChecked = true,
        };

        this.otherCheckbox = new CheckBox
        {
            Position = new(70, 50),
            Text = "Other checkbox",
            IsChecked = false,
        };

        this.slider = new Slider();
        this.btnIncreaseSliderValue = new Button();
        this.btnIncreaseSliderValue.Text = "U";
        this.btnIncreaseSliderValue.Width = 30;
        this.btnIncreaseSliderValue.Click += (sender, args) => this.slider.Value += 10;

        this.bntDecreaseSliderValue = new Button();
        this.bntDecreaseSliderValue.Text = "D";
        this.bntDecreaseSliderValue.Width = 30;
        this.bntDecreaseSliderValue.Click += (sender, args) => this.slider.Value -= 10;

        this.button = new Button();
        this.button.Click += (sender, args) => this.checkbox.IsChecked = !this.checkbox.IsChecked;

        this.label = new Label();
        this.label.Text = "This is a label";

        this.arrowButton = new ArrowButton();

        this.option1 = new Option();
        this.option1.Position = new(400, 400);
        this.option1.GroupNumber = 200;

        this.option2 = new Option();
        this.option2.Position = new(400, this.option1.Position.Y + this.option1.Height);
        this.option2.GroupNumber = 200;

        this.option3 = new Option();
        this.option3.Position = new(400, this.option2.Position.Y + this.option2.Height);
        this.option3.GroupNumber = 200;

        this.option4 = new Option();
        this.option4.Position = new(400, this.option3.Position.Y + this.option3.Height);
        this.option4.GroupNumber = 200;

        this.dropdown = new DropDown();
        this.dropdown.AddItem("Item 1");
        this.dropdown.AddItem("Item 2");
        this.dropdown.AddItem("Item 3");
        this.dropdown.AddItem("Item 4");
        this.dropdown.AddItem("Item 5");
        this.dropdown.AddItem("Item 6");
        this.dropdown.AddItem("Item 7");
        this.dropdown.AddItem("Item 8");
    }

    public override void LoadContent()
    {
        this.container.Load();

        // this.option1.Load();
        // this.option2.Load();
        // this.option3.Load();
        // this.option4.Load();
        // this.checkbox.Load();
        // this.otherCheckbox.Load();

        this.slider.Load();
        this.btnIncreaseSliderValue.Load();
        this.bntDecreaseSliderValue.Load();

        // this.button.Load();
        // this.label.Load();
        // this.arrowButton.Load();
        // this.dropdown.Load();

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.container.Unload();
        // this.option1.Unload();
        // this.option2.Unload();
        // this.option3.Unload();
        // this.option4.Unload();

        // this.checkbox.Unload();
        // this.otherCheckbox.Unload();

        this.slider.Unload();
        this.btnIncreaseSliderValue.Load();
        this.bntDecreaseSliderValue.Load();

        // this.button.Unload();
        // this.arrowButton.Unload();
        // this.label.Unload();
        // this.dropdown.Unload();

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.container.Update();
        // this.option1.Update();
        // this.option2.Update();
        // this.option3.Update();
        // this.option4.Update();

        this.slider.Position = new (50, 100);
        this.slider.Update();

        this.btnIncreaseSliderValue.Update();
        this.btnIncreaseSliderValue.Position = new (this.slider.Position.X, this.slider.Position.Y + this.slider.Height + 10);

        this.bntDecreaseSliderValue.Update();
        this.bntDecreaseSliderValue.Position = new (
            this.btnIncreaseSliderValue.Position.X + this.btnIncreaseSliderValue.Width + 10,
            this.slider.Position.Y + this.slider.Height + 10);

        // this.checkbox.Position = new (this.slider.Position.X, this.slider.Position.Y + this.slider.Height);
        // this.checkbox.Update();

        // this.otherCheckbox.Position = new (50, this.checkbox.Position.Y + this.checkbox.Height);
        // this.otherCheckbox.Update();

        // this.checkbox.Position = new (50, 50);
        // this.checkbox.Update();

        // this.button.Position = new (500, 250);
        // this.button.Update();

        // this.arrowButton.Position = new (600, 300);
        // this.arrowButton.Update();

        // this.label.Position = new (300, 50);
        // this.label.Update();

        // this.dropdown.Position = new (500, 500);
        // this.dropdown.Update();

        base.Update(frameTime);
    }

    public override void Render()
    {
        this.batcher.Begin();

        this.container.Render();
        // this.option1.Render();
        // this.option2.Render();
        // this.option3.Render();
        // this.option4.Render();

        // this.checkbox.Render();
        // this.otherCheckbox.Render();

        // this.checkbox.Render();

        this.slider.Render();
        this.btnIncreaseSliderValue.Render();
        this.bntDecreaseSliderValue.Render();

        // this.button.Render();
        // this.arrowButton.Render();
        // this.label.Render();
        // this.dropdown.Render();

        // var rect = new RectShape
        // {
        //     Position = new (300, 300),
        //     Color = Color.IndianRed,
        //     IsSolid = true,
        //     Width = 200,
        //     Height = 200,
        // };
        //
        // var circle = new CircleShape
        // {
        //     Position = new (600, 300),
        //     Color = Color.Orange,
        //     IsSolid = true,
        //     Radius = 100,
        // };
        //
        // var line = new Line
        // {
        //     P1 = new (300, 600),
        //     P2 = new (600, 600),
        //     Color = Color.LimeGreen,
        //     Thickness = 5,
        // };
        //
        // this.shapeRenderer.Render(rect);
        // this.shapeRenderer.Render(circle);
        // this.lineRenderer.Render(line);

        this.batcher.End();

        base.Render();
    }
}
