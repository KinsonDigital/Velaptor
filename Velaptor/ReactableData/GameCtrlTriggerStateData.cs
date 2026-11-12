// <copyright file="GameCtrlTriggerStateData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data about the state of a game controller trigger.
/// </summary>
internal readonly record struct GameCtrlTriggerStateData
{
    /// <summary>
    /// Gets the position of a game controller trigger.
    /// </summary>
    /// <remarks>
    /// The value is represented as a percentage from <c>0%</c> to <c>100%</c>.
    /// </remarks>
    public float Position { get; init; }

    /// <summary>
    /// Gets a value indicating whether if the trigger is the left trigger.
    /// </summary>
    public bool IsLeftTrigger { get; init; }

    /// <summary>
    /// Gets a value indicating whether if the trigger is the right trigger.
    /// </summary>
    public bool IsRightTrigger { get; init; }
}
