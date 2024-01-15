// <copyright file="INextPrevious.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.UI;

using System;

/// <summary>
/// A next and previous control.
/// </summary>
public interface INextPrevious : IControl
{
    /// <summary>
    /// Invoked when the next button is clicked
    /// </summary>
    event EventHandler? Next;

    /// <summary>
    /// Invoked when the previous button is clicked.
    /// </summary>
    event EventHandler? Previous;
}
