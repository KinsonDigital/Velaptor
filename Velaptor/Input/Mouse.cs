// <copyright file="Mouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using Carbonate.OneWay;
using Factories;
using Guards;
using ReactableData;
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
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public Mouse(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var reactable = reactableFactory.CreateMouseReactable();

        var mouseStateChangeName = this.GetExecutionMemberName(nameof(PushNotifications.MouseStateChangedId));
        this.unsubscriber = reactable.Subscribe(new ReceiveSubscription<MouseStateData>(
            id: PushNotifications.MouseStateChangedId,
            name: mouseStateChangeName,
            onReceive: data =>
            {
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
                        throw new EnumOutOfRangeException<MouseButton>($"The enum '{nameof(MouseButton)}' is out of range.");
                }
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
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
