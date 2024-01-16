// <copyright file="ICheckBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// A checkbox control.
/// </summary>
public interface ICheckBox : IControl
{
    /// <summary>
    /// Invoked when the state of the checkbox changes.
    /// </summary>
    event EventHandler<bool>? CheckedChanged;

    /// <summary>
    /// Gets or sets the label of the checkbox when it is in the checked state.
    /// </summary>
    string LabelWhenChecked { get; set; }

    /// <summary>
    /// Gets or sets the label of the checkbox when it is in the unchecked state.
    /// </summary>
    string LabelWhenUnchecked { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the checkbox is in the check state.
    /// </summary>
    bool IsChecked { get; }
}
