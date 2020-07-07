// <copyright file="InternalGLWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using OpenToolkit.Windowing.Desktop;
    using OpenToolkit.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// The internal OpenGL window that extends the <see cref="GameWindow"/>
    /// for the purpose of getting access to the window's pointer.
    /// </summary>
    internal class InternalGLWindow : GameWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalGLWindow"/> class.
        /// </summary>
        /// <param name="gameWindowSettings">The OpenToolkit.Windowing.Desktop.GameWindow related settings.</param>
        /// <param name="nativeWindowSettings">The OpenToolkit.Windowing.Desktop.NativeWindow related settings.</param>
        public InternalGLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        /// <summary>
        /// Gets the pointer to the window.
        /// </summary>
        public new unsafe Window* WindowPtr => base.WindowPtr;
    }
}
