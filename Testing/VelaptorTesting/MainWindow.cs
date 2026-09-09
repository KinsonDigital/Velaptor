// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Scenes;
using Velum;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.UI;
using Velaptor.WebGpu.Batching;

/// <summary>
/// The main window to the testing application.
/// </summary>
public class MainWindow : Window
{
    private const int WindowPadding = 10;
    private static readonly char[] UpperCaseChars =
    [
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
        'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
        'U', 'V', 'W', 'X', 'Y', 'Z'
    ];
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
        TypeOfBorder = WindowBorder.Fixed;

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

        var textRenderingScene = new TextRenderingScene
        {
            Name = SplitByUpperCase(nameof(TextRenderingScene)),
        };

        var layeredTextRenderingScene = new LayeredTextRenderingScene
        {
            Name = SplitByUpperCase(nameof(LayeredTextRenderingScene)),
        };

        var cameraScene = new CameraScene
        {
            Name = SplitByUpperCase(nameof(CameraScene)),
        };

        var keyboardScene = new KeyboardScene
        {
            Name = SplitByUpperCase(nameof(KeyboardScene)),
        };

        var mouseScene = new MouseScene
        {
            Name = SplitByUpperCase(nameof(MouseScene)),
        };

        var layeredTextureRenderingScene = new LayeredTextureRenderingScene
        {
            Name = SplitByUpperCase(nameof(LayeredTextureRenderingScene)),
        };

        var renderNonAnimatedGraphicsScene = new NonAnimatedGraphicsScene
        {
            Name = SplitByUpperCase(nameof(NonAnimatedGraphicsScene)),
        };

        var renderAnimatedGraphicsScene = new AnimatedGraphicsScene
        {
            Name = SplitByUpperCase(nameof(AnimatedGraphicsScene)),
        };

        var shapeScene = new ShapeScene
        {
            Name = SplitByUpperCase(nameof(ShapeScene)),
        };

        var layeredRectScene = new LayeredRectRenderingScene
        {
            Name = SplitByUpperCase(nameof(LayeredRectRenderingScene)),
        };

        var lineScene = new LineRenderingScene
        {
            Name = SplitByUpperCase(nameof(LineRenderingScene)),
        };

        var layeredLineScene = new LayeredLineRenderingScene
        {
            Name = SplitByUpperCase(nameof(LayeredLineRenderingScene)),
        };

        var batchPerfScene = new BatchPerfScene
        {
            Name = SplitByUpperCase(nameof(BatchPerfScene)),
        };

        var audioScene = new AudioScene
        {
            Name = SplitByUpperCase(nameof(AudioScene)),
        };

        SceneManager.AddScene(textRenderingScene, true);
        SceneManager.AddScene(layeredTextRenderingScene);
        SceneManager.AddScene(cameraScene);
        SceneManager.AddScene(keyboardScene);
        SceneManager.AddScene(mouseScene);
        SceneManager.AddScene(layeredTextureRenderingScene);
        SceneManager.AddScene(renderNonAnimatedGraphicsScene);
        SceneManager.AddScene(renderAnimatedGraphicsScene);
        SceneManager.AddScene(shapeScene);
        SceneManager.AddScene(layeredRectScene);
        SceneManager.AddScene(lineScene);
        SceneManager.AddScene(layeredLineScene);
        SceneManager.AddScene(batchPerfScene);
        SceneManager.AddScene(audioScene);
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

    /// <summary>
    /// Splits the given <param name="value"></param> based on uppercase characters.
    /// </summary>
    /// <param name="value">The value to split.</param>
    /// <returns>The value returned as a list of sections.</returns>
    private static string SplitByUpperCase(string value)
    {
        var sections = new List<string>();

        var currentSection = new StringBuilder();

        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];

            if (UpperCaseChars.Contains(character) && i != 0)
            {
                sections.Add(currentSection.ToString());

                currentSection.Clear();
                currentSection.Append(character);
            }
            else
            {
                currentSection.Append(character);
            }
        }

        sections.Add(currentSection.ToString());

        var result = sections.Aggregate(string.Empty, (current, section) => current + $"{section} ");

        return result.TrimEnd(' ');
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
