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
    private readonly IContentManager contentManager;
    private readonly IFontRenderer fontRenderer;
    private readonly StringBuilder downKeyText = new (Instructions);
    private IFont? font;
    private Vector2 textPos = Vector2.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardScene"/> class.
    /// </summary>
    public KeyboardScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.backgroundManager = new BackgroundManager();
        this.contentManager = ContentManager.Create();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
    }

    /// <inheritdoc cref="IScene.LoadContent"/>.
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

    /// <inheritdoc cref="IScene.UnloadContent"/>.
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

    /// <inheritdoc cref="IUpdatable.Update"/>.
    public override void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.GetDownKeys().Length > 0)
        {
            this.downKeyText.Clear();

            foreach (var key in currentKeyState.GetDownKeys())
            {
                this.downKeyText.Append(key);
                this.downKeyText.Append(", ");
            }

            this.downKeyText.ToString().TrimEnd(' ').TrimEnd(',');
        }
        else
        {
            this.downKeyText.Clear();
            this.downKeyText.Append("No Keys Pressed");
        }

        base.Update(frameTime);
    }

    /// <inheritdoc cref="IDrawable.Render"/>
    public override void Render()
    {
        this.backgroundManager.Render();

        var instructionsPos = new Vector2(WindowCenter.X, WindowCenter.Y - 50);
        this.fontRenderer.Render(this.font, Instructions, instructionsPos);
        this.fontRenderer.Render(this.font, this.downKeyText.ToString(), this.textPos);

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
