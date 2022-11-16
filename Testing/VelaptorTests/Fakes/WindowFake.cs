// <copyright file="WindowFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.UI;

namespace VelaptorTests.Fakes;

/// <summary>
/// Used for the purpose of testing the abstract <see cref="Window"/> class.
/// </summary>
public class WindowFake : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFake"/> class.
    /// </summary>
    /// <param name="window">Window implementation.</param>
    /// <remarks>This is used to help test the abstract <see cref="Window"/> class.</remarks>
    internal WindowFake(IWindow window)
        : base(window)
    {
    }
}
