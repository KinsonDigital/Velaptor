// <copyright file="IWgpuInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu;

using System;
using Handles;
using Structures;
using Silk.NET.Core;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

/// <summary>
/// Invokes WebGPU functions.
/// </summary>
internal interface IWgpuInvoker : IDisposable
{
    /// <summary>
    /// Gets the WebGPU API instance.
    /// </summary>
    WebGPU Wgpu { get; }

    /// <summary>
    /// Gets or sets the GPU device handle.
    /// </summary>
    SafeDeviceHandle Device { get; set; }

    /// <summary>
    /// Gets or sets the GPU queue handle.
    /// </summary>
    SafeQueueHandle Queue { get; set; }

    /// <summary>
    /// Creates a WebGPU instance.
    /// </summary>
    /// <param name="descriptor">The instance descriptor.</param>
    /// <returns>The instance handle.</returns>
    SafeInstanceHandle CreateInstance(in InstanceDescriptor descriptor);

    /// <summary>
    /// Releases a WebGPU instance.
    /// </summary>
    /// <param name="instance">The instance pointer to release.</param>
    void InstanceRelease(nint instance);

    /// <summary>
    /// Requests a GPU adapter from the instance.
    /// </summary>
    /// <param name="instance">The instance handle.</param>
    /// <param name="surface">The surface the adapter must support.</param>
    /// <param name="callback">The callback invoked when the request completes.</param>
    void InstanceRequestAdapter(SafeInstanceHandle instance, SafeSurfaceHandle surface, SafeRequestAdapterCallback callback);

    /// <summary>
    /// Gets the adapter limits.
    /// </summary>
    /// <param name="adapter">The adapter handle.</param>
    /// <param name="limits">The limits to populate.</param>
    /// <returns>True if the query succeeded.</returns>
    Bool32 AdapterGetLimits(SafeAdapterHandle adapter, ref SupportedLimits limits);

    /// <summary>
    /// Requests a device from the adapter.
    /// </summary>
    /// <param name="adapter">The adapter handle.</param>
    /// <param name="descriptor">The device descriptor.</param>
    /// <param name="callback">The callback invoked when the request completes.</param>
    void AdapterRequestDevice(SafeAdapterHandle adapter, in DeviceDescriptor descriptor, SafeRequestDeviceCallback callback);

    /// <summary>
    /// Sets the uncaptured error callback on the device.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="callback">The error callback.</param>
    void DeviceSetUncapturedErrorCallback(SafeDeviceHandle device, SafeErrorCallback callback);

    /// <summary>
    /// Gets the queue from the device.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <returns>A pointer to the queue.</returns>
    nint DeviceGetQueue(SafeDeviceHandle device);

    /// <summary>
    /// Releases a device.
    /// </summary>
    /// <param name="device">The device pointer to release.</param>
    void DeviceRelease(nint device);

    /// <summary>
    /// Creates a WebGPU surface from a window.
    /// </summary>
    /// <param name="wgpu">A WebGPU instance.</param>
    /// <param name="window">The window representation.</param>
    /// <param name="instance">The handle to the instance.</param>
    /// <returns>A newly created Surface.</returns>
    nint CreateWebGpuSurface(WebGPU wgpu, IWindow window, SafeInstanceHandle instance);

    /// <summary>
    /// Releases a queue.
    /// </summary>
    /// <param name="queue">The queue pointer to release.</param>
    void QueueRelease(nint queue);

    /// <summary>
    /// Creates a shader module from WGSL source code. The native pointer
    /// marshaling is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="wgsl">The WGSL shader source to compile.</param>
    /// <returns>A safe handle to the compiled shader module.</returns>
    SafeShaderModuleHandle DeviceCreateShaderModule(SafeDeviceHandle device, string wgsl);

    /// <summary>
    /// Releases a shader module.
    /// </summary>
    /// <param name="handle">The shader module pointer.</param>
    void ShaderModuleRelease(nint handle);

    /// <summary>
    /// Releases an adapter.
    /// </summary>
    /// <param name="adapter">The adapter pointer to release.</param>
    void AdapterRelease(nint adapter);

    /// <summary>
    /// Configures a surface for presentation.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    /// <param name="config">The surface configuration.</param>
    void SurfaceConfigure(SafeSurfaceHandle surface, in SurfaceConfiguration config);

