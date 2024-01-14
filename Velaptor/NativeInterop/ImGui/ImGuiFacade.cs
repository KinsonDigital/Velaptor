// <copyright file="ImGuiFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;
using OpenGL;
using Services;
using Velaptor.OpenGL;

/// <inheritdoc/>
internal sealed class ImGuiFacade : IImGuiFacade
{
    private const uint DefaultFontSize = 22;
    private readonly IGLInvoker glInvoker;
    private readonly IImGuiManager imGuiManager;
    private readonly IImGuiService imGuiService;
    private bool isDisposed;
    private bool isInitialized;
    private bool updateInvokedFirst;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiFacade"/> class.
    /// </summary>
    /// <param name="glInvoker">Invokes OpenGL functions.</param>
    /// <param name="imGuiManager">Provides ImGui related services.</param>
    /// <param name="imGuiService">Provides ImGui extensions.</param>
    public ImGuiFacade(
        IGLInvoker glInvoker,
        IImGuiManager imGuiManager,
        IImGuiService imGuiService)
    {
        ArgumentNullException.ThrowIfNull(glInvoker);
        ArgumentNullException.ThrowIfNull(imGuiManager);
        ArgumentNullException.ThrowIfNull(imGuiService);

        this.glInvoker = glInvoker;
        this.imGuiManager = imGuiManager;
        this.imGuiService = imGuiService;
    }

    /// <inheritdoc/>
    public void Update(double timeSeconds)
    {
        SetupImGui();

        this.imGuiManager.Update(timeSeconds);

        this.updateInvokedFirst = true;
    }

    /// <inheritdoc/>
    public void Render()
    {
        if (!this.updateInvokedFirst)
        {
            const string updateMethod = $"{nameof(ImGuiFacade)}.{nameof(Update)}";
            const string renderMethod = $"{nameof(ImGuiFacade)}.{nameof(Render)}";
            throw new InvalidOperationException($"The '{updateMethod}' method must be invoked before the '{renderMethod}' method.");
        }

        this.imGuiManager.Render();
        this.updateInvokedFirst = false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.imGuiManager.Dispose();

        this.isDisposed = true;
    }

    /// <summary>
    /// Sets up <see cref="ImGui"/>.
    /// </summary>
    private void SetupImGui()
    {
        if (this.isInitialized)
        {
            return;
        }

        this.imGuiService.ClearFonts();
        this.imGuiService.AddEmbeddedFont(DefaultFontSize);

        RebuildFontAtlas();

        this.isInitialized = true;
    }

    /// <summary>
    /// Rebuilds the font atlas based off of the set font and font size.
    /// </summary>
    private void RebuildFontAtlas()
    {
        // Get the font texture from ImGui
        var (pixels, textureWidth, textureHeight) = this.imGuiService.GetTexDataAsRGBA32();

        var textureId = LoadTexture(pixels, textureWidth, textureHeight);
        this.imGuiService.SetTexID(textureId);

        // Clear the temporary data
        this.imGuiService.ClearTexData();
    }

    /// <summary>
    /// Loads the texture to the GPU.
    /// </summary>
    /// <param name="pixelData">The pixel data to upload.</param>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>The generated texture id.</returns>
    private uint LoadTexture(byte[] pixelData, int width, int height)
    {
        var textureId = this.glInvoker.GenTexture();
        this.glInvoker.PixelStore(GLPixelStoreParameter.UnpackAlignment, 1);
        this.glInvoker.BindTexture(GLTextureTarget.Texture2D, textureId);
        this.glInvoker.TexImage2D<byte>(
            target: GLTextureTarget.Texture2D,
            level: 0,
            internalformat: GLInternalFormat.Rgba,
            width: (uint)width,
            height: (uint)height,
            border: 0,
            format: GLPixelFormat.Rgba,
            type: GLPixelType.UnsignedByte,
            pixels: pixelData);

        this.glInvoker.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.TextureMinFilter, GLTextureMinFilter.Linear);
        this.glInvoker.TexParameter(GLTextureTarget.Texture2D, GLTextureParameterName.TextureMagFilter, GLTextureMagFilter.Linear);
        this.glInvoker.BindTexture(GLTextureTarget.Texture2D, 0);

        return textureId;
    }
}
