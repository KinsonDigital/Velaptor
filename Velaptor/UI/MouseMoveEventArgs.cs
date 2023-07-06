// <copyright file="MouseMoveEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

/// <summary>
/// Holds information about the mouse position over a control.
/// </summary>
public readonly record struct MouseMoveEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MouseMoveEventArgs"/> struct.
    /// </summary>
    /// <param name="globalPos">The global position of the mouse.</param>
    /// <param name="localPos">The local position of the mouse.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "Reported incorrectly.  This is a struct, not a class")]
    public MouseMoveEventArgs(Point globalPos, Point localPos)
    {
        GlobalPos = globalPos;
        LocalPos = localPos;
    }

    /// <summary>
    /// Gets the position of the mouse relative to the top left corner of the window.
    /// </summary>
    public Point GlobalPos { get; }

    /// <summary>
    /// Gets the position of the mouse relative to the top left corner of the control.
    /// </summary>
    public Point LocalPos { get; }
}
