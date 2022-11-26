// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using Graphics;
using Guards;
using Velaptor.NativeInterop.OpenGL;
using Reactables.Core;
using Reactables.ReactableData;

/// <summary>
/// Creates <see cref="ITexture"/> objects for rendering.
/// </summary>
internal sealed class TextureFactory : ITextureFactory
{
    private readonly IGLInvoker gl;
    private readonly IOpenGLService mockGLService;
    private readonly IReactable<DisposeTextureData> disposeTexturesReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public TextureFactory()
    {
        this.gl = IoC.Container.GetInstance<IGLInvoker>();
        this.mockGLService = IoC.Container.GetInstance<IOpenGLService>();
        this.disposeTexturesReactable = IoC.Container.GetInstance<IReactable<DisposeTextureData>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    /// <param name="gl">Provides access to OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="disposeTexturesReactable">Sends push notifications to dispose of textures.</param>
    internal TextureFactory(IGLInvoker gl, IOpenGLService openGLService, IReactable<DisposeTextureData> disposeTexturesReactable)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(disposeTexturesReactable);

        this.gl = gl;
        this.mockGLService = openGLService;
        this.disposeTexturesReactable = disposeTexturesReactable;
    }

    /// <inheritdoc/>
    public ITexture Create(string name, string filePath, ImageData imageData)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "The string parameter must not be null or empty.");
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath), "The string parameter must not be null or empty.");
        }

        return new Texture(this.gl, this.mockGLService, this.disposeTexturesReactable, name, filePath, imageData);
    }
}
