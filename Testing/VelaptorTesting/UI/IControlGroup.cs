// <copyright file="IControlGroup.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;
using System.Drawing;

/// <summary>
/// A container to hold a group of controls.
/// </summary>
public interface IControlGroup : IDisposable
{
    /// <summary>
    /// Invoked when the control group is initialized.
    /// </summary>
    event EventHandler Initialized;

    /// <summary>
    /// Gets or sets the title of the control group.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets the unique identifier of the control group.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets or sets the position of the control group.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets or sets the width of the control group.
    /// </summary>
    int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the control group.
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// Gets the half width of the control group.
    /// </summary>
    int HalfWidth { get; }

    /// <summary>
    /// Gets the half height of the control group.
    /// </summary>
    int HalfHeight { get; }

    /// <summary>
    /// Gets the position of the left side of the control group.
    /// </summary>
    int Left { get; }

    /// <summary>
    /// Gets the position of the top of the control group.
    /// </summary>
    int Top { get; }

    /// <summary>
    /// Gets the position of the right side of the control group.
    /// </summary>
    int Right { get; }

    /// <summary>
    /// Gets the position of the bottom of the control group.
    /// </summary>
    int Bottom { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the title bar is visible.
    /// </summary>
    bool TitleBarVisible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the control group should
    /// automatically fit to the size of its content.
    /// </summary>
    bool AutoSizeToFitContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the control group is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Adds the given <paramref name="control"/> to the control group.
    /// </summary>
    /// <param name="control">The control to add.</param>
    void Add(IControl control);

    /// <summary>
    /// Gets a control of type <typeparamref name="T"/> that has the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the control.</param>
    /// <typeparam name="T">The type of control to return.</typeparam>
    /// <returns>The control.</returns>
    /// <remarks>Returns null if the control is not found.</remarks>
    T? GetControl<T>(string name)
        where T : IControl;

    /// <summary>
    /// Renders the group of controls including the group container.
    /// </summary>
    void Render();
}
