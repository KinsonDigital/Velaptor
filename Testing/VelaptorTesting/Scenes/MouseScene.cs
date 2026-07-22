// <copyright file="MouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Numerics;
using System.Text;
using UILib;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Used to test that the mouse works correctly.
/// </summary>
public class MouseScene : SceneBase
{
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblMouseState;
    private readonly IAppInput<MouseState>? mouse;
    private readonly StringBuilder mouseText = new ();
    private MouseScrollDirection scrollDirection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseScene"/> class.
    /// </summary>
    public MouseScene()
    {
        this.mouse = HardwareFactory.GetMouse();
        this.backgroundManager = new BackgroundManager();

        this.lblMouseState = new Label();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblMouseState.Load();

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentMouseState = this.mouse.GetState();

        this.mouseText.Clear();
        this.mouseText.Append("Mouse State");
        this.mouseText.Append($"Mouse Position: {currentMouseState.GetX()}, {currentMouseState.GetY()}");
        this.mouseText.Append($"{Environment.NewLine}Left Button: {(currentMouseState.IsLeftButtonDown() ? "Down" : "Up")}");
        this.mouseText.Append($"{Environment.NewLine}Right Button: {(currentMouseState.IsRightButtonDown() ? "Down" : "Up")}");
        this.mouseText.Append($"{Environment.NewLine}Middle Button: {(currentMouseState.IsMiddleButtonDown() ? "Down" : "Up")}");

        if (currentMouseState.GetScrollWheelValue() != 0)
        {
            this.scrollDirection = currentMouseState.GetScrollDirection();
        }

        this.mouseText.Append($"{Environment.NewLine}Mouse Scroll Direction: {this.scrollDirection}");

        this.lblMouseState.Text = this.mouseText.ToString();
        this.lblMouseState.Position = new Vector2(WindowCenter.X - this.lblMouseState.HalfWidth, WindowCenter.Y - this.lblMouseState.HalfHeight);
        this.lblMouseState.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();
        this.lblMouseState.Render();

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.backgroundManager.Unload();
        this.lblMouseState.Unload();

        base.UnloadContent();
    }
}
