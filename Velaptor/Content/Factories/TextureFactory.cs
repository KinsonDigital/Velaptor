// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using Graphics;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;
using Velaptor.Factories;
using WebGpu;

/// <summary>
/// Creates <see cref="ITexture"/> objects for rendering.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal sealed class TextureFactory : ITextureFactory
{
    private readonly IWgpuInvoker wgpu;
    private readonly IGraphicsDevice gd;
    private readonly IReactableFactory reactableFactory;
    private SafeBindGroupLayoutHandle? bindGroupLayout;
    private readonly TextureBindGroupRegistry? bindGroupRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    public TextureFactory()
    {
        this.wgpu = IoC.Container.GetInstance<IWgpuInvoker>();
        this.gd = IoC.Container.GetInstance<IGraphicsDevice>();
        this.reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        this.bindGroupRegistry = IoC.Container.GetInstance<TextureBindGroupRegistry>();
        this.bindGroupLayout = null; // Deferred until first Create() — pipeline may not be initialized yet.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    /// <param name="wgpu">Invokes WebGPU functions.</param>
    /// <param name="gd">The WebGPU graphics device.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="bindGroupLayout">The bind group layout from the texture pipeline. Optional.</param>
    /// <param name="bindGroupRegistry">The registry for texture bind group lookup by renderers. Optional.</param>
    internal TextureFactory(
        IWgpuInvoker wgpu,
        IGraphicsDevice gd,
        IReactableFactory reactableFactory,
        SafeBindGroupLayoutHandle? bindGroupLayout = null,
        TextureBindGroupRegistry? bindGroupRegistry = null)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(gd);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        this.wgpu = wgpu;
        this.gd = gd;
        this.reactableFactory = reactableFactory;
        this.bindGroupLayout = bindGroupLayout;
        this.bindGroupRegistry = bindGroupRegistry;
    }

    /// <inheritdoc/>
    public ITexture Create(string name, string filePath, ImageData imageData)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        // Resolve bind group layout lazily — the pipeline may not be initialized
        // during DI resolution but will be ready by the time content is loaded.
        this.bindGroupLayout ??= IoC.Container.GetInstance<GraphicsTexturePipeline>().BindGroupLayout;

        return new Texture(
            this.wgpu,
            this.gd,
            this.bindGroupLayout,
            this.reactableFactory,
            name,
            filePath,
            imageData,
            this.bindGroupRegistry);
    }
}
