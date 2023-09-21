// <copyright file="SceneBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Scene;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Carbonate.Fluent;
using Content;
using Factories;
using Guards;
using ReactableData;
using UI;

/// <summary>
/// A base scene to be used for creating new custom scenes.
/// </summary>
[DebuggerDisplay("Name = {Name}({Id})")]
public abstract class SceneBase : IScene
{
    private readonly List<IControl> controls = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBase"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    protected SceneBase()
    {
        ContentLoader = ContentLoaderFactory.CreateContentLoader();
        Init(IoC.Container.GetInstance<IReactableFactory>());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBase"/> class.
    /// </summary>
    /// <param name="contentLoader">Loads various kinds of content.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    private protected SceneBase(IContentLoader contentLoader, IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(contentLoader);
        EnsureThat.ParamIsNotNull(reactableFactory);

        ContentLoader = contentLoader;
        Init(reactableFactory);
    }

    /// <inheritdoc cref="IScene.Name"/>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the list of controls that have been added to the scene.
    /// </summary>
    public IReadOnlyList<IControl> Controls => this.controls.AsReadOnly();

    /// <inheritdoc cref="IScene.Id"/>
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc cref="IScene.Name"/>
    public bool IsLoaded { get; private set; }

    /// <inheritdoc/>
    public SizeU WindowSize { get; private set; }

    /// <inheritdoc/>
    public Point WindowCenter => new ((int)WindowSize.Width / 2, (int)WindowSize.Height / 2);

    /// <inheritdoc/>
    public IContentLoader ContentLoader { get; }

    /// <summary>
    /// Gets a value indicating whether or not the scene has been disposed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public void AddControl(IControl control) => this.controls.Add(control);

    /// <inheritdoc/>
    public void RemoveControl(IControl control) => this.controls.Remove(control);

    /// <inheritdoc/>
    public virtual void LoadContent()
    {
        foreach (var control in this.controls)
        {
            control.LoadContent();
        }

        IsLoaded = true;
    }

    /// <inheritdoc/>
    public virtual void UnloadContent()
    {
        if (!IsLoaded)
        {
            return;
        }

        UnloadAllControls();

        this.controls.Clear();
        IsLoaded = false;
    }

    /// <inheritdoc/>
    public virtual void Update(FrameTime frameTime)
    {
        if (!IsLoaded)
        {
            return;
        }

        foreach (var control in this.controls)
        {
            control.Update(frameTime);
        }
    }

    /// <inheritdoc/>
    public virtual void Render()
    {
        if (!IsLoaded)
        {
            return;
        }

        foreach (var control in this.controls)
        {
            control.Render();
        }
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

        if (disposing)
        {
            UnloadAllControls();

            this.controls.Clear();
        }

        IsDisposed = true;
    }

    /// <summary>
    /// Unloads all of the controls.
    /// </summary>
    private void UnloadAllControls()
    {
        foreach (var control in this.controls)
        {
            control.UnloadContent();
        }
    }

    /// <summary>
    /// Initializes the manager.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    private void Init(IReactableFactory reactableFactory)
    {
        var winSizeReactable = reactableFactory.CreateWindowSizeReactable();

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
