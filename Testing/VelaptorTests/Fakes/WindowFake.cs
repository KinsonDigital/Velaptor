// <copyright file="WindowFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Velaptor.UI;

    /// <summary>
    /// Used for the purpose of testing the abstract <see cref="Window"/> class.
    /// </summary>
    public class WindowFake : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowFake"/> class.
        /// </summary>
        /// <param name="window">Window implementation.</param>
        /// <param name="shutDownReactor">Mocked <see cref="IReactor{T}"/> for application shutdown.</param>
        /// <remarks>This is used to help test the abstract <see cref="Window"/> class.</remarks>
        internal WindowFake(IWindow window, IReactor<ShutDownData> shutDownReactor)
            : base(window, shutDownReactor)
        {
        }
    }
}
