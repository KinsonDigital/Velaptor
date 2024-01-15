// <copyright file="MouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using UI;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Used to test that the mouse works correctly.
/// </summary>
public class MouseScene : SceneBase
{
    private IAppInput<MouseState>? mouse;
    private IControlGroup? grpControls;
    private BackgroundManager? backgroundManager;
    private MouseScrollDirection scrollDirection;
    private string? mouseStateLabelName;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.mouse = HardwareFactory.GetMouse();
        var mouseStateLabel = TestingApp.Container.GetInstance<ILabel>();
        mouseStateLabel.Name = nameof(mouseStateLabel);
        this.mouseStateLabelName = nameof(mouseStateLabel);

        this.grpControls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpControls.Title = "Mouse State";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowCenter.Y - this.grpControls.HalfHeight);
        };

        this.grpControls.Add(mouseStateLabel);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        var currentMouseState = this.mouse.GetState();

        var mouseState = "Mouse State";
        mouseState += $"Mouse Position: {currentMouseState.GetX()}, {currentMouseState.GetY()}";
        mouseState += $"{Environment.NewLine}Left Button: {(currentMouseState.IsLeftButtonDown() ? "Down" : "Up")}";
        mouseState += $"{Environment.NewLine}Right Button: {(currentMouseState.IsRightButtonDown() ? "Down" : "Up")}";
        mouseState += $"{Environment.NewLine}Middle Button: {(currentMouseState.IsMiddleButtonDown() ? "Down" : "Up")}";

        if (currentMouseState.GetScrollWheelValue() != 0)
        {
            this.scrollDirection = currentMouseState.GetScrollDirection();
        }

        mouseState += $"{Environment.NewLine}Mouse Scroll Direction: {this.scrollDirection}";

        var mouseStateLabelCtrl = this.grpControls.GetControl<ILabel>(this.mouseStateLabelName);
        mouseStateLabelCtrl.Text = mouseState;

        this.grpControls.AutoSizeToFitContent = false;
        this.grpControls.AutoSizeToFitContent = true;

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager?.Render();
        this.grpControls.Render();

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.scrollDirection = MouseScrollDirection.None;
        this.mouse = default;

        this.backgroundManager?.Unload();
        this.grpControls.Dispose();
        this.grpControls = null;

        base.UnloadContent();
    }
}
