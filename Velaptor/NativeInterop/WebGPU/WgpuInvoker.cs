// <copyright file="WgpuInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu;

using System.Diagnostics.CodeAnalysis;
using Handles;
using Structures;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using WebGpuBuffer = Silk.NET.WebGPU.Buffer;

/// <summary>
/// Invokes WebGPU calls.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test it due to direct interaction with the Silk.NET library.")]
internal sealed class WgpuInvoker : IWgpuInvoker
{
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WgpuInvoker"/> class.
    /// </summary>
    public WgpuInvoker() => Wgpu = WebGPU.GetApi();

    /// <inheritdoc/>
    public WebGPU Wgpu { get; }

    /// <inheritdoc/>
    public SafeDeviceHandle Device { get; set; } = null!;

    /// <inheritdoc/>
    public SafeQueueHandle Queue { get; set; } = null!;

    /// <inheritdoc/>
    public SafeInstanceHandle CreateInstance(in InstanceDescriptor descriptor)
    {
        unsafe
        {
            var pointer = Wgpu.CreateInstance(in descriptor);
            return new SafeInstanceHandle(this, (nint)pointer);
        }
    }

    /// <inheritdoc/>
    public void InstanceRelease(nint instance)
    {
        unsafe
        {
            Wgpu.InstanceRelease((Instance*)instance);
        }
    }

    /// <inheritdoc/>
    public void InstanceRequestAdapter(SafeInstanceHandle instance, SafeSurfaceHandle surface, SafeRequestAdapterCallback callback)
    {
        unsafe
        {
            var opts = new RequestAdapterOptions { CompatibleSurface = (Surface*)surface.DangerousGetHandle(), };
            Wgpu.InstanceRequestAdapter((Instance*)instance.DangerousGetHandle(),
                in opts,
                new PfnRequestAdapterCallback((status, a, msg, _) => callback(status, (nint)a, (nint)msg, nint.Zero)),
                null);
        }
    }

    /// <inheritdoc/>
    public Bool32 AdapterGetLimits(SafeAdapterHandle adapter, ref SupportedLimits limits)
    {
        unsafe
        {
            fixed (SupportedLimits* pLimits = &limits)
            {
                return Wgpu.AdapterGetLimits((Adapter*)adapter.DangerousGetHandle(), pLimits);
            }
        }
    }

    /// <inheritdoc/>
    public void AdapterRequestDevice(SafeAdapterHandle adapter, in DeviceDescriptor descriptor, SafeRequestDeviceCallback callback)
    {
        unsafe
        {
            Wgpu.AdapterRequestDevice((Adapter*)adapter.DangerousGetHandle(),
                in descriptor,
                new PfnRequestDeviceCallback((status, d, msg, _) => callback(status, (nint)d, (nint)msg, nint.Zero)),
                null);
        }
    }

    /// <inheritdoc/>
    public void DeviceSetUncapturedErrorCallback(SafeDeviceHandle device, SafeErrorCallback callback)
    {
        unsafe
        {
            Wgpu.DeviceSetUncapturedErrorCallback((Device*)device.DangerousGetHandle(),
                new PfnErrorCallback((type, msg, _) => callback(type, (nint)msg, nint.Zero)),
                null);
        }
    }

