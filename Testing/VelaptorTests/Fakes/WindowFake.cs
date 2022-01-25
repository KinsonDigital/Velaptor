// <copyright file="WindowFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using System;
    using Velaptor.UI;
    using VelObservable = Velaptor.Observables.Core.IObservable<bool>;

    /// <summary>
    /// Used for the purpose of testing the abstract <see cref="Window"/> class.
    /// </summary>
    public class WindowFake : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowFake"/> class.
        /// </summary>
        /// <param name="window">Window implementation.</param>
        /// <param name="shutDownObservable">Mocked <see cref="IObservable{T}"/> for application shutdown.</param>
        /// <remarks>This is used to help test the abstract <see cref="Window"/> class.</remarks>
        public WindowFake(IWindow window, VelObservable shutDownObservable)
            : base(window, shutDownObservable)
        {
        }
    }
}
