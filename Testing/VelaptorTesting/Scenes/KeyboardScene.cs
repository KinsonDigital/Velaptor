// <copyright file="KeyboardScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System.Drawing;
using System.Numerics;
using System.Text;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.Scene;
using Velaptor.UI;

/// <summary>
/// Used to test that the keyboard works correctly.
/// </summary>
public class KeyboardScene : SceneBase
{
    private const int TopMargin = 50;
    private readonly IAppInput<KeyboardState> keyboard;
    private Label? downKeys;
    private BackgroundManager? backgroundManager;

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

        var instructions = new Label
        {
            Name = "Instructions",
            Color = Color.White,
            Text = "Hit a key on the keyboard to see if it is correct.",
        };

        instructions.Left = WindowCenter.X - (int)(instructions.Width / 2);
        instructions.Top = (int)(instructions.Height / 2) + TopMargin;

        this.downKeys = new Label
        {
            Name = "DownKeys",
            Color = Color.White,
        };

        AddControl(instructions);
        AddControl(this.downKeys);

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
        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.GetDownKeys().Length > 0)
        {
            var downKeyText = new StringBuilder();

            foreach (var key in currentKeyState.GetDownKeys())
            {
                downKeyText.Append(key);
                downKeyText.Append(", ");
            }

            this.downKeys.Text = downKeyText.ToString().TrimEnd(' ').TrimEnd(',');
        }
        else
        {
            this.downKeys.Text = "No Keys Pressed";
        }

        var posX = WindowCenter.X;
        var posY = WindowCenter.Y;

        this.downKeys.Position = new Point(posX, posY);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager?.Render();
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
