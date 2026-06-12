// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using StbImageSharp;
using System.Numerics;
using System.Runtime.CompilerServices;
using NETColor = System.Drawing.Color;

/// <summary>
/// The main program.
/// </summary>
public sealed class Program
{
    private static readonly int[] CtrlModeValues = Enum.GetValues<ControlMode>().Cast<int>().ToArray();
    private static readonly int MinCtrlMode = 0;
    private static readonly int MaxCtrlMode = CtrlModeValues.Max();
    private static IWindow? window;
    private static GraphicsDevice? gd;
    private static GraphicsSurface? surface;
    private static GraphicsPipeline? pipeline;
    private static GraphicsTexture? texture;
    private static GraphicsTextureBuffer? textureBuffer;
    private static GraphicsRectPipeline? rectPipeline;
    private static GraphicsRectBuffer? rectBuffer;
    private static Camera2D? camera;
    private static IInputContext? input;
    private static IKeyboard keyboard;
    private static bool shiftKeyDown;
    private static int dinoWidth;
    private static int dinoHeight;
    private static Vector2 dinoWorldPos = new (400f, 300f);
    private static Vector2 rectPos = new (180f, 150f);
    private static int rectWidth = 180;
    private static int rectHeight = 100;
    private static ControlMode ctrlMode = ControlMode.Texture;
    private static RectShape rect;

    private static void Main()
    {
        var opts = WindowOptions.Default;
        opts.Size = new Vector2D<int>(800, 600);
        opts.API = GraphicsAPI.None;

        window = Window.Create(opts);
        window.Title = "MODE: TEXTURE";
        window.Load += OnLoad;
        window.Render += OnRender;
        window.FramebufferResize += OnResize;
        window.Closing += OnClose;

        window.Run();
    }

    /// <summary>
    /// Initializes the graphics device, surface, pipelines, buffers, camera, and input.
    /// </summary>
    private static void OnLoad()
    {
        if (window is null)
        {
            throw new Exception("Window failed to initialize.");
        }

        gd = new GraphicsDevice();
        surface = new GraphicsSurface(gd, window);

        gd.InitializeAdapter(surface.Handle);
        gd.InitializeDevice();

        surface.Configure();

        // Read just the image header to get native dimensions without a full decode.
        using (var imgStream = File.OpenRead("Content/dino.png"))
        {
            var imgInfo = ImageInfo.FromStream(imgStream)
                ?? throw new InvalidOperationException("Could not read image header from dino.png.");
            dinoWidth = imgInfo.Width;
            dinoHeight = imgInfo.Height;
        }

        using var shader = new GraphicsShader(gd, "Content/shader.wgsl", 0f, 0f);
        pipeline = new GraphicsPipeline(gd, surface, shader);
        texture = new GraphicsTexture(gd, "Content/dino.png", pipeline.BindGroupLayout);

        var fb = window!.FramebufferSize;

        textureBuffer = new GraphicsTextureBuffer(gd, initialQuadCount: 1);
        textureBuffer.WindowSize = new Vector2(fb.X, fb.Y);

        using var rectShader = new GraphicsShader(gd, "Content/rect-shape.wgsl", 0f, 0f);
        rectPipeline = new GraphicsRectPipeline(gd, surface, rectShader);
        rectBuffer = new GraphicsRectBuffer(gd, initialRectCount: 64);
        rectBuffer.WindowSize = new Vector2(fb.X, fb.Y);

        camera = new Camera2D { WindowSize = new Vector2(fb.X, fb.Y) };

        // Set up keyboard controls for camera pan and zoom.
        input = window.CreateInput();
        keyboard = input.Keyboards[0];
        keyboard.KeyDown += KeyboardKeyDown;
        keyboard.KeyUp += KeyboardKeyUp;

        rect = new RectShape
        {
            Position = camera.TransformPosition(rectPos),
            Width = camera.TransformSize(rectWidth),
            Height = camera.TransformSize(rectHeight),
            Color = NETColor.Orange,
            CornerRadius = new CornerRadius(camera.TransformSize(15f)),
            IsSolid = true,
        };
    }

    /// <summary>
    /// Renders a frame: clears to cornflower blue, draws the texture, draws rectangles.
    /// All world positions and sizes are run through the camera transform so pan and
    /// zoom are applied uniformly to every rendered object.
    /// </summary>
    /// <param name="_">Elapsed time (unused).</param>
    private static void OnRender(double _)
    {
        if (pipeline is null || texture is null || gd is null || surface is null)
        {
            throw new Exception("Core rendering objects are not initialized.");
        }

        using var frame = new Frame(gd, surface);

        if (!frame.Begin(NETColor.FromArgb(25, 26, 28)))
        {
            return;
        }

        // ── Texture draw ──────────────────────────────────────────────────────
        // World position (400, 300) = centre of an 800x600 window.
        // The camera transform shifts and scales it before NDC conversion.
        if (textureBuffer is not null && camera is not null)
        {
            var dinoQuad = new TextureQuad
            {
                Position = camera.TransformPosition(dinoWorldPos),
                Width = dinoWidth,
                Height = dinoHeight,
                Size = camera.TransformSize(1f), // scale by zoom

                // ── Optional Velaptor-equivalent features ─────────────────────
                // Angle     = 45f,
                // TintColor = NETColor.FromArgb(255, 200, 100, 100),
                // Effects   = RenderEffects.FlipHorizontally,
                // SrcRect   = new System.Drawing.RectangleF(0, 0, dinoWidth / 2f, dinoHeight),
            };

            textureBuffer.Upload(dinoQuad);
            frame.Draw(pipeline, textureBuffer, texture.BindGroup);
        }

        // ── Rectangle draws ───────────────────────────────────────────────────
        // Rect world positions are also run through the camera so panning and
        // zooming affect all objects uniformly.
        if (rectBuffer is not null && rectPipeline is not null && camera is not null)
        {
            rect.Position = camera.TransformPosition(rectPos);
            rect.Width = camera.TransformSize(rectWidth);
            rect.Height = camera.TransformSize(rectHeight);
            rectBuffer.Upload(rect, rectIndex: 0);

            frame.DrawRectangles(rectPipeline, rectBuffer, rectCount: 2);
        }

        frame.Submit();
    }

