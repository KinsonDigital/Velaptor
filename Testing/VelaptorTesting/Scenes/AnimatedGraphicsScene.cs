// <copyright file="AnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Drawing;
using System.Numerics;
using KdGui;
using KdGui.Factories;
using Velaptor;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Scene;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class AnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 10;
    private readonly ITextureRenderer textureRenderer;
    private readonly BackgroundManager backgroundManager;
    private readonly ILoader<IAtlasData> atlasLoader;
    private IAtlasData? mainAtlas;
    private AtlasSubTextureData[]? frames;
    private IControlGroup? grpInstructions;
    private IControlGroup? grpAnimation;
    private int elapsedTime;
    private int currentFrame;
    private float animSpeed = 32;
    private bool runningForward = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedGraphicsScene"/> class.
    /// </summary>
    public AnimatedGraphicsScene()
    {
        this.backgroundManager = new BackgroundManager();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.atlasLoader = ContentLoaderFactory.CreateAtlasLoader();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.mainAtlas = this.atlasLoader.Load("Main-Atlas");
        this.frames = this.mainAtlas.GetFrames("samus");

        var ctrlFactory = new ControlFactory();
        var instructions = ctrlFactory.CreateLabel();
        instructions.Text = "Verify that the Samus is running.";

        this.grpInstructions = ctrlFactory.CreateControlGroup();
        this.grpInstructions.Title = "Instructions";
        this.grpInstructions.AutoSizeToFitContent = true;
        this.grpInstructions.TitleBarVisible = false;
        this.grpInstructions.Add(instructions);

        var optForward = ctrlFactory.CreateRadioButton();
        optForward.Name = "optForward";
        optForward.Text = "Forwards";
        optForward.IsSelected = true;

        var optBackward = ctrlFactory.CreateRadioButton();
        optBackward.Name = "optBackward";
        optBackward.Text = "Backwards";

        var sldSpeed = ctrlFactory.CreateSlider();
        sldSpeed.Name = "sldSpeed";
        sldSpeed.Text = "Speed:";
        sldSpeed.Min = 8;
        sldSpeed.Max = 500;
        sldSpeed.Value = 32;
        sldSpeed.ValueChanged += (_, speed) =>
        {
            this.animSpeed = speed;
        };

        optForward.Selected += (_, _) =>
        {
            optBackward.IsSelected = false;
            this.runningForward = !this.runningForward;
        };

        optBackward.Selected += (_, _) =>
        {
            optForward.IsSelected = false;
            this.runningForward = !this.runningForward;
        };

        this.grpAnimation = ctrlFactory.CreateControlGroup();
        this.grpAnimation.Title = "Animation";
        this.grpAnimation.AutoSizeToFitContent = true;
        this.grpAnimation.Initialized += (_, _) =>
        {
            this.grpAnimation.Position = new Point(WindowPadding, WindowCenter.Y + WindowPadding);
        };
        this.grpAnimation.Add(optForward);
        this.grpAnimation.Add(optBackward);
        this.grpAnimation.Add(sldSpeed);

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager.Unload();
        this.atlasLoader.Unload(this.mainAtlas);
        this.grpInstructions.Dispose();
        this.grpInstructions = null;
        this.grpAnimation.Dispose();
        this.grpAnimation = null;

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        if (this.elapsedTime >= this.animSpeed)
        {
            this.elapsedTime = 0;

            if (this.runningForward)
            {
                this.currentFrame = this.currentFrame >= this.frames.Length - 1
                    ? 0
                    : this.currentFrame + 1;
            }
            else
            {
                this.currentFrame = this.currentFrame <= 0
                    ? this.frames.Length - 1
                    : this.currentFrame - 1;
            }
        }
        else
        {
            this.elapsedTime += frameTime.ElapsedTime.Milliseconds;
        }

        this.grpInstructions.Position = new Point(WindowCenter.X - this.grpInstructions.HalfWidth, WindowPadding);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        this.textureRenderer.Render(
            this.mainAtlas,
            "samus",
            new Vector2(WindowCenter.X, WindowCenter.Y),
            0,
            3f,
            this.currentFrame);

        this.grpInstructions.Render();
        this.grpAnimation.Render();

        base.Render();
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
