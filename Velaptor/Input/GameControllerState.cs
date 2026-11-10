// <copyright file="GameControllerState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1642 // This warning shows it should be described as a class but it is a struct
namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using ReactableData;

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
    internal GameCtrlThumbstickStateData LeftGameCtrlThumbstick = default;

    /// <summary>
    /// Gets the state of the right thumbstick.
    /// </summary>
    internal GameCtrlThumbstickStateData RightGameCtrlThumbstick = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameControllerState"/> struct.
    /// </summary>
    public GameControllerState()
    {
        InitButtonStates();
    }

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
    /// <exception cref="ArgumentOutOfRangeException">TODO: add docs</exception>
    public float GetThumbstickDirection(Thumbsticks thumbstick)
    {
        const float deadzone = 0.01f;
        switch (thumbstick)
        {
            case Thumbsticks.Left:
                var angleDegrees = (this.LeftGameCtrlThumbstick.Direction + (float)Math.PI) * 180f / (float)Math.PI;

                if (Math.Abs(this.LeftGameCtrlThumbstick.X) < deadzone && Math.Abs(this.LeftGameCtrlThumbstick.Y) < deadzone)
                {
                    return -1f;
                }

                angleDegrees = (angleDegrees - 90) % 360;

                return angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;
            case Thumbsticks.Right:
                return this.RightGameCtrlThumbstick.Direction.ToDegrees();
            default:
                // TODO: Ensure that this is the way to throw an exception if an enum is not correct
                // Other instances exist in the code base. Use them as ref.
                throw new ArgumentOutOfRangeException(nameof(thumbstick), thumbstick, null);
        }
    }

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