    /// <summary>
    /// Reconfigures the surface and updates buffer/camera window sizes on resize.
    /// </summary>
    private static void OnResize(Vector2D<int> newSize)
    {
        if (surface is null)
        {
            throw new Exception("Surface is not initialized.");
        }

        if (newSize.X > 0 && newSize.Y > 0)
        {
            surface.Configure();

            var size = new Vector2(newSize.X, newSize.Y);

            if (textureBuffer is not null)
            {
                textureBuffer.WindowSize = size;
            }

            if (rectBuffer is not null)
            {
                rectBuffer.WindowSize = size;
            }

            if (camera is not null)
            {
                camera.WindowSize = size;
            }
        }
    }

    /// <summary>
    /// Disposes all GPU resources in reverse creation order.
    /// </summary>
    private static void OnClose()
    {
        keyboard.KeyDown -= KeyboardKeyDown;
        rectBuffer?.Dispose();
        rectPipeline?.Dispose();
        textureBuffer?.Dispose();
        texture?.Dispose();
        pipeline?.Dispose();
        surface?.Dispose();
        gd?.Dispose();
        input?.Dispose();
    }

    private static void KeyboardKeyDown(IKeyboard kb, Key key, int _)
    {
        var currentValue = (int)ctrlMode;
        var nextValue = currentValue;

        if (key == Key.PageUp)
        {
            nextValue = currentValue <= MinCtrlMode ? MaxCtrlMode : currentValue - 1;
        }
        else if (key == Key.PageDown)
        {
            nextValue = currentValue >= MaxCtrlMode ? MinCtrlMode : currentValue + 1;
        }

        if (key is Key.ShiftLeft or Key.ShiftRight)
        {
            shiftKeyDown = true;
        }

        ctrlMode = (ControlMode)nextValue;

        switch (ctrlMode)
        {
            case ControlMode.Texture:
                ControlTexture(key);
                window?.Title = "MODE: TEXTURE";
                break;
            case ControlMode.Rectangle:
                ControlRectangle(key);
                window?.Title = "MODE: RECTANGLE";
                break;
            case ControlMode.Camera:
                ControlCamera(key);
                window?.Title = "MODE: CAMERA";
                break;
        }
    }

    private static void KeyboardKeyUp(IKeyboard kb, Key key, int _)
    {
        if (key is Key.ShiftLeft or Key.ShiftRight)
        {
            shiftKeyDown = false;
        }
    }

    private static void ControlCamera(Key key)
    {
        if (camera is null)
        {
            return;
        }

        if (key == Key.I)
        {
            camera.Zoom += 0.10f;
            camera.Update();
        }
        else if (key == Key.O)
        {
            camera.Zoom -= 0.10f;
            camera.Update();
        }

        // Camera positioning
        if (key == Key.Left)
        {
            camera.Position -= new Vector2(10f, 0f);
            camera.Update();
        }
        else if (key == Key.Right)
        {
            camera.Position += new Vector2(10f, 0f);
            camera.Update();
        }
        else if (key == Key.Up)
        {
            camera.Position -= new Vector2(0f, 10f);
            camera.Update();
        }
        else if (key == Key.Down)
        {
            camera.Position += new Vector2(0f, 10f);
            camera.Update();
        }
    }

    private static void ControlTexture(Key key)
    {
        // Texture positioning
        if (key == Key.Left)
        {
            dinoWorldPos -= new Vector2(10f, 0f);
        }

        if (key == Key.Right)
        {
            dinoWorldPos += new Vector2(10f, 0f);
        }

        if (key == Key.Up)
        {
            dinoWorldPos -= new Vector2(0f, 10f);
        }

        if (key == Key.Down)
        {
            dinoWorldPos += new Vector2(0f, 10f);
        }
    }

    private static void ControlRectangle(Key key)
    {
        // Rectangle positioning
        if (shiftKeyDown)
        {
            if (key == Key.Left)
            {
                rectWidth -= 10;
            }

            if (key == Key.Right)
            {
                rectWidth += 10;
            }

            if (key == Key.Up)
            {
                rectHeight -= 10;
            }

            if (key == Key.Down)
            {
                rectHeight += 10;
            }
        }
        else
        {
            if (key == Key.Left)
            {
                rectPos -= new Vector2(10f, 0f);
            }

            if (key == Key.Right)
            {
                rectPos += new Vector2(10f, 0f);
            }

            if (key == Key.Up)
            {
                rectPos -= new Vector2(0f, 10f);
            }

            if (key == Key.Down)
            {
                rectPos += new Vector2(0f, 10f);
            }
        }
    }
}
