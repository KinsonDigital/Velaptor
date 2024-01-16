// <copyright file="ISlider.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// A control that allows the user to select a value within a range between a minimum and maximum value.
/// </summary>
public interface ISlider : IControl
{
    /// <summary>
    /// Invoked when the value has changed.
    /// </summary>
    event EventHandler<float>? ValueChanged;

    /// <summary>
    /// Gets or sets the text of the slider.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Gets or sets the value of the slider.
    /// </summary>
    float Value { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the slider.
    /// </summary>
    float Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the slider.
    /// </summary>
    float Max { get; set; }
}
