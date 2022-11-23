// <copyright file="MainWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.UI;
using VelaptorTesting.Core;
using VelaptorTesting.Scenes;

namespace VelaptorTesting;

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
    private readonly SceneManager sceneManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="window">The native window implementation.</param>
    public MainWindow(IWindow window)
        : base(window)
    {
        var contentLoader = ContentLoaderFactory.CreateContentLoader();

        WindowWidth = Width;
        WindowHeight = Height;
        var renderer = RendererFactory.CreateRenderer(Width, Height);
        renderer.ClearColor = Color.FromArgb(255, 42, 42, 46);
        window.WinResize = (size) =>
        {
            renderer.RenderSurfaceWidth = size.Width;
            renderer.RenderSurfaceHeight = size.Height;
        };

        this.sceneManager = new SceneManager(renderer);

        var renderTextScene = new RenderTextScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(RenderTextScene)),
        };

        var layeredTextRenderingScene = new LayeredTextRenderingScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(LayeredTextRenderingScene)),
        };

        var mouseScene = new MouseScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(MouseScene)),
        };

        var keyboardScene = new KeyboardScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(KeyboardScene)),
        };

        var renderNonAnimatedGraphicsScene = new NonAnimatedGraphicsScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(NonAnimatedGraphicsScene)),
        };

        var layeredRenderingScene = new LayeredTextureRenderingScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(LayeredTextureRenderingScene)),
        };

        var renderAnimatedGraphicsScene = new AnimatedGraphicsScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(AnimatedGraphicsScene)),
        };

        var soundScene = new SoundScene(ContentLoader)
        {
            Name = SplitByUpperCase(nameof(SoundScene)),
        };

        var layeredRectScene = new LayeredRectRenderingScene(contentLoader)
        {
            Name = SplitByUpperCase(nameof(LayeredRectRenderingScene)),
        };

        var rectScene = new RectangleScene(ContentLoader)
        {
            Name = SplitByUpperCase(nameof(RectangleScene)),
        };

        this.sceneManager.AddScene(renderTextScene, true);
        this.sceneManager.AddScene(layeredTextRenderingScene);
        this.sceneManager.AddScene(keyboardScene);
        this.sceneManager.AddScene(renderNonAnimatedGraphicsScene);
        this.sceneManager.AddScene(layeredRenderingScene);
        this.sceneManager.AddScene(renderAnimatedGraphicsScene);
        this.sceneManager.AddScene(soundScene);
        this.sceneManager.AddScene(layeredRectScene);
        this.sceneManager.AddScene(rectScene);
    }

    /// <summary>
    /// Gets the width of the window.
    /// </summary>
    public static uint WindowWidth { get; private set; }

    /// <summary>
    /// Gets the height of the window.
    /// </summary>
    public static uint WindowHeight { get; private set; }

    /// <inheritdoc cref="Window.OnLoad"/>
    protected override void OnLoad()
    {
        this.sceneManager.LoadContent();
        base.OnLoad();
    }

    /// <inheritdoc cref="Window.OnUpdate"/>
    protected override void OnUpdate(FrameTime frameTime)
    {
        this.sceneManager.Update(frameTime);

        Title = $"Scene: {this.sceneManager.CurrentScene?.Name ?? "No Scene Loaded"}";

        base.OnUpdate(frameTime);
    }

    /// <inheritdoc cref="Window.OnDraw"/>
    protected override void OnDraw(FrameTime frameTime)
    {
        this.sceneManager.Render();

        base.OnDraw(frameTime);
    }

    /// <inheritdoc cref="Window.OnUnload"/>
    protected override void OnUnload()
    {
        this.sceneManager.UnloadContent();
        base.OnUnload();
    }

    /// <inheritdoc cref="Window.OnResize"/>
    protected override void OnResize(SizeU size)
    {
        WindowWidth = Width;
        WindowHeight = Height;

        base.OnResize(size);
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
