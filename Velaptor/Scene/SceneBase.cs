// <copyright file="SceneBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Carbonate;
using Factories;

/// <summary>
/// A base scene to be used for creating new custom scenes.
/// </summary>
[DebuggerDisplay("Name = {Name}({Id})")]
public abstract class SceneBase : IScene
{
    private IDisposable? unsubscriber;

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
        var pushWinSizeReactable = reactableFactory.CreatePushWindowSizeReactable();

        this.unsubscriber = pushWinSizeReactable.CreateOneWayReceive(
            PushNotifications.WindowSizeChangedId,
            (data) =>
            {
                WindowSize = new SizeU(data.Width, data.Height);
            },
            () => this.unsubscriber?.Dispose());

        // Get the size of the window just in case the scene is being created before
        // the loading of the window and gl init has completed.
        var pullWinSizeReactable = reactableFactory.CreatePullWindowSizeReactable();
        var winSizeData = pullWinSizeReactable.Pull(PullNotifications.GetWindowSizeId);
        WindowSize = new SizeU(winSizeData.Width, winSizeData.Height);
    }
}
