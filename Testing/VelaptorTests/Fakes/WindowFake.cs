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

#pragma warning disable CS0108, CS0114
    public void OnDraw(FrameTime frameTime) => base.OnDraw(frameTime);
#pragma warning restore CS0108, CS0114
}
