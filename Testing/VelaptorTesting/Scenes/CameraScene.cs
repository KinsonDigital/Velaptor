// <copyright file="CameraScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using System.Numerics;
using UILib;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using VelaptorTesting;

/// <summary>
/// Tests the Camera system.
/// </summary>
public class CameraScene : SceneBase
{
    private const float CamPanSpeed = 300f;
    private const float CamZoomSpeed = 0.6f;
    private const int WindowPadding = 15;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IContentManager contentManager;
    private readonly ICamera2D camera;
    private readonly Label lblHelpText;
    private readonly Label lblInstructions;
    private readonly Color fontClr = Color.CornflowerBlue;
    private Vector2 mapWorldPos;
    private ITexture? zeldaMapTexture;
    private IFont? font;
    private bool renderInstructions = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScene"/> class.
    /// </summary>
    public CameraScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();

        this.contentManager = ContentManager.Create();
        this.camera = CameraFactory.CreateCamera();

        this.lblHelpText = new Label();
        this.lblHelpText.Text = "Ctrl = ?";
        this.lblHelpText.TextColor = this.fontClr;
        this.lblHelpText.Position = new Vector2(WindowPadding, WindowPadding);

        this.lblInstructions = new Label();
        this.lblInstructions.Text = "Camera Controls:\n" +
            "- Arrow Keys: Pan the camera\n" +
            "- Shift + Up Arrow: Zoom in\n" +
            "- Shift + Down Arrow: Zoom out";
        this.lblInstructions.TextColor = this.fontClr;
    }

    public override void LoadContent()
    {
        this.lblHelpText.Load();
        this.lblInstructions.Load();

        this.mapWorldPos = new Vector2(WindowCenter.X, WindowCenter.Y);
        this.zeldaMapTexture = this.contentManager.Load<ITexture>("zelda-light-world");
        this.font = this.contentManager.LoadFont(Program.DefaultFontBold, 24);

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.lblHelpText.Unload();
        this.lblInstructions.Unload();
        this.contentManager.Unload(this.zeldaMapTexture);
        this.contentManager.Unload(this.font);

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.lblHelpText.Update();
        this.lblInstructions.Update();
        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);

        var camVelocityX = (float)frameTime.ElapsedTime.TotalSeconds * CamPanSpeed;
        var camVelocityY = (float)frameTime.ElapsedTime.TotalSeconds * CamPanSpeed;
        var camZoomVelocityIn = (float)frameTime.ElapsedTime.TotalSeconds * CamZoomSpeed;
        var camZoomVelocityOut = (float)frameTime.ElapsedTime.TotalSeconds * CamZoomSpeed;

        var currentKeyState = this.keyboard.GetState();

        // Toggle instructions on/off
        if (currentKeyState.IsKeyDown(KeyCode.LeftControl))
        {
            this.renderInstructions = true;
        }
        else
        {
            this.renderInstructions = false;
        }

        var isShiftDown = currentKeyState.IsKeyDown(KeyCode.LeftShift) ||
            currentKeyState.IsKeyDown(KeyCode.RightShift);

        if (isShiftDown)
        {
            // Zoom in the camera
            if (currentKeyState.IsKeyDown(KeyCode.Up))
            {
                this.camera.Zoom += camZoomVelocityIn;
            }

            // Zoom out the camera
            if (currentKeyState.IsKeyDown(KeyCode.Down))
            {
                this.camera.Zoom -= camZoomVelocityOut;
            }
        }
        else
        {
            // Pan the camera with the arrow keys.
            if (currentKeyState.IsKeyDown(KeyCode.Left))
            {
                this.camera.Position -= new Vector2(camVelocityX, 0f);
            }

            if (currentKeyState.IsKeyDown(KeyCode.Right))
            {
                this.camera.Position += new Vector2(camVelocityX, 0f);
            }

            if (currentKeyState.IsKeyDown(KeyCode.Up))
            {
                this.camera.Position -= new Vector2(0f, camVelocityY);
            }

            if (currentKeyState.IsKeyDown(KeyCode.Down))
            {
                this.camera.Position += new Vector2(0f, camVelocityY);
            }
        }

        base.Update(frameTime);
    }

    public override void Render()
    {
        if (this.font is null)
        {
            throw new InvalidOperationException("Font was not loaded.");
        }

        var mapPos = this.camera.TransformPosition(this.mapWorldPos);
        var textureScale = this.camera.TransformSize(2f);

        this.textureRenderer.Render(
            this.zeldaMapTexture,
            mapPos,
            0f,
            textureScale,
            -100);

        this.lblHelpText.Render();

        if (this.renderInstructions)
        {
            this.lblInstructions.Render();
        }

        base.Render();
    }
}
