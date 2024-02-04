// <copyright file="IRadioButton.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// A control to represent a selection of an option from a list of options.
/// </summary>
public interface IRadioButton : IControl
{
    /// <summary>
    /// Invoked when the radio button is selected.
    /// </summary>
    event EventHandler<EventArgs>? Selected;

    /// <summary>
    /// Gets or sets the text of the radio button.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the radio button is selected.
    /// </summary>
    public bool IsSelected { get; set; }
}
