// <copyright file="AnimatedGraphicsScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Tests that animated graphics properly render to the screen.
/// </summary>
public class AnimatedGraphicsScene : SceneBase
{
    private const int WindowPadding = 100;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly BackgroundManager backgroundManager;
    private readonly IContentManager contentManager;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly StringBuilder animationFps = new ("Speed(fps): 60.00");
    private string instructions = string.Empty;
    private IAtlasData? mainAtlas;
    private AtlasSubTextureData[]? frames;
    private IFont? font;
    private int elapsedTime;
    private int currentFrame;
    private float animSpeed = 32;
    private bool runningForward = true;
    private KeyboardState prevKeyState;
    private float speed = 60;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedGraphicsScene"/> class.
    /// </summary>
    public AnimatedGraphicsScene()
    {
        this.backgroundManager = new BackgroundManager();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.contentManager = ContentManager.Create();
        this.keyboard = HardwareFactory.GetKeyboard();
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
        this.font = this.contentManager.LoadFont(Program.DefaultFontName, 12);

        var textLines = new string[]
        {
            "Verify that the Samus is running.",
            "Use the controls to change the direction and speed of the animation.",
            "1. Press right arrow key to run forwards",
            "2. Press left arrow key to run backwards",
            "3. Press up/down arrow keys to change the speed of the animation",
        };
        this.instructions = string.Join(Environment.NewLine, textLines);

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
        this.contentManager.Unload(this.font);
        this.mainAtlas = null;
        this.font = null;

        this.backgroundManager.Unload();

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

        this.animSpeed = 1000f / this.speed;
        this.animationFps.Clear();
        this.animationFps.Append($"Speed(fps): {Math.Round(this.speed, 2):F2}");

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

        this.fontRenderer.Render(this.font, this.instructions, new Vector2(WindowCenter.X, WindowPadding));
        this.fontRenderer.Render(this.font, this.animationFps.ToString(), new Vector2(WindowCenter.X, WindowPadding + 300));

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