    /// <summary>
    /// Configures a surface for presentation. All pointer marshaling is handled
    /// internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    /// <param name="device">The device handle.</param>
    /// <param name="format">The pixel format for the swap chain.</param>
    /// <param name="usage">How the surface textures will be used.</param>
    /// <param name="width">The framebuffer width in pixels.</param>
    /// <param name="height">The framebuffer height in pixels.</param>
    /// <param name="presentMode">The presentation mode (e.g. <see cref="PresentMode.Fifo"/>).</param>
    void SurfaceConfigure(
        SafeSurfaceHandle surface,
        SafeDeviceHandle device,
        TextureFormat format,
        TextureUsage usage,
        uint width,
        uint height,
        PresentMode presentMode);

    /// <summary>
    /// Gets the current texture for a surface.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    /// <param name="surfaceTexture">The surface texture to populate.</param>
    void SurfaceGetCurrentTexture(SafeSurfaceHandle surface, ref SurfaceTexture surfaceTexture);

    /// <summary>
    /// Gets the current surface texture and wraps it in a safe handle. The caller
    /// is responsible for disposing the returned handle before requesting the next frame.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    /// <returns>A safe handle to the current surface texture.</returns>
    SafeSurfaceTextureHandle SurfaceGetCurrentTexture(SafeSurfaceHandle surface);

    /// <summary>
    /// Unconfigures a surface.
    /// </summary>
    /// <param name="surface">The surface pointer.</param>
    void SurfaceUnconfigure(nint surface);

    /// <summary>
    /// Releases a surface.
    /// </summary>
    /// <param name="surface">The surface pointer to release.</param>
    void SurfaceRelease(nint surface);

    /// <summary>
    /// Gets the preferred texture format for a surface.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    /// <param name="adapter">The adapter handle.</param>
    /// <returns>The preferred texture format.</returns>
    TextureFormat SurfaceGetPreferredFormat(SafeSurfaceHandle surface, SafeAdapterHandle adapter);

    /// <summary>
    /// Releases a texture.
    /// </summary>
    /// <param name="texture">The texture pointer to release.</param>
    void TextureRelease(nint texture);

    /// <summary>
    /// Sets the render pipeline on a render pass encoder.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder handle.</param>
    /// <param name="pipeline">The render pipeline handle.</param>
    void RenderPassEncoderSetPipeline(SafeRenderPassEncoderHandle renderPassEncoder, SafeRenderPipelineHandle pipeline);

    /// <summary>
    /// Creates a pipeline layout whose only resource is a debug label.
    /// Convenience overload that eliminates the <c>fixed(byte*)</c> ceremony
    /// for the most common case (zero bind groups).
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="label">An optional UTF-8 debug label, or <see langword="null"/>.</param>
    /// <returns>The pipeline layout handle.</returns>
    SafePipelineLayoutHandle DeviceCreatePipelineLayout(SafeDeviceHandle device, string? label);

    /// <summary>
    /// Creates a pipeline layout with a debug label and one or more bind group
    /// layouts. All pointer marshaling is handled internally.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="label">An optional UTF-8 debug label, or <see langword="null"/>.</param>
    /// <param name="bindGroupLayouts">The bind group layout handles to attach.</param>
    /// <returns>The pipeline layout handle.</returns>
    SafePipelineLayoutHandle DeviceCreatePipelineLayout(
        SafeDeviceHandle device, string? label, SafeBindGroupLayoutHandle[] bindGroupLayouts);

    /// <summary>
    /// Releases a pipeline layout.
    /// </summary>
    /// <param name="pipelineLayout">The pipeline layout pointer.</param>
    void PipelineLayoutRelease(nint pipelineLayout);

    /// <summary>
    /// Creates a render pipeline from a safe descriptor. All pointer marshaling
    /// (entry-point strings, handle casts, address-of for nested structs) is
    /// performed inside the implementation so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The safe pipeline descriptor.</param>
    /// <returns>A safe handle to the compiled render pipeline.</returns>
    SafeRenderPipelineHandle DeviceCreateRenderPipeline(SafeDeviceHandle device, in SafeRenderPipelineDescriptor descriptor);
    nint DeviceCreateRenderPipeline(SafeDeviceHandle device, in RenderPipelineDescriptor descriptor);

    /// <summary>
    /// Releases a render pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline pointer.</param>
    void RenderPipelineRelease(nint pipeline);

    /// <summary>
    /// Creates a bind group.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The bind group descriptor.</param>
    /// <returns>A pointer to the bind group.</returns>
    nint DeviceCreateBindGroup(SafeDeviceHandle device, in BindGroupDescriptor descriptor);

