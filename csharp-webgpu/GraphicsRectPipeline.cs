// <copyright file="GraphicsRectPipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;

/// <summary>
/// A separate render pipeline for drawing rounded rectangles. Unlike the main
/// texture pipeline, this pipeline uses a <b>vertex buffer</b> with per-vertex
/// shape attributes (position, bounding box, color, corner radii, etc.) and an
/// <b>index buffer</b> for indexed drawing. The vertex shader is a pass-through
/// (positions arrive already in NDC) and the fragment shader performs the
/// rounded-rect per-pixel math on the GPU.
/// </summary>
/// <remarks>
/// <para>
/// Pipeline layout is <b>empty</b> — no bind groups (no textures, no camera
/// uniform). The fragment shader uses <c>@builtin(position)</c> directly for
/// pixel-space coordinates.
/// </para>
/// </remarks>
internal sealed class GraphicsRectPipeline : IDisposable
{
    private readonly GraphicsDevice gd;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsRectPipeline"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="surface">The graphics surface (used to obtain the swap-chain pixel format).</param>
    /// <param name="shader">The compiled shader providing <c>vs_main</c> and <c>fs_main</c>.</param>
    public GraphicsRectPipeline(GraphicsDevice gd, GraphicsSurface surface, GraphicsShader shader)
    {
        this.gd = gd;

        Handle = BuildPipeline(shader.VertexHandle, shader.FragmentHandle, surface.Format);

        if (Handle == null)
        {
            throw new Exception("Failed to create rectangle render pipeline.");
        }
    }

    /// <summary>
    /// Gets the compiled GPU pipeline handle.
    /// </summary>
    public SafeRenderPipelineHandle Handle { get; private set; }

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw
    /// calls on the pass will use the rounded-rect shaders.
    /// </summary>
    /// <param name="pass">The active render pass encoder.</param>
    public void Bind(SafeRenderPassEncoderHandle pass)
    {
        this.gd.Wgpu.RenderPassEncoderSetPipeline(pass, Handle);
    }

    /// <summary>
    /// Releases the GPU pipeline handle.
    /// </summary>
    public void Dispose() => Handle.Dispose();

    /// <summary>
    /// Builds the render pipeline descriptor.
    /// </summary>
    private unsafe SafeRenderPipelineHandle BuildPipeline(SafeShaderModuleHandle vertModule, SafeShaderModuleHandle fragModule, TextureFormat format)
    {
        var vertexEntry = SilkMarshal.StringToPtr("vs_main");
        var fragmentEntry = SilkMarshal.StringToPtr("fs_main");

        // ── Empty pipeline layout: no bind groups ────────────────────────
        SafePipelineLayoutHandle pipelineLayoutHandle;
        ReadOnlySpan<byte> rectPipelineLayoutLabel = "Rect Pipeline Layout"u8;

        fixed (byte* rectPipelineLayoutStrPtr = rectPipelineLayoutLabel)
        {
            var pipelineDescriptor = new PipelineLayoutDescriptor
            {
                Label = rectPipelineLayoutStrPtr,
                BindGroupLayoutCount = 0,
                BindGroupLayouts = null,
            };

            pipelineLayoutHandle = this.gd.Wgpu.DeviceCreatePipelineLayout(
                this.gd.Handle,
                in pipelineDescriptor);
        }

        // ── Vertex buffer layout ─────────────────────────────────────────
        // 9 attributes packed into a 64-byte stride:
        //   location 0: vec2<f32> position          (8 bytes,  offset  0)
        //   location 1: vec4<f32> shape             (16 bytes, offset  8)
        //   location 2: vec4<f32> color             (16 bytes, offset 24)
        //   location 3: f32      isFilled           ( 4 bytes, offset 40)
        //   location 4: f32      borderThickness    ( 4 bytes, offset 44)
        //   location 5: f32      topLeftRadius      ( 4 bytes, offset 48)
        //   location 6: f32      topRightRadius     ( 4 bytes, offset 52)
        //   location 7: f32      bottomRightRadius  ( 4 bytes, offset 56)
        //   location 8: f32      bottomLeftRadius   ( 4 bytes, offset 60)
        var attributes = stackalloc VertexAttribute[9];
        attributes[0] = new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0, ShaderLocation = 0 };
        attributes[1] = new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 8, ShaderLocation = 1 };
        attributes[2] = new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 24, ShaderLocation = 2 };
        attributes[3] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 40, ShaderLocation = 3 };
        attributes[4] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 44, ShaderLocation = 4 };
        attributes[5] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 48, ShaderLocation = 5 };
        attributes[6] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 52, ShaderLocation = 6 };
        attributes[7] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 56, ShaderLocation = 7 };
        attributes[8] = new VertexAttribute { Format = VertexFormat.Float32, Offset = 60, ShaderLocation = 8 };

        var vertexBufferLayout = new VertexBufferLayout
        {
            ArrayStride = 64,
            StepMode = VertexStepMode.Vertex,
            AttributeCount = 9,
            Attributes = attributes,
        };

        // ── Blend state (same as texture pipeline) ───────────────────────
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

        var colorTarget = new ColorTargetState
        {
            Format = format,
            WriteMask = ColorWriteMask.All,
            Blend = &blend,
        };

        var fragmentState = new FragmentState
        {
            Module = (ShaderModule*)fragModule.DangerousGetHandle(),
            EntryPoint = (byte*)fragmentEntry,
            TargetCount = 1,
            Targets = &colorTarget,
        };

        // ── Pipeline descriptor ──────────────────────────────────────────
        var pipelineDesc = new RenderPipelineDescriptor
        {
            Layout = (PipelineLayout*)pipelineLayoutHandle.DangerousGetHandle(),
            Vertex = new VertexState
            {
                Module = (ShaderModule*)vertModule.DangerousGetHandle(),
                EntryPoint = (byte*)vertexEntry,
                BufferCount = 1,
                Buffers = &vertexBufferLayout,
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
            Fragment = &fragmentState,
            DepthStencil = null,
        };

        var pipeline = new SafeRenderPipelineHandle(this.gd.Wgpu, this.gd.Handle, ref pipelineDesc);

        pipelineLayoutHandle.Dispose();

        SilkMarshal.Free(vertexEntry);
        SilkMarshal.Free(fragmentEntry);

        return pipeline;
    }
}
