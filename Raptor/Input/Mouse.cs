// <copyright file="Mouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;

    /// <summary>
    /// Provides functionality for the mouse.
    /// </summary>
    public class Mouse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mouse"/> class.
        /// </summary>
        /// <param name="mouse">The mouse implementation.</param>
        public Mouse()
        {
        }

        /// <summary>
        /// Occurs when the left mouse button has been pushed to the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnLeftButtonDown;

        /// <summary>
        /// Occurs when the left mouse button has been released from the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnLeftButtonPressed;

        /// <summary>
        /// Occurs when the right mouse button has been pushed to the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnRightButtonDown;

        /// <summary>
        /// Occurs when the right mouse button has been released from the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnRightButtonPressed;

        /// <summary>
        /// Occurs when the middle mouse button has been pushed to the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnMiddleButtonDown;

        /// <summary>
        /// Occurs when the middle mouse button has been released from the down position.
        /// </summary>
        public event EventHandler<MouseEventArgs>? OnMiddleButtonPressed;

        /// <summary>
        /// Gets or sets the X position of the mouse in the game window.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the mouse in the game window.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Returns true if the given input is in the down position.
        /// </summary>
        /// <param name="input">The input button to check.</param>
        /// <returns>True if the mouse button is down.</returns>
        public bool IsButtonDown(InputButton input) => throw new NotImplementedException();

        /// <summary>
        /// Returns true if the given input is in the up position.
        /// </summary>
        /// <param name="input">The input button to check.</param>
        /// <returns>True if the mouse button is up.</returns>
        public bool IsButtonUp(InputButton input) => throw new NotImplementedException();

        /// <summary>
        /// Returns true if the given mouse input button has been pushed to the down position then released.
        /// </summary>
        /// <param name="input">The mouse input button to check.</param>
        /// <returns>True if button is down then up.</returns>
        public bool IsButtonPressed(InputButton input) => throw new NotImplementedException();

        /// <summary>
        /// Sets the position of the mouse.
        /// </summary>
        /// <param name="x">The horizontal X position to set the mouse to over the game window.</param>
        /// <param name="y">The vertical Y position to set the mouse to over the game window.</param>
        public void SetPosition(int x, int y) => throw new NotImplementedException();

        /// <summary>
        /// Sets the mouse to the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position to set the mouse to over the game window.</param>
        public void SetPosition(Vector2 position) => throw new NotImplementedException();

        /// <summary>
        /// Update the current state of the mouse.
        /// </summary>
        public void UpdateCurrentState()
        {
            // InternalMouse.UpdateCurrentState();

            ////If the left mouse button has been pressed down
            // if (InternalMouse.IsButtonDown(InputButton.LeftButton))
            // {
            //    //Invoke the OnLeftButtonDown event and send the current state of the mouse
            //    OnLeftButtonDown?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        LeftButtonDown = true,
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }

            ////If the left mouse button has been pressed
            // if (InternalMouse.IsButtonPressed(InputButton.LeftButton))
            // {
            //    OnLeftButtonPressed?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }

            ////If the right mouse button has been pressed down
            // if (InternalMouse.IsButtonDown(InputButton.RightButton))
            // {
            //    //Invoke the OnRightButtonDown event and send the current state of the mouse
            //    OnRightButtonDown?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        RightButtonDown = true,
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }

            ////If the right mouse button has been pressed
            // if (InternalMouse.IsButtonPressed(InputButton.RightButton))
            // {
            //    OnRightButtonPressed?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }

            ////If the middle mouse button has been pressed down
            // if (InternalMouse.IsButtonDown(InputButton.MiddleButton))
            // {
            //    //Invoke the OnMiddleButtonDown event and send the current state of the mouse
            //    OnMiddleButtonDown?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        MiddleButtonDown = true,
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }

            ////If the middle mouse button has been pressed
            // if (InternalMouse.IsButtonPressed(InputButton.MiddleButton))
            // {
            //    OnMiddleButtonPressed?.Invoke(this, new MouseEventArgs(new MouseInputState()
            //    {
            //        X = InternalMouse.X,
            //        Y = InternalMouse.Y
            //    }));
            // }
        }

        /// <summary>
        /// Update the previous state of the mouse.
        /// </summary>
        public void UpdatePreviousState()
        {
            // if (InternalMouse is null)
            //    return;

            // InternalMouse.UpdatePreviousState();
        }
    }
}
