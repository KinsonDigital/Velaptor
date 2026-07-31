// <copyright file="GraphicsLinePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Structures;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for drawing 2-D lines. The vertex shader
/// passes through NDC positions and per-vertex color; the fragment shader applies
/// sRGB→linear conversion and outputs the color directly.
/// </summary>
/// <remarks>
/// <para>
/// Pipeline layout is <b>empty</b> — no bind groups. Vertex stride is 24 bytes:
/// vec2 position + vec4 color.
/// </para>
/// </remarks>
internal sealed class GraphicsLinePipeline : IDisposable
{
    private readonly IGraphicsDevice graphicsDevice;
    private readonly IGraphicsSurface surface;
    private readonly IGraphicsShader shader;
    private SafeDeviceHandle? deviceHandle;
    private SafeRenderPipelineHandle? handle;
    private bool isDisposed;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsLinePipeline"/> class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="surface">The graphics surface (used to obtain the swap-chain pixel format).</param>
    /// <param name="shader">The shader source holder (WGSL modules are compiled during <see cref="Initialize"/>).</param>
    public GraphicsLinePipeline(IGraphicsDevice graphicsDevice, IGraphicsSurface surface, IGraphicsShader shader)
    {
        ArgumentNullException.ThrowIfNull(graphicsDevice);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(shader);

        this.graphicsDevice = graphicsDevice;
        this.surface = surface;
        this.shader = shader;
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
        this.shader.Initialize(this.graphicsDevice);

        this.deviceHandle = this.graphicsDevice.Handle;

        this.handle = BuildPipeline(this.shader.VertexHandle, this.shader.FragmentHandle, this.surface.Format);

        // Shader modules can be released after the pipeline is created.
        this.shader.Dispose();

        this.isInitialized = true;
    }

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's line shaders and fixed-function state.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    public void Bind(SafeRenderPassEncoderHandle pass)
    {
        if (!this.isInitialized || this.handle is null)
        {
            throw new InvalidOperationException(
                "Pipeline has not been initialized. Call Initialize() first.");
        }

        this.graphicsDevice.Wgpu.RenderPassEncoderSetPipeline(pass, this.handle);
    }

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
        var pipelineLayoutHandle = this.graphicsDevice.Wgpu.DeviceCreatePipelineLayout(this.deviceHandle!, "Line Pipeline Layout");

        try
        {
            var desc = new SafeRenderPipelineDescriptor
            {
                Layout = pipelineLayoutHandle,
                Vertex = new SafeVertexState
                {
                    Module = vertModule,
                    EntryPoint = "vs_main",
                    Buffers =
                    [
                        new SafeVertexBufferLayout
                        {
                            ArrayStride = 24,
                            StepMode = VertexStepMode.Vertex,
                            Attributes =
                            [
                                new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0, ShaderLocation = 0 },
                                new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 8, ShaderLocation = 1 }
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

            return this.graphicsDevice.Wgpu.DeviceCreateRenderPipeline(this.deviceHandle!, in desc);
        }
        finally
        {
            pipelineLayoutHandle.Dispose();
        }
    }
}
