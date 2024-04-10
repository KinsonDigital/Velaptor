// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Scenes;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.UI;
using UI;

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
    private readonly IControlGroup grpSceneCtrls;
    private KeyboardState prevKeyState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.batcher = RendererFactory.CreateBatcher();

        this.keyboard = HardwareFactory.GetKeyboard();

        this.batcher.ClearColor = Color.FromArgb(255, 42, 42, 46);

        this.grpSceneCtrls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpSceneCtrls.Title = "Scene Group";
        this.grpSceneCtrls.TitleBarVisible = false;
        this.grpSceneCtrls.AutoSizeToFitContent = true;
        this.grpSceneCtrls.Initialized += (_, _) =>
        {
            this.grpSceneCtrls.Position = new Point(
                (int)Width - (this.grpSceneCtrls.Width + WindowPadding),
                (int)Height - (this.grpSceneCtrls.Height + WindowPadding));
        };

        var nextPrevious = TestingApp.Container.GetInstance<INextPrevious>();
        nextPrevious.Next += (_, _) => SceneManager.NextScene();
        nextPrevious.Previous += (_, _) => SceneManager.PreviousScene();

        this.grpSceneCtrls.Add(nextPrevious);

        var textRenderingScene = new TextRenderingScene
        {
            Name = SplitByUpperCase(nameof(TextRenderingScene)),
        };

        var layeredTextRenderingScene = new LayeredTextRenderingScene
        {
            Name = SplitByUpperCase(nameof(LayeredTextRenderingScene)),
        };

        var keyboardScene = new KeyboardScene
        {
            Name = SplitByUpperCase(nameof(KeyboardScene)),
        };

        var mouseScene = new MouseScene
        {
            Name = SplitByUpperCase(nameof(MouseScene)),
        };

        var layeredRenderingScene = new LayeredTextureRenderingScene
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

        var audioScene = new AudioScene
        {
            Name = SplitByUpperCase(nameof(AudioScene)),
        };

        SceneManager.AddScene(textRenderingScene, true);
        SceneManager.AddScene(layeredTextRenderingScene);
        SceneManager.AddScene(keyboardScene);
        SceneManager.AddScene(mouseScene);
        SceneManager.AddScene(layeredRenderingScene);
        SceneManager.AddScene(renderNonAnimatedGraphicsScene);
        SceneManager.AddScene(renderAnimatedGraphicsScene);
        SceneManager.AddScene(shapeScene);
        SceneManager.AddScene(layeredRectScene);
        SceneManager.AddScene(lineScene);
        SceneManager.AddScene(layeredLineScene);
        SceneManager.AddScene(audioScene);
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

        this.prevKeyState = currentKeyState;

        base.OnUpdate(frameTime);
    }

    /// <inheritdoc cref="Window.OnDraw"/>
    protected override void OnDraw(FrameTime frameTime)
    {
        base.OnDraw(frameTime);

        // Render the buttons after the 'base.OnDraw()'.  With the rendering being set to auto,
        // additional drawings have to be done after the base.OnDraw() call with the use of
        // the 'Begin()' and 'End()` methods.
        this.batcher.Begin();

        this.grpSceneCtrls.Render();

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
}
