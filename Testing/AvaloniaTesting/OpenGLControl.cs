// <copyright file="OpenGLControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AvaloniaTesting;

using System;
using System.Drawing;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Silk.NET.OpenGL;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;

/// <summary>
/// Control used for OpenGL rendering.
/// </summary>
public class OpenGLControl : OpenGlControlBase
{
    private bool isInitialized;
    private ITextureRenderer? textureRenderer;
    private IBatcher? batcher;
    private ITexture? texture;
    private IRenderContext? renderContext;
    private float scaleFactor;

    /// <summary>
    /// Gets or sets the position of the texture.
    /// </summary>
    public Vector2 RenderPosition { get; set; }

    /// <summary>
    /// Sets the position of the texture.
    /// </summary>
    /// <param name="point">The position of the mouse when it was released.</param>
    public void OnMouseReleased(Vector2 point) => RenderPosition = point;

    /// <summary>
    /// Sets up the control for OpenGL rendering.
    /// </summary>
    /// <param name="gl">The OpenGL object.</param>
    protected override void OnOpenGlInit(GlInterface gl)
    {
        if (this.isInitialized)
        {
            return;
        }

        base.OnOpenGlInit(gl);

        var glObj = GL.GetApi(gl.GetProcAddress);

        var width = (uint)Bounds.Width;
        var height = (uint)Bounds.Height;

        this.scaleFactor = HardwareFactory.GetMainDisplay().HorizontalScale;
        this.renderContext = Velaptor.App.UseAvaloniaRenderContext(glObj, width, height);

        var rendererFactory = new RendererFactory();
        this.batcher = rendererFactory.CreateBatcher();
        this.textureRenderer = rendererFactory.CreateTextureRenderer();
        var contentLoader = ContentLoaderFactory.CreateTextureLoader();

        this.texture = contentLoader.Load("velaptor-logo");
        this.batcher.ClearColor = Color.FromArgb(255, 34, 34, 34);

        this.isInitialized = true;
    }

    /// <summary>
    /// Updates the size of the render area.
    /// </summary>
    /// <param name="e">The size changed event data.</param>
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (!this.isInitialized)
        {
            return;
        }

        base.OnSizeChanged(e);

        Dispatcher.UIThread.Invoke(() =>
        {
            this.renderContext.SetRenderAreaSize((int)e.NewSize.Width, (int)e.NewSize.Height);
        });
    }

    /// <summary>
    /// Renders the content inside of the control.
    /// </summary>
    /// <param name="gl">The OpenGL object.</param>
    /// <param name="fb">No idea what this is.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the texture is null.
    /// </exception>
    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (this.texture is null)
        {
            throw new InvalidOperationException("The texture cannot be null");
        }

        Dispatcher.UIThread.Invoke(() =>
        {
            var width = (int)Bounds.Width;
            var height = (int)Bounds.Height;
            this.renderContext.SetRenderAreaSize(width, height);

            var renderPosition = new Vector2(RenderPosition.X * this.scaleFactor, RenderPosition.Y * this.scaleFactor);

            this.batcher?.Clear();
            this.batcher?.Begin();
            this.textureRenderer?.Render(this.texture, renderPosition);
            this.batcher?.End();

            RequestNextFrameRendering();
        });
    }
}
