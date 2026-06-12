// <copyright file="GraphicsPipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;

/// <summary>
/// Represents a compiled, immutable render pipeline — the GPU state object that
/// controls how vertices are transformed and how pixels are colored.
/// </summary>
/// <remarks>
/// <para>
/// A render pipeline hard-codes two programmable stages (vertex and fragment shaders)
/// together with all fixed-function state: primitive topology, blend mode, the pixel
/// format of the color target, and multisampling settings. Creating a pipeline is
/// expensive — the driver compiles and validates the combined state against the device.
/// Binding one during rendering is cheap.
/// </para>
/// <para>
/// Pipelines are immutable by design. To change shaders or blend behavior you create a
/// This moves the expensive compilation cost out of the render loop entirely.
/// new pipeline up front, then switch between pre-compiled pipelines at draw time.
/// </para>
/// <para>
/// Named <c>GraphicsPipeline</c> rather than <c>RenderPipeline</c> to avoid colliding
/// with <see cref="RenderPipeline"/>, which is the raw GPU handle type.
/// </para>
/// </remarks>
internal sealed class GraphicsPipeline : IDisposable
{
    private readonly GraphicsDevice gd;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsPipeline"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="surface">The graphics surface.</param>
    /// <param name="shader">The compiled shader providing the vertex and fragment entry points.</param>
    /// <remarks>
    /// Constructs the pipeline descriptor using the surface's pixel format and the
    /// shader module from <paramref name="shader"/>. The pipeline retains its own internal
    /// reference to the compiled code, so <paramref name="shader"/> may be disposed
    /// immediately after this constructor returns.
    /// </remarks>
    public GraphicsPipeline(GraphicsDevice gd, GraphicsSurface surface, GraphicsShader shader)
    {
        this.gd = gd;

        // The pipeline retains its own internal reference to the compiled shader module.
        // GraphicsShader.Dispose() releases the module handle once the caller is done with it.
        Handle = BuildPipeline(shader.Handle, surface.Format);

        if (Handle == null)
        {
            throw new Exception("Failed to create render pipeline.");
        }
    }

    /// <summary>
    /// Gets the compiled GPU pipeline handle — an opaque, device-side object that
    /// encapsulates the shader stages and fixed-function state. Used by <see cref="Bind"/>
    /// to activate the pipeline during a render pass.
    /// </summary>
    public SafeRenderPipelineHandle Handle { get; private set; }

    /// <summary>
    /// Gets the bind group layout for <c>@group(0)</c>: binding 0 = a 2-D float texture,
    /// binding 1 = a filtering sampler. Pass this to <see cref="GraphicsTexture"/> so it
    /// can create a compatible bind group.
    /// </summary>
    public SafeBindGroupLayoutHandle BindGroupLayout { get; private set; }

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's shaders and fixed-function state until a different
    /// pipeline is bound or the pass ends.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    public void Bind(SafeRenderPassEncoderHandle pass)
    {
        unsafe
        {
            this.gd.Wgpu.RenderPassEncoderSetPipeline((RenderPassEncoder*)pass.DangerousGetHandle(), (RenderPipeline*)Handle.DangerousGetHandle());
        }
    }

    /// <summary>
    /// Releases the GPU pipeline handle, the texture bind group layout, and the camera
    /// bind group layout. After disposal <see cref="Handle"/> is invalid and the pipeline
    /// must not be bound to any further render passes. Dispose <see cref="Camera2D"/> and
    /// any <see cref="GraphicsTexture"/> instances before disposing the pipeline.
    /// </summary>
    public void Dispose()
    {
        Handle.Dispose();

        BindGroupLayout.Dispose();
    }