    /// <summary>
    /// Creates a bind group from a managed array of entries. Pointer marshaling
    /// is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="layout">The bind group layout this group is compatible with.</param>
    /// <param name="entries">The bind group entries describing each binding.</param>
    /// <returns>A safe handle to the bind group.</returns>
    SafeBindGroupHandle DeviceCreateBindGroup(
        SafeDeviceHandle device, SafeBindGroupLayoutHandle layout, BindGroupEntry[] entries);

    /// <summary>
    /// Creates a bind group with a texture view at binding 0 and a sampler
    /// at binding 1. All pointer marshaling is handled internally so callers
    /// avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="layout">The bind group layout this group is compatible with.</param>
    /// <param name="textureView">The texture view handle for binding 0.</param>
    /// <param name="sampler">The sampler handle for binding 1.</param>
    /// <returns>A safe handle to the bind group.</returns>
    SafeBindGroupHandle DeviceCreateBindGroup(
        SafeDeviceHandle device,
        SafeBindGroupLayoutHandle layout,
        SafeTextureViewHandle textureView,
        SafeSamplerHandle sampler);

    /// <summary>
    /// Releases a bind group.
    /// </summary>
    /// <param name="bindGroup">The bind group pointer.</param>
    void BindGroupRelease(nint bindGroup);

    /// <summary>
    /// Creates a bind group layout.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The bind group layout descriptor.</param>
    /// <returns>A pointer to the bind group layout.</returns>
    nint DeviceCreateBindGroupLayout(SafeDeviceHandle device, in BindGroupLayoutDescriptor descriptor);

    /// <summary>
    /// Creates a bind group layout from a managed array of entries.
    /// The <c>fixed</c> pinning is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="entries">The bind group layout entries describing each binding.</param>
    /// <returns>A safe handle to the bind group layout.</returns>
    SafeBindGroupLayoutHandle DeviceCreateBindGroupLayout(
        SafeDeviceHandle device, BindGroupLayoutEntry[] entries);

    /// <summary>
    /// Releases a bind group layout.
    /// </summary>
    /// <param name="bindGroupLayout">The bind group layout pointer.</param>
    void BindGroupLayoutRelease(nint bindGroupLayout);

    /// <summary>
    /// Creates a sampler.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The sampler descriptor.</param>
    /// <returns>A pointer to the sampler.</returns>
    SafeSamplerHandle DeviceCreateSampler(SafeDeviceHandle device, in SamplerDescriptor descriptor);

    /// <summary>
    /// Releases a sampler.
    /// </summary>
    /// <param name="sampler">The sampler pointer.</param>
    void SamplerRelease(nint sampler);

    /// <summary>
    /// Creates a texture.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The texture descriptor.</param>
    /// <returns>A pointer to the texture.</returns>
    nint DeviceCreateTexture(SafeDeviceHandle device, in TextureDescriptor descriptor);

    /// <summary>
    /// Destroys a texture.
    /// </summary>
    /// <param name="texture">The texture pointer.</param>
    void TextureDestroy(nint texture);

    /// <summary>
    /// Creates a command encoder.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The command encoder descriptor.</param>
    /// <returns>A pointer to the command encoder.</returns>
    SafeCommandEncoderHandle DeviceCreateCommandEncoder(SafeDeviceHandle device, in CommandEncoderDescriptor descriptor);

    /// <summary>
    /// Releases a command encoder.
    /// </summary>
    /// <param name="encoder">The encoder pointer.</param>
    void CommandEncoderRelease(nint encoder);

    /// <summary>
    /// Creates a GPU buffer.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="descriptor">The buffer descriptor.</param>
    /// <returns>A pointer to the buffer.</returns>
    nint DeviceCreateBuffer(SafeDeviceHandle device, in BufferDescriptor descriptor);

    /// <summary>
    /// Creates a vertex buffer on the device. The label pointer marshaling is
    /// handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="label">An optional UTF-8 debug label, or <see langword="null"/>.</param>
    /// <param name="size">The buffer size in bytes.</param>
    /// <param name="usage">The allowed usage flags for this buffer.</param>
    /// <returns>A safe handle to the vertex buffer.</returns>
    SafeVertexBufferHandle DeviceCreateVertexBuffer(SafeDeviceHandle device, string? label, ulong size, BufferUsage usage);

