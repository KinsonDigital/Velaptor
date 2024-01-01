// <copyright file="MouseState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System.ComponentModel;
using System.Drawing;

/// <summary>
/// Represents the state of the mouse.
/// </summary>
public readonly struct MouseState
{
    private readonly Point position;
    private readonly int scrollWheelValue;
    private readonly bool isLeftButtonDown;
    private readonly bool isRightButtonDown;
    private readonly bool isMiddleButtonDown;
    private readonly MouseScrollDirection scrollDirection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseState"/> struct.
    /// </summary>
    /// <param name="pos">The position of the mouse.</param>
    /// <param name="isLeftButtonDown">True if the left button is down.</param>
    /// <param name="isRightButtonDown">True if the right button is down.</param>
    /// <param name="isMiddleButtonDown">True if the middle button is down.</param>
    /// <param name="scrollDirection">The travel direction of the mouse wheel.</param>
    /// <param name="scrollWheelValue">The value of the mouse wheel.</param>
    public MouseState(
        Point pos,
        bool isLeftButtonDown,
        bool isRightButtonDown,
        bool isMiddleButtonDown,
        MouseScrollDirection scrollDirection,
        int scrollWheelValue)
    {
        this.position = pos;
        this.isLeftButtonDown = isLeftButtonDown;
        this.isRightButtonDown = isRightButtonDown;
        this.isMiddleButtonDown = isMiddleButtonDown;
        this.scrollDirection = scrollDirection;
        this.scrollWheelValue = scrollWheelValue;
    }

    /// <summary>
    /// Gets or sets the position of the mouse.
    /// </summary>
    /// <returns>The position relative to the top left corner of the window.</returns>
    public Point GetPosition() => this.position;

    /// <summary>
    /// Gets or sets the X position of the mouse.
    /// </summary>
    /// <returns>The X position relative to the top left corner of the window.</returns>
    public int GetX() => this.position.X;

    /// <summary>
    /// Gets or sets the Y position of the mouse.
    /// </summary>
    /// <returns>The Y position relative to the top left corner of the window.</returns>
    public int GetY() => this.position.Y;

    /// <summary>
    /// Returns a value indicating whether or not the given mouse <paramref name="button"/>
    /// is in the down position.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button is in the down position.</returns>
    public bool IsButtonDown(MouseButton button) =>
        button switch
        {
            MouseButton.LeftButton => this.isLeftButtonDown,
            MouseButton.MiddleButton => this.isMiddleButtonDown,
            MouseButton.RightButton => this.isRightButtonDown,
            _ => throw new InvalidEnumArgumentException(
                nameof(button),
                (int)button,
                typeof(MouseButton)),
        };

    /// <summary>
    /// Returns a value indicating whether or not the given mouse <paramref name="button"/>
    /// is in the up position.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button is in the up position.</returns>
    /// <exception cref="InvalidEnumArgumentException">
    ///     Occurs if the <see cref="MouseButton"/> is an invalid value.
    /// </exception>
    public bool IsButtonUp(MouseButton button) =>
        button switch
        {
            MouseButton.LeftButton => !this.isLeftButtonDown,
            MouseButton.MiddleButton => !this.isMiddleButtonDown,
            MouseButton.RightButton => !this.isRightButtonDown,
            _ => throw new InvalidEnumArgumentException(
                nameof(button),
                (int)button,
                typeof(MouseButton))
        };

    /// <summary>
    /// Gets or sets a value indicating whether or not the left mouse button is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the button is down.</returns>
    public bool IsLeftButtonDown() => this.isLeftButtonDown;

    /// <summary>
    /// Returns a value indicating whether or not the left mouse button is in the up position.
    /// </summary>
    /// <returns><c>true</c> if the button is up.</returns>
    public bool IsLeftButtonUp() => !IsLeftButtonDown();

    /// <summary>
    /// Gets or sets a value indicating whether or not the middle mouse button is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the button is down.</returns>
    public bool IsMiddleButtonDown() => this.isMiddleButtonDown;

    /// <summary>
    /// Returns a value indicating whether or not the middle mouse button is in the up position.
    /// </summary>
    /// <returns><c>true</c> if the button is up.</returns>
    public bool IsMiddleButtonUp() => !IsMiddleButtonDown();

    /// <summary>
    /// Gets or sets a value indicating whether or not the right mouse button is in the down position.
    /// </summary>
    /// <returns><c>true</c> if the button is down.</returns>
    public bool IsRightButtonDown() => this.isRightButtonDown;

    /// <summary>
    /// Returns a value indicating whether or not the right mouse button is in the up position.
    /// </summary>
    /// <returns><c>true</c> if the button is up.</returns>
    public bool IsRightButtonUp() => !IsRightButtonDown();

    /// <summary>
    /// Returns a value indicating whether or not the state for the given <paramref name="mouseButton"/>
    /// is in the down or up position.
    /// </summary>
    /// <param name="mouseButton">The button state to retrieve.</param>
    /// <returns><c>true</c> if the button is down.</returns>
    public bool GetButtonState(MouseButton mouseButton)
        => mouseButton switch
        {
            MouseButton.LeftButton => this.isLeftButtonDown,
            MouseButton.RightButton => this.isRightButtonDown,
            MouseButton.MiddleButton => this.isMiddleButtonDown,
            _ => false,
        };

    /// <summary>
    /// Returns a value indicating whether or not any of the mouse buttons are in the down position.
    /// </summary>
    /// <returns>True if any buttons are in the down position.</returns>
    public bool AnyButtonsDown() => IsButtonDown(MouseButton.LeftButton) ||
                                    IsButtonDown(MouseButton.MiddleButton) ||
                                    IsButtonDown(MouseButton.RightButton);

    /// <summary>
    /// Gets the position value of the mouse scroll wheel.
    /// </summary>
    /// <returns>The value of the scroll wheel.</returns>
    public int GetScrollWheelValue() => this.scrollWheelValue;

    /// <summary>
    /// Gets the direction that the mouse wheel has been turned.
    /// </summary>
    /// <returns>The scroll direction of the mouse wheel.</returns>
    public MouseScrollDirection GetScrollDirection() => this.scrollDirection;
}
