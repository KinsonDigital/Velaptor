// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Scenes;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.UI;

/// <summary>
/// The main window to the testing application.
/// </summary>
public class MainWindow : Window
{
    private static readonly char[] UpperCaseChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
        'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
        'U', 'V', 'W', 'X', 'Y', 'Z',
    };
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly Button nextButton;
    private readonly Button previousButton;
    private KeyboardState prevKeyState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.keyboard = InputFactory.CreateKeyboard();

        this.nextButton = new Button { Text = "-->" };
        this.previousButton = new Button { Text = "<--" };

        IRenderer.ClearColor = Color.FromArgb(255, 42, 42, 46);

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

        var textBoxScene = new TextBoxScene
        {
            Name = SplitByUpperCase(nameof(SoundScene)),
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

        var soundScene = new SoundScene
        {
            Name = SplitByUpperCase(nameof(SoundScene)),
        };

        SceneManager.AddScene(textRenderingScene, true);
        SceneManager.AddScene(layeredTextRenderingScene);
        SceneManager.AddScene(keyboardScene);
        SceneManager.AddScene(mouseScene);
        SceneManager.AddScene(textBoxScene);
        SceneManager.AddScene(layeredRenderingScene);
        SceneManager.AddScene(renderNonAnimatedGraphicsScene);
        SceneManager.AddScene(renderAnimatedGraphicsScene);
        SceneManager.AddScene(shapeScene);
        SceneManager.AddScene(layeredRectScene);
        SceneManager.AddScene(lineScene);
        SceneManager.AddScene(layeredLineScene);
        SceneManager.AddScene(soundScene);
    }

    /// <inheritdoc cref="Window.OnLoad"/>
    protected override void OnLoad()
    {
        const int buttonSpacing = 15;
        const int rightMargin = 15;

        this.nextButton.Click += (_, _) => SceneManager.NextScene();

        this.previousButton.Click += (_, _) => SceneManager.PreviousScene();

        this.nextButton.LoadContent();
        this.previousButton.LoadContent();

        var buttonTops = (int)(Height - (new[] { this.nextButton.Height, this.previousButton.Height }.Max() + 20));
        var buttonGroupLeft = (int)(Width - (this.nextButton.Width + this.previousButton.Width + buttonSpacing + rightMargin));
        this.previousButton.Position = new Point(buttonGroupLeft, buttonTops);
        this.nextButton.Position = new Point(this.previousButton.Position.X + (int)this.previousButton.Width + buttonSpacing, buttonTops);

        SceneManager.LoadContent();
        base.OnLoad();
    }

    /// <inheritdoc cref="Window.OnUpdate"/>
    protected override void OnUpdate(FrameTime frameTime)
    {
        SceneManager.Update(frameTime);

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

        this.nextButton.Update(frameTime);
        this.previousButton.Update(frameTime);

        this.prevKeyState = currentKeyState;

        base.OnUpdate(frameTime);
    }

    /// <inheritdoc cref="Window.OnDraw"/>
    protected override void OnDraw(FrameTime frameTime)
    {
        IRenderer.Clear();
        IRenderer.Begin();

        SceneManager.Render();

        // Render the scene manager UI on top of all other textures
        this.nextButton.Render();
        this.previousButton.Render();

        IRenderer.End();
        base.OnDraw(frameTime);
    }

    /// <inheritdoc cref="Window.OnUnload"/>
    protected override void OnUnload()
    {
        this.previousButton.UnloadContent();
        this.nextButton.UnloadContent();

        base.OnUnload();
    }

    /// <summary>
    /// Splits the given <param name="value"></param> based on uppercase characters.
    /// </summary>
    /// <param name="value">The value to split.</param>
    /// <returns>The value returned as a list of sections.</returns>
    private static string SplitByUpperCase(string value)
    {
        var sections = new List<string>();

        var currentSection = string.Empty;

        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];

            if (UpperCaseChars.Contains(character) && i != 0)
            {
                sections.Add(currentSection);

                currentSection = string.Empty;
                currentSection += character;
            }
            else
            {
                currentSection += character;
            }
        }

        sections.Add(currentSection);

        var result = sections.Aggregate(string.Empty, (current, section) => current + $"{section} ");

        return result.TrimEnd(' ');
    }
}
