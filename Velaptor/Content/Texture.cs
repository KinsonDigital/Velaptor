// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Carbonate.Fluent;
using Carbonate.OneWay;
using Graphics;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using OpenGL;
using ReactableData;
using Velaptor.Factories;

/// <summary>
/// The texture to render to a screen.
/// </summary>
public sealed class Texture : ITexture
{
    private readonly IGLInvoker gl;
    private readonly IOpenGLService openGLService;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture"/> class.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="imageData">The image data of the texture.</param>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="imageData"/> is empty.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public Texture(string name, ImageData imageData)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(imageData.FilePath);

        this.gl = IoC.Container.GetInstance<IGLInvoker>();
        this.openGLService = IoC.Container.GetInstance<IOpenGLService>();
        var disposeReactable = IoC.Container.GetInstance<IPushReactable<DisposeTextureData>>();

        FilePath = imageData.FilePath;
        Init(disposeReactable, name, imageData);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture"/> class.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="filePath">The file path to the image file.</param>
    /// <exception cref="FileNotFoundException">Thrown if the <paramref name="filePath"/> is not found.</exception>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Texture(string name, string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        this.gl = IoC.Container.GetInstance<IGLInvoker>();
        this.openGLService = IoC.Container.GetInstance<IOpenGLService>();
        var imageLoader = IoC.Container.GetInstance<IImageLoader>();
        var disposeReactable = IoC.Container.GetInstance<IPushReactable<DisposeTextureData>>();

        var imageData = imageLoader.LoadImage(filePath);
        FilePath = filePath;
        Init(disposeReactable, name, imageData);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Texture"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="name">The name of the texture.</param>
    /// <param name="filePath">The file path to the image file.</param>
    /// <param name="imageData">The image data of the texture.</param>
    internal Texture(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory,
        string name,
        string filePath,
        ImageData imageData)
    {
        ArgumentNullException.ThrowIfNull(gl);
        ArgumentNullException.ThrowIfNull(openGLService);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        this.gl = gl;
        this.openGLService = openGLService;

        FilePath = filePath;

        var disposeReactable = reactableFactory.CreateDisposeTextureReactable();
        Init(disposeReactable, name, imageData);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Texture"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "De-constructors cannot be unit tested.")]
    ~Texture()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        Unload(new DisposeTextureData { TextureId = Id });
    }

    /// <inheritdoc/>
    public uint Id { get; private set; }

    /// <inheritdoc/>
    public string Name { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string FilePath { get; }

    /// <inheritdoc/>
    public uint Width { get; private set; }

    /// <inheritdoc/>
    public uint Height { get; private set; }

    /// <summary>
    /// Disposes of the texture if this texture's <see cref="Id"/> matches the texture ID in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data of the texture to dispose.</param>
    private void Unload(DisposeTextureData data)
    {
        if (this.isDisposed || Id != data.TextureId)
        {
            return;
        }

        this.gl.DeleteTexture(Id);

        this.isDisposed = true;
    }

    /// <summary>
    /// Initializes the <see cref="Texture"/>.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="name">The name of the texture.</param>
    /// <param name="imageData">The image data of the texture.</param>
    private void Init(IPushReactable<DisposeTextureData> disposeReactable, string name, ImageData imageData)
    {
        var disposeSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.TextureDisposedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.TextureDisposedId)))
            .BuildOneWayReceive<DisposeTextureData>(Unload);

        disposeReactable.Subscribe(disposeSubscription);

        if (imageData.IsEmpty())
        {
            throw new ArgumentException("The image data must not be empty.", nameof(imageData));
        }

        Id = this.gl.GenTexture();

        this.gl.BindTexture(GLTextureTarget.Texture2D, Id);
        this.openGLService.BindTexture2D(Id);

        Width = imageData.Width;
        Height = imageData.Height;

        Name = name;

        UploadDataToGpu(name, imageData);

        this.openGLService.UnbindTexture2D();
    }

    /// <summary>
    /// Uploads the given pixel data to the GPU.
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="imageData">The image data of the texture.</param>
    private void UploadDataToGpu(string name, ImageData imageData)
    {
        /*NOTE:
         * The incoming image data is in the ARGB byte layout.
         * The data layout required by OpenGL is RGBA.
         */
        var pixelData = new List<byte>();

        for (var y = 0; y < imageData.Height; y++)
        {
            var rowBytes = new List<byte>();

            for (var x = 0; x < imageData.Width; x++)
            {
                rowBytes.Add(imageData.Pixels[x, y].R);
                rowBytes.Add(imageData.Pixels[x, y].G);
                rowBytes.Add(imageData.Pixels[x, y].B);
                rowBytes.Add(imageData.Pixels[x, y].A);
            }

            pixelData.AddRange(rowBytes);
            rowBytes.Clear();
        }

        this.openGLService.LabelTexture(Id, name);

        // Set the min and mag filters to linear
        this.gl.TexParameter(
            target: GLTextureTarget.Texture2D,
            pname: GLTextureParameterName.TextureMinFilter,
            param: GLTextureMinFilter.Linear);

        this.gl.TexParameter(
            target: GLTextureTarget.Texture2D,
            pname: GLTextureParameterName.TextureMagFilter,
            param: GLTextureMagFilter.Linear);

        // Set the x(S) and y(T) axis wrap mode to repeat
        this.gl.TexParameter(
            target: GLTextureTarget.Texture2D,
            pname: GLTextureParameterName.TextureWrapS,
            param: GLTextureWrapMode.ClampToEdge);

        this.gl.TexParameter(
            target: GLTextureTarget.Texture2D,
            pname: GLTextureParameterName.TextureWrapT,
            param: GLTextureWrapMode.ClampToEdge);

        // Load the texture data to the GPU for the currently active texture slot
        this.gl.TexImage2D<byte>(
            target: GLTextureTarget.Texture2D,
            level: 0,
            GLInternalFormat.Rgba,
            width: imageData.Width,
            height: imageData.Height,
            border: 0,
            format: GLPixelFormat.Rgba,
            type: GLPixelType.UnsignedByte,
            pixels: pixelData.ToArray());
    }
}
