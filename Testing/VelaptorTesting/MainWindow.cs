// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System;
using System.Drawing;
using System.Numerics;
using Scenes;
using Velum;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.UI;

/// <summary>
/// The main window to the testing application.
/// </summary>
public class MainWindow : Window
{
    private const int WindowPadding = 10;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly IBatcher batcher;
    private readonly Button btnPrevScene;
    private readonly Button btnNextScene;
    private KeyboardState prevKeyState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        Width = 1920;
        Height = 1080;
        TypeOfBorder = WindowBorder.Resizable;

        AutoSceneRendering = false;

        this.batcher = RendererFactory.CreateBatcher();
        this.keyboard = HardwareFactory.GetKeyboard();

        this.batcher.ClearColor = Color.FromArgb(255, 42, 42, 46);

        this.btnPrevScene = new Button();
        this.btnPrevScene.Text = "<=";
        this.btnPrevScene.Width = 50;
        this.btnPrevScene.Click += PrevScene_OnClick;

        this.btnNextScene = new Button();
        this.btnNextScene.Text = "=>";
        this.btnNextScene.Width = 50;
        this.btnNextScene.Click += NextScene_OnClick;

        // var textRenderingScene = new TextRenderingScene
        // {
        //     Name = nameof(TextRenderingScene).SplitByUpperCase(),
        // };
        //
        // var layeredTextRenderingScene = new LayeredTextRenderingScene
        // {
        //     Name = nameof(LayeredTextRenderingScene).SplitByUpperCase(),
        // };
        //
        // var cameraScene = new CameraScene
        // {
        //     Name = nameof(CameraScene).SplitByUpperCase(),
        // };
        //
        // var keyboardScene = new KeyboardScene
        // {
        //     Name = nameof(KeyboardScene).SplitByUpperCase(),
        // };
        //
        // var mouseScene = new MouseScene
        // {
        //     Name = nameof(MouseScene).SplitByUpperCase(),
        // };

        var gameCtrlScene = new GameControllerScene { Name = nameof(GameControllerScene).SplitByUpperCase(), };

        // var layeredTextureRenderingScene = new LayeredTextureRenderingScene
        // {
        //     Name = nameof(LayeredTextureRenderingScene).SplitByUpperCase(),
        // };
        //
        // var renderNonAnimatedGraphicsScene = new NonAnimatedGraphicsScene
        // {
        //     Name = nameof(NonAnimatedGraphicsScene).SplitByUpperCase(),
        // };
        //
        // var renderAnimatedGraphicsScene = new AnimatedGraphicsScene
        // {
        //     Name = nameof(AnimatedGraphicsScene).SplitByUpperCase(),
        // };
        //
        // var shapeScene = new ShapeScene
        // {
        //     Name = nameof(ShapeScene).SplitByUpperCase(),
        // };
        //
        // var layeredRectScene = new LayeredRectRenderingScene
        // {
        //     Name = nameof(LayeredRectRenderingScene).SplitByUpperCase(),
        // };
        //
        // var lineScene = new LineRenderingScene
        // {
        //     Name = nameof(LineRenderingScene).SplitByUpperCase(),
        // };
        //
        // var layeredLineScene = new LayeredLineRenderingScene
        // {
        //     Name = nameof(LayeredLineRenderingScene).SplitByUpperCase(),
        // };
        //
        // var batchPerfScene = new BatchPerfScene
        // {
        //     Name = nameof(BatchPerfScene).SplitByUpperCase(),
        // };
        //
        // var audioScene = new AudioScene
        // {
        //     Name = nameof(AudioScene).SplitByUpperCase(),
        // };

        // SceneManager.AddScene(textRenderingScene, true);
        // SceneManager.AddScene(layeredTextRenderingScene);
        // SceneManager.AddScene(cameraScene);
        // SceneManager.AddScene(keyboardScene);
        // SceneManager.AddScene(mouseScene);
        SceneManager.AddScene(gameCtrlScene);
        // SceneManager.AddScene(layeredTextureRenderingScene);
        // SceneManager.AddScene(renderNonAnimatedGraphicsScene);
        // SceneManager.AddScene(renderAnimatedGraphicsScene);
        // SceneManager.AddScene(shapeScene);
        // SceneManager.AddScene(layeredRectScene);
        // SceneManager.AddScene(lineScene);
        // SceneManager.AddScene(layeredLineScene);
        // SceneManager.AddScene(batchPerfScene);
        // SceneManager.AddScene(audioScene);
    }

    protected override void OnLoad()
    {
        this.btnPrevScene.Load();
        this.btnNextScene.Load();

        base.OnLoad();
    }

    protected override void OnUnload()
    {
        this.btnPrevScene.Click -= PrevScene_OnClick;
        this.btnNextScene.Click -= NextScene_OnClick;

        this.btnPrevScene.Unload();
        this.btnNextScene.Unload();

        base.OnUnload();
    }

    /// <inheritdoc cref="Window.OnUpdate"/>
    protected override void OnUpdate(FrameTime frameTime)
    {
        Title = $"Scene: {SceneManager.CurrentScene?.Name ?? "No Scene Loaded"}";

        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.IsKeyUp(KeyCode.PageDown) && this.prevKeyState.IsKeyDown(KeyCode.PageDown))
        {
            SceneManager.NextScene();
        }

        if (currentKeyState.IsKeyUp(KeyCode.PageUp) && this.prevKeyState.IsKeyDown(KeyCode.PageUp))
        {
            SceneManager.PreviousScene();
        }

        this.btnNextScene.Position = new Vector2(
            Width - (this.btnNextScene.Width + WindowPadding),
            Height - (this.btnNextScene.Height + WindowPadding));

        this.btnPrevScene.Position = new Vector2(this.btnNextScene.Left - this.btnNextScene.Width - WindowPadding, this.btnNextScene.Top);

        this.btnPrevScene.Update();
        this.btnNextScene.Update();

        this.prevKeyState = currentKeyState;

        base.OnUpdate(frameTime);
    }

    /// <inheritdoc cref="Window.OnDraw"/>
    protected override void OnDraw(FrameTime frameTime)
    {
        this.batcher.Begin();

        SceneManager.Render();

        this.btnPrevScene.Render(int.MaxValue - 100);
        this.btnNextScene.Render(int.MaxValue - 100);

        base.OnDraw(frameTime);

        this.batcher.End();
    }

    private void PrevScene_OnClick(object? sender, EventArgs e)
    {
        SceneManager.PreviousScene();
    }

    private void NextScene_OnClick(object? sender, EventArgs e)
    {
        SceneManager.NextScene();
    }
}
