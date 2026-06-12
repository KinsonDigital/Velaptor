// <copyright file="GraphicsTexture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.WebGPU;
using StbImageSharp;

/// <summary>
/// Loads a PNG image from disk, uploads it to the GPU as a 2-D texture, and creates
/// the <see cref="Silk.NET.WebGPU.BindGroup"/> that binds the texture view (binding 0)
/// and a linear sampler (binding 1) so the fragment shader can sample it.
/// </summary>
/// <remarks>
/// <para>
/// The upload uses <c>QueueWriteTexture</c>, which is the simplest path: the CPU
/// copies pixel bytes into a staging area managed by wgpu-native, which then
/// schedules a GPU-side copy into the final texture. No explicit staging buffer or
/// command encoder is required from application code.
/// </para>
/// <para>
/// WebGPU requires <c>bytesPerRow</c> to be a multiple of 256. When the image width
/// does not naturally satisfy that constraint, each row is padded with zeroed bytes
/// before uploading.
/// </para>
/// </remarks>
internal sealed class GraphicsTexture : IDisposable
{
    private const uint BytesPerRowAlignment = 256;

    private readonly GraphicsDevice gd;
    private SafeTextureHandle texture;
    private SafeTextureViewHandle textureView;
    private SafeSamplerHandle samplerHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsTexture"/> class.
    /// Loads the PNG at <paramref name="filePath"/>, uploads pixel data to the GPU,
    /// and creates the bind group that the shader reads from.
    /// </summary>
    /// <param name="gd">The graphics device that owns all GPU resources.</param>
    /// <param name="filePath">Path to the PNG image file to load.</param>
    /// <param name="bindGroupLayout">
    /// The bind group layout produced by <see cref="GraphicsPipeline"/>. The texture
    /// view and sampler are registered into a bind group that matches this layout
    /// (binding 0 = texture, binding 1 = sampler).
    /// </param>
    public GraphicsTexture(GraphicsDevice gd, string filePath, SafeBindGroupLayoutHandle bindGroupLayout)
    {
        this.gd = gd;
        Load(filePath, bindGroupLayout);
    }

    /// <summary>
    /// Gets the GPU bind group that wires the texture view to binding 0 and the
    /// sampler to binding 1. Pass this to <see cref="Frame.Draw"/> each frame so the
    /// fragment shader can sample the texture.
    /// </summary>
    public SafeBindGroupHandle BindGroup { get; private set; }

    /// <summary>
    /// Releases the bind group, sampler, texture view, and the underlying GPU texture.
    /// Call this after all in-flight frames that reference this texture have completed —
    /// in practice, after the device queue is idle or as part of the application teardown.
    /// </summary>
    public void Dispose()
    {
        BindGroup.Dispose();
        this.samplerHandle.Dispose();
        this.textureView.Dispose();
        this.texture.Dispose();
    }

