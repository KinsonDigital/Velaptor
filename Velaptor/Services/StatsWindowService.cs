// <copyright file="StatsWindowService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using ImGuiNET;
using Input;
using NativeInterop.ImGui;

/// <inheritdoc/>
internal sealed class StatsWindowService : IStatsWindowService
{
    private const int PreRenderCount = 5;
    private const int CollapseArrowButtonWidth = 57;
    private const string Title = "Runtime Stats";
    private readonly IImGuiInvoker imGuiInvoker;
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
    /// <param name="imGuiInvoker">Invokes ImGui functions.</param>
    /// <param name="keyboard">Manages keyboard input.</param>
    public StatsWindowService(IImGuiInvoker imGuiInvoker, IAppInput<KeyboardState> keyboard)
    {
        ArgumentNullException.ThrowIfNull(imGuiInvoker);
        ArgumentNullException.ThrowIfNull(keyboard);

        this.imGuiInvoker = imGuiInvoker;
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

        this.imGuiInvoker.Begin(Title, ImGuiWindowFlags.None);

        if (Visible || !this.isInitialized)
        {
            this.imGuiInvoker.Text($"FPS: {this.fps.ToString(new CultureInfo("en-US"))}");
        }

        if (this.invokeCount < PreRenderCount)
        {
            this.imGuiInvoker.SetWindowSize(Vector2.Zero);
        }

        if (this.shouldSetPos)
        {
            this.imGuiInvoker.SetWindowPos(this.position.ToVector2());
            this.shouldSetPos = false;
        }

        if (!this.isInitialized && this.invokeCount >= PreRenderCount)
        {
           Init();
        }

        this.imGuiInvoker.End();

        this.invokeCount += 1;
        if (this.invokeCount < PreRenderCount)
        {
            Render();
        }
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

    /// <summary>
    /// Initializes the window.
    /// </summary>
    private void Init()
    {
        var winSize = this.imGuiInvoker.GetWindowSize();
        var textSize = this.imGuiInvoker.CalcTextSize(Title);

        var padding = this.imGuiInvoker.GetStyle().WindowPadding;
        var horizontalPadding = winSize.X < textSize.X + CollapseArrowButtonWidth
            ? padding.X + CollapseArrowButtonWidth
            : padding.X;
        var verticalPadding = padding.Y;

        var size = new Vector2((int)winSize.X + horizontalPadding, (int)winSize.Y + verticalPadding);
        Size = new Size((int)size.X, (int)size.Y);
        this.imGuiInvoker.SetWindowSize(size);

        this.Initialized?.Invoke(this, EventArgs.Empty);
        this.isInitialized = true;
    }
}
