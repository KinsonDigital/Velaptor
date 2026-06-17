// <copyright file="WebGPUTestingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using UILib;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

public class WebGPUTestingScene : SceneBase
{
    private readonly IShapeRenderer shapeRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly UIContainer container;
    private readonly IBatcher batcher;

    public WebGPUTestingScene()
    {
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.batcher = RendererFactory.CreateBatcher();
        this.container = new UIContainer();
    }

    public override void Render()
    {
        this.batcher.Begin();

        this.container.Render();
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
