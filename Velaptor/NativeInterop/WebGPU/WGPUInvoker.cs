// <copyright file="WgpuInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu;

using System;
using System.Diagnostics.CodeAnalysis;
using Handles;
using Structures;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
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
    public void InstanceRequestAdapter(
        SafeInstanceHandle instance,
        SafeSurfaceHandle surface,
        SafeRequestAdapterCallback callback)
    {
        unsafe
        {
            var opts = new RequestAdapterOptions
            {
                CompatibleSurface = (Surface*)surface.DangerousGetHandle(),
            };

            Wgpu.InstanceRequestAdapter(
                (Instance*)instance.DangerousGetHandle(),
                in opts,
                new PfnRequestAdapterCallback((status, a, msg, _) =>
                    callback(status, (nint)a, (nint)msg, nint.Zero)),
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
    public void AdapterRequestDevice(
        SafeAdapterHandle adapter,
        in DeviceDescriptor descriptor,
        SafeRequestDeviceCallback callback)
    {
        unsafe
        {
            Wgpu.AdapterRequestDevice(
                (Adapter*)adapter.DangerousGetHandle(),
                in descriptor,
                new PfnRequestDeviceCallback((status, d, msg, _) =>
                    callback(status, (nint)d, (nint)msg, nint.Zero)),
                null);
        }
    }

    /// <inheritdoc/>
    public void DeviceSetUncapturedErrorCallback(SafeDeviceHandle device, SafeErrorCallback callback)
    {
        unsafe
        {
            Wgpu.DeviceSetUncapturedErrorCallback(
                (Device*)device.DangerousGetHandle(),
                new PfnErrorCallback((type, msg, _) =>
                    callback(type, (nint)msg, nint.Zero)),
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
                Chain = new ChainedStruct { SType = SType.ShaderModuleWgslDescriptor },
                Code = (byte*)wgslPtr,
            };

            var moduleDesc = new ShaderModuleDescriptor
            {
                NextInChain = (ChainedStruct*)&wgslDesc,
            };

            var moduleHandle = (nint)Wgpu.DeviceCreateShaderModule(
                (Device*)device.DangerousGetHandle(),
                in moduleDesc);

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
    public void SurfaceConfigure(SafeSurfaceHandle surface, in SurfaceConfiguration config)
    {
        unsafe
        {
            Wgpu.SurfaceConfigure((Surface*)surface.DangerousGetHandle(), in config);
        }
    }

    /// <inheritdoc/>
    public void SurfaceGetCurrentTexture(SafeSurfaceHandle surface, ref SurfaceTexture surfaceTexture)
    {
        unsafe
        {
            fixed (SurfaceTexture* ptr = &surfaceTexture)
            {
                Wgpu.SurfaceGetCurrentTexture((Surface*)surface.DangerousGetHandle(), ptr);
            }
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
            return Wgpu.SurfaceGetPreferredFormat(
                (Surface*)surface.DangerousGetHandle(),
                (Adapter*)adapter.DangerousGetHandle());
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
    public void RenderPassEncoderSetPipeline(
        SafeRenderPassEncoderHandle renderPassEncoder,
        SafeRenderPipelineHandle pipeline)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetPipeline(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (RenderPipeline*)pipeline.DangerousGetHandle());
        }
    }

    /// <inheritdoc/>
    public SafePipelineLayoutHandle DeviceCreatePipelineLayout(SafeDeviceHandle device, in PipelineLayoutDescriptor descriptor)
    {
        unsafe
        {
            var handle = (nint)Wgpu.DeviceCreatePipelineLayout(
                (Device*)device.DangerousGetHandle(),
                in descriptor);

            return new SafePipelineLayoutHandle(this, handle);
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
                var desc = new PipelineLayoutDescriptor
                {
                    Label = (byte*)labelPtr,
                    BindGroupLayoutCount = 0,
                    BindGroupLayouts = null,
                };

                var handle = (nint)Wgpu.DeviceCreatePipelineLayout(
                    (Device*)device.DangerousGetHandle(),
                    in desc);

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
    public SafePipelineLayoutHandle DeviceCreatePipelineLayout(
        SafeDeviceHandle device, string? label, SafeBindGroupLayoutHandle[] bindGroupLayouts)
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
                        Label = (byte*)labelPtr,
                        BindGroupLayoutCount = (uint)bindGroupLayouts.Length,
                        BindGroupLayouts = pLayouts,
                    };

                    var handle = (nint)Wgpu.DeviceCreatePipelineLayout(
                        (Device*)device.DangerousGetHandle(),
                        in desc);

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
    public SafeRenderPipelineHandle DeviceCreateRenderPipeline(
        SafeDeviceHandle device,
        in SafeRenderPipelineDescriptor descriptor)
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
                fixed (VertexAttribute* pAttribs = vertBuffers[0].Attributes)
                {
                    var vbLayout = new VertexBufferLayout
                    {
                        ArrayStride = vertBuffers[0].ArrayStride,
                        StepMode = vertBuffers[0].StepMode,
                        AttributeCount = (uint)vertBuffers[0].Attributes.Length,
                        Attributes = pAttribs,
                    };

                    BlendState* pBlend = null;

                    if (fragTargets[0].Blend is { } blendValue)
                    {
                        BlendState blend = blendValue;
                        pBlend = &blend;
                    }

                    var colorTarget = new ColorTargetState
                    {
                        Format = fragTargets[0].Format,
                        WriteMask = fragTargets[0].WriteMask,
                        Blend = pBlend,
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

                    var handle = (nint)Wgpu.DeviceCreateRenderPipeline(
                        (Device*)device.DangerousGetHandle(),
                        in nativeDesc);

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
    public nint DeviceCreateRenderPipeline(SafeDeviceHandle device, in RenderPipelineDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateRenderPipeline(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
    public nint DeviceCreateBindGroup(SafeDeviceHandle device, in BindGroupDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBindGroup(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
    public nint DeviceCreateBindGroupLayout(SafeDeviceHandle device, in BindGroupLayoutDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBindGroupLayout(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
        }
    }

    /// <inheritdoc/>
    public SafeBindGroupLayoutHandle DeviceCreateBindGroupLayout(
        SafeDeviceHandle device, BindGroupLayoutEntry[] entries)
    {
        unsafe
        {
            fixed (BindGroupLayoutEntry* pEntries = entries)
            {
                var desc = new BindGroupLayoutDescriptor
                {
                    EntryCount = (uint)entries.Length,
                    Entries = pEntries,
                };

                var handle = (nint)Wgpu.DeviceCreateBindGroupLayout(
                    (Device*)device.DangerousGetHandle(),
                    in desc);

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
    public nint DeviceCreateSampler(SafeDeviceHandle device, in SamplerDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateSampler(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
            return (nint)Wgpu.DeviceCreateTexture(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
    public nint DeviceCreateCommandEncoder(SafeDeviceHandle device, in CommandEncoderDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateCommandEncoder(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
    public nint DeviceCreateBuffer(SafeDeviceHandle device, in BufferDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBuffer(
                (Device*)device.DangerousGetHandle(),
                in descriptor);
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
    public nint TextureCreateView(nint texture, in TextureViewDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.TextureCreateView((Texture*)texture, in descriptor);
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
    public nint CommandEncoderBeginRenderPass(SafeCommandEncoderHandle encoder, in RenderPassDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.CommandEncoderBeginRenderPass(
                (CommandEncoder*)encoder.DangerousGetHandle(),
                in descriptor);
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
            return (nint)Wgpu.CommandEncoderFinish(
                (CommandEncoder*)encoder.DangerousGetHandle(),
                in descriptor);
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
    public void RenderPassEncoderSetVertexBuffer(
        SafeRenderPassEncoderHandle renderPassEncoder,
        uint slot,
        nint buffer,
        ulong offset,
        ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetVertexBuffer(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                slot,
                (WebGpuBuffer*)buffer,
                offset,
                size);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetIndexBuffer(
        SafeRenderPassEncoderHandle renderPassEncoder,
        nint buffer,
        IndexFormat format,
        ulong offset,
        ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetIndexBuffer(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (WebGpuBuffer*)buffer,
                format,
                offset,
                size);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderDrawIndexed(
        SafeRenderPassEncoderHandle renderPassEncoder,
        uint indexCount,
        uint instanceCount,
        uint firstIndex,
        int baseVertex,
        uint firstInstance)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderDrawIndexed(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                indexCount,
                instanceCount,
                firstIndex,
                baseVertex,
                firstInstance);
        }
    }

    /// <inheritdoc/>
    public void RenderPassEncoderSetBindGroup(
        SafeRenderPassEncoderHandle renderPassEncoder,
        uint groupIndex,
        SafeBindGroupHandle bindGroup,
        nuint dynamicOffsetCount,
        nint dynamicOffsets)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetBindGroup(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                groupIndex,
                (BindGroup*)bindGroup.DangerousGetHandle(),
                dynamicOffsetCount,
                (uint*)dynamicOffsets);
        }
    }

    /// <inheritdoc/>
    public void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, nint data, nuint size)
    {
        unsafe
        {
            Wgpu.QueueWriteBuffer(
                (Queue*)queue.DangerousGetHandle(),
                (WebGpuBuffer*)buffer,
                bufferOffset,
                (void*)data,
                size);
        }
    }

    /// <inheritdoc/>
    public void QueueWriteTexture(
        SafeQueueHandle queue,
        in ImageCopyTexture destination,
        nint data,
        nuint dataSize,
        in TextureDataLayout dataLayout,
        in Extent3D writeSize)
    {
        unsafe
        {
            fixed (ImageCopyTexture* destPtr = &destination)
            fixed (TextureDataLayout* layoutPtr = &dataLayout)
            {
                Wgpu.QueueWriteTexture(
                    (Queue*)queue.DangerousGetHandle(),
                    destPtr,
                    (void*)data,
                    dataSize,
                    layoutPtr,
                    in writeSize);
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
        GC.SuppressFinalize(this);
    }
}
