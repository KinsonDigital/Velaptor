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
        public WindowFake(IWindow window, IContentLoader contentLoader)
            : base(window, contentLoader)
        {
        }
    }
}
