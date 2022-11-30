// <copyright file="GPUBufferFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using Velaptor.NativeInterop.OpenGL;
using OpenGL;
using OpenGL.Buffers;
using Reactables.Core;
using Reactables.ReactableData;

/// <summary>
/// Creates singleton instances of <see cref="TextureGPUBuffer"/> and <see cref="FontGPUBuffer"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class GPUBufferFactory : IGPUBufferFactory
{
    private static IGPUBuffer<TextureBatchItem>? textureBuffer;
    private static IGPUBuffer<FontGlyphBatchItem>? fontBuffer;
    private static IGPUBuffer<RectBatchItem>? rectBuffer;

    /// <inheritdoc/>
    public IGPUBuffer<TextureBatchItem> CreateTextureGPUBuffer()
    {
        if (textureBuffer is not null)
        {
            return textureBuffer;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        textureBuffer = new TextureGPUBuffer(glInvoker, glInvokerExtensions, glInitReactor, shutDownReactor);

        return textureBuffer;
    }

    /// <inheritdoc/>
    public IGPUBuffer<FontGlyphBatchItem> CreateFontGPUBuffer()
    {
        if (fontBuffer is not null)
        {
            return fontBuffer;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var batchSizeReactable = IoC.Container.GetInstance<IReactable<BatchSizeData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        fontBuffer = new FontGPUBuffer(
            glInvoker,
            glInvokerExtensions,
            glInitReactor,
            batchSizeReactable,
            shutDownReactor);

        return fontBuffer;
    }

    /// <inheritdoc/>
    public IGPUBuffer<RectBatchItem> CreateRectGPUBuffer()
    {
        if (rectBuffer is not null)
        {
            return rectBuffer;
        }

        var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
        var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
        var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
        var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

        rectBuffer = new RectGPUBuffer(glInvoker, glInvokerExtensions, glInitReactor, shutDownReactor);

        return rectBuffer;
    }
}
