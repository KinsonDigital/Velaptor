// <copyright file="GraphicsShapePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Structures;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for drawing rounded rectangles. Unlike the
/// texture pipeline, this pipeline uses a per-vertex shape attribute layout
/// (position, bounding box, color, corner radii, etc.) with no bind groups.
/// </summary>
/// <remarks>
/// <para>
/// Pipeline layout is <b>empty</b> — no bind groups (no textures, no camera uniform).
/// The fragment shader uses <c>@builtin(position)</c> directly for pixel-space
/// coordinates.
/// </para>
/// <para>
/// Vertex stride is 64 bytes with 9 attributes.
/// </para>
/// </remarks>
internal sealed class GraphicsShapePipeline : IDisposable
{
    private readonly GraphicsDevice gd;
    private readonly GraphicsSurface surface;
    private readonly GraphicsShader shader;
    private IWgpuInvoker? wgpu;
    private SafeDeviceHandle? device;
    private SafeRenderPipelineHandle? handle;
    private bool isDisposed;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShapePipeline"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="surface">The graphics surface (used to obtain the swap-chain pixel format).</param>
    /// <param name="shader">The shader source holder (WGSL modules are compiled during <see cref="Initialize"/>).</param>
    public GraphicsShapePipeline(GraphicsDevice gd, GraphicsSurface surface, GraphicsShader shader)
    {
        ArgumentNullException.ThrowIfNull(gd);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(shader);

        this.gd = gd;
        this.surface = surface;
        this.shader = shader;
    }

    /// <summary>
    /// Gets the compiled GPU pipeline handle.
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
    /// Compiles the shader and builds the GPU render pipeline. Must be called after
    /// the WebGPU device has been initialized and the surface has been configured.
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
    /// the pass will use this pipeline's shape shaders and fixed-function state.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    public void Bind(SafeRenderPassEncoderHandle pass) => this.wgpu!.RenderPassEncoderSetPipeline(pass, Handle);

    /// <summary>
    /// Releases the GPU pipeline handle.
    /// </summary>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.handle?.Dispose();
    }

    /// <summary>
    /// Builds the render pipeline descriptor.
    /// </summary>
    private SafeRenderPipelineHandle BuildPipeline(
        SafeShaderModuleHandle vertModule,
        SafeShaderModuleHandle fragModule,
        TextureFormat format)
    {
        var pipelineLayout = this.wgpu!.DeviceCreatePipelineLayout(this.device!, "Shape Pipeline Layout");

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
                            ArrayStride = 64,
                            StepMode = VertexStepMode.Vertex,
                            Attributes =
                            [
                                new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0, ShaderLocation = 0 },
                                new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 8, ShaderLocation = 1 },
                                new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 24, ShaderLocation = 2 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 40, ShaderLocation = 3 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 44, ShaderLocation = 4 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 48, ShaderLocation = 5 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 52, ShaderLocation = 6 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 56, ShaderLocation = 7 },
                                new VertexAttribute { Format = VertexFormat.Float32, Offset = 60, ShaderLocation = 8 }
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