    private void Load(string filePath, SafeBindGroupLayoutHandle bindGroupLayout)
    {
        // Decode the PNG into a contiguous RGBA8 byte array.
        // StbImageSharp wraps stb_image.h — a single-file public-domain C library that
        // handles PNG, JPEG, BMP, TGA, and more. ColorComponents.RedGreenBlueAlpha forces
        // a 4-channel output regardless of the source format.
        ImageResult image;
        using (var stream = File.OpenRead(filePath))
        {
            image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        }

        var width = (uint)image.Width;
        var height = (uint)image.Height;
        var rawPixels = image.Data;

        // WebGPU requires bytesPerRow to be a multiple of 256.
        // If the image width does not satisfy that, pad each row with zeroes.
        var unalignedBytesPerRow = width * 4;
        var alignedBytesPerRow = (unalignedBytesPerRow + (BytesPerRowAlignment - 1)) & ~(BytesPerRowAlignment - 1);

        byte[] uploadBuffer;
        if (alignedBytesPerRow == unalignedBytesPerRow)
        {
            uploadBuffer = rawPixels;
        }
        else
        {
            uploadBuffer = new byte[alignedBytesPerRow * height];
            for (var row = 0u; row < height; row++)
            {
                Array.Copy(
                    rawPixels,
                    (int)(row * unalignedBytesPerRow),
                    uploadBuffer,
                    (int)(row * alignedBytesPerRow),
                    (int)unalignedBytesPerRow);
            }
        }

        // Create the GPU texture: 2-D, RGBA8 sRGB, with TextureBinding + CopyDst usage.
        // Rgba8UnormSrgb tells the GPU the stored bytes are sRGB-encoded (as PNG files always are).
        // textureSample() will automatically linearise the values before the shader sees them,
        // so colours written to the (also sRGB) surface are correctly round-tripped.
        var textureDesc = new TextureDescriptor
        {
            Usage = TextureUsage.TextureBinding | TextureUsage.CopyDst,
            Dimension = TextureDimension.Dimension2D,
            Size = new Extent3D { Width = width, Height = height, DepthOrArrayLayers = 1 },
            Format = TextureFormat.Rgba8UnormSrgb,
            MipLevelCount = 1,
            SampleCount = 1,
        };

        this.texture = new SafeTextureHandle(this.gd.Wgpu, this.gd.Handle, in textureDesc);

        unsafe
        {
            // Upload the CPU pixel data to the GPU texture via the queue.
            // QueueWriteTexture is the simplest upload path — wgpu-native manages the staging internally.
            fixed (byte* uploadPtr = uploadBuffer)
            {
                var destination = new ImageCopyTexture
                {
                    Texture = (Texture*)this.texture.DangerousGetHandle(),
                    MipLevel = 0,
                    Origin = new Origin3D { X = 0, Y = 0, Z = 0 },
                    Aspect = TextureAspect.All,
                };

                var dataLayout = new TextureDataLayout
                {
                    Offset = 0,
                    BytesPerRow = alignedBytesPerRow,
                    RowsPerImage = height,
                };

                var copySize = new Extent3D { Width = width, Height = height, DepthOrArrayLayers = 1 };

                this.gd.Wgpu.QueueWriteTexture(
                    (Queue*)this.gd.Queue.DangerousGetHandle(),
                    in destination,
                    uploadPtr,
                    (nuint)uploadBuffer.Length,
                    in dataLayout,
                    in copySize);
            }
        }

        // Create a texture view — the typed lens the shader samples through.
        var viewDesc = new TextureViewDescriptor
        {
            Format = TextureFormat.Rgba8UnormSrgb,
            Dimension = TextureViewDimension.Dimension2D,
            MipLevelCount = 1,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All,
        };

        this.textureView = new SafeTextureViewHandle(this.gd.Wgpu, this.texture.DangerousGetHandle(), in viewDesc);

        // Create a linear sampler. ClampToEdge prevents colour bleeding at the texture border.
        var samplerDesc = new SamplerDescriptor
        {
            AddressModeU = AddressMode.ClampToEdge,
            AddressModeV = AddressMode.ClampToEdge,
            AddressModeW = AddressMode.ClampToEdge,
            MagFilter = FilterMode.Linear,
            MinFilter = FilterMode.Linear,
            MipmapFilter = MipmapFilterMode.Nearest,
            LodMinClamp = 0f,
            LodMaxClamp = 1f,
            Compare = CompareFunction.Undefined,
            MaxAnisotropy = 1,
        };

        this.samplerHandle = new SafeSamplerHandle(this.gd, in samplerDesc);

        unsafe
        {
            // Create the bind group, wiring the texture view to binding 0 and the sampler to binding 1.
            // These slot numbers must match the @group(0) @binding(...) declarations in the WGSL shader.
            var entries = stackalloc BindGroupEntry[2];
            entries[0] = new BindGroupEntry { Binding = 0, TextureView = (TextureView*)this.textureView.DangerousGetHandle() };
            entries[1] = new BindGroupEntry { Binding = 1, Sampler = (Sampler*)this.samplerHandle.DangerousGetHandle() };

            var bgDesc = new BindGroupDescriptor
            {
                Layout = (BindGroupLayout*)bindGroupLayout.DangerousGetHandle(),
                EntryCount = 2,
                Entries = entries,
            };

            BindGroup = new SafeBindGroupHandle(this.gd, in bgDesc);
        }
    }
}
