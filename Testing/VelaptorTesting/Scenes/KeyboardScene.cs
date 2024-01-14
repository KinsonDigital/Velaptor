// <copyright file="KeyboardScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Drawing;
using System.Numerics;
using System.Text;
using UI;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;

/// <summary>
/// Used to test that the keyboard works correctly.
/// </summary>
public class KeyboardScene : SceneBase
{
    private readonly IAppInput<KeyboardState> keyboard;
    private BackgroundManager? backgroundManager;
    private IControlGroup? grpControls;
    private string? downKeysName;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardScene"/> class.
    /// </summary>
    public KeyboardScene() => this.keyboard = HardwareFactory.GetKeyboard();

    /// <inheritdoc cref="IScene.LoadContent"/>.
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager = new BackgroundManager();
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));

        var instructions = TestingApp.Container.GetInstance<ILabel>();
        instructions.Name = nameof(instructions);

        instructions.Text = "Hit a key on the keyboard to see if it is correct.";

        var downKeys = TestingApp.Container.GetInstance<ILabel>();
        downKeys.Name = nameof(downKeys);
        this.downKeysName = nameof(downKeys);

        this.grpControls = TestingApp.Container.GetInstance<IControlGroup>();
        this.grpControls.Title = "Keyboard Info";
        this.grpControls.AutoSizeToFitContent = true;
        this.grpControls.TitleBarVisible = false;
        this.grpControls.Initialized += (_, _) =>
        {
            this.grpControls.Position = new Point(WindowCenter.X - this.grpControls.HalfWidth, WindowCenter.Y - this.grpControls.HalfHeight);
        };

        this.grpControls.Add(downKeys);
        this.grpControls.Add(instructions);

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>.
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

    /// <inheritdoc cref="IUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        var downKeysCtrl = this.grpControls.GetControl<ILabel>(this.downKeysName);

        if (currentKeyState.GetDownKeys().Length > 0)
        {
            var downKeyText = new StringBuilder();

            foreach (var key in currentKeyState.GetDownKeys())
            {
                downKeyText.Append(key);
                downKeyText.Append(", ");
            }

            downKeysCtrl.Text = downKeyText.ToString().TrimEnd(' ').TrimEnd(',');
        }
        else
        {
            downKeysCtrl.Text = "No Keys Pressed";
        }

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager?.Render();

        this.grpControls.Render();
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
}
