// <copyright file="CameraScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Scenes;

using System;
using System.Drawing;
using System.Numerics;
using Velum;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
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
    private const float CamZoomSpeed = 1.0f;
    private const float CameraAngleSpeed = 40f;
    private const int WindowPadding = 15;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IContentManager contentManager;
    private readonly BackgroundManager backgroundManager;
    private readonly ICamera2D camera;
    private readonly Label lblInstructions;
    private Vector2 worldMapPos;
    private ITexture? zeldaMapTexture;
    private KeyboardState prevKeyState;
    private IFont? font;
    private float mapAngle;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScene"/> class.
    /// </summary>
    public CameraScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();

        this.backgroundManager = new BackgroundManager();
        this.contentManager = ContentManager.Create();
        this.camera = CameraFactory.CreateCamera();

        this.lblInstructions = new Label();
        this.lblInstructions.Text = "Camera Controls:\n" +
            "- Arrow Keys: Pan the camera\n" +
            "- Shift + Up Arrow: Zoom in\n" +
            "- Shift + Down Arrow: Zoom out\n" +
            "- Ctrl + Right Arrow: Rotate CC\n" +
            "- Ctrl + Left Arrow: Rotate CWW\n" +
            "- Esc: Reset";
        this.lblInstructions.TextColor = Color.White;
    }

    public override void LoadContent()
    {
        this.backgroundManager.Load(new Vector2(WindowCenter.X, WindowCenter.Y));
        this.lblInstructions.Load();

        this.worldMapPos = new Vector2(WindowCenter.X, WindowCenter.Y);
        this.zeldaMapTexture = this.contentManager.Load<ITexture>("zelda-light-world");
        this.font = this.contentManager.LoadFont(Program.DefaultFontBold, 24);

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.backgroundManager.Unload();
        this.lblInstructions.Unload();
        this.contentManager.Unload(this.zeldaMapTexture);
        this.contentManager.Unload(this.font);

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
        this.lblInstructions.Update();
        this.lblInstructions.Position = new Vector2(WindowCenter.X - this.lblInstructions.HalfWidth, WindowPadding);

        var camVelocityX = (float)frameTime.ElapsedTime.TotalSeconds * CamPanSpeed;
        var camVelocityY = (float)frameTime.ElapsedTime.TotalSeconds * CamPanSpeed;
        var camZoomVelocityIn = (float)frameTime.ElapsedTime.TotalSeconds * CamZoomSpeed;
        var camZoomVelocityOut = (float)frameTime.ElapsedTime.TotalSeconds * CamZoomSpeed;
        var camAngleVelocity = (float)frameTime.ElapsedTime.TotalSeconds * CameraAngleSpeed;

        var currentKeyState = this.keyboard.GetState();

        var isShiftDown = currentKeyState.IsKeyDown(KeyCode.LeftShift) ||
            currentKeyState.IsKeyDown(KeyCode.RightShift);

        var isCtrlDown = currentKeyState.IsKeyDown(KeyCode.LeftControl) ||
            currentKeyState.IsKeyDown(KeyCode.RightControl);

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
        else if (isCtrlDown)
        {
            if (currentKeyState.IsKeyDown(KeyCode.Left))
            {
                // this.mapAngle -= camAngleVelocity;
                this.camera.Rotation -= camAngleVelocity;
            }

            if (currentKeyState.IsKeyDown(KeyCode.Right))
            {
                // this.mapAngle += camAngleVelocity;
                this.camera.Rotation += camAngleVelocity;
            }
        }
        else if (this.prevKeyState.IsKeyDown(KeyCode.Escape) && currentKeyState.IsKeyUp(KeyCode.Escape))
        {
            this.worldMapPos = new Vector2(WindowCenter.X, WindowCenter.Y);
            this.camera.Zoom = 0.0f;
            this.camera.Position = new Vector2(WindowCenter.X, WindowCenter.Y);
            this.camera.Rotation = 0.0f;
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

        this.prevKeyState = currentKeyState;

        base.Update(frameTime);
    }

    public override void Render()
    {
        if (this.font is null)
        {
            throw new InvalidOperationException("Font was not loaded.");
        }

        this.backgroundManager.Render();

        var mapPos = this.camera.TransformPosition(this.worldMapPos);
        var textureScale = this.camera.TransformSize(2f);
        // var angle = this.camera.TransformRotation(this.mapAngle);

        this.textureRenderer.Render(
            this.zeldaMapTexture,
            mapPos,
            // angle,
            this.camera.Rotation,
            textureScale,
            -100);

        this.lblInstructions.Render();

        base.Render();
    }
}
