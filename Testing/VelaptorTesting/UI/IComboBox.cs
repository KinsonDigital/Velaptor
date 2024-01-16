// <copyright file="IComboBox.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a combo box control.
/// </summary>
public interface IComboBox : IControl
{
    /// <summary>
    /// Invokes when the selected item index has changed.
    /// </summary>
    event EventHandler<int>? SelectedItemIndexChanged;

    /// <summary>
    /// Gets or sets the label of the combo box.
    /// </summary>
    string Label { get; set; }

    /// <summary>
    /// Gets or sets the width of the combo box.
    /// </summary>
    new int Width { get; set; }

    /// <summary>
    /// Gets or sets the list of items to display in the combo box.
    /// </summary>
    List<string> Items { get; set; }

    /// <summary>
    /// Gets or sets the index of the selected item.
    /// </summary>
    int SelectedItemIndex { get; set; }
}
