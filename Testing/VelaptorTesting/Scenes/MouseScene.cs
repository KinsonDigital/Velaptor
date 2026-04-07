// <copyright file="MouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using KdGui;
using KdGui.Factories;
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
    private IAppInput<MouseState>? mouse;
    private IControlGroup? grpControls;
    private MouseScrollDirection scrollDirection;
    private string? mouseStateLabelName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseScene"/> class.
    /// </summary>
    public MouseScene() => this.backgroundManager = new BackgroundManager();

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        this.mouse = HardwareFactory.GetMouse();

        var ctrlFactory = new ControlFactory();
        var mouseStateLabel = ctrlFactory.CreateLabel();
        mouseStateLabel.Name = nameof(mouseStateLabel);
        this.mouseStateLabelName = nameof(mouseStateLabel);

        this.grpControls = ctrlFactory.CreateControlGroup();
        this.grpControls.Title = "Mouse State";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;

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

        this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowCenter.Y - this.grpControls.HalfHeight);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();
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
        this.mouse = null;

        this.backgroundManager.Unload();
        this.grpControls.Dispose();
        this.grpControls = null;

        base.UnloadContent();
    }
}
