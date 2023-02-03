// <copyright file="RendererBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using Carbonate.NonDirectional;
using Factories;
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
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    protected RendererBase(IGLInvoker gl, IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(gl);
        EnsureThat.ParamIsNotNull(reactableFactory);

        GL = gl;

        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        var shutDownName = this.GetExecutionMemberName(nameof(PushNotifications.SystemShuttingDownId));
        this.shutDownUnsubscriber = pushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.SystemShuttingDownId,
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
