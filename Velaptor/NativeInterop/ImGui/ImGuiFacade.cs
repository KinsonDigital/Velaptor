// <copyright file="ImGuiFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;
using Services;

/// <inheritdoc/>
internal sealed class ImGuiFacade : IImGuiFacade
{
    private const uint DefaultFontSize = 22;
    private readonly IImGuiManager imGuiManager;
    private readonly IImGuiService imGuiService;
    private bool isDisposed;
    private bool isInitialized;
    private bool updateInvokedFirst;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiFacade"/> class.
    /// </summary>
    /// <param name="imGuiManager">Provides ImGui related services.</param>
    /// <param name="imGuiService">Provides ImGui extensions.</param>
    public ImGuiFacade(IImGuiManager imGuiManager, IImGuiService imGuiService)
    {
        ArgumentNullException.ThrowIfNull(imGuiManager);
        ArgumentNullException.ThrowIfNull(imGuiService);

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
        this.imGuiService.DisableIniFile();

        this.isInitialized = true;
    }

    /// <summary>
    /// Rebuilds the font atlas. In WebGPU mode the pixel data is built so
    /// ImGui's IsBuilt() check passes, but the texture is not uploaded to the GPU.
    /// A dummy texture ID of 1 is used so ImGui.NewFrame() does not assert.
    /// </summary>
    private void RebuildFontAtlas()
    {
        // Force the font atlas to build so io.Fonts.IsBuilt() returns true.
        this.imGuiService.GetTexDataAsRGBA32();

        this.imGuiService.SetTexID(1u);
        this.imGuiService.ClearTexData();
    }
}

