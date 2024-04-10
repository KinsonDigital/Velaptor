// <copyright file="IControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Drawing;

/// <summary>
/// Represents a UI control.
/// </summary>
public interface IControl : IDisposable
{
    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the window that owns the control.
    /// </summary>
    Guid WindowOwnerId { get; set; }

    /// <summary>
    /// Gets or sets the position of the control.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets the width of the control.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the control.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is visible.
    /// </summary>
    bool Visible { get; set; }
}
