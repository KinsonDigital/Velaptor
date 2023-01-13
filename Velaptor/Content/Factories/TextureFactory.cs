// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using Graphics;
using Guards;
using NativeInterop.OpenGL;
using Velaptor.Factories;

/// <summary>
/// Creates <see cref="ITexture"/> objects for rendering.
/// </summary>
internal sealed class TextureFactory : ITextureFactory
{
    private readonly IGLInvoker gl;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory reactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public TextureFactory()
    {
        this.gl = IoC.Container.GetInstance<IGLInvoker>();
        this.mockGLService = IoC.Container.GetInstance<IOpenGLService>();
        this.reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Sends and receives push notifications.</param>
    internal TextureFactory(IGLInvoker gl, IOpenGLService openGLService, IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.gl = gl;
        this.mockGLService = openGLService;
        this.reactableFactory = reactableFactory;
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

        return new Texture(this.gl, this.mockGLService, this.reactableFactory, name, filePath, imageData);
    }
}
