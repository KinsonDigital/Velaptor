// <copyright file="WindowFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor;
using Velaptor.Batching;
using Velaptor.UI;

/// <summary>
/// Used for the purpose of testing the abstract <see cref="Window"/> class.
/// </summary>
public class WindowFake : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFake"/> class.
    /// </summary>
    /// <param name="window">Mocked window.</param>
    /// <param name="batcher">Mocked batcher.</param>
    /// <remarks>This is used to help test the abstract <see cref="Window"/> class.</remarks>
    internal WindowFake(IWindow window, IBatcher batcher)
        : base(window, batcher)
    {
    }

// CS0114 - A declaration in a class conflicts with a declaration in a base class such that the base class member will be hidden.
#pragma warning disable CS0114
    public void OnDraw(FrameTime frameTime) => base.OnDraw(frameTime);

    public void OnUnload() => base.OnUnload();
#pragma warning restore CS0114
}
