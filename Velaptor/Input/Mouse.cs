// <copyright file="Mouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Reactables.Core;

namespace Velaptor.Input;

/// <summary>
/// Gets or sets the state of the mouse.
/// </summary>
internal sealed class Mouse : IAppInput<MouseState>
{
    private readonly IDisposable mousePosUnsubscriber;
    private readonly IDisposable mouseButtonUnsubscriber;
    private readonly IDisposable mouseWheelUnsubscriber;
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
    /// <param name="mousePositionReactable">Used to get push notifications about the position of the mouse.</param>
    /// <param name="mouseButtonReactable">Used to get push notifications about the state of the mouse buttons.</param>
    /// <param name="mouseWheelReactable">Used to get push notifications about the state of the mouse wheel.</param>
    public Mouse(
        IReactable<(int x, int y)> mousePositionReactable,
        IReactable<(MouseButton button, bool isDown)> mouseButtonReactable,
        IReactable<(MouseScrollDirection wheelDirection, int mouseWheelValue)> mouseWheelReactable)
    {
        this.mousePosUnsubscriber = mousePositionReactable.Subscribe(new Reactor<(int x, int y)>(
            onNext: position =>
            {
                this.xPos = position.x;
                this.yPos = position.y;
            }, () =>
            {
                this.mousePosUnsubscriber?.Dispose();
            }));

        this.mouseButtonUnsubscriber = mouseButtonReactable.Subscribe(new Reactor<(MouseButton button, bool isDown)>(
            onNext: buttonState =>
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (buttonState.button)
                {
                    case MouseButton.LeftButton:
                        this.leftMouseButton.isDown = buttonState.isDown;
                        break;
                    case MouseButton.MiddleButton:
                        this.middleMouseButton.isDown = buttonState.isDown;
                        break;
                    case MouseButton.RightButton:
                        this.rightMouseButton.isDown = buttonState.isDown;
                        break;
                }
            }, () =>
            {
                this.mouseButtonUnsubscriber?.Dispose();
            }));

        this.mouseWheelUnsubscriber = mouseWheelReactable.Subscribe(new Reactor<(MouseScrollDirection wheelDirection, int mouseWheelValue)>(
            onNext: wheelState =>
            {
                this.scrollWheelValue = wheelState.mouseWheelValue;
                this.mouseScrollDirection = wheelState.wheelDirection;
            }, () =>
            {
                this.mouseWheelUnsubscriber?.Dispose();
            }));
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
