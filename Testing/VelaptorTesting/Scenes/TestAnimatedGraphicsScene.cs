// <copyright file="TestAnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Drawing;
using Velaptor;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.UI;
using VelaptorTesting.Core;

namespace VelaptorTesting.Scenes;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class TestAnimatedGraphicsScene : SceneBase
{
    private const int TopMargin = 50;
    private IAtlasData? mainAtlas;
    private AtlasSubTextureData[]? frames;
    private Label? lblInstructions;
    private int elapsedTime;
    private int currentFrame;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAnimatedGraphicsScene"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the scene.</param>
    public TestAnimatedGraphicsScene(IContentLoader contentLoader)
        : base(contentLoader)
    {
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.mainAtlas = ContentLoader.LoadAtlas("Main-Atlas");
        this.frames = this.mainAtlas.GetFrames("circle");

        this.lblInstructions = new Label();
        this.lblInstructions.Text = "Verify that the Kinson Digital logo is rotating clockwise.";
        this.lblInstructions.Color = Color.White;

        AddControl(this.lblInstructions);

        this.lblInstructions.Left = (int)(MainWindow.WindowWidth / 2) - (int)(this.lblInstructions.Width / 2);
        this.lblInstructions.Top = TopMargin;

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        ContentLoader.UnloadAtlas(this.mainAtlas);

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        if (this.elapsedTime >= 124)
        {
            this.elapsedTime = 0;

            this.currentFrame = this.currentFrame >= this.frames.Length - 1
                ? 0
                : this.currentFrame + 1;
        }
        else
        {
            this.elapsedTime += frameTime.ElapsedTime.Milliseconds;
        }
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render(IRenderer renderer)
    {
        var posX = ((int)MainWindow.WindowWidth / 2) - (this.frames[this.currentFrame].Bounds.Width / 2);
        var posY = ((int)MainWindow.WindowHeight / 2) - (this.frames[this.currentFrame].Bounds.Height / 2);

        renderer.Render(
            this.mainAtlas.Texture,
            this.frames[this.currentFrame].Bounds,
            new Rectangle(posX, posY, (int)this.mainAtlas.Width, (int)this.mainAtlas.Height),
            1f,
            0f,
            Color.White,
            RenderEffects.None);
        base.Render(renderer);
    }

    /// <inheritdoc cref="SceneBase.Dispose(bool)"/>
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed || !IsLoaded)
        {
            return;
        }

        base.Dispose(disposing);
    }
}
