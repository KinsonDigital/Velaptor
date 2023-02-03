// <copyright file="RendererBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;

/// <summary>
/// Used to test the <see cref="RendererBase"/> class.
/// </summary>
internal sealed class RendererBaseFake : RendererBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBaseFake"/> class.
    /// </summary>
    /// <param name="gl"><see cref="IGLInvoker"/> mock used for testing.</param>
    /// <param name="reactableFactory"><see cref="IReactableFactory"/> mock used for creating reactables.</param>
    public RendererBaseFake(IGLInvoker gl, IReactableFactory reactableFactory)
        : base(gl, reactableFactory)
    {
    }

    /// <summary>
    /// Gets the mocked <see cref="IGLInvoker"/> for testing.
    /// </summary>
    public new IGLInvoker GL => base.GL;
}
