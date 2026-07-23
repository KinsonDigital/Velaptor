// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Carbonate;
using Carbonate.OneWay;
using Graphics;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;
using ReactableData;
using Silk.NET.WebGPU;
using Velaptor.Factories;
using WebGpu;

/// <summary>
/// The texture to render to a screen.
/// </summary>
public sealed class Texture : ITexture
{
    private const uint BytesPerRowAlignment = 256;
    private static uint nextId = 1;

    private readonly IWgpuInvoker wgpu;
    private readonly IGraphicsDevice gd;
    private readonly TextureBindGroupRegistry? bindGroupRegistry;
    private SafeTextureHandle? gpuTexture;
    private SafeTextureViewHandle? textureView;
    private SafeSamplerHandle? sampler;
    private IDisposable? unsubscriber;
    private int isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture"/> class.
    /// </summary>
    /// <param name="wgpu">Invokes WebGPU functions.</param>
    /// <param name="gd">The WebGPU graphics device.</param>
    /// <param name="bindGroupLayout">The bind group layout matching the texture pipeline.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="name">The name of the texture.</param>
    /// <param name="filePath">The file path to the image file.</param>
    /// <param name="imageData">The image data of the texture.</param>
    /// <param name="bindGroupRegistry">The registry for texture bind group lookup by renderers. Optional.</param>
    internal Texture(
        IWgpuInvoker wgpu,
        IGraphicsDevice gd,
        SafeBindGroupLayoutHandle bindGroupLayout,
        IReactableFactory reactableFactory,
        string name,
        string filePath,
        ImageData imageData,
        TextureBindGroupRegistry? bindGroupRegistry = null)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(gd);
        ArgumentNullException.ThrowIfNull(bindGroupLayout);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        this.wgpu = wgpu;
        this.gd = gd;
        this.bindGroupRegistry = bindGroupRegistry;

        FilePath = filePath;

