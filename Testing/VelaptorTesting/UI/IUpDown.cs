// <copyright file="IUpDown.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// A control to increase or decrease a value.
/// </summary>
public interface IUpDown : IControl
{
    event EventHandler<float>? ValueChanged;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    float Value { get; set; }

    /// <summary>
    /// Gets or sets the amount to increment the value.
    /// </summary>
    float Increment { get; set; }

    /// <summary>
    /// Gets or sets the amount to decrement the value.
    /// </summary>
    float Decrement { get; set; }

    /// <summary>
    /// Gets or sets the minimum that the <see cref="Value"/> can be.
    /// </summary>
    float Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum that the <see cref="Value"/> can be.
    /// </summary>
    float Max { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    string Text { get; set; }
}
