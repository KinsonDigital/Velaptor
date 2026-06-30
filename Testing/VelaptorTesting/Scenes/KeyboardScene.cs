// <copyright file="KeyboardScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

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
/// Used to test that the keyboard works correctly.
/// </summary>
public class KeyboardScene : SceneBase
{
    private const string Instructions = "Hit a key on the keyboard to see if it is correct.";
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly BackgroundManager backgroundManager;
    private readonly Label lblInstructions;
    private readonly Label lblDownKeys;
    private readonly StringBuilder downKeyText = new (Instructions);

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardScene"/> class.
    /// </summary>
    public KeyboardScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.backgroundManager = new BackgroundManager();

        this.lblInstructions = new Label
        {
            Text = Instructions,
        };

        this.lblDownKeys = new Label();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>.
    public override void LoadContent()
    {
        if (IsLoaded)
        {
            return;
        }

        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblInstructions.Load();
        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowCenter.Y - this.lblInstructions.HalfHeight);

        this.lblDownKeys.Load();

        base.LoadContent();
    }

    /// <inheritdoc cref="IScene.UnloadContent"/>.
    public override void UnloadContent()
    {
        if (!IsLoaded || IsDisposed)
        {
            return;
        }

        this.lblInstructions.Unload();
        this.lblDownKeys.Unload();
        this.backgroundManager.Unload();

        base.UnloadContent();
    }

    /// <inheritdoc cref="IUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.GetDownKeys().Length > 0)
        {
            this.downKeyText.Clear();

            var keys = currentKeyState.GetDownKeys();
            for (var i = 0; i < keys.Length; i++)
            {
                this.downKeyText.Append(keys[i]);
                this.downKeyText.Append(i == keys.Length - 1 ? string.Empty : ", ");
            }
        }
        else
        {
            this.downKeyText.Clear();
            this.downKeyText.Append("No Keys Pressed");
        }

        this.lblDownKeys.Text = this.downKeyText.ToString();
        this.lblDownKeys.Position = new Vector2(WindowCenter.X - this.lblDownKeys.HalfWidth, WindowCenter.Y - this.lblDownKeys.HalfHeight + 50);
        this.lblDownKeys.Update();
        this.lblInstructions.Update();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        this.lblInstructions.Render();
        this.lblDownKeys.Render();

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