        var disposeReactable = reactableFactory.CreateDisposeTextureReactable();
        Init(disposeReactable, bindGroupLayout, name, imageData);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Texture"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Finalizers cannot be unit tested.")]
    ~Texture()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        Unload(new DisposeTextureData { TextureId = Id });
    }

    /// <inheritdoc/>
    public uint Id { get; private set; }

    /// <inheritdoc/>
    public string Name { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string FilePath { get; }

    /// <inheritdoc/>
    public uint Width { get; private set; }

    /// <inheritdoc/>
    public uint Height { get; private set; }

    /// <summary>
    /// Gets the WebGPU bind group that wires the texture view to binding 0 and the
    /// sampler to binding 1, compatible with the texture pipeline's bind group layout.
    /// </summary>
    internal SafeBindGroupHandle? BindGroup { get; private set; }

    /// <summary>
    /// Disposes of the texture if this texture's <see cref="Id"/> matches the texture ID in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data of the texture to dispose.</param>
    private void Unload(DisposeTextureData data)
    {
        if (Id != data.TextureId)
        {
            return;
        }

        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        this.bindGroupRegistry?.Unregister(Id);
        BindGroup?.Dispose();
        this.sampler?.Dispose();
        this.textureView?.Dispose();
        this.gpuTexture?.Dispose();
    }

    /// <summary>
    /// Initializes the <see cref="Texture"/>.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="bindGroupLayout">The bind group layout from the texture pipeline.</param>
    /// <param name="name">The name of the texture.</param>
    /// <param name="imageData">The image data of the texture.</param>
    private void Init(
        IPushReactable<DisposeTextureData> disposeReactable,
        SafeBindGroupLayoutHandle bindGroupLayout,
        string name,
        ImageData imageData)
    {
        this.unsubscriber = disposeReactable.CreateOneWayReceive(
            PushNotifications.TextureDisposedId,
            Unload,
            () => this.unsubscriber?.Dispose());

        if (imageData.IsEmpty())
        {
            throw new ArgumentException("The image data must not be empty.", nameof(imageData));
        }

        Id = Interlocked.Increment(ref nextId) - 1;

        Width = imageData.Width;
        Height = imageData.Height;
        Name = name;

        UploadDataToGpu(bindGroupLayout, imageData);
    }

    /// <summary>
    /// Uploads the given pixel data to the GPU.
    /// </summary>
    /// <param name="bindGroupLayout">The bind group layout for the bind group.</param>
    /// <param name="imageData">The image data of the texture.</param>
    private void UploadDataToGpu(SafeBindGroupLayoutHandle bindGroupLayout, ImageData imageData)
    {
        var width = imageData.Width;
        var height = imageData.Height;

        // Convert Color[,] to a flat RGBA byte array
        var unalignedBytesPerRow = width * 4;
        var alignedBytesPerRow = (unalignedBytesPerRow + (BytesPerRowAlignment - 1)) & ~(BytesPerRowAlignment - 1);

        var pixels = imageData.Pixels;
        var rawPixels = new byte[alignedBytesPerRow * height];

        for (var y = 0u; y < height; y++)
        {
            for (var x = 0u; x < width; x++)
            {
                var pixel = pixels[x, y];
                var srcIdx = (int)((y * width) + x) * 4;
                var dstIdx = (int)((y * alignedBytesPerRow) + (x * 4));
                rawPixels[dstIdx] = pixel.R;
                rawPixels[dstIdx + 1] = pixel.G;
                rawPixels[dstIdx + 2] = pixel.B;
                rawPixels[dstIdx + 3] = pixel.A;
            }
        }

        // Create the GPU texture: 2-D, RGBA8 sRGB, with TextureBinding + CopyDst usage.
        var textureDesc = new TextureDescriptor
        {
            Usage = TextureUsage.TextureBinding | TextureUsage.CopyDst,
            Dimension = TextureDimension.Dimension2D,
            Size = new Extent3D { Width = width, Height = height, DepthOrArrayLayers = 1 },
            Format = TextureFormat.Rgba8UnormSrgb,
            MipLevelCount = 1,
            SampleCount = 1,
        };

        this.gpuTexture = new SafeTextureHandle(this.wgpu, this.gd.Handle!, in textureDesc);

        this.wgpu.QueueWriteTexture(
            this.gd.Queue!,
            this.gpuTexture.DangerousGetHandle(),
            width,
            height,
            alignedBytesPerRow,
            rawPixels);

        // Create a texture view
        var viewDesc = new TextureViewDescriptor
        {
            Format = TextureFormat.Rgba8UnormSrgb,
            Dimension = TextureViewDimension.Dimension2D,
            MipLevelCount = 1,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All,
        };

        this.textureView = new SafeTextureViewHandle(
            this.wgpu,
            this.gpuTexture.DangerousGetHandle(),
            in viewDesc);

        // Create a linear sampler. ClampToEdge prevents colour bleeding at the texture border.
        var samplerDesc = new SamplerDescriptor
        {
            AddressModeU = AddressMode.ClampToEdge,
            AddressModeV = AddressMode.ClampToEdge,
            AddressModeW = AddressMode.ClampToEdge,
            MagFilter = FilterMode.Linear,
            MinFilter = FilterMode.Linear,
            MipmapFilter = MipmapFilterMode.Nearest,
            LodMinClamp = 0f,
            LodMaxClamp = 1f,
            Compare = CompareFunction.Undefined,
            MaxAnisotropy = 1,
        };

        this.sampler = new SafeSamplerHandle(this.wgpu, this.wgpu.DeviceCreateSampler(this.gd.Handle!, in samplerDesc));

        // Create the bind group: binding 0 = texture view, binding 1 = sampler
        BindGroup = this.wgpu.DeviceCreateBindGroup(
            this.gd.Handle!,
            bindGroupLayout,
            this.textureView,
            this.sampler);

        this.bindGroupRegistry?.Register(Id, BindGroup);
    }
}
