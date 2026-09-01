// <copyright file="Window.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using Batching;
using Content;
using Content.Fonts;
using Factories;
using Graphics;
using Graphics.Renderers;
using Input;
using Scene;
using WebGpu.Batching;

/// <summary>
/// A system window where graphics can be rendered.
/// </summary>
public abstract class Window : IWindow
{
    private readonly IWindow nativeWindow;
    private readonly IBatcher batcher;
    private readonly IAppInput<KeyboardState> keyboard;
    private readonly IContentManager contentManager;
    private readonly IFontRenderer fontRenderer;
    private readonly Dictionary<decimal, (string text, SizeF size)> fpsCache = new ();
    private readonly Queue<decimal> cacheInsertionOrder = new ();
    private KeyboardState prevKeyState;
    private IFont? font;
    private bool vpsVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    protected Window()
    {
        this.nativeWindow = WindowFactory.CreateWindow();
        this.batcher = IoC.Container.GetInstance<IBatcher>();
        this.contentManager = IoC.Container.GetInstance<IContentManager>();
        this.fontRenderer = IoC.Container.GetInstance<IFontRenderer>();
        this.keyboard = IoC.Container.GetInstance<IAppInput<KeyboardState>>();

        // Eagerly create the render coordinator and batch manager so they subscribe
        // to their reactables BEFORE the window fires GLInitializedId.
        // Without this:
        //   • RenderMediator is never created (IRenderMediator has no explicit consumer),
        //     so BatchHasEndedId goes unhandled, and no render-batch notifications are ever
        //     pushed to the individual renderers.
        //   • BatchingManager misses the BatchSizeChangedId notification that WgpuBatcher
        //     pushes inside its GLInitializedId handler, leaving all batch arrays at
        //     length-zero and causing IndexOutOfRangeException on the first AddXxxItem call.
        IoC.Container.GetInstance<IRenderMediator>();
        IoC.Container.GetInstance<IBatchingManager>();

        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    /// <param name="window">The window implementation that contains the window functionality.</param>
    /// <param name="batcher">Controls the batching start and end process.</param>
    /// <param name="contentManager">Manages content.</param>
    /// <param name="fontRenderer">Renders fonts.</param>
    /// <param name="keyboard">Provides keyboard input.</param>
    private protected Window(
        IWindow window,
        IBatcher batcher,
        IContentManager contentManager,
        IFontRenderer fontRenderer,
        IAppInput<KeyboardState> keyboard)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(batcher);
        ArgumentNullException.ThrowIfNull(contentManager);
        ArgumentNullException.ThrowIfNull(fontRenderer);
        ArgumentNullException.ThrowIfNull(keyboard);

        this.nativeWindow = window;
        this.batcher = batcher;
        this.keyboard = keyboard;
        this.contentManager = contentManager;
        this.fontRenderer = fontRenderer;

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

    /// <summary>
    /// Gets or sets the color of the FPS display.
    /// </summary>
    public Color FpsDisplayColor { get; set; } = Color.White;

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

    /// <inheritdoc/>
    public void Close() => this.nativeWindow.Close();

    /// <summary>
    /// Invoked when the window is loaded.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    protected virtual void OnLoad()
    {
        this.font = this.contentManager.LoadFont("TimesNewRoman-Regular.ttf", 14);

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
        ProcessInput();

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
            RenderStats();
            return;
        }

        this.batcher.Begin();

        this.nativeWindow.SceneManager.Render();

        RenderStats();

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
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API for users.")]
    protected virtual void OnResize(SizeU size) => this.nativeWindow.SceneManager.Resize(size);

    /// <summary>
    /// Processes input.
    /// </summary>
    private void ProcessInput()
    {
        var currentKeyState = this.keyboard.GetState();

        var currentKeysUp = !currentKeyState.AnyCtrlKeysDown() || !currentKeyState.AnyAltKeysDown() ||
                            !currentKeyState.AnyShiftKeysDown() || currentKeyState.IsKeyUp(KeyCode.S);

        var prevKeysDown = this.prevKeyState.AnyCtrlKeysDown() && this.prevKeyState.AnyAltKeysDown() &&
                           this.prevKeyState.AnyShiftKeysDown() && this.prevKeyState.IsKeyDown(KeyCode.S);

        if (currentKeysUp && prevKeysDown)
        {
            this.vpsVisible = !this.vpsVisible;
        }

        this.prevKeyState = currentKeyState;
    }

    /// <summary>
    /// Renders various stats to the screen.
    /// </summary>
    private void RenderStats()
    {
        if (!this.vpsVisible || this.font is null)
        {
            return;
        }

        // NOTE: Case the Fps value to a decimal to ensure 2 decimal places due to IEEE 754 binary floating-point representation
        var roundedFps = Math.Round((decimal)Fps, 2);

        // If the value has not been cached, cache it
        if (!this.fpsCache.TryGetValue(roundedFps, out var fpsData))
        {
            // Remove the oldest item if at capacity
            if (this.fpsCache.Count >= 200)
            {
                var oldestKey = this.cacheInsertionOrder.Dequeue();
                this.fpsCache.Remove(oldestKey);
            }

            fpsData.text = roundedFps.ToString(CultureInfo.CurrentCulture);
            var size = this.font.Measure(fpsData.text);
            fpsData.size = size;

            this.fpsCache[roundedFps] = fpsData;
            this.cacheInsertionOrder.Enqueue(roundedFps);
        }

        var halfWidth = fpsData.size.Width / 2f;
        var halfHeight = fpsData.size.Height / 2f;

        this.fontRenderer.Render(this.font, fpsData.text, new Vector2(halfWidth + 10, Height - (halfHeight + 10)), FpsDisplayColor);
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
