// <copyright file="AnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Numerics;
using UILib;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using VelUpdatable = Velaptor.IUpdatable;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class AnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 25;
    private readonly ITextureRenderer textureRenderer;
    private readonly BackgroundManager backgroundManager;
    private readonly IContentManager contentManager;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly Container conMain;
    private IAtlasData? mainAtlas;
    private AtlasSubTextureData[]? frames;
    private KeyboardState prevKeyState;
    private Layout layDirection;
    private Layout laySpeed;
    private Option? optForward;
    private Option? optBackward;
    private Label? lblSpeed;
    private Slider? sldSpeed;
    private int elapsedTime;
    private int currentFrame;
    private float animSpeed = 32;
    private bool runningForward = true;
    private float speed = 60;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedGraphicsScene"/> class.
    /// </summary>
    public AnimatedGraphicsScene()
    {
        this.backgroundManager = new BackgroundManager();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.contentManager = ContentManager.Create();
        this.keyboard = HardwareFactory.GetKeyboard();

        CreateOptionCtrls();
        CreateSpeedCtrls();

        var layMain = new Layout();
        layMain.AddControl(this.layDirection);
        layMain.AddControl(this.laySpeed);

        this.conMain = new Container();
        this.conMain.AddLayoutControl(layMain);
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.mainAtlas = this.contentManager.Load<IAtlasData>("Main-Atlas");
        this.frames = this.mainAtlas.GetFrames("samus");

        this.conMain.Load();
        this.conMain.Position = new Vector2(WindowPadding, WindowCenter.Y - this.conMain.HalfHeight);

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.contentManager.Unload(this.mainAtlas);
        this.mainAtlas = null;
        this.backgroundManager.Unload();
        this.conMain.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="VelUpdatable.Update"/>
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

        this.conMain.Update();

        this.animSpeed = 1000f / this.speed;

        this.sldSpeed.Value = (float)Math.Round(this.speed, 2);

        ProcessInput();
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

        this.conMain.Render();

        base.Render();
    }

    private void CreateOptionCtrls()
    {
        this.optForward = new Option
        {
            Text = "Forwards",
            IsChecked = true,
            GroupNumber = 1,
        };
        this.optForward.CheckChanged += (_, args) => this.runningForward = !args.IsChecked;

        this.optBackward = new Option
        {
            Text = "Backwards",
            GroupNumber = 1,
        };
        this.optBackward.CheckChanged += (_, args) => this.runningForward = args.IsChecked;

        this.layDirection = new Layout();
        this.layDirection.StackDirection = StackDirection.Vertical;

        this.layDirection.AddControl(this.optForward);
        this.layDirection.AddControl(this.optBackward);
    }

    private void CreateSpeedCtrls()
    {
        this.lblSpeed = new Label
        {
            Text = "Speed(fps):",
        };

        this.sldSpeed = new Slider
        {
            Max = 60,
            Value = 60,
        };
        this.sldSpeed.ValueChanged += (_, args) => this.speed = args.NewValue;

        this.laySpeed = new Layout();
        this.laySpeed.StackDirection = StackDirection.Horizontal;
        this.laySpeed.AddControl(this.lblSpeed);
        this.laySpeed.AddControl(this.sldSpeed);
    }

    private void ProcessInput()
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.IsKeyUp(KeyCode.Right) && this.prevKeyState.IsKeyDown(KeyCode.Right))
        {
            this.runningForward = true;
        }

        if (currentKeyState.IsKeyUp(KeyCode.Left) && this.prevKeyState.IsKeyDown(KeyCode.Left))
        {
            this.runningForward = false;
        }

        if (currentKeyState.IsKeyDown(KeyCode.Up))
        {
            this.speed += 0.5f;
        }

        if (currentKeyState.IsKeyDown(KeyCode.Down))
        {
            this.speed -= 0.5f;
        }

        this.speed = this.speed < 0 ? 0 : this.speed;
        this.speed = this.speed > 60 ? 60 : this.speed;

        this.prevKeyState = currentKeyState;
    }
}
