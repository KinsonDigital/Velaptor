// <copyright file="MouseState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Velaptor.Exceptions;

/// <summary>
/// Represents the state of the mouse.
/// </summary>
public struct MouseState : IEquatable<MouseState>
{
    private Point position;
    private int scrollWheelValue;
    private bool isLeftButtonDown;
    private bool isRightButtonDown;
    private bool isMiddleButtonDown;
    private MouseScrollDirection scrollDirection;

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left side of the operator.</param>
    /// <param name="right">The right side of the operator.</param>
    /// <returns><c>true</c> if the operands are equal.</returns>
    public static bool operator ==(MouseState left, MouseState right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left side of the operator.</param>
    /// <param name="right">The right side of the operator.</param>
    /// <returns><c>true</c> if the operands are not equal.</returns>
    public static bool operator !=(MouseState left, MouseState right) => !left.Equals(right);

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
    public bool IsButtonDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.LeftButton:
                return this.isLeftButtonDown;
            case MouseButton.MiddleButton:
                return this.isMiddleButtonDown;
            case MouseButton.RightButton:
                return this.isRightButtonDown;
            default:
                var enumTypeStr = nameof(Velaptor);
                enumTypeStr += $".{nameof(Input)}";
                enumTypeStr += $".{nameof(MouseButton)}";

                var exceptionMsg = $"The enum '{enumTypeStr}' is invalid because it is out of range.";
                throw new EnumOutOfRangeException(exceptionMsg);
        }
    }

    /// <summary>
    /// Returns a value indicating whether or not the given mouse <paramref name="button"/>
    /// is in the up position.
    /// </summary>
    /// <param name="button">The mouse button to check.</param>
    /// <returns>True if the mouse button is in the up position.</returns>
    public bool IsButtonUp(MouseButton button) => !IsButtonDown(button);

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

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not MouseState state)
        {
            return false;
        }

        return Equals(state);
    }

    /// <inheritdoc/>
    public bool Equals(MouseState other)
        => this.scrollWheelValue == other.scrollWheelValue &&
           this.position.X == other.position.X &&
           this.position.Y == other.position.Y &&
           this.isLeftButtonDown == other.isLeftButtonDown &&
           this.isMiddleButtonDown == other.isMiddleButtonDown &&
           this.isRightButtonDown == other.isRightButtonDown;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode()
        => HashCode.Combine(
            this.scrollWheelValue,
            this.position.X,
            this.position.Y,
            this.isLeftButtonDown,
            this.isMiddleButtonDown,
            this.isRightButtonDown);

    /// <summary>
    /// Sets the position of the mouse using the given <paramref name="x"/> and <paramref name="y"/> values.
    /// </summary>
    /// <param name="x">The X position of the mouse.</param>
    /// <param name="y">The Y position of the mouse.</param>
    internal void SetPosition(int x, int y) => this.position = new Point(x, y);

    /// <summary>
    /// Sets the value of the scroll wheel.
    /// </summary>
    /// <param name="value">The value to set the scroll to.</param>
    internal void SetScrollWheelValue(int value) => this.scrollWheelValue = value;

    /// <summary>
    /// Sets the scroll direction of the mouse wheel.
    /// </summary>
    /// <param name="direction">The scroll direction.</param>
    internal void SetScrollWheelDirection(MouseScrollDirection direction) => this.scrollDirection = direction;

    /// <summary>
    /// Sets the given <paramref name="mouseButton"/> to the given <paramref name="state"/>.
    /// </summary>
    /// <param name="mouseButton">The button to set.</param>
    /// <param name="state">Sets the state of the <paramref name="mouseButton"/>.</param>
    internal void SetButtonState(MouseButton mouseButton, bool state)
    {
        // ReSharper disable ConvertIfStatementToSwitchStatement
        if (mouseButton == MouseButton.LeftButton)
        {
            this.isLeftButtonDown = state;
        }
        else if (mouseButton == MouseButton.RightButton)
        {
            this.isRightButtonDown = state;
        }
        else if (mouseButton == MouseButton.MiddleButton)
        {
            this.isMiddleButtonDown = state;
        }

        // ReSharper restore ConvertIfStatementToSwitchStatement
    }
}
