// <copyright file="ImGuiService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using ImGui;
using Velaptor.Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to ImGui related calls and unsafe code.")]
internal sealed class ImGuiService : IImGuiService
{
    private const string FontFileExtension = ".ttf";
    private const string DefaultRegularFontName = $"TimesNewRoman-Regular{FontFileExtension}";
    private readonly IImGuiInvoker imGuiInvoker;
    private readonly IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService;
    private string defaultIniFileName = "imgui.ini";

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiService"/> class.
    /// </summary>
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="embeddedFontResourceService">Gives access to embedded font file resources.</param>
    public ImGuiService(IImGuiInvoker imGuiInvoker, IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService)
    {
        this.imGuiInvoker = imGuiInvoker;
        this.embeddedFontResourceService = embeddedFontResourceService;
    }

    /// <inheritdoc/>
    public (byte[] bytes, int textureWidth, int textureHeight) GetTexDataAsRGBA32()
    {
        var fonts = this.imGuiInvoker.GetIO().Fonts;

        unsafe
        {
            fonts.GetTexDataAsRGBA32(out byte* pixels, out var textureWidth, out var textureHeight, out var bytesPerPixel);

            var dataLen = textureWidth * textureHeight * bytesPerPixel;
            var pixelData = new byte[dataLen];

            Marshal.Copy((IntPtr)pixels, pixelData, 0, dataLen);

            return (pixelData, textureWidth, textureHeight);
        }
    }

    /// <inheritdoc/>
    public void SetTexID(uint textureId)
    {
        var fonts = this.imGuiInvoker.GetIO().Fonts;
        fonts.TexID = (IntPtr)textureId;
    }

    /// <inheritdoc/>
    public void ClearFonts()
    {
        var fonts = this.imGuiInvoker.GetIO().Fonts;
        fonts.Clear();
    }

    /// <inheritdoc/>
    public void ClearTexData()
    {
        var fonts = this.imGuiInvoker.GetIO().Fonts;
        fonts.ClearTexData();
    }

    /// <inheritdoc/>
    public void AddEmbeddedFont(uint fontSize)
    {
        if (fontSize >= 100)
        {
            throw new ArgumentException("The font size must be less than or equal 100.", nameof(fontSize));
        }

        using var fontFileStream = this.embeddedFontResourceService.LoadResource(DefaultRegularFontName);

        ArgumentNullException.ThrowIfNull(fontFileStream);

        using var binReader = new BinaryReader(fontFileStream);

        var rawFontData = binReader.ReadBytes((int)fontFileStream.Length);

        // Pin the array of raw font data so that it can be added to ImGui
        // This pinning will prevent the GC from moving the array around in memory during the operation
        var pinArrayHandle = GCHandle.Alloc(rawFontData, GCHandleType.Pinned);

        var rawDataPtr = pinArrayHandle.AddrOfPinnedObject();

        var io = this.imGuiInvoker.GetIO();
        io.FontGlobalScale = 1.0f / io.DisplayFramebufferScale.Y;

        io.Fonts.AddFontFromMemoryTTF(rawDataPtr, rawFontData.Length, fontSize);

        // Free the pinned array for GC to move or collect
        pinArrayHandle.Free();
    }

    /// <inheritdoc/>
    public void EnableIniFile()
    {
        unsafe
        {
            fixed (char* iniFileNamePtr = this.defaultIniFileName)
            {
                this.imGuiInvoker.GetIO().NativePtr->IniFilename = (byte*)iniFileNamePtr;
            }
        }
    }

    /// <inheritdoc/>
    public void DisableIniFile()
    {
        this.defaultIniFileName = this.imGuiInvoker.GetIO().IniFilename;

        unsafe
        {
            this.imGuiInvoker.GetIO().NativePtr->IniFilename = null;
        }
    }
}
