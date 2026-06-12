// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using StbImageSharp;
using System.Numerics;
using NETColor = System.Drawing.Color;

/// <summary>
/// The main program.
/// </summary>
public sealed class Program
{
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
    private static int dinoWidth;
    private static int dinoHeight;
    private static Vector2 dinoWorldPos = new (400f, 300f);

    private static void Main()
    {
        var opts = WindowOptions.Default;
        opts.Size = new Vector2D<int>(800, 600);
        opts.Title = "WebGPU — Cornflower Blue Rectangle";
        opts.API = GraphicsAPI.None;

        window = Window.Create(opts);
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
            dinoWidth  = imgInfo.Width;
            dinoHeight = imgInfo.Height;
        }

        using var shader = new GraphicsShader(gd, "Content/shader.wgsl", 0f, 0f);
        pipeline = new GraphicsPipeline(gd, surface, shader);
        texture  = new GraphicsTexture(gd, "Content/dino.png", pipeline.BindGroupLayout);

        var fb = window!.FramebufferSize;

        textureBuffer = new GraphicsTextureBuffer(gd, initialQuadCount: 1);
        textureBuffer.WindowSize = new Vector2(fb.X, fb.Y);

        using var rectShader = new GraphicsShader(gd, "Content/rect-shape.wgsl", 0f, 0f);
        rectPipeline = new GraphicsRectPipeline(gd, surface, rectShader);
        rectBuffer   = new GraphicsRectBuffer(gd, initialRectCount: 64);
        rectBuffer.WindowSize = new Vector2(fb.X, fb.Y);

        camera = new Camera2D { WindowSize = new Vector2(fb.X, fb.Y) };

        // Set up keyboard controls for camera pan and zoom.
        input = window.CreateInput();
        var keyboard = input.Keyboards[0];

        keyboard.KeyDown += (kb, key, _) =>
        {
            if (camera is null)
            {
                return;
            }

            if (key == Key.R)
            {
                camera.Zoom += 0.10f;
                camera.Update();
            }
            else if (key == Key.F)
            {
                camera.Zoom -= 0.10f;
                camera.Update();
            }

            // Camera positioning
            if (key == Key.A)
            {
                camera.Position -= new Vector2(10f, 0f);
                camera.Update();
            }
            else if (key == Key.D)
            {
                camera.Position += new Vector2(10f, 0f);
                camera.Update();
            }
            else if (key == Key.W)
            {
                camera.Position -= new Vector2(0f, 10f);
                camera.Update();
            }
            else if (key == Key.S)
            {
                camera.Position += new Vector2(0f, 10f);
                camera.Update();
            }

            // Texture positioning
            if (key == Key.Left)
            {
                dinoWorldPos -= new Vector2(10f, 0f);
            }
            else if (key == Key.Right)
            {
                dinoWorldPos += new Vector2(10f, 0f);
            }
            else if (key == Key.Up)
            {
                dinoWorldPos -= new Vector2(0f, 10f);
            }
            else if (key == Key.Down)
            {
                dinoWorldPos += new Vector2(0f, 10f);
            }
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

        if (!frame.Begin(NETColor.FromArgb(100, 149, 237)))
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
                Position  = camera.TransformPosition(dinoWorldPos),
                Width     = dinoWidth,
                Height    = dinoHeight,
                Size      = camera.TransformSize(1f),  // scale by zoom

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
            const float rectW = 180f;
            const float rectH = 100f;

            var rect = new RectShape
            {
                Position     = camera.TransformPosition(new Vector2(180f, 150f)),
                Width        = camera.TransformSize(rectW),
                Height       = camera.TransformSize(rectH),
                Color        = NETColor.Orange,
                CornerRadius = new CornerRadius(camera.TransformSize(15f)),
                IsSolid      = true,
            };

            rectBuffer.Upload(rect, rectIndex: 0);

            var borderRect = new RectShape
            {
                Position        = camera.TransformPosition(new Vector2(620f, 450f)),
                Width           = camera.TransformSize(rectW),
                Height          = camera.TransformSize(rectH),
                Color           = NETColor.LimeGreen,
                IsSolid         = false,
                BorderThickness = camera.TransformSize(3f),
            };

            rectBuffer.Upload(borderRect, rectIndex: 1);

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
        rectBuffer?.Dispose();
        rectPipeline?.Dispose();
        textureBuffer?.Dispose();
        texture?.Dispose();
        pipeline?.Dispose();
        surface?.Dispose();
        gd?.Dispose();
        input?.Dispose();
    }
}
