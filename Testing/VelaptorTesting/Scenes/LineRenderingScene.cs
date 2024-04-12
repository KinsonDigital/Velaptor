// <copyright file="LineRenderingScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Used to test whether lines are properly rendered to the screen.
/// </summary>
public class LineRenderingScene : SceneBase
{
    private const int WindowPadding = 10;
    private const float LineMoveSpeed = 200f;
    private IAppInput<MouseState>? mouse;
    private IAppInput<KeyboardState>? keyboard;
    private ILineRenderer? lineRenderer;
    private Line line;
    private MouseState currentMouseState;
    private KeyboardState currentKeyState;
    private BackgroundManager? backgroundManager;
    private IControlGroup? grpControls;
    private bool mouseEnteredAtLeastOnce;

    /// <inheritdoc cref="IContentLoadable.LoadContent"/>
    public override void LoadContent()
    {
        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.keyboard = HardwareFactory.GetKeyboard();
        this.mouse = HardwareFactory.GetMouse();

        this.line = default;
        this.line.Color = Color.SteelBlue;
        this.line.Thickness = 20;
        this.line.P1 = new Vector2(WindowCenter.X, WindowCenter.Y);
        this.line.P2 = new Vector2(WindowCenter.X + 100, WindowCenter.Y);

        var instructionLines = new[]
        {
            "1. Move the mouse to control the end of the line.",
            "2. Use the mouse wheel to change the line thickness.",
            "3. Use the keyboard arrow keys to move the line.",
        };

        var instructions = string.Join(Environment.NewLine, instructionLines);

        var lblInstructions = TestingApp.Container.GetInstance<ILabel>();
        lblInstructions.Name = nameof(lblInstructions);
        lblInstructions.Text = instructions;

        this.grpControls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpControls.Title = "Instructions";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowPadding);
        };

        this.grpControls.Add(lblInstructions);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentKeyState = this.keyboard.GetState();
        this.currentMouseState = this.mouse.GetState();

        if (!this.mouseEnteredAtLeastOnce)
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
    public override void Render()
    {
        this.backgroundManager?.Render();
        this.lineRenderer.Render(this.line);

        this.grpControls.Render();

        base.Render();
    }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager?.Unload();
        this.grpControls.Dispose();
        this.grpControls = null;

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
