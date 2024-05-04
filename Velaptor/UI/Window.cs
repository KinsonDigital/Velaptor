// <copyright file="Window.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading.Tasks;
using Batching;
using Factories;
using Scene;

/// <summary>
/// A system window where graphics can be rendered.
/// </summary>
public abstract class Window : IWindow
{
    private readonly IWindow nativeWindow;
    private readonly IBatcher batcher;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    protected Window()
    {
        this.nativeWindow = WindowFactory.CreateWindow();
        this.batcher = IoC.Container.GetInstance<IBatcher>();

        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    /// <param name="window">The window implementation that contains the window functionality.</param>
    /// <param name="batcher">Controls the batching start and end process.</param>
    private protected Window(IWindow window, IBatcher batcher)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(batcher);

        this.nativeWindow = window;
        this.batcher = batcher;
        Init();
    }

    /// <inheritdoc/>
    public Action? Initialize
    {
        get => this.nativeWindow.Initialize;
        set => this.nativeWindow.Initialize = value;
    }

    /// <inheritdoc/>
    public Action<FrameTime>? Update
    {
        get => this.nativeWindow.Update;
        set => this.nativeWindow.Update = value;
    }

    /// <inheritdoc/>
    public Action<FrameTime>? Draw
    {
        get => this.nativeWindow.Draw;
        set => this.nativeWindow.Draw = value;
    }

    /// <inheritdoc/>
    public Action<SizeU>? WinResize
    {
        get => this.nativeWindow.WinResize;
        set => this.nativeWindow.WinResize = value;
    }

    /// <inheritdoc/>
    public Action? Uninitialize
    {
        get => this.nativeWindow.Uninitialize;
        set => this.nativeWindow.Uninitialize = value;
    }

    /// <inheritdoc/>
    public string Title
    {
        get => this.nativeWindow.Title;
        set => this.nativeWindow.Title = value;
    }

    /// <inheritdoc/>
    public Vector2 Position
    {
        get => this.nativeWindow.Position;
        set => this.nativeWindow.Position = value;
    }

    /// <inheritdoc/>
    public uint Width
    {
        get => this.nativeWindow.Width;
        set => this.nativeWindow.Width = value;
    }

    /// <inheritdoc/>
    public uint Height
    {
        get => this.nativeWindow.Height;
        set => this.nativeWindow.Height = value;
    }

    /// <inheritdoc/>
    public int UpdateFrequency
    {
        get => this.nativeWindow.UpdateFrequency;
        set => this.nativeWindow.UpdateFrequency = value;
    }

    /// <inheritdoc/>
    public bool AutoClearBuffer
    {
        get => this.nativeWindow.AutoClearBuffer;
        set => this.nativeWindow.AutoClearBuffer = value;
    }

    /// <inheritdoc/>
    public bool MouseCursorVisible
    {
        get => this.nativeWindow.MouseCursorVisible;
        set => this.nativeWindow.MouseCursorVisible = value;
    }

    /// <inheritdoc/>
    public StateOfWindow WindowState
    {
        get => this.nativeWindow.WindowState;
        set => this.nativeWindow.WindowState = value;
    }

    /// <inheritdoc/>
    public WindowBorder TypeOfBorder
    {
        get => this.nativeWindow.TypeOfBorder;
        set => this.nativeWindow.TypeOfBorder = value;
    }

    /// <inheritdoc/>
    public ISceneManager SceneManager => this.nativeWindow.SceneManager;

    /// <inheritdoc/>
    public bool AutoSceneLoading { get; set; } = true;

    /// <inheritdoc/>
    public bool AutoSceneUnloading { get; set; } = true;

    /// <inheritdoc/>
    public bool AutoSceneUpdating { get; set; } = true;

    /// <inheritdoc/>
    /// <remarks>
    /// If this is set to <c>false</c>, using <see cref="IBatcher.Begin"/> and <see cref="IBatcher.End"/>
    /// will be required to render the scene.
    /// </remarks>
    public bool AutoSceneRendering { get; set; } = true;

    /// <inheritdoc/>
    public bool Initialized => this.nativeWindow.Initialized;

    /// <inheritdoc/>
    public float Fps => this.nativeWindow.Fps;

    /// <summary>
    /// Shows the window.
    /// </summary>
    public void Show() => this.nativeWindow.Show();

    /// <summary>
    /// Shows the window asynchronously.
    /// </summary>
    /// <param name="afterStart">Executed after the application starts asynchronously.</param>
    /// <param name="afterUnload">Executed after the window has been unloaded.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    /// <remarks>
    ///     This runs the window on another thread.
    /// </remarks>
    public async Task ShowAsync(Action? afterStart = null, Action? afterUnload = null) =>
        await this.nativeWindow.ShowAsync(afterStart, afterUnload).ConfigureAwait(true);

    /// <inheritdoc/>
    public void Close() => this.nativeWindow.Close();

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Invoked when the window is loaded.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    protected virtual void OnLoad()
    {
        if (!AutoSceneLoading)
        {
            return;
        }

        this.nativeWindow.SceneManager.LoadContent();
    }

    /// <summary>
    /// Invoked when the window is updated.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    protected virtual void OnUpdate(FrameTime frameTime)
    {
        if (!AutoSceneUpdating)
        {
            return;
        }

        this.nativeWindow.SceneManager.Update(frameTime);
    }

    /// <summary>
    /// Invoked when the window renders its content.
    /// </summary>
    /// <param name="frameTime">The amount of time that has passed for the current frame.</param>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    protected virtual void OnDraw(FrameTime frameTime)
    {
        if (!AutoSceneRendering || this.nativeWindow.SceneManager.TotalScenes <= 0)
        {
            return;
        }

        this.batcher.Begin();

        this.nativeWindow.SceneManager.Render();

        this.batcher.End();
    }

    /// <summary>
    /// Invoked when the window is unloaded.
    /// </summary>
    protected virtual void OnUnload()
    {
        if (!AutoSceneUnloading)
        {
            return;
        }

        this.nativeWindow.SceneManager.UnloadContent();
    }

    /// <summary>
    /// Invoked when the window size changes.
    /// </summary>
    /// <param name="size">The new size.</param>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Used by library users.")]
    protected virtual void OnResize(SizeU size)
    {
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    [SuppressMessage(
        "ReSharper",
        "VirtualMemberNeverOverridden.Global",
        Justification = "Left for library users to override if needed.")]
    protected virtual void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.nativeWindow.Dispose();
        }

        this.isDisposed = true;

        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        // Only when not running unit tests, dispose of all Carbonate types
        IoC.DisposeOfRegisteredTypes();
    }

    /// <summary>
    /// Initializes the window.
    /// </summary>
    private void Init()
    {
        this.nativeWindow.Initialize = OnLoad;
        this.nativeWindow.Update = OnUpdate;
        this.nativeWindow.Draw = OnDraw;
        this.nativeWindow.WinResize = OnResize;
        this.nativeWindow.Uninitialize = OnUnload;

        // Set the update frequency to default value of 60
        // just in case the IWindow implementation is not
        this.nativeWindow.UpdateFrequency = 60;
    }
}
