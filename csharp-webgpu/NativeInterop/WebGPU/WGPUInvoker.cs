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
}
