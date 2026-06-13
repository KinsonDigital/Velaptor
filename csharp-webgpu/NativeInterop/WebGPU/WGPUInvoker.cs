// <copyright file="WGPUInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.NativeInterop.WebGPU;

using Handles;
using Silk.NET.Core;
using Silk.NET.WebGPU;

/*
 * Classes that use wgpu API:
 *
 * Camera2D
 * Frame
 * GraphicsDevice
 * GraphicsRectBuffer
 * GraphicsRectPipeline
 * GraphicsShader
 * GraphicsSurface
 * GraphicsTexture
 * GraphicsTextureBuffer
 */

internal class WGPUInvoker
{
    public WGPUInvoker()
    {
        Wgpu = WebGPU.GetApi();
    }

    public WebGPU Wgpu { get; }

    public SafeInstanceHandle CreateInstance(ref readonly InstanceDescriptor descriptor)
    {
        unsafe
        {
            var pointer = Wgpu.CreateInstance(in descriptor);

            return new SafeInstanceHandle(this, (nint)pointer);
        }
    }

    public void InstanceRelease(nint instance)
    {
        unsafe
        {
            Wgpu.InstanceRelease((Instance*)instance);
        }
    }

    public void InstanceRequestAdapter(SafeInstanceHandle instance,
        ref readonly RequestAdapterOptions options,
        PfnRequestAdapterCallback callback)
    {
        unsafe
        {
            Wgpu.InstanceRequestAdapter((Instance*)instance.DangerousGetHandle(),
                in options,
                new PfnRequestAdapterCallback(callback),
                null);
        }
    }

    public Bool32 AdapterGetLimits(SafeAdapterHandle adapter, SupportedLimits limits)
    {
        unsafe
        {
            return Wgpu.AdapterGetLimits((Adapter*)adapter.DangerousGetHandle(), &limits);
        }
    }

    public void AdapterRequestDevice(SafeAdapterHandle adapter,
        ref readonly DeviceDescriptor descriptor,
        PfnRequestDeviceCallback callback)
    {
        unsafe
        {
            Wgpu.AdapterRequestDevice((Adapter*)adapter.DangerousGetHandle(), in descriptor, callback, null);
        }
    }

    public void DeviceSetUncapturedErrorCallback(SafeDeviceHandle device, PfnErrorCallback callback)
    {
        unsafe
        {
            Wgpu.DeviceSetUncapturedErrorCallback((Device*)device.DangerousGetHandle(), callback, null);
        }
    }

