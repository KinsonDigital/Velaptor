using Raptor;
using Raptor.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaptorTests.Fakes
{
    /// <summary>
    /// Used for the purpose of testing the abstract <see cref="Window"/> class.
    /// </summary>
    public class WindowFake : Window
    {
        public WindowFake(IWindow window, IContentLoader contentLoader)
            : base(window, contentLoader) { }

        public WindowFake(IWindow window)
            : base(window) { }
    }
}
