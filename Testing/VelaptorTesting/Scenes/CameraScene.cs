// <copyright file="CameraScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using System.Numerics;
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
    private const int WindowPadding = 100;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IShapeRenderer shapeRenderer;
    private readonly IContentManager contentManager;
    private readonly ICamera2D camera;
    private readonly string helpText = "Hold the Ctrl key to view instructions.";
    private string instructions = string.Empty;
    private Vector2 helpTextPos;
    private Vector2 instructionTextPos;
    private Vector2 mapWorldPos;
    private Vector2 cameraVelocity;
    private ITexture? zeldaMapTexture;
    private IFont? font;
    private KeyboardState prevKeyState;
    private RectShape helpBackground;
    private RectShape instructionBackground;
    private bool renderInstructions = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScene"/> class.
    /// </summary>
    public CameraScene()
    {
        this.keyboard = HardwareFactory.GetKeyboard();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.shapeRenderer = RendererFactory.CreateShapeRenderer();

        this.contentManager = ContentManager.Create();
        this.camera = CameraFactory.CreateCamera();
    }

    public override void LoadContent()
    {
        this.mapWorldPos = new Vector2(WindowCenter.X, WindowCenter.Y);
        this.zeldaMapTexture = this.contentManager.Load<ITexture>("zelda-light-world");
        this.font = this.contentManager.LoadFont(Program.DefaultFontBold, 24);
        this.instructions = "Camera Controls:\n" +
            "- Arrow Keys: Pan the camera\n" +
            "- Shift + Up Arrow: Zoom in\n" +
            "- Shift + Down Arrow: Zoom out";

        this.helpTextPos = new Vector2(WindowCenter.X, WindowPadding + 50);
        var helpTextSize = this.font.Measure(this.helpText);

        this.helpBackground = default;
        this.helpBackground.Position = new Vector2(this.helpTextPos.X, this.helpTextPos.Y);
        this.helpBackground.Width = helpTextSize.Width + (helpTextSize.Width *= 0.05f);
        this.helpBackground.Height = helpTextSize.Height + (helpTextSize.Height *= 0.80f);
        this.helpBackground.IsSolid = true;
        this.helpBackground.Color = Color.CornflowerBlue;

        this.instructionTextPos = new Vector2(WindowCenter.X, WindowPadding + 150);
        var instructionTextSize = this.font.Measure(this.instructions);

        this.instructionBackground = default;
        this.instructionBackground.Position = new Vector2(this.instructionTextPos.X, this.instructionTextPos.Y);
        this.instructionBackground.Width = instructionTextSize.Width + (instructionTextSize.Width *= 0.05f);
        this.instructionBackground.Height = instructionTextSize.Height + (instructionTextSize.Height *= 0.30f);
        this.instructionBackground.IsSolid = true;
        this.instructionBackground.Color = Color.CornflowerBlue;

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        this.contentManager.Unload(this.zeldaMapTexture);
        this.contentManager.Unload(this.font);

        base.UnloadContent();
    }

    public override void Update(FrameTime frameTime)
    {
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

        this.prevKeyState = currentKeyState;

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

        // Set a minimum size to the scale. If the scale goes negative, the image flips upside down
        // and zooming in becomes zooming out and vice versa. Setting a minimum scale prevents this from happening.
        // textureScale = textureScale < 0.5f ? 0.5f : textureScale;

        this.textureRenderer.Render(
            this.zeldaMapTexture,
            mapPos,
            0f,
            textureScale,
            -100);

        if (this.renderInstructions)
        {
            this.shapeRenderer.Render(this.instructionBackground, -10);
            this.fontRenderer.Render(this.font, this.instructions, this.instructionTextPos, Color.Black);
        }
        else
        {
            this.shapeRenderer.Render(this.helpBackground, -10);
            this.fontRenderer.Render(this.font, this.helpText, this.helpTextPos, Color.Black, 0);
        }

        base.Render();
    }
}