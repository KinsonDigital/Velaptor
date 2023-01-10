// <copyright file="RendererBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using Carbonate;
using Guards;
using NativeInterop.OpenGL;

/// <inheritdoc cref="IRenderer"/>
internal abstract class RendererBase : IRenderer
{
    private readonly IDisposable shutDownUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererBase"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    protected RendererBase(IGLInvoker gl, IPushReactable reactable)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(reactable);

        GL = gl;

        var shutDownName = this.GetExecutionMemberName(nameof(NotificationIds.SystemShuttingDownId));
        this.shutDownUnsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.SystemShuttingDownId,
            name: shutDownName,
            onReceive: ShutDown));
    }

    /// <summary>
    /// Gets the OpenGL invoker.
    /// </summary>
    protected IGLInvoker GL { get; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="RendererBase"/> is disposed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>
    /// Shuts down the application by disposing resources.
    /// </summary>
    protected virtual void ShutDown()
    {
        if (IsDisposed)
        {
            return;
        }

        this.shutDownUnsubscriber.Dispose();

        IsDisposed = true;
    }
}