    /// <summary>
    /// Creates an index buffer on the device. The label pointer marshaling is
    /// handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="device">The device handle.</param>
    /// <param name="label">An optional UTF-8 debug label, or <see langword="null"/>.</param>
    /// <param name="size">The buffer size in bytes.</param>
    /// <param name="usage">The allowed usage flags for this buffer.</param>
    /// <returns>A safe handle to the index buffer.</returns>
    SafeIndexBufferHandle DeviceCreateIndexBuffer(SafeDeviceHandle device, string? label, ulong size, BufferUsage usage);

    /// <summary>
    /// Destroys a buffer.
    /// </summary>
    /// <param name="buffer">The buffer pointer.</param>
    void BufferDestroy(nint buffer);

    /// <summary>
    /// Releases a buffer.
    /// </summary>
    /// <param name="buffer">The buffer pointer.</param>
    void BufferRelease(nint buffer);

    /// <summary>
    /// Creates a texture view.
    /// </summary>
    /// <param name="texture">The texture pointer.</param>
    /// <param name="descriptor">The texture view descriptor.</param>
    /// <returns>A pointer to the texture view.</returns>
    nint TextureCreateView(nint texture, in TextureViewDescriptor descriptor);

    /// <summary>
    /// Releases a texture view.
    /// </summary>
    /// <param name="textureView">The texture view pointer.</param>
    void TextureViewRelease(nint textureView);

    /// <summary>
    /// Begins a render pass.
    /// </summary>
    /// <param name="encoder">The command encoder handle.</param>
    /// <param name="descriptor">The render pass descriptor.</param>
    /// <returns>A pointer to the render pass encoder.</returns>
    nint CommandEncoderBeginRenderPass(SafeCommandEncoderHandle encoder, in RenderPassDescriptor descriptor);

    /// <summary>
    /// Begins a render pass with a single color attachment. The pointer
    /// marshaling is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="encoder">The command encoder to begin the pass on.</param>
    /// <param name="textureView">The texture view to render into.</param>
    /// <param name="loadOp">How the attachment is loaded at pass start.</param>
    /// <param name="storeOp">How the attachment is stored at pass end.</param>
    /// <param name="r">The red clear-value component (linear-light for sRGB formats).</param>
    /// <param name="g">The green clear-value component.</param>
    /// <param name="b">The blue clear-value component.</param>
    /// <param name="a">The alpha clear-value component.</param>
    /// <returns>A safe handle to the active render pass encoder.</returns>
    SafeRenderPassEncoderHandle CommandEncoderBeginRenderPass(
        SafeCommandEncoderHandle encoder,
        SafeTextureViewHandle textureView,
        LoadOp loadOp,
        StoreOp storeOp,
        double r,
        double g,
        double b,
        double a);

    /// <summary>
    /// Ends a render pass.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder pointer.</param>
    void RenderPassEncoderEnd(nint renderPassEncoder);

    /// <summary>
    /// Releases a render pass encoder.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder pointer.</param>
    void RenderPassEncoderRelease(nint renderPassEncoder);

    /// <summary>
    /// Finishes recording a command encoder and returns the command buffer.
    /// </summary>
    /// <param name="encoder">The command encoder handle.</param>
    /// <param name="descriptor">The command buffer descriptor.</param>
    /// <returns>A pointer to the command buffer.</returns>
    nint CommandEncoderFinish(SafeCommandEncoderHandle encoder, in CommandBufferDescriptor descriptor);

    /// <summary>
    /// Releases a command buffer.
    /// </summary>
    /// <param name="commandBuffer">The command buffer pointer.</param>
    void CommandBufferRelease(nint commandBuffer);

    /// <summary>
    /// Submits command buffers to the queue.
    /// </summary>
    /// <param name="queue">The queue handle.</param>
    /// <param name="commandCount">The number of command buffers.</param>
    /// <param name="commands">Pointer to the command buffer array.</param>
    void QueueSubmit(SafeQueueHandle queue, uint commandCount, nint commands);

    /// <summary>
    /// Presents the surface texture to the screen.
    /// </summary>
    /// <param name="surface">The surface handle.</param>
    void SurfacePresent(SafeSurfaceHandle surface);

    /// <summary>
    /// Sets the vertex buffer on a render pass encoder.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder handle.</param>
    /// <param name="slot">The vertex buffer slot.</param>
    /// <param name="buffer">The buffer pointer.</param>
    /// <param name="offset">The byte offset into the buffer.</param>
    /// <param name="size">The size of the vertex buffer data in bytes.</param>
    void RenderPassEncoderSetVertexBuffer(SafeRenderPassEncoderHandle renderPassEncoder, uint slot, nint buffer, ulong offset, ulong size);

