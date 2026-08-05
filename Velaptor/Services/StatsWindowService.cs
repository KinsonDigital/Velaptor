// <copyright file="StatsWindowService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Drawing;
using Input;

/// <inheritdoc/>
internal sealed class StatsWindowService : IStatsWindowService
{
    private const int PreRenderCount = 5;
    private const int CollapseArrowButtonWidth = 57;
    private const string Title = "Runtime Stats";
    private readonly IAppInput<KeyboardState> keyboard;
    private KeyboardState prevKeyState;
    private float fps;
    private bool isInitialized;
    private bool isDisposed;
    private bool shouldSetPos = true;
    private int invokeCount;
    private Point position;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatsWindowService"/> class.
    /// </summary>
    /// <param name="keyboard">Manages keyboard input.</param>
    public StatsWindowService(IAppInput<KeyboardState> keyboard)
    {
        ArgumentNullException.ThrowIfNull(keyboard);

        this.keyboard = keyboard;
    }

    /// <inheritdoc/>
    public event EventHandler? Initialized;

    /// <inheritdoc/>
    public Point Position
    {
        get => this.position;
        set
        {
            this.position = value;
            this.shouldSetPos = true;
        }
    }

    /// <inheritdoc/>
    public Size Size { get; private set; }

    /// <inheritdoc/>
    public bool Visible { get; set; }

    /// <inheritdoc/>
    public void UpdateFpsStat(float fpsStat) => this.fps = (float)Math.Round(fpsStat, 2);

    /// <inheritdoc/>
    public void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        var currentKeysUp = !currentKeyState.AnyCtrlKeysDown() || !currentKeyState.AnyAltKeysDown() ||
                            !currentKeyState.AnyShiftKeysDown() || currentKeyState.IsKeyUp(KeyCode.S);

        var prevKeysDown = this.prevKeyState.AnyCtrlKeysDown() && this.prevKeyState.AnyAltKeysDown() &&
                           this.prevKeyState.AnyShiftKeysDown() && this.prevKeyState.IsKeyDown(KeyCode.S);

        if (currentKeysUp && prevKeysDown)
        {
            Visible = !Visible;
        }

        this.prevKeyState = currentKeyState;
    }

    /// <inheritdoc/>
    public void Render()
    {
        if (!Visible)
        {
            return;
        }

        // TODO: Add ability to display stats to the user here.
        // This used to be done with the IMGUI invoker and implementation but that
        // needed to be removed. This needs to use the UILib project in the Testing folder
        // to render stats such as 'FPS'. This is used in the WgpuWindow to render the stats
        // at the highest most layer to prevent any game objects for rendering over it.

        // var fps = $"FPS: {this.fps.ToString(new CultureInfo("en-US"))}";

        // Init();  // Not sure that we need this anymore
        // Render();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.Initialized = null;

        this.isDisposed = true;
    }
}
