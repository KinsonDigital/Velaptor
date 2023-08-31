// <copyright file="ControlBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Content;
using Factories;
using Input;

/// <summary>
/// Represents a basic control with behavior that is shared among all controls.
/// </summary>
public abstract class ControlBase : IControl
{
    private readonly IAppInput<MouseState> mouse;
    private MouseState prevMouseState;
    private KeyboardState prevKeyboardState;
    private Point prevMousePos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlBase"/> class.
    /// </summary>
    /// <param name="keyboard">Manages keyboard input.</param>
    /// <param name="mouse">The system mouse.</param>
    protected ControlBase(IAppInput<KeyboardState> keyboard, IAppInput<MouseState> mouse)
    {
        Keyboard = keyboard;
        this.mouse = mouse;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlBase"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the IoC container.")]
    protected ControlBase()
    {
        Keyboard = HardwareFactory.GetKeyboard();
        this.mouse = HardwareFactory.CreateMouse();
    }

    /// <inheritdoc cref="IControl.Click"/>
    public event EventHandler<EventArgs>? Click;

    /// <inheritdoc cref="IControl.MouseDown"/>
    public event EventHandler<EventArgs>? MouseDown;

    /// <inheritdoc cref="IControl.MouseUp"/>
    public event EventHandler<EventArgs>? MouseUp;

    /// <inheritdoc cref="IControl.MouseMove"/>
    public event EventHandler<MouseMoveEventArgs>? MouseMove;

    /// <inheritdoc cref="IControl.KeyDown"/>
    public event EventHandler<KeyEventArgs>? KeyDown;

    /// <inheritdoc cref="IControl.KeyUp"/>
    public event EventHandler<KeyEventArgs>? KeyUp;

    /// <inheritdoc cref="IControl.Name"/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc cref="IControl.Position"/>
    public virtual Point Position { get; set; }

    /// <inheritdoc cref="IControl.Left"/>
    public virtual int Left
    {
        get => (int)(Position.X - (Width / 2f));
        set => Position = new Point((int)(value + (Width / 2f)), Position.Y);
    }

    /// <inheritdoc cref="IControl.Right"/>
    public virtual int Right
    {
        get => (int)(Position.X + (Width / 2f));
        set => Position = new Point((int)(value - (Width / 2f)), Position.Y);
    }

    /// <inheritdoc cref="IControl.Top"/>
    public virtual int Top
    {
        get => (int)(Position.Y - (Height / 2f));
        set => Position = new Point(Position.X, (int)(value + (Height / 2f)));
    }

    /// <inheritdoc cref="IControl.Bottom"/>
    public virtual int Bottom
    {
        get => (int)(Position.Y + (Height / 2f));
        set => Position = new Point(Position.X, (int)(value - (Height / 2f)));
    }

    /// <summary>
    /// Gets or sets the width of the <see cref="ControlBase"/>.
    /// </summary>
    public virtual uint Width { get; set; }

    /// <summary>
    /// Gets the half width of the <see cref="ControlBase"/>.
    /// </summary>
    public virtual uint HalfWidth => Width / 2;

    /// <summary>
    /// Gets or sets the height of the <see cref="ControlBase"/>.
    /// </summary>
    public virtual uint Height { get; set; }

    /// <summary>
    /// Gets the half height of the <see cref="ControlBase"/>.
    /// </summary>
    public virtual uint HalfHeight => Height / 2;

    /// <inheritdoc cref="IControl.Visible"/>
    public virtual bool Visible { get; set; } = true;

    /// <inheritdoc cref="IControl.Enabled"/>
    public virtual bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether or not the mouse is hovering over the button.
    /// </summary>
    public bool IsMouseOver { get; private set; }

    /// <inheritdoc cref="IContentLoadable.IsLoaded"/>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Gets or sets the color to apply to the control when the
    /// mouse button is in the down position over the control.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used by library users.")]
    public Color MouseDownColor { get; set; } = Color.FromArgb(255, 190, 190, 190);

    /// <summary>
    /// Gets or sets the color to apply to the control when the mouse button is hovering over the control.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Used by library users.")]
    public Color MouseHoverColor { get; set; } = Color.FromArgb(255, 230, 230, 230);

    /// <summary>
    /// Gets the tint color to apply the control surface when the mouse hovers over the control.
    /// </summary>
    /// <remarks>
    ///     This is used to signify to the user that the mouse is hovering over the control or
    ///     the mouse button is in the down position over the control.
    /// </remarks>
    protected Color TintColor { get; private set; } = Color.White;

    /// <summary>
    /// Gets the keyboard object used to process keyboard input.
    /// </summary>
    protected IAppInput<KeyboardState> Keyboard { get; init; }

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public virtual void LoadContent() => IsLoaded = true;

    /// <inheritdoc cref="IContentLoadable.UnloadContent"/>
    public virtual void UnloadContent()
    {
        foreach (var clickDelegate in this.Click?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.Click -= clickDelegate as EventHandler<EventArgs>;
        }

        foreach (var clickDelegate in this.MouseDown?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.MouseDown -= clickDelegate as EventHandler<EventArgs>;
        }

        foreach (var clickDelegate in this.MouseUp?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.MouseUp -= clickDelegate as EventHandler<EventArgs>;
        }

        foreach (var clickDelegate in this.MouseMove?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.MouseMove -= clickDelegate as EventHandler<MouseMoveEventArgs>;
        }

        foreach (var clickDelegate in this.KeyDown?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.KeyDown -= clickDelegate as EventHandler<KeyEventArgs>;
        }

        foreach (var clickDelegate in this.KeyUp?.GetInvocationList() ?? Array.Empty<Delegate>())
        {
            this.KeyUp -= clickDelegate as EventHandler<KeyEventArgs>;
        }

        IsLoaded = false;
    }

    /// <inheritdoc cref="IControl"/>
    public virtual void Update(FrameTime frameTime)
    {
        if (IsLoaded is false || Enabled is false)
        {
            return;
        }

        ProcessMouse();
        ProcessKeyboard();
    }

    /// <summary>
    /// Renders the control to the screen.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not originally intended to have a method body.")]
    public virtual void Render()
    {
    }

    /// <summary>
    /// Invoked when the mouse is in the down position over the control.
    /// Used when a child control needs to be notified if the mouse is the down position.
    /// </summary>
    protected virtual void OnMouseDown() => this.MouseDown?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Invoked when the mouse is in the up position after the mouse was in the up position over the control.
    /// Used when a child control needs to be notified if the mouse is in the up position.
    /// </summary>
    protected virtual void OnMouseUp()
    {
        this.MouseUp?.Invoke(this, EventArgs.Empty);
        this.Click?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Invoked when the mouse moves over the control.
    /// </summary>
    /// <param name="mousePosArgs">The position of the mouse over the control.</param>
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Used by library users.")]
    protected virtual void OnMouseMove(MouseMoveEventArgs mousePosArgs) => this.MouseMove?.Invoke(this, mousePosArgs);

    /// <summary>
    /// Invoked when a keyboard key is pressed into the down position from the up position.
    /// </summary>
    /// <param name="key">The key that is down.</param>
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Used by library users.")]
    protected virtual void OnKeyDown(KeyCode key) => this.KeyDown?.Invoke(this, new KeyEventArgs(key));

    /// <summary>
    /// Invoked when a keyboard key is lifted from the down position to the up position.
    /// </summary>
    /// <param name="key">The key that is up.</param>
    protected virtual void OnKeyUp(KeyCode key) => this.KeyUp?.Invoke(this, new KeyEventArgs(key));

    /// <summary>
    /// Process keyboard input.
    /// </summary>
    private void ProcessKeyboard()
    {
        var currKeyboardState = Keyboard.GetState();

        ProcessKeyDownEvents(currKeyboardState);
        ProcessKeyUpEvents(currKeyboardState, this.prevKeyboardState);

        this.prevKeyboardState = currKeyboardState;
    }

    /// <summary>
    /// Process any keyboard key down events.
    /// </summary>
    /// <param name="currState">The current state of the keyboard this frame.</param>
    [SuppressMessage("csharpsquid", "S3267", Justification = "Want to stick with a for-each loop. Not LINQ.")]
    private void ProcessKeyDownEvents(KeyboardState currState)
    {
        foreach (var downKey in currState.GetDownKeys())
        {
            OnKeyDown(downKey);
        }
    }

    /// <summary>
    /// Process any keyboard key up events.
    /// </summary>
    /// <param name="currState">The current state of the keyboard in this frame.</param>
    /// <param name="prevState">The previous state of the keyboard in the previous frame.</param>
    [SuppressMessage("csharpsquid", "S3267", Justification = "Want to stick with a for-each loop. Not LINQ.")]
    private void ProcessKeyUpEvents(KeyboardState currState, KeyboardState prevState)
    {
        foreach (var downKey in prevState.GetDownKeys())
        {
            if (currState.IsKeyUp(downKey))
            {
                OnKeyUp(downKey);
            }
        }
    }

    /// <summary>
    /// Process mouse input.
    /// </summary>
    private void ProcessMouse()
    {
        var currMouseState = this.mouse.GetState();

        var currMousePos = currMouseState.GetPosition();

        var halfWidth = (int)Width / 2;
        var halfHeight = (int)Height / 2;

        var controlRect = new Rectangle(Position.X - halfWidth, Position.Y - halfHeight, (int)Width, (int)Height);

        IsMouseOver = controlRect.Contains(currMouseState.GetX(), currMouseState.GetY());

        // If the mouse button is in the down position
        if (currMouseState.IsLeftButtonDown() && IsMouseOver)
        {
            TintColor = MouseDownColor;
        }
        else
        {
            TintColor = IsMouseOver ? MouseHoverColor : Color.White;
        }

        if (IsMouseOver)
        {
            // Invoke the mouse move event if the mouse has been moved
            if (currMousePos != this.prevMousePos)
            {
                // Position of the mouse relative to the top left corner of the window
                var globalPos = new Point(currMousePos.X, currMousePos.Y);

                // Get the X and Y position relative to the top right corner of the control
                // NOTE: Even though the origin of the control is still the center,
                // the local mouse pos is not relative to the origin, it is relative to the top left corner
                var localX = Math.Abs(Position.X - currMousePos.X - halfWidth);
                var localY = Math.Abs(Position.Y - currMousePos.Y - halfHeight);

                // Position of the mouse relative to the top left corner of the control
                var localPos = new Point(localX, localY);

                OnMouseMove(new MouseMoveEventArgs(globalPos, localPos));
            }

            if (currMouseState.IsLeftButtonDown())
            {
                OnMouseDown();
            }
        }

        if (currMouseState.IsLeftButtonUp() &&
            this.prevMouseState.IsLeftButtonDown() &&
            IsMouseOver)
        {
            OnMouseUp();
        }

        this.prevMouseState = currMouseState;
        this.prevMousePos = currMousePos;
    }
}