    public nint DeviceGetQueue(SafeDeviceHandle device)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceGetQueue((Device*)device.DangerousGetHandle());
        }
    }

    public void DeviceRelease(nint device)
    {
        unsafe
        {
            Wgpu.DeviceRelease((Device*)device);
        }
    }

    public void QueueRelease(nint queue)
    {
        unsafe
        {
            Wgpu.QueueRelease((Queue*)queue);
        }
    }

    public nint DeviceCreateShaderModule(SafeDeviceHandle device, ref readonly ShaderModuleDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateShaderModule((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void ShaderModuleRelease(nint handle)
    {
        unsafe
        {
            Wgpu.ShaderModuleRelease((ShaderModule*)handle);
        }
    }

    public void AdapterRelease(nint adapter)
    {
        unsafe
        {
            Wgpu.AdapterRelease((Adapter*)adapter);
        }
    }

    public void SurfaceConfigure(SafeSurfaceHandle surface, ref readonly SurfaceConfiguration config)
    {
        unsafe
        {
            Wgpu.SurfaceConfigure((Surface*)surface.DangerousGetHandle(), in config);
        }
    }

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

    public void SurfaceUnconfigure(nint surface)
    {
        unsafe
        {
            Wgpu.SurfaceUnconfigure((Surface*)surface);
        }
    }

    public void SurfaceRelease(nint surface)
    {
        unsafe
        {
            Wgpu.SurfaceRelease((Surface*)surface);
        }
    }

    public TextureFormat SurfaceGetPreferredFormat(SafeSurfaceHandle surface, SafeAdapterHandle adapter)
    {
        unsafe
        {
            return Wgpu.SurfaceGetPreferredFormat((Surface*)surface.DangerousGetHandle(), (Adapter*)adapter.DangerousGetHandle());
        }
    }

    public void TextureRelease(nint texture)
    {
        unsafe
        {
            Wgpu.TextureRelease((Texture*)texture);
        }
    }

    public void RenderPassEncoderSetPipeline(SafeRenderPassEncoderHandle renderPassEncoder, SafeRenderPipelineHandle pipeline)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetPipeline(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (RenderPipeline*)pipeline.DangerousGetHandle());
        }
    }

    public SafePipelineLayoutHandle DeviceCreatePipelineLayout(SafeDeviceHandle device, ref readonly PipelineLayoutDescriptor descriptor)
    {
        unsafe
        {
            var handle = (nint)Wgpu.DeviceCreatePipelineLayout((Device*)device.DangerousGetHandle(), in descriptor);

            return new SafePipelineLayoutHandle(this, handle);
        }
    }

    public void PipelineLayoutRelease(nint pipelineLayout)
    {
        unsafe
        {
            Wgpu.PipelineLayoutRelease((PipelineLayout*)pipelineLayout);
        }
    }

    public nint DeviceCreateRenderPipeline(SafeDeviceHandle device, ref readonly RenderPipelineDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateRenderPipeline((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void RenderPipelineRelease(nint pipeline)
    {
        unsafe
        {
            Wgpu.RenderPipelineRelease((RenderPipeline*)pipeline);
        }
    }

    public nint DeviceCreateBindGroup(SafeDeviceHandle device, ref readonly BindGroupDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBindGroup((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void BindGroupRelease(nint bindGroup)
    {
        unsafe
        {
            Wgpu.BindGroupRelease((BindGroup*)bindGroup);
        }
    }

    public nint DeviceCreateBindGroupLayout(SafeDeviceHandle device, ref readonly BindGroupLayoutDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBindGroupLayout((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void BindGroupLayoutRelease(nint bindGroupLayout)
    {
        unsafe
        {
            Wgpu.BindGroupLayoutRelease((BindGroupLayout*)bindGroupLayout);
        }
    }

    public nint DeviceCreateSampler(SafeDeviceHandle device, ref readonly SamplerDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateSampler((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void SamplerRelease(nint sampler)
    {
        unsafe
        {
            Wgpu.SamplerRelease((Sampler*)sampler);
        }
    }

    public nint DeviceCreateTexture(SafeDeviceHandle device, ref readonly TextureDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateTexture((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void TextureDestroy(nint texture)
    {
        unsafe
        {
            Wgpu.TextureDestroy((Texture*)texture);
        }
    }

    public nint DeviceCreateCommandEncoder(SafeDeviceHandle device, ref readonly CommandEncoderDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateCommandEncoder((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void CommandEncoderRelease(nint encoder)
    {
        unsafe
        {
            Wgpu.CommandEncoderRelease((CommandEncoder*)encoder);
        }
    }

    public nint DeviceCreateBuffer(SafeDeviceHandle device, ref readonly BufferDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.DeviceCreateBuffer((Device*)device.DangerousGetHandle(), in descriptor);
        }
    }

    public void BufferDestroy(nint buffer)
    {
        unsafe
        {
            Wgpu.BufferDestroy((Buffer*)buffer);
        }
    }

    public void BufferRelease(nint buffer)
    {
        unsafe
        {
            Wgpu.BufferRelease((Buffer*)buffer);
        }
    }

    public nint TextureCreateView(nint texture, ref readonly TextureViewDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.TextureCreateView((Texture*)texture, in descriptor);
        }
    }

    public void TextureViewRelease(nint textureView)
    {
        unsafe
        {
            Wgpu.TextureViewRelease((TextureView*)textureView);
        }
    }

    public nint CommandEncoderBeginRenderPass(SafeCommandEncoderHandle encoder, ref readonly RenderPassDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.CommandEncoderBeginRenderPass((CommandEncoder*)encoder.DangerousGetHandle(), in descriptor);
        }
    }

    public void RenderPassEncoderEnd(nint renderPassEncoder)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderEnd((RenderPassEncoder*)renderPassEncoder);
        }
    }

    public void RenderPassEncoderRelease(nint renderPassEncoder)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderRelease((RenderPassEncoder*)renderPassEncoder);
        }
    }

    public nint CommandEncoderFinish(SafeCommandEncoderHandle encoder, ref readonly CommandBufferDescriptor descriptor)
    {
        unsafe
        {
            return (nint)Wgpu.CommandEncoderFinish((CommandEncoder*)encoder.DangerousGetHandle(), in descriptor);
        }
    }

    public void CommandBufferRelease(nint commandBuffer)
    {
        unsafe
        {
            Wgpu.CommandBufferRelease((CommandBuffer*)commandBuffer);
        }
    }

    public void QueueSubmit(SafeQueueHandle queue, uint commandCount, nint commands)
    {
        unsafe
        {
            var cmdBufPtr = (CommandBuffer*)commands;
            Wgpu.QueueSubmit((Queue*)queue.DangerousGetHandle(), commandCount, &cmdBufPtr);
        }
    }

    public void SurfacePresent(SafeSurfaceHandle surface)
    {
        unsafe
        {
            Wgpu.SurfacePresent((Surface*)surface.DangerousGetHandle());
        }
    }

    public void RenderPassEncoderSetVertexBuffer(SafeRenderPassEncoderHandle renderPassEncoder, uint slot, nint buffer, ulong offset, ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetVertexBuffer(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                slot,
                (Buffer*)buffer,
                offset,
                size);
        }
    }

    public void RenderPassEncoderSetIndexBuffer(SafeRenderPassEncoderHandle renderPassEncoder, nint buffer, IndexFormat format, ulong offset, ulong size)
    {
        unsafe
        {
            Wgpu.RenderPassEncoderSetIndexBuffer(
                (RenderPassEncoder*)renderPassEncoder.DangerousGetHandle(),
                (Buffer*)buffer,
                format,
                offset,
                size);
        }
    }

    public void RenderPassEncoderDrawIndexed(SafeRenderPassEncoderHandle renderPassEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance)
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

    public void RenderPassEncoderSetBindGroup(SafeRenderPassEncoderHandle renderPassEncoder, uint groupIndex, SafeBindGroupHandle bindGroup, nuint dynamicOffsetCount, nint dynamicOffsets)
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

    public void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, nint data, nuint size)
    {
        unsafe
        {
            Wgpu.QueueWriteBuffer((Queue*)queue.DangerousGetHandle(), (Buffer*)buffer, bufferOffset, (void*)data, size);
        }
    }

    public void QueueWriteTexture(SafeQueueHandle queue, ref readonly ImageCopyTexture destination, nint data, nuint dataSize, ref readonly TextureDataLayout dataLayout, ref readonly Extent3D writeSize)
    {
        unsafe
        {
            fixed (ImageCopyTexture* destPtr = &destination)
            fixed (TextureDataLayout* layoutPtr = &dataLayout)
            fixed (Extent3D* sizePtr = &writeSize)
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
}
