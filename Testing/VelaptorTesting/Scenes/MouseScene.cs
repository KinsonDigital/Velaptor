// <copyright file="MouseScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using Velaptor.Scene;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.UI;

/// <summary>
/// Used to test that the mouse works correctly.
/// </summary>
public class MouseScene : SceneBase
{
    private IAppInput<MouseState> mouse;
    private Label? mouseInfoLabel;
    private MouseState currentMouseState;
    private MouseScrollDirection scrollDirection;

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.mouse = AppInputFactory.CreateMouse();
        this.mouseInfoLabel = new Label { Color = Color.White };

        this.mouseInfoLabel.LoadContent();
        this.mouseInfoLabel.Position = new Point(WindowCenter.X, WindowCenter.Y);

        AddControl(this.mouseInfoLabel);

        base.LoadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>
    public override void Update(FrameTime frameTime)
    {
        this.currentMouseState = this.mouse.GetState();

        var mouseInfo = $"Mouse Position: {this.currentMouseState.GetX()}, {this.currentMouseState.GetY()}";
        mouseInfo += $"{Environment.NewLine}Left Button: {(this.currentMouseState.IsLeftButtonDown() ? "Down" : "Up")}";
        mouseInfo += $"{Environment.NewLine}Right Button: {(this.currentMouseState.IsRightButtonDown() ? "Down" : "Up")}";
        mouseInfo += $"{Environment.NewLine}Middle Button: {(this.currentMouseState.IsMiddleButtonDown() ? "Down" : "Up")}";

        if (this.currentMouseState.GetScrollWheelValue() != 0)
        {
            this.scrollDirection = this.currentMouseState.GetScrollDirection();
        }

        mouseInfo += $"{Environment.NewLine}Mouse Scroll Direction: {this.scrollDirection}";

        this.mouseInfoLabel.Text = mouseInfo;

        base.Update(frameTime);
    }

    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.currentMouseState = default;
        this.scrollDirection = MouseScrollDirection.None;
        this.mouseInfoLabel = null;
        this.mouse = default;

        base.UnloadContent();
    }
}
