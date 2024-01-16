// <copyright file="ILabel.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

/// <summary>
/// Displays text on the screen.
/// </summary>
public interface ILabel : IControl
{
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    string Text { get; set; }
}
