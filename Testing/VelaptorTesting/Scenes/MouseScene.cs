// <copyright file="MouseScene.cs" company="KinsonDigital">
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
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Used to test that the mouse works correctly.
/// </summary>
public class MouseScene : SceneBase
{
    private readonly BackgroundManager backgroundManager;
    private readonly IContentManager contentManager;
    private readonly IFontRenderer fontRenderer;
    private readonly IAppInput<MouseState>? mouse;
    private readonly StringBuilder mouseText = new ();
    private MouseScrollDirection scrollDirection;
    private IFont? font;
    private Vector2 textPos;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseScene"/> class.
    /// </summary>
    public MouseScene()
    {
        this.mouse = HardwareFactory.GetMouse();
        this.backgroundManager = new BackgroundManager();
        this.contentManager = ContentManager.Create();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.font = this.contentManager.LoadFont(Program.DefaultFontRegular, 12);
        this.textPos = new Vector2(WindowCenter.X, WindowCenter.Y);

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

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();
        this.fontRenderer.Render(this.font, this.mouseText.ToString(), this.textPos);

        base.Render();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.contentManager.Unload(this.font);
        this.backgroundManager.Unload();

        base.UnloadContent();
    }
}
