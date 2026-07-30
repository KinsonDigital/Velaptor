// <copyright file="GraphicsTexturePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Structures;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for 2-D textured quads — the GPU state
/// object that controls how vertices are transformed and how pixels are colored
/// when drawing textures and font atlases.
/// </summary>
/// <remarks>
/// <para>
/// This pipeline hard-codes two programmable stages (vertex and fragment shaders)
/// together with all fixed-function state: primitive topology, blend mode, the pixel
/// format of the color target, and multisampling settings.
/// </para>
/// <para>
/// The bind group layout for <c>@group(0)</c> exposes binding 0 (2-D float texture)
/// and binding 1 (filtering sampler). Pass <see cref="BindGroupLayout"/> to
/// the texture buffer so it can create a compatible bind group.
/// </para>
/// <para>
/// Pipelines are immutable by design. Creating a pipeline is expensive — the driver
/// compiles and validates the combined state against the device. Binding one during
/// rendering is cheap. To change shaders or blend behavior create a new pipeline up
/// front, then switch between pre‑compiled pipelines at draw time.
/// </para>
/// </remarks>
internal sealed class GraphicsTexturePipeline : IDisposable
{
    private readonly IGraphicsDevice gd;
    private readonly IGraphicsSurface surface;
    private readonly GraphicsShader shader;
    private IWgpuInvoker? wgpu;
    private SafeDeviceHandle? device;
    private SafeRenderPipelineHandle? handle;
    private SafeBindGroupLayoutHandle? bindGroupLayout;
    private bool isDisposed;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsTexturePipeline"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="surface">The graphics surface (used to obtain the swap-chain pixel format).</param>
    /// <param name="shader">The shader source holder (WGSL modules are compiled during <see cref="Initialize"/>).</param>
    public GraphicsTexturePipeline(IGraphicsDevice gd, IGraphicsSurface surface, GraphicsShader shader)
    {
        ArgumentNullException.ThrowIfNull(gd);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(shader);

        this.gd = gd;
        this.surface = surface;
        this.shader = shader;
    }

    /// <summary>
    /// Gets the bind group layout for <c>@group(0)</c>: binding 0 = a 2-D float texture,
    /// binding 1 = a filtering sampler. Pass this to the texture buffer so it can create a
    /// compatible bind group.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    public SafeBindGroupLayoutHandle BindGroupLayout
    {
        get
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException(
                    "Pipeline has not been initialized. Call Initialize() first.");
            }

            return this.bindGroupLayout!;
        }
    }

    /// <summary>
    /// Gets the compiled GPU pipeline handle — an opaque, device-side object that
    /// encapsulates the shader stages and fixed-function state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    private SafeRenderPipelineHandle Handle
    {
        get
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException(
                    "Pipeline has not been initialized. Call Initialize() first.");
            }

            return this.handle!;
        }
    }

    /// <summary>
    /// Compiles the shader, creates the bind group layout, and builds the GPU render
    /// pipeline. Must be called after the WebGPU device has been initialized and the
    /// surface has been configured.
    /// </summary>
    public void Initialize()
    {
        if (this.isInitialized)
        {
            return;
        }

        // Compile shaders first — they need the device.
        this.shader.Initialize(this.gd);

        this.wgpu = this.gd.Wgpu;
        this.device = this.gd.Handle;

        this.handle = BuildPipeline(this.shader.VertexHandle, this.shader.FragmentHandle, this.surface.Format);

        // Shader modules can be released after the pipeline is created.
        this.shader.Dispose();

        this.isInitialized = true;
    }

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's shaders and fixed-function state until a different
    /// pipeline is bound or the pass ends.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    public void Bind(SafeRenderPassEncoderHandle pass) => this.wgpu!.RenderPassEncoderSetPipeline(pass, Handle);

    /// <summary>
    /// Releases the GPU pipeline handle and the bind group layout.
    /// </summary>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.handle?.Dispose();
        this.bindGroupLayout?.Dispose();
    }

    /// <summary>
    /// Builds the render pipeline descriptor, creating the pipeline layout and
    /// bind group layout internally.
    /// </summary>
    private SafeRenderPipelineHandle BuildPipeline(
        SafeShaderModuleHandle vertModule,
        SafeShaderModuleHandle fragModule,
        TextureFormat format)
    {
        var textureBindingLayout = new BindGroupLayoutEntry
            {
                Binding = 0,
                Visibility = ShaderStage.Fragment,
                Texture = new TextureBindingLayout
                {
                    SampleType = TextureSampleType.Float, ViewDimension = TextureViewDimension.Dimension2D, Multisampled = false,
                },
            };

        var samplerBindingLayout = new BindGroupLayoutEntry
        {
            Binding = 1, Visibility = ShaderStage.Fragment, Sampler = new SamplerBindingLayout { Type = SamplerBindingType.Filtering, },
        };

        this.bindGroupLayout = this.wgpu!.DeviceCreateBindGroupLayout(
            this.device!,
            [textureBindingLayout, samplerBindingLayout]);

        var pipelineLayout = this.wgpu!.DeviceCreatePipelineLayout(
            this.device!,
            "Texture Pipeline Layout",
            [this.bindGroupLayout]);

        try
        {
            var desc = new SafeRenderPipelineDescriptor
            {
                Layout = pipelineLayout,
                Vertex = new SafeVertexState
                {
                    Module = vertModule,
                    EntryPoint = "vs_main",
                    Buffers =
                    [
                        new SafeVertexBufferLayout
                        {
                            ArrayStride = 32,
                            StepMode = VertexStepMode.Vertex,
                            Attributes =
                            [
                                new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0, ShaderLocation = 0 },
                                new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 8, ShaderLocation = 1 },
                                new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 16, ShaderLocation = 2 }
                            ],
                        },
                    ],
                },
                Fragment = new SafeFragmentState
                {
                    Module = fragModule,
                    EntryPoint = "fs_main",
                    Targets =
                    [
                        new SafeColorTarget
                        {
                            Format = format,
                            WriteMask = ColorWriteMask.All,
                            Blend = new BlendState
                            {
                                Color = new BlendComponent
                                {
                                    SrcFactor = BlendFactor.SrcAlpha,
                                    DstFactor = BlendFactor.OneMinusSrcAlpha,
                                    Operation = BlendOperation.Add,
                                },
                                Alpha = new BlendComponent
                                {
                                    SrcFactor = BlendFactor.One,
                                    DstFactor = BlendFactor.OneMinusSrcAlpha,
                                    Operation = BlendOperation.Add,
                                },
                            },
                        },
                    ],
                },
                Primitive = new PrimitiveState
                {
                    Topology = PrimitiveTopology.TriangleList,
                    FrontFace = FrontFace.Ccw,
                    CullMode = CullMode.None,
                },
                Multisample = new MultisampleState
                {
                    Count = 1,
                    Mask = uint.MaxValue,
                },
            };

            return this.wgpu!.DeviceCreateRenderPipeline(this.device!, in desc);
        }
        finally
        {
            pipelineLayout.Dispose();
        }
    }
}
