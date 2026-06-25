// <copyright file="WebGPUTestingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Drawing;
using System.Numerics;
using Silk.NET.Windowing;
using UILib;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

public class WebGPUTestingScene : SceneBase
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly UIContainer container;
    private readonly IBatcher batcher;
    private readonly CheckBox checkbox;
    private readonly Slider slider;
    private readonly Button button;
    private readonly Label label;
    private readonly ArrowButton arrowButton;
    private readonly DropDown dropdown;

    public WebGPUTestingScene()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.batcher = RendererFactory.CreateBatcher();

        this.container = new UIContainer();
        this.container.Position = new Vector2(300, 300);

        this.checkbox = new CheckBox
        {
            Position = new (0, 0),
            IsChecked = true,
        };
        this.slider = new Slider();

        this.button = new Button();
        this.button.Click += (sender, args) => this.checkbox.IsChecked = !this.checkbox.IsChecked;

        this.label = new Label();
        this.label.Text = "This is a label";

        this.arrowButton = new ArrowButton();

        this.dropdown = new DropDown();
        this.dropdown.AddItem("Item 1");
        this.dropdown.AddItem("Item 2");
        this.dropdown.AddItem("Item 3");
        this.dropdown.AddItem("Item 4");
        this.dropdown.AddItem("Item 5");
        this.dropdown.AddItem("Item 6");
        this.dropdown.AddItem("Item 7");
        this.dropdown.AddItem("Item 8");


        this.container.AddControl(this.checkbox);
        // this.container.AddControl(this.slider);
        // this.container.AddControl(this.button);
        // this.container.AddControl(this.label);
    }

    public override void LoadContent()
    {
        this.container.Load();
        this.checkbox.Load();
        this.slider.Load();
        this.button.Load();
        this.label.Load();
        this.arrowButton.Load();
        this.dropdown.Load();

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.container.Unload();
        this.checkbox.Unload();
        this.slider.Unload();
        this.button.Unload();
        this.arrowButton.Unload();
        this.label.Unload();
        this.dropdown.Unload();

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.container.Update();

        // this.checkbox.Position = new (50, 50);
        this.checkbox.Update();

        this.slider.Position = new (100, 100);
        this.slider.Update();

        this.button.Position = new (500, 250);
        this.button.Update();

        this.arrowButton.Position = new (600, 300);
        this.arrowButton.Update();

        this.label.Position = new (300, 50);
        this.label.Update();

        this.dropdown.Position = new (500, 500);
        this.dropdown.Update();

        base.Update(frameTime);
    }

    public override void Render()
    {
        this.batcher.Begin();

        this.container.Render();

        this.checkbox.Render();
        this.slider.Render();
        this.button.Render();
        this.arrowButton.Render();
        this.label.Render();
        this.dropdown.Render();

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
