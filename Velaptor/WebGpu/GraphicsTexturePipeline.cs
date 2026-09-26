// <copyright file="GraphicsTexturePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Structures;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu.Handles;

/// <inheritdoc/>
internal sealed class GraphicsTexturePipeline : IGraphicsTexturePipeline
{
    private readonly IGraphicsDevice grfxDevice;
    private readonly IGraphicsSurface surface;
    private readonly IGraphicsShader shader;
    private SafeRenderPipelineHandle? pipelineHandle;
    private SafeBindGroupLayoutHandle? bindGroupLayout;
    private bool isDisposed;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsTexturePipeline"/> class.
    /// </summary>
    /// <param name="grfxDevice">The graphics device.</param>
    /// <param name="surface">The graphics surface (used to get the swap-chain pixel format).</param>
    /// <param name="shader">The shader source holder (WGSL modules are compiled during <see cref="Initialize"/>).</param>
    public GraphicsTexturePipeline(IGraphicsDevice grfxDevice, IGraphicsSurface surface, IGraphicsShader shader)
    {
        ArgumentNullException.ThrowIfNull(grfxDevice);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(shader);

        this.grfxDevice = grfxDevice;
        this.surface = surface;
        this.shader = shader;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Initialize()
    {
        if (this.isInitialized)
        {
            return;
        }

        // Compile shaders first — they need the device.
        this.shader.Initialize(this.grfxDevice, TypeOfShader.Texture, (vertHandle, fragHandle) =>
        {
            this.pipelineHandle = BuildPipeline(vertHandle, fragHandle, this.surface.Format);
        });

        this.isInitialized = true;
    }

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's shaders and fixed-function state until a different
    /// pipeline is bound or the pass ends.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    public void Bind(SafeRenderPassEncoderHandle pass)
    {
        if (!this.isInitialized || this.pipelineHandle is null)
        {
            throw new InvalidOperationException(
                "Pipeline has not been initialized. Call Initialize() first.");
        }

        this.grfxDevice.Wgpu.RenderPassEncoderSetPipeline(pass, this.pipelineHandle);
    }

    /// <inheritdoc/>
    public void Dispose() => Dispose(true);

    /// <inheritdoc cref="IDisposable.Dispose"/>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.pipelineHandle?.Dispose();
            this.bindGroupLayout?.Dispose();
        }

        this.isDisposed = true;
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
        if (this.grfxDevice.Handle is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeDeviceHandle)}' cannot be null. Cannot build texture pipeline.");
        }

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

        this.bindGroupLayout = this.grfxDevice.Wgpu.DeviceCreateBindGroupLayout(
            this.grfxDevice.Handle,
            [textureBindingLayout, samplerBindingLayout]);

        var pipelineLayout = this.grfxDevice.Wgpu.DeviceCreatePipelineLayout(
            this.grfxDevice.Handle,
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

            return this.grfxDevice.Wgpu.DeviceCreateRenderPipeline(this.grfxDevice.Handle, in desc);
        }
        finally
        {
            pipelineLayout.Dispose();
        }
    }
}