    /// <summary>
    /// Sets the index buffer on a render pass encoder.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder handle.</param>
    /// <param name="buffer">The buffer pointer.</param>
    /// <param name="format">The index format.</param>
    /// <param name="offset">The byte offset into the buffer.</param>
    /// <param name="size">The size of the index buffer data in bytes.</param>
    void RenderPassEncoderSetIndexBuffer(SafeRenderPassEncoderHandle renderPassEncoder, nint buffer, IndexFormat format, ulong offset, ulong size);

    /// <summary>
    /// Issues an indexed draw call.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder handle.</param>
    /// <param name="indexCount">The number of indices to draw.</param>
    /// <param name="instanceCount">The number of instances.</param>
    /// <param name="firstIndex">The first index offset.</param>
    /// <param name="baseVertex">The base vertex offset.</param>
    /// <param name="firstInstance">The first instance index.</param>
    void RenderPassEncoderDrawIndexed(SafeRenderPassEncoderHandle renderPassEncoder, uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance);

    /// <summary>
    /// Sets a bind group on a render pass encoder.
    /// </summary>
    /// <param name="renderPassEncoder">The render pass encoder handle.</param>
    /// <param name="groupIndex">The bind group index.</param>
    /// <param name="bindGroup">The bind group handle.</param>
    /// <param name="dynamicOffsetCount">The number of dynamic offsets.</param>
    /// <param name="dynamicOffsets">Pointer to the dynamic offsets.</param>
    void RenderPassEncoderSetBindGroup(SafeRenderPassEncoderHandle renderPassEncoder, uint groupIndex, SafeBindGroupHandle bindGroup, nuint dynamicOffsetCount, nint dynamicOffsets);

    /// <summary>
    /// Writes data to a buffer.
    /// </summary>
    /// <param name="queue">The queue handle.</param>
    /// <param name="buffer">The buffer pointer.</param>
    /// <param name="bufferOffset">The byte offset into the buffer.</param>
    /// <param name="data">The data pointer.</param>
    /// <param name="size">The size of data in bytes.</param>
    void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, nint data, nuint size);

    /// <summary>
    /// Writes a managed float array to a GPU buffer. The <c>fixed</c> pinning
    /// is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="queue">The queue to submit write to.</param>
    /// <param name="buffer">The destination GPU buffer.</param>
    /// <param name="bufferOffset">Byte offset into the destination buffer.</param>
    /// <param name="data">The float data to upload.</param>
    void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, float[] data);

    /// <summary>
    /// Writes a managed uint array to a GPU buffer. The <c>fixed</c> pinning
    /// is handled internally so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="queue">The queue to submit write to.</param>
    /// <param name="buffer">The destination GPU buffer.</param>
    /// <param name="bufferOffset">Byte offset into the destination buffer.</param>
    /// <param name="data">The uint data to upload.</param>
    void QueueWriteBuffer(SafeQueueHandle queue, nint buffer, ulong bufferOffset, uint[] data);

    /// <summary>
    /// Writes data to a texture.
    /// </summary>
    /// <param name="queue">The queue handle.</param>
    /// <param name="destination">The texture copy destination.</param>
    /// <param name="data">The data pointer.</param>
    /// <param name="dataSize">The data size in bytes.</param>
    /// <param name="dataLayout">The texture data layout.</param>
    /// <param name="writeSize">The size of the region to write.</param>
    void QueueWriteTexture(SafeQueueHandle queue, in ImageCopyTexture destination, nint data, nuint dataSize, in TextureDataLayout dataLayout, in Extent3D writeSize);

    /// <summary>
    /// Writes a managed byte array of RGBA pixel data to a GPU texture. The
    /// <c>fixed</c> pinning and descriptor construction are handled internally
    /// so callers avoid <c>unsafe</c> context.
    /// </summary>
    /// <param name="queue">The queue to submit write to.</param>
    /// <param name="texture">The destination texture handle.</param>
    /// <param name="width">The texture width in pixels.</param>
    /// <param name="height">The texture height in pixels.</param>
    /// <param name="alignedBytesPerRow">The row stride including alignment padding.</param>
    /// <param name="data">The pixel data in RGBA byte order.</param>
    void QueueWriteTexture(
        SafeQueueHandle queue,
        nint texture,
        uint width,
        uint height,
        uint alignedBytesPerRow,
        byte[] data);
}
