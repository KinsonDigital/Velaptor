// <copyright file="MouseState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using Velaptor.Input.Exceptions;

    /// <summary>
    /// Represents the state of the mouse.
    /// </summary>
    public struct MouseState : IEquatable<MouseState>
    {
        private Vector2 position;
        private int scrollWheelValue;
        private bool isLeftButtonDown;
        private bool isRightButtonDown;
        private bool isMiddleButtonDown;

        /// <summary>
        /// Returns a value indicating if both operands are equal.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><see langword="true"/> if the operands are equal.</returns>
        public static bool operator ==(MouseState left, MouseState right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating if both operands are not equal.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><see langword="true"/> if the operands are not equal.</returns>
        public static bool operator !=(MouseState left, MouseState right) => !left.Equals(right);

        /// <summary>
        /// Gets or sets the position of the mouse.
        /// </summary>
        /// <returns>The position relative to the top left corner of the window.</returns>
        public Vector2 GetPosition() => this.position;

        /// <summary>
        /// Gets or sets the X position of the mouse.
        /// </summary>
        /// <returns>The X position relative to the top left corner of the window.</returns>
        public int GetX() => (int)this.position.X;

        /// <summary>
        /// Gets or sets the Y position of the mouse.
        /// </summary>
        /// <returns>The Y position relative to the top left corner of the window.</returns>
        public int GetY() => (int)this.position.Y;

        /// <summary>
        /// Gets or sets a value indicating whether the left mouse button is in the down position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is down.</returns>
        public bool IsLeftButtonDown() => this.isLeftButtonDown;

        /// <summary>
        /// Returns a value indicating whether the left mouse button is in the up position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is up.</returns>
        public bool IsLeftButtonUp() => !IsLeftButtonDown();

        /// <summary>
        /// Gets or sets a value indicating whether the middle mouse button is in the down position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is down.</returns>
        public bool IsMiddleButtonDown() => this.isMiddleButtonDown;

        /// <summary>
        /// Returns a value indicating whether the middle mouse button is in the up position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is up.</returns>
        public bool IsMiddleButtonUp() => !IsMiddleButtonDown();

        /// <summary>
        /// Gets or sets a value indicating whether the right mouse button is in the down position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is down.</returns>
        public bool IsRightButtonDown() => this.isRightButtonDown;

        /// <summary>
        /// Returns a value indicating whether the right mouse button is in the up position.
        /// </summary>
        /// <returns><see langword="true"/> if the button is up.</returns>
        public bool IsRightButtonUp() => !IsRightButtonDown();

        /// <summary>
        /// Returns the state for the given <paramref name="mouseButton"/>.
        /// </summary>
        /// <param name="mouseButton">The button state to retrieve.</param>
        /// <returns><see langword="true"/> if the button is down.</returns>
        public bool GetButtonState(MouseButton mouseButton)
            => mouseButton switch
            {
                MouseButton.LeftButton => this.isLeftButtonDown,
                MouseButton.RightButton => this.isRightButtonDown,
                MouseButton.MiddleButton => this.isMiddleButtonDown,
                _ => false,
            };

        /// <summary>
        /// Gets or sets the position value of the mouse scroll wheel.
        /// </summary>
        /// <returns>The value of the scroll wheel.</returns>
        public int GetScrollWheelValue() => this.scrollWheelValue;

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
        [ExcludeFromCodeCoverage]
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
        public void SetPosition(int x, int y) => this.position = new Vector2(x, y);

        /// <summary>
        /// Sets the value of the scroll wheel.
        /// </summary>
        /// <param name="value">The value to set the scroll to.</param>
        public void SetScrollWheelValue(int value) => this.scrollWheelValue = value;

        /// <summary>
        /// Sets the given <paramref name="mouseButton"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="mouseButton">The button to set.</param>
        /// <param name="state">The state to set the button to.</param>
        public void SetButtonState(MouseButton mouseButton, bool state)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftButton:
                    this.isLeftButtonDown = state;
                    break;
                case MouseButton.RightButton:
                    this.isRightButtonDown = state;
                    break;
                case MouseButton.MiddleButton:
                    this.isMiddleButtonDown = state;
                    break;
                default:
                    throw new InvalidInputException("Invalid Mouse Input");
            }
        }
    }
}
