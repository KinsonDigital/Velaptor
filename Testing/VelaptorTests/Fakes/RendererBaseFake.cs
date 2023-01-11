// <copyright file="RendererBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Carbonate;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.OpenGL;

/// <summary>
/// Used to test the <see cref="RendererBase"/> class.
/// </summary>
internal class RendererBaseFake : RendererBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBaseFake"/> class.
    /// </summary>
    /// <param name="gl"><see cref="IGLInvoker"/> mock used for testing.</param>
    /// <param name="reactable"><see cref="IPushReactable"/> mock used for testing.</param>
    public RendererBaseFake(IGLInvoker gl, IPushReactable reactable)
        : base(gl, reactable)
    {
    }

    /// <summary>
    /// Gets the mocked <see cref="IGLInvoker"/> for testing.
    /// </summary>
    public new IGLInvoker GL => base.GL;
}
