// <copyright file="GameControllerState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1642 // This warning shows it should be described as a class but it is a struct
namespace Velaptor.Input;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents the state of a game controller device.
/// </summary>
public record struct GameControllerState
{
    private static readonly int Capacity = Enum.GetNames<ControllerButton>().Length;

    /// <summary>
    /// Gets the state of the buttons.
    /// </summary>
    internal Dictionary<ControllerButton, bool> ButtonStates { get; private set; } = new (Capacity);

    /// <summary>
    /// Gets the state of the left thumbstick.
    /// </summary>
    internal GameCtrlThumbstickState LeftGameCtrlThumbstick = default;

    /// <summary>
    /// Gets the state of the right thumbstick.
    /// </summary>
    internal GameCtrlThumbstickState RightGameCtrlThumbstick = default;

    /// <summary>
    /// Gets the state of the left trigger.
    /// </summary>
    internal GameCtrlTriggerState LeftGameCtrlTrigger = default;

    /// <summary>
    /// Gets the state of the right trigger.
    /// </summary>
    internal GameCtrlTriggerState RightGameCtrlTrigger = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerState"/> struct.
    /// </summary>
    public GameControllerState()
    {
        InitButtonStates();
    }

    /// <summary>
    /// Returns a list of all the buttons that are currently in the down position.
    /// </summary>
    /// <returns>The list of buttons.</returns>
    public Span<ControllerButton> GetDownButtons()
    {
        InitButtonStates();

        var results = new List<ControllerButton>();
        foreach ((ControllerButton key, var state) in ButtonStates)
        {
            if (state)
            {
                results.Add(key);
            }
        }

        return results.ToArray().AsSpan();
    }

    /// <summary>
    /// Returns a value indicating if the given <paramref name="button"/> is in the down position.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>The down state of the button.</returns>
    public bool IsButtonDown(ControllerButton button)
    {
        InitButtonStates();

        return ButtonStates.TryGetValue(button, out var value) && value;
    }

    /// <summary>
    /// Gets the direction/angle of the given <paramref name="thumbstick"/> in degrees.
    /// </summary>
    /// <param name="thumbstick">The thumbstick to get the angle from.</param>
    /// <returns>The angle in degrees.</returns>
    /// <remarks>
    ///     <para>
    ///         The angle increases in the clockwise direction as follows:
    ///         <list type="bullet">
    ///             <item>A value of <c>0/360</c> represents the stick being pushed up.</item>
    ///             <item>A value of <c>90</c> represents the stick being pushed to the right.</item>
    ///             <item>A value of <c>180</c> represents the stick being pushed down.</item>
    ///             <item>A value of <c>270</c> represents the stick being pushed to the left.</item>
    ///         </list>
    ///     </para>
    /// <para>
    ///     A value of <c>-1</c> represents that the stick at its resting state in the center.
    /// </para>
    /// </remarks>
    public float GetThumbstickDirection(Thumbsticks thumbstick)
    {
        const float deadZone = 0.01f;

        var thumbDirection = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.Direction
            : this.RightGameCtrlThumbstick.Direction;

        var thumbX = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.X
            : this.RightGameCtrlThumbstick.X;

        var thumbY = thumbstick == Thumbsticks.Left
            ? this.LeftGameCtrlThumbstick.Y
            : this.RightGameCtrlThumbstick.Y;

        var angleDegrees = (thumbDirection + (float)Math.PI) * 180f / (float)Math.PI;

        if (Math.Abs(thumbX) < deadZone && Math.Abs(thumbY) < deadZone)
        {
            return -1f;
        }

        angleDegrees = (angleDegrees - 90) % 360;

        return angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;
    }

    /// <summary>
    /// Gets the position of the <paramref name="trigger"/> as a value between <c>0%</c> and <c>100%</c>.
    /// </summary>
    /// <param name="trigger">The <paramref name="trigger"/> to check.</param>
    /// <returns>The position of the <paramref name="trigger"/>.</returns>
    public float GetTriggerPosition(Triggers trigger)
    {
        var triggerPos = trigger == Triggers.Left
            ? this.LeftGameCtrlTrigger.Position
            : this.RightGameCtrlTrigger.Position;

        return triggerPos.MapValue(-1f, 1f, 0f, 100f);
    }

    /// <summary>
    /// Gets the X position of the <paramref name="thumbstick"/> as a value between <c>-1</c> and <c>1</c>.
    /// </summary>
    /// <param name="thumbstick">The <paramref name="thumbstick"/> to check.</param>
    /// <remarks>
    ///     The value <c>-1</c> represents the stick pushed to the left of center. <c>1</c> represents
    ///     the stick pushed to the right of center. <c>0</c> represents the stick in the center and in the center.
    /// </remarks>
    /// <returns>The position of the <paramref name="thumbstick"/>.</returns>
    public float GetThumbstickX(Thumbsticks thumbstick)
    {
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                return this.LeftGameCtrlThumbstick.X;
            case Thumbsticks.Right:
            default:
                return this.RightGameCtrlThumbstick.X;
        }
    }

    /// <summary>
    /// Gets the Y position of the <paramref name="thumbstick"/> as a value between <c>-1</c> and <c>1</c>.
    /// </summary>
    /// <param name="thumbstick">The <paramref name="thumbstick"/> to check.</param>
    /// <remarks>
    ///     The value <c>-1</c> represents the stick pushed to the up from the center. <c>1</c> represents
    ///     the stick pushed to the down from the center. <c>0</c> represents the stick in the center and in the center.
    /// </remarks>
    /// <returns>The position of the <paramref name="thumbstick"/>.</returns>
    public float GetThumbstickY(Thumbsticks thumbstick)
    {
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                return this.LeftGameCtrlThumbstick.Y;
            case Thumbsticks.Right:
            default:
                return this.RightGameCtrlThumbstick.Y;
        }
    }

    /// <summary>
    /// Initializes the button states if they are null.
    /// </summary>
    private void InitButtonStates() => ButtonStates ??= new (Capacity);
}