    /// <inheritdoc/>
    public nint DeviceGetQueue(SafeDeviceHandle device)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceGetQueue((Device*)device.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public void DeviceRelease(nint device)
    {
        unsafe
        {
            Wgpu.DeviceRelease((Device*)device);
        }
    }

    /// <summary>
    /// Creates a WebGPU surface from a window.
    /// </summary>
    /// <param name="wgpu">A WebGPU instance.</param>
    /// <param name="window">The window representation.</param>
    /// <param name="instance">The handle to the instance.</param>
    /// <returns>A newly created Surface.</returns>
    public nint CreateWebGpuSurface(WebGPU wgpu, IWindow window, SafeInstanceHandle instance)
    {
        unsafe
        {
            return (nint)window.CreateWebGPUSurface(wgpu, (Instance*)instance.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public void QueueRelease(nint queue)
    {
        unsafe
        {
            Wgpu.QueueRelease((Queue*)queue);
        }
    }

    /// <inheritdoc/>
    public SafeShaderModuleHandle DeviceCreateShaderModule(SafeDeviceHandle device, string wgsl)
    {
        var wgslPtr = SilkMarshal.StringToPtr(wgsl);
        unsafe
        {
            var wgslDesc = new ShaderModuleWGSLDescriptor
            {
                Chain = new ChainedStruct { SType = SType.ShaderModuleWgslDescriptor }, Code = (byte*)wgslPtr,
            };
            var moduleDesc = new ShaderModuleDescriptor { NextInChain = (ChainedStruct*)&wgslDesc, };
            var moduleHandle = (nint)Wgpu.DeviceCreateShaderModule((Device*)device.DangerousGetHandle(), in moduleDesc);

            // DeviceCreateShaderModule copies the source internally, so the
            // temporary pointer is no longer needed after the module is created.
            SilkMarshal.Free(wgslPtr);
            return new SafeShaderModuleHandle(this, moduleHandle);
        }
    }

    /// <inheritdoc/>
    public void ShaderModuleRelease(nint handle)
    {
        unsafe
        {
            Wgpu.ShaderModuleRelease((ShaderModule*)handle);
        }
    }

    /// <inheritdoc/>
    public void AdapterRelease(nint adapter)
    {
        unsafe
        {
            Wgpu.AdapterRelease((Adapter*)adapter);
        }
    }

    /// <inheritdoc/>
    public void SurfaceConfigure(SafeSurfaceHandle surface,
        SafeDeviceHandle device,
        TextureFormat format,
        TextureUsage usage,
        uint width,
        uint height,
        PresentMode presentMode)
    {
        unsafe
        {
            var config = new SurfaceConfiguration
            {
                Device = (Device*)device.DangerousGetHandle(),
                Format = format,
                Usage = usage,
                Width = width,
                Height = height,
                PresentMode = presentMode,
            };
            Wgpu.SurfaceConfigure((Surface*)surface.DangerousGetHandle(), in config);
        }
    }

    /// <inheritdoc/>
    public nint UnsafeSurfaceGetCurrentTexture(SafeSurfaceHandle surface)
    {
        unsafe
        {
            SurfaceTexture st = default;
            Wgpu.SurfaceGetCurrentTexture((Surface*)surface.DangerousGetHandle(), &st);
            return (nint)st.Texture;
        }
    }

    public SafeSurfaceTextureHandle SurfaceGetCurrentTexture(SafeSurfaceHandle surface)
    {
        unsafe
        {
            SurfaceTexture st = default;
            Wgpu.SurfaceGetCurrentTexture((Surface*)surface.DangerousGetHandle(), &st);
            return new SafeSurfaceTextureHandle(this, (nint)st.Texture, st.Status);
        }
    }

    /// <inheritdoc/>
    public void SurfaceUnconfigure(nint surface)
    {
        unsafe
        {
            Wgpu.SurfaceUnconfigure((Surface*)surface);
        }
    }

    /// <inheritdoc/>
    public void SurfaceRelease(nint surface)
    {
        unsafe
        {
            Wgpu.SurfaceRelease((Surface*)surface);
        }
    }

    /// <inheritdoc/>
    public TextureFormat SurfaceGetPreferredFormat(SafeSurfaceHandle surface, SafeAdapterHandle adapter)
    {
        unsafe
        {
            return Wgpu.SurfaceGetPreferredFormat((Surface*)surface.DangerousGetHandle(), (Adapter*)adapter.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public void TextureRelease(nint texture)
    {
        unsafe
        {
            Wgpu.TextureRelease((Texture*)texture);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetPipeline(SafeRenderPassEncoderHandle renderPassEncoder, SafeRenderPipelineHandle pipeline)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetPipeline((RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (RenderPipeline*)pipeline.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public SafePipelineLayoutHandle DeviceCreatePipelineLayout(SafeDeviceHandle device, string? label)
    {
        var labelPtr = label is not null ? SilkMarshal.StringToPtr(label) : 0;
        try
        {
            unsafe
            {
                var desc = new PipelineLayoutDescriptor { Label = (byte*)labelPtr, BindGroupLayoutCount = 0, BindGroupLayouts = null, };
                var handle = (nint)Wgpu.DeviceCreatePipelineLayout((Device*)device.DangerousGetHandle(), in desc);
                return new SafePipelineLayoutHandle(this, handle);
            }
        }
        finally
        {
            if (labelPtr != 0)
            {
                SilkMarshal.Free(labelPtr);
            }
        }
    }

    /// <inheritdoc/>
    public SafePipelineLayoutHandle DeviceCreatePipelineLayout(SafeDeviceHandle device,
        string? label,
        SafeBindGroupLayoutHandle[] bindGroupLayouts)
    {
        var labelPtr = label is not null ? SilkMarshal.StringToPtr(label) : 0;
        try
        {
            unsafe
            {
                // Extract native pointers from safe handles into a managed array, then pin it.
                var nativeLayouts = new BindGroupLayout*[bindGroupLayouts.Length];
                for (var i = 0; i < bindGroupLayouts.Length; i++)
                {
                    nativeLayouts[i] = (BindGroupLayout*)bindGroupLayouts[i].DangerousGetHandle();
                }

                fixed (BindGroupLayout** pLayouts = nativeLayouts)
                {
                    var desc = new PipelineLayoutDescriptor
                    {
                        Label = (byte*)labelPtr, BindGroupLayoutCount = (uint)bindGroupLayouts.Length, BindGroupLayouts = pLayouts,
                    };
                    var handle = (nint)Wgpu.DeviceCreatePipelineLayout((Device*)device.DangerousGetHandle(), in desc);
                    return new SafePipelineLayoutHandle(this, handle);
                }
            }
        }
        finally
        {
            if (labelPtr != 0)
            {
                SilkMarshal.Free(labelPtr);
            }
        }
    }

    /// <inheritdoc/>
    public SafeRenderPipelineHandle DeviceCreateRenderPipeline(SafeDeviceHandle device, in SafeRenderPipelineDescriptor descriptor)
    {
        var vertEntryPtr = SilkMarshal.StringToPtr(descriptor.Vertex.EntryPoint);
        var fragEntryPtr = SilkMarshal.StringToPtr(descriptor.Fragment.EntryPoint);

        try
        {
            unsafe
            {
                var vertBuffers = descriptor.Vertex.Buffers;
                var fragTargets = descriptor.Fragment.Targets;

                // Pin the vertex attribute arrays, build the buffer layout,
                // then construct the full descriptor tree on the stack.
                fixed (VertexAttribute* ptrAttributes = vertBuffers[0].Attributes)
                {
                    var vbLayout = new VertexBufferLayout
                    {
                        ArrayStride = vertBuffers[0].ArrayStride,
                        StepMode = vertBuffers[0].StepMode,
                        AttributeCount = (uint)vertBuffers[0].Attributes.Length,
                        Attributes = ptrAttributes,
                    };

                    BlendState* pBlend = null;

                    if (fragTargets[0].Blend is { } blendValue)
                    {
                        BlendState blend = blendValue;
                        pBlend = &blend;
                    }

                    var colorTarget = new ColorTargetState
                    {
                        Format = fragTargets[0].Format, WriteMask = fragTargets[0].WriteMask, Blend = pBlend,
                    };

                    var fragState = new FragmentState
                    {
                        Module = (ShaderModule*)descriptor.Fragment.Module.DangerousGetHandle(),
                        EntryPoint = (byte*)fragEntryPtr,
                        TargetCount = (uint)fragTargets.Length,
                        Targets = &colorTarget,
                    };

                    var vertState = new VertexState
                    {
                        Module = (ShaderModule*)descriptor.Vertex.Module.DangerousGetHandle(),
                        EntryPoint = (byte*)vertEntryPtr,
                        BufferCount = (uint)vertBuffers.Length,
                        Buffers = &vbLayout,
                    };

                    var nativeDesc = new RenderPipelineDescriptor
                    {
                        Layout = (PipelineLayout*)descriptor.Layout.DangerousGetHandle(),
                        Vertex = vertState,
                        Primitive = descriptor.Primitive,
                        Multisample = descriptor.Multisample,
                        Fragment = &fragState,
                        DepthStencil = null,
                    };

                    var handle = (nint)Wgpu.DeviceCreateRenderPipeline((Device*)device.DangerousGetHandle(), in nativeDesc);

                    return new SafeRenderPipelineHandle(this, handle);
                }
            }
        }
        finally
        {
            SilkMarshal.Free(vertEntryPtr);
            SilkMarshal.Free(fragEntryPtr);
        }
    }

    /// <inheritdoc/>
    public void PipelineLayoutRelease(nint pipelineLayout)
    {
        unsafe
        {
            Wgpu.PipelineLayoutRelease((PipelineLayout*)pipelineLayout);
        }
    }

    /// <inheritdoc/>
    public void RenderPipelineRelease(nint pipeline)
    {
        unsafe
        {
            Wgpu.RenderPipelineRelease((RenderPipeline*)pipeline);
        }
    }

    /// <inheritdoc/>
    public SafeBindGroupHandle DeviceCreateBindGroup(SafeDeviceHandle device,
        SafeBindGroupLayoutHandle layout,
        SafeTextureViewHandle textureView,
        SafeSamplerHandle sampler)
    {
        unsafe
        {
            var entries = stackalloc BindGroupEntry[2];
            entries[0] = new BindGroupEntry { Binding = 0, TextureView = (TextureView*)textureView.DangerousGetHandle(), };
            entries[1] = new BindGroupEntry { Binding = 1, Sampler = (Sampler*)sampler.DangerousGetHandle(), };
            var desc = new BindGroupDescriptor { Layout = (BindGroupLayout*)layout.DangerousGetHandle(), EntryCount = 2, Entries = entries, };
            var handle = (nint)Wgpu.DeviceCreateBindGroup((Device*)device.DangerousGetHandle(), in desc);
            return new SafeBindGroupHandle(this, handle);
        }
    }

    /// <inheritdoc/>
    public void BindGroupRelease(nint bindGroup)
    {
        unsafe
        {
            Wgpu.BindGroupRelease((BindGroup*)bindGroup);
        }
    }

    /// <inheritdoc/>
    public SafeBindGroupLayoutHandle DeviceCreateBindGroupLayout(SafeDeviceHandle device, BindGroupLayoutEntry[] entries)
    {
        unsafe
        {
            fixed (BindGroupLayoutEntry* pEntries = entries)
            {
                var desc = new BindGroupLayoutDescriptor { EntryCount = (uint)entries.Length, Entries = pEntries, };
                var handle = (nint)Wgpu.DeviceCreateBindGroupLayout((Device*)device.DangerousGetHandle(), in desc);
                return new SafeBindGroupLayoutHandle(this, handle);
            }
        }
    }

    /// <inheritdoc/>
    public void BindGroupLayoutRelease(nint bindGroupLayout)
    {
        unsafe
        {
            Wgpu.BindGroupLayoutRelease((BindGroupLayout*)bindGroupLayout);
        }
    }

    /// <inheritdoc/>
    public SafeSamplerHandle DeviceCreateSampler(SafeDeviceHandle device, in SamplerDescriptor descriptor)
    {
        unsafe
        {
            var handle = (nint)Wgpu.DeviceCreateSampler((Device*)device.DangerousGetHandle(), in descriptor);
            return new SafeSamplerHandle(this, handle);
        }
    }

    /// <inheritdoc/>
    public void SamplerRelease(nint sampler)
    {
        unsafe
        {
            Wgpu.SamplerRelease((Sampler*)sampler);
        }
    }

    /// <inheritdoc/>
    public nint DeviceCreateTexture(SafeDeviceHandle device, in TextureDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateTexture((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    /// <inheritdoc/>
    public void TextureDestroy(nint texture)
    {
        unsafe
        {
            Wgpu.TextureDestroy((Texture*)texture);
        }
    }

    /// <inheritdoc/>
    public nint UnsafeDeviceCreateCommandEncoder(SafeDeviceHandle device, in CommandEncoderDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateCommandEncoder((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    /// <inheritdoc/>
    public SafeCommandEncoderHandle DeviceCreateCommandEncoder(SafeDeviceHandle device, in CommandEncoderDescriptor descriptor)
    {
        unsafe
        {
            var handle = (nint)Wgpu.DeviceCreateCommandEncoder((Device*)device.DangerousGetHandle(), in descriptor);

            return new SafeCommandEncoderHandle(this, handle);
        }
    }

    /// <inheritdoc/>
    public void CommandEncoderRelease(nint encoder)
    {
        unsafe
        {
            Wgpu.CommandEncoderRelease((CommandEncoder*)encoder);
        }
    }

    /// <inheritdoc/>
    public SafeVertexBufferHandle DeviceCreateVertexBuffer(SafeDeviceHandle device, string? label, ulong size, BufferUsage usage)
    {
        var labelPtr = label is not null ? SilkMarshal.StringToPtr(label) : 0;
        try
        {
            unsafe
            {
                var desc = new BufferDescriptor
                {
                    Label = (byte*)labelPtr, Size = size, Usage = usage, MappedAtCreation = false,
                };
                var handle = (nint)Wgpu.DeviceCreateBuffer((Device*)device.DangerousGetHandle(), in desc);
                return new SafeVertexBufferHandle(this, handle);
            }
        }
        finally
        {
            if (labelPtr != 0)
            {
                SilkMarshal.Free(labelPtr);
            }
        }
    }

    /// <inheritdoc/>
    public SafeIndexBufferHandle DeviceCreateIndexBuffer(SafeDeviceHandle device, string? label, ulong size, BufferUsage usage)
    {
        var labelPtr = label is not null ? SilkMarshal.StringToPtr(label) : 0;
        try
        {
            unsafe
            {
                var desc = new BufferDescriptor
                {
                    Label = (byte*)labelPtr, Size = size, Usage = usage, MappedAtCreation = false,
                };
                var handle = (nint)Wgpu.DeviceCreateBuffer((Device*)device.DangerousGetHandle(), in desc);
                return new SafeIndexBufferHandle(this, handle);
            }
        }
        finally
        {
            if (labelPtr != 0)
            {
                SilkMarshal.Free(labelPtr);
            }
        }
    }

    /// <inheritdoc/>
    public void BufferDestroy(nint buffer)
    {
        unsafe
        {
            Wgpu.BufferDestroy((WebGpuBuffer*)buffer);
        }
    }

    /// <inheritdoc/>
    public void BufferRelease(nint buffer)
    {
        unsafe
        {
            Wgpu.BufferRelease((WebGpuBuffer*)buffer);
        }
    }

    /// <inheritdoc/>
    public nint TextureCreateView(SafeTextureHandle texture, in TextureViewDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.TextureCreateView((Texture*)texture.DangerousGetHandle(), in descriptor);
        }
    }

    /// <inheritdoc/>
    public nint TextureCreateView(SafeSurfaceTextureHandle surfaceTexture, in TextureViewDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.TextureCreateView((Texture*)surfaceTexture.DangerousGetHandle(), in descriptor);
        }
    }

    /// <inheritdoc/>
    public void TextureViewRelease(nint textureView)
    {
        unsafe
        {
            Wgpu.TextureViewRelease((TextureView*)textureView);
        }
    }

    /// <inheritdoc/>
    public nint UnsafeCommandEncoderBeginRenderPass(SafeCommandEncoderHandle encoder,
        SafeTextureViewHandle textureView,
        LoadOp loadOp,
        StoreOp storeOp,
        double r,
        double g,
        double b,
        double a)
    {
        unsafe
        {
            var colorAttachment = new RenderPassColorAttachment
            {
                View = (TextureView*)textureView.DangerousGetHandle(),
                LoadOp = loadOp,
                StoreOp = storeOp,
                ClearValue = new Color(r, g, b, a),
            };

            var passDesc = new RenderPassDescriptor { ColorAttachmentCount = 1, ColorAttachments = &colorAttachment, };

            return (nint)Wgpu.CommandEncoderBeginRenderPass((CommandEncoder*)encoder.DangerousGetHandle(), in passDesc);
        }
    }

    /// <inheritdoc/>
    public SafeRenderPassEncoderHandle CommandEncoderBeginRenderPass(SafeCommandEncoderHandle encoder,
        SafeTextureViewHandle textureView,
        LoadOp loadOp,
        StoreOp storeOp,
        double r,
        double g,
        double b,
        double a)
    {
        unsafe
        {
            var colorAttachment = new RenderPassColorAttachment
            {
                View = (TextureView*)textureView.DangerousGetHandle(),
                LoadOp = loadOp,
                StoreOp = storeOp,
                ClearValue = new Color(r, g, b, a),
            };
            var passDesc = new RenderPassDescriptor { ColorAttachmentCount = 1, ColorAttachments = &colorAttachment, };
            var handle = (nint)Wgpu.CommandEncoderBeginRenderPass((CommandEncoder*)encoder.DangerousGetHandle(), in passDesc);
            return new SafeRenderPassEncoderHandle(this, handle);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderEnd(nint renderPassEncoder)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderEnd((RenderPassEncoder*)renderPassEncoder);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderRelease(nint renderPassEncoder)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderRelease((RenderPassEncoder*)renderPassEncoder);
        }
    }

    /// <inheritdoc/>
    public nint CommandEncoderFinish(SafeCommandEncoderHandle encoder, in CommandBufferDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.CommandEncoderFinish((CommandEncoder*)encoder.DangerousGetHandle(), in descriptor);
        }
    }

    /// <inheritdoc/>
    public void CommandBufferRelease(nint commandBuffer)
    {
        unsafe
        {
            Wgpu.CommandBufferRelease((CommandBuffer*)commandBuffer);
        }
    }

    /// <inheritdoc/>
    public void QueueSubmit(SafeQueueHandle queue, uint commandCount, nint commands)
    {
        unsafe
        {
            var cmdBufPtr = (CommandBuffer*)commands;
            Wgpu.QueueSubmit((Queue*)queue.DangerousGetHandle(), commandCount, &cmdBufPtr);
        }
    }

    /// <inheritdoc/>
    public void SurfacePresent(SafeSurfaceHandle surface)
    {
        unsafe
        {
            Wgpu.SurfacePresent((Surface*)surface.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetVertexBuffer(SafeRenderPassEncoderHandle renderPassEncoder, uint slot, nint buffer, ulong offset, ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetVertexBuffer((RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                slot,
                (WebGpuBuffer*)buffer,
                offset,
                size);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetIndexBuffer(SafeRenderPassEncoderHandle renderPassEncoder,
        nint buffer,
        IndexFormat format,
        ulong offset,
        ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetIndexBuffer((RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (WebGpuBuffer*)buffer,
                format,
                offset,
                size);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderDrawIndexed(SafeRenderPassEncoderHandle renderPassEncoder,
        uint indexCount,
        uint instanceCount,
        uint firstIndex,
        int baseVertex,
        uint firstInstance)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderDrawIndexed((RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                indexCount,
                instanceCount,
                firstIndex,
                baseVertex,
                firstInstance);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetBindGroup(SafeRenderPassEncoderHandle renderPassEncoder,
        uint groupIndex,
        SafeBindGroupHandle bindGroup,
        nuint dynamicOffsetCount,
        nint dynamicOffsets)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetBindGroup((RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                groupIndex,
                (BindGroup*)bindGroup.DangerousGetHandle(),
                dynamicOffsetCount,
                (uint*)dynamicOffsets);
        }
    }

    /// <inheritdoc/>
    public void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, float[] data)
    {
        unsafe
        {
            fixed (float* pData = data)
            {
                Wgpu.QueueWriteBuffer((Queue*)queue.DangerousGetHandle(),
                    (WebGpuBuffer*)buffer,
                    bufferOffset,
                    pData,
                    (nuint)(data.Length * sizeof(float)));
            }
        }
    }

    /// <inheritdoc/>
    public void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, uint[] data)
    {
        unsafe
        {
            fixed (uint* pData = data)
            {
                Wgpu.QueueWriteBuffer((Queue*)queue.DangerousGetHandle(),
                    (WebGpuBuffer*)buffer,
                    bufferOffset,
                    pData,
                    (nuint)(data.Length * sizeof(uint)));
            }
        }
    }

    /// <inheritdoc/>
    public void QueueWriteTexture(SafeQueueHandle queue, nint texture, uint width, uint height, uint alignedBytesPerRow, byte[] data)
    {
        unsafe
        {
            fixed (byte* pData = data)
            {
                var destination = new ImageCopyTexture
                {
                    Texture = (Texture*)texture, MipLevel = 0, Origin = new Origin3D { X = 0, Y = 0, Z = 0 }, Aspect = TextureAspect.All,
                };
                var dataLayout = new TextureDataLayout { Offset = 0, BytesPerRow = alignedBytesPerRow, RowsPerImage = height, };
                var copySize = new Extent3D { Width = width, Height = height, DepthOrArrayLayers = 1, };
                Wgpu.QueueWriteTexture((Queue*)queue.DangerousGetHandle(), &destination, pData, (nuint)data.Length, &dataLayout, in copySize);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        Device.Dispose();
        Queue.Dispose();
        Wgpu.Dispose();
    }
}
