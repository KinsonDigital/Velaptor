// <copyright file="SceneBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Carbonate.Fluent;
using Factories;
using ReactableData;

/// <summary>
/// A base scene to be used for creating new custom scenes.
/// </summary>
[DebuggerDisplay("Name = {Name}({Id})")]
public abstract class SceneBase : IScene
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBase"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    protected SceneBase() => Init(IoC.Container.GetInstance<IReactableFactory>());

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBase"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    private protected SceneBase(IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(reactableFactory);

        Init(reactableFactory);
    }

    /// <inheritdoc cref="IScene.Name"/>
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc cref="IScene.Id"/>
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc cref="IScene.Name"/>
    public bool IsLoaded { get; private set; }

    /// <inheritdoc/>
    public SizeU WindowSize { get; private set; }

    /// <inheritdoc/>
    public Point WindowCenter => new ((int)WindowSize.Width / 2, (int)WindowSize.Height / 2);

    /// <summary>
    /// Gets a value indicating whether or not the scene has been disposed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public virtual void LoadContent() => IsLoaded = true;

    /// <inheritdoc/>
    public virtual void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        IsLoaded = false;
    }

    /// <inheritdoc/>
    public virtual void Update(FrameTime frameTime)
    {
        if (!IsLoaded)
        {
        }
    }

    /// <inheritdoc/>
    public virtual void Render()
    {
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
    }

    /// <summary>
    /// Initializes the manager.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    private void Init(IReactableFactory reactableFactory)
    {
        var winSizeReactable = reactableFactory.CreatePushWindowSizeReactable();

        var winSizeSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.WindowSizeChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.WindowSizeChangedId)))
            .BuildOneWayReceive<WindowSizeData>(data =>
            {
                WindowSize = new SizeU(data.Width, data.Height);
            });

        winSizeReactable.Subscribe(winSizeSubscription);
    }
}
