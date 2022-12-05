// <copyright file="LineRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using Core;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;

/// <summary>
/// Used to test whether or not lines are properly rendered to the screen.
/// </summary>
public class LineRenderingScene : SceneBase
{
    private const float LineMoveSpeed = 200f;
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private IAppInput<MouseState>? mouse;
    private IAppInput<KeyboardState>? keyboard;
    private IFont? font;
    private Line line;
    private MouseState currentMouseState;
    private KeyboardState currentKeyState;
    private float halfWidth;
    private float halfHeight;
    private string instructions = string.Empty;
    private Vector2 instructionsPos;
    private bool mouseEnteredAtLeastOnce;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineRenderingScene"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads content for the scene.</param>
    public LineRenderingScene(IContentLoader contentLoader)
        : base(contentLoader)
    {
    }

    /// <inheritdoc cref="IContentLoadable.LoadContent"/>
    public override void LoadContent()
    {
        this.keyboard = AppInputFactory.CreateKeyboard();
        this.font = ContentLoader.LoadFont(DefaultRegularFont, 12);
        this.mouse = AppInputFactory.CreateMouse();

        this.halfWidth = MainWindow.WindowWidth / 2f;
        this.halfHeight = MainWindow.WindowHeight / 2f;

        this.line = default;
        this.line.Color = Color.SteelBlue;
        this.line.Thickness = 20;
        this.line.P1 = new Vector2(this.halfWidth, this.halfHeight);
        this.line.P2 = new Vector2(this.halfWidth + 100, this.halfHeight);

        var instructionLines = new[]
        {
            "Move the mouse to control the end of the line.",
            "Use the mouse wheel to change the line thickness.",
            "Use the keyboard arrow keys to move the line.",
        };

        this.instructions = string.Join(Environment.NewLine, instructionLines);
        var textSize = this.font.Measure(this.instructions);
        this.instructionsPos = new Vector2(this.halfWidth, (textSize.Height / 2) + 20);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();
        this.currentMouseState = this.mouse.GetState();

        if (this.mouseEnteredAtLeastOnce is false)
        {
            var mousePos = this.currentMouseState.GetPosition();

            if (mousePos != Point.Empty)
            {
                this.mouseEnteredAtLeastOnce = true;
            }
        }

        UpdateLine(frameTime);
        MoveLine(frameTime);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render(IRenderer renderer)
    {
        renderer.Render(this.line);

        renderer.Render(this.font, this.instructions, this.instructionsPos, Color.White);

        base.Render(renderer);
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        ContentLoader.UnloadFont(this.font);

        base.UnloadContent();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed || !IsLoaded)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
        }

        base.Dispose(disposing);
    }

    private void UpdateLine(FrameTime frameTime)
    {
        const float thicknessSpeed = 200f;
        var thicknessChangeAmount = thicknessSpeed * (float)frameTime.ElapsedTime.TotalSeconds;
        this.line.Thickness += this.currentMouseState.GetScrollWheelValue() * thicknessChangeAmount;

        if (this.mouseEnteredAtLeastOnce)
        {
            this.line.P2 = new Vector2(this.currentMouseState.GetPosition().X, this.currentMouseState.GetPosition().Y);
        }
    }

    private void MoveLine(FrameTime frameTime)
    {
        var changeAmount = LineMoveSpeed * (float)frameTime.ElapsedTime.TotalSeconds;

        var velocity = Vector2.Zero;

        if (this.currentKeyState.IsKeyDown(KeyCode.Left))
        {
            velocity.X = changeAmount * -1;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Right))
        {
            velocity.X = changeAmount;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Up))
        {
            velocity.Y = changeAmount * -1;
        }

        if (this.currentKeyState.IsKeyDown(KeyCode.Down))
        {
            velocity.Y = changeAmount;
        }

        this.line.P1 += velocity;
    }
}
