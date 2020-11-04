// <copyright file="WindowFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Fakes
{
    using Raptor;
    using Raptor.Content;

    /// <summary>
    /// Used for the purpose of testing the abstract <see cref="Window"/> class.
    /// </summary>
    public class WindowFake : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowFake"/> class.
        /// </summary>
        /// <param name="window">Window implementation.</param>
        /// <param name="contentLoader">Content loader implementation.</param>
        /// <remarks>This is used to help test the abstract <see cref="Window"/> class.</remarks>
        public WindowFake(IWindow window)
            : base(window)
        {
        }
    }
}
