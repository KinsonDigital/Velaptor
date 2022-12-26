// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate;
using Graphics;
using Guards;
using Velaptor.NativeInterop.OpenGL;

/// <summary>
/// Creates <see cref="ITexture"/> objects for rendering.
/// </summary>
internal sealed class TextureFactory : ITextureFactory
{
    private readonly IGLInvoker gl;
    private readonly IOpenGLService mockGLService;
    private readonly IReactable reactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public TextureFactory()
    {
        this.gl = IoC.Container.GetInstance<IGLInvoker>();
        this.mockGLService = IoC.Container.GetInstance<IOpenGLService>();
        this.reactable = IoC.Container.GetInstance<IReactable>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactory"/> class.
    /// </summary>
    /// <param name="gl">Provides access to OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    internal TextureFactory(IGLInvoker gl, IOpenGLService openGLService, IReactable reactable)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(openGLService);
        EnsureThat.ParamIsNotNull(reactable);

        this.gl = gl;
        this.mockGLService = openGLService;
        this.reactable = reactable;
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

        return new Texture(this.gl, this.mockGLService, this.reactable, name, filePath, imageData);
    }
}