    private unsafe SafeRenderPipelineHandle BuildPipeline(SafeShaderModuleHandle shaderModule, TextureFormat format)
    {
        var vertexEntry = SilkMarshal.StringToPtr("vs_main");
        var fragmentEntry = SilkMarshal.StringToPtr("fs_main");

        BindGroupLayout = CreateTextureBindGroupLayout();

        var bindGrpLayout = (BindGroupLayout*)BindGroupLayout.DangerousGetHandle();

        ReadOnlySpan<byte> mainPipelineLayoutLabel = "Main Pipeline Layout"u8;
        PipelineLayout* pipelineLayout;

        fixed (byte* mainPipelineLayoutStrPtr = mainPipelineLayoutLabel)
        {
            var pipelineDescriptor = new PipelineLayoutDescriptor
            {
                Label = mainPipelineLayoutStrPtr,
                BindGroupLayoutCount = 1,
                BindGroupLayouts = &bindGrpLayout,
            };

            pipelineLayout = this.gd.Wgpu.DeviceCreatePipelineLayout((Device*)this.gd.Handle.DangerousGetHandle(), in pipelineDescriptor);
        }

        // ── Vertex buffer layout ─────────────────────────────────────────────
        // stride = 32 bytes (8 × f32), matching GraphicsTextureBuffer:
        //   location 0: vec2<f32> position   (8 bytes, offset  0)
        //   location 1: vec2<f32> uv         (8 bytes, offset  8)
        //   location 2: vec4<f32> tintColor  (16 bytes, offset 16)
        var attributes = stackalloc VertexAttribute[3];
        attributes[0] = new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0,  ShaderLocation = 0 };
        attributes[1] = new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 8,  ShaderLocation = 1 };
        attributes[2] = new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 16, ShaderLocation = 2 };

        var vertexBufferLayout = new VertexBufferLayout
        {
            ArrayStride = 32,
            StepMode = VertexStepMode.Vertex,
            AttributeCount = 3,
            Attributes = attributes,
        };

        // Standard over-compositing blend: new pixels are blended over existing ones
        // using their alpha value. Result = src × srcα + dst × (1 − srcα).
        var blend = new BlendState
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
        };

        // Describes the single render target this pipeline writes to.
        // The format must match what the surface's swap chain was configured with,
        // so that the GPU knows how to pack color values into each pixel.
        var colorTarget = new ColorTargetState
        {
            Format = format,
            WriteMask = ColorWriteMask.All,
            Blend = &blend,
        };

        var fragmentState = new FragmentState
        {
            Module = (ShaderModule*)shaderModule.DangerousGetHandle(),
            EntryPoint = (byte*)fragmentEntry,
            TargetCount = 1,
            Targets = &colorTarget,
        };

        var pipelineDesc = new RenderPipelineDescriptor
        {
            Layout = pipelineLayout,

            Vertex = new VertexState
            {
                Module = (ShaderModule*)shaderModule.DangerousGetHandle(),
                EntryPoint = (byte*)vertexEntry,
                BufferCount = 1,
                Buffers = &vertexBufferLayout,
            },

            // TriangleList: every 3 consecutive vertices form one independent triangle.
            // 6 vertices → triangle (0,1,2) + triangle (3,4,5) → one rectangle.
            // CullMode.None means both front- and back-facing triangles are drawn.
            Primitive = new PrimitiveState
            {
                Topology = PrimitiveTopology.TriangleList,
                FrontFace = FrontFace.Ccw,
                CullMode = CullMode.None,
            },

            // 1 sample per pixel — no MSAA. Count=1 with all mask bits set is
            // the minimum valid configuration required by the WebGPU spec.
            Multisample = new MultisampleState
            {
                Count = 1,
                Mask = uint.MaxValue,
            },

            Fragment = &fragmentState,
            DepthStencil = null, // No depth testing — a flat 2D shape cannot occlude itself
        };

        var pipeline = new SafeRenderPipelineHandle(this.gd.Wgpu, this.gd.Handle, pipelineDesc);

        // The pipeline holds its own reference to the layout — release our handle.
        this.gd.Wgpu.PipelineLayoutRelease(pipelineLayout);

        SilkMarshal.Free(vertexEntry);
        SilkMarshal.Free(fragmentEntry);

        return pipeline;
    }

    /// <summary>
    /// Creates the bind group layout for <c>@group(0)</c>:
    ///   binding 0 → 2-D float texture (sampled in the fragment stage)
    ///   binding 1 → filtering sampler (used alongside the texture).
    /// </summary>
    private unsafe SafeBindGroupLayoutHandle CreateTextureBindGroupLayout()
    {
        var textureBinding = new BindGroupLayoutEntry
        {
            Binding = 0,
            Visibility = ShaderStage.Fragment,
            Texture = new TextureBindingLayout
            {
                SampleType = TextureSampleType.Float,
                ViewDimension = TextureViewDimension.Dimension2D,
                Multisampled = false,
            },
        };

        var samplerBinding = new BindGroupLayoutEntry
        {
            Binding = 1,
            Visibility = ShaderStage.Fragment,
            Sampler = new SamplerBindingLayout
            {
                Type = SamplerBindingType.Filtering,
            },
        };

        var entries = stackalloc BindGroupLayoutEntry[2];
        entries[0] = textureBinding;
        entries[1] = samplerBinding;

        var desc = new BindGroupLayoutDescriptor
        {
            EntryCount = 2,
            Entries = entries,
        };

        var handle = this.gd.Wgpu.DeviceCreateBindGroupLayout((Device*)this.gd.Handle.DangerousGetHandle(), in desc);

        return new SafeBindGroupLayoutHandle(this.gd, (nint)handle);
    }
}
