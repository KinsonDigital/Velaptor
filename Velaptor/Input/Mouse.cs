// <copyright file="Mouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using Carbonate;
using Guards;
using Reactables.ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// Gets or sets the state of the mouse.
/// </summary>
internal sealed class Mouse : IAppInput<MouseState>
{
    private readonly IDisposable unsubscriber;
    private (MouseButton button, bool isDown) leftMouseButton = (MouseButton.LeftButton, false);
    private (MouseButton button, bool isDown) middleMouseButton = (MouseButton.MiddleButton, false);
    private (MouseButton button, bool isDown) rightMouseButton = (MouseButton.RightButton, false);
    private int xPos;
    private int yPos;
    private int scrollWheelValue;
    private MouseScrollDirection mouseScrollDirection = MouseScrollDirection.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mouse"/> class.
    /// </summary>
    /// <param name="reactable">Used to get push notifications about the position of the mouse.</param>
    public Mouse(IReactable reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.unsubscriber = reactable.Subscribe(new Reactor(
            NotificationIds.MouseId,
            onNext: msg =>
            {
                var data = msg.GetData<MouseStateData>();

                if (data is null)
                {
                    throw new PushNotificationException($"{nameof(Mouse)}.Constructor()", NotificationIds.MouseId);
                }

                this.xPos = data.X;
                this.yPos = data.Y;
                this.mouseScrollDirection = data.ScrollDirection;
                this.scrollWheelValue = data.ScrollWheelValue;

                switch (data.Button)
                {
                    case MouseButton.LeftButton:
                        this.leftMouseButton.isDown = data.ButtonIsDown;
                        break;
                    case MouseButton.MiddleButton:
                        this.middleMouseButton.isDown = data.ButtonIsDown;
                        break;
                    case MouseButton.RightButton:
                        this.rightMouseButton.isDown = data.ButtonIsDown;
                        break;
                    default:
                        throw new EnumOutOfRangeException($"The enum '{nameof(MouseButton)}' is out of range.");
                }
            }, () => this.unsubscriber?.Dispose()));
    }

    /// <summary>
    /// Gets the current state of the mouse.
    /// </summary>
    /// <returns>The state of the mouse.</returns>
    public MouseState GetState()
    {
        var result = default(MouseState);
        result.SetPosition(this.xPos, this.yPos);
        result.SetScrollWheelValue(this.scrollWheelValue);
        result.SetScrollWheelDirection(this.mouseScrollDirection);

        // Set all of the states for the buttons
        result.SetButtonState(MouseButton.LeftButton, this.leftMouseButton.isDown);
        result.SetButtonState(MouseButton.MiddleButton, this.middleMouseButton.isDown);
        result.SetButtonState(MouseButton.RightButton, this.rightMouseButton.isDown);

        return result;
    }
}
