// <copyright file="WindowSizeEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // TODO: Try and make this a stuct and see if it works as an EventArgs type
    // This means you cannot inherit from EventArgs
    internal class WindowSizeEventArgs
    {
        public WindowSizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }
    }
}
