// <copyright file="MouseInputState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Represents the state of the mouse.
    /// </summary>
    public struct MouseInputState : IEquatable<MouseInputState>
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the down state of the left mouse button.
        /// </summary>
        public bool LeftButtonDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the down state of the middle mouse button.
        /// </summary>
        public bool MiddleButtonDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets the down state of the right mouse button.
        /// </summary>
        public bool RightButtonDown { get; set; }

        /// <summary>
        /// Gets or sets the X position of the mouse.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the mouse.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the position of the mouse.
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the position value of the mouse scroll wheel.
        /// </summary>
        public int ScrollWheelValue { get; set; }

        public static bool operator ==(MouseInputState left, MouseInputState right) => left.Equals(right);

        public static bool operator !=(MouseInputState left, MouseInputState right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is MouseInputState state))
                return false;

            return Equals(state);
        }

        /// <inheritdoc/>
        public bool Equals(MouseInputState other) => X == other.X && Y == other.Y &&
                LeftButtonDown == other.LeftButtonDown &&
                MiddleButtonDown == other.MiddleButtonDown &&
                RightButtonDown == other.RightButtonDown &&
                ScrollWheelValue == other.ScrollWheelValue &&
                Position == other.Position;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y, ScrollWheelValue, LeftButtonDown, MiddleButtonDown, RightButtonDown);
    }
}
