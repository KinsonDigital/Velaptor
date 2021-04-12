// <copyright file="GameWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// The internal OpenGL window that extends the <see cref="GameWindow"/>
    /// for the purpose of getting access to the window's pointer.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class GameWindowFacade : IGameWindowFacade
    {
        private readonly object objectLock = new object();
        private GameWindow? gameWindow;
        private bool isDisposed;

        /// <inheritdoc/>
        public event Action? Load
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Load += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Load -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action? Unload
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Unload += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Unload -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<FrameEventArgs>? UpdateFrame
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.UpdateFrame += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.UpdateFrame -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<FrameEventArgs>? RenderFrame
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.RenderFrame += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.RenderFrame -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<ResizeEventArgs>? Resize
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Resize += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Resize -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<KeyboardKeyEventArgs>? KeyDown
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.KeyDown += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.KeyDown -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<KeyboardKeyEventArgs>? KeyUp
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.KeyUp += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.KeyUp -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<MouseButtonEventArgs>? MouseDown
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseDown += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseDown -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<MouseButtonEventArgs>? MouseUp
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseUp += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseUp -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action<MouseMoveEventArgs>? MouseMove
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseMove += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.MouseMove -= value;
                }
            }
        }

        /// <inheritdoc/>
        public event Action? Closed
        {
            add
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Closed += value;
                }
            }
            remove
            {
                lock (this.objectLock)
                {
                    if (this.gameWindow is null)
                    {
                        throw new InvalidOperationException("The game window must not be null.");
                    }

                    this.gameWindow.Closed -= value;
                }
            }
        }

        /// <inheritdoc/>
        public unsafe Vector2i Size
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.Size;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.Size = value;
            }
        }

        /// <inheritdoc/>
        public unsafe Vector2i Location
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.Location;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.Location = value;
            }
        }

        /// <inheritdoc/>
        public double UpdateFrequency
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.UpdateFrequency;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.UpdateFrequency = value;
            }
        }

        /// <inheritdoc/>
        public unsafe bool CursorVisible
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.CursorVisible;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.CursorVisible = value;
            }
        }

        /// <inheritdoc/>
        public unsafe WindowState WindowState
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.WindowState;
            }

            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.WindowState = value;
            }
        }

        /// <inheritdoc/>
        public unsafe WindowBorder WindowBorder
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.WindowBorder;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.WindowBorder = value;
            }
        }

        /// <inheritdoc/>
        public unsafe string Title
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.Title;
            }
            set
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                this.gameWindow.Title = value;
            }
        }

        /// <inheritdoc/>
        public unsafe Window* WindowPtr
        {
            get
            {
                if (this.gameWindow is null)
                {
                    throw new InvalidOperationException("The game window must not be null.");
                }

                return this.gameWindow.WindowPtr;
            }
        }

        /// <inheritdoc/>
        public void Init(int width, int height)
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(width, height),
                StartVisible = false,
            };

            this.gameWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);
        }

        /// <inheritdoc/>
        public void Run()
        {
            if (this.gameWindow is null)
            {
                throw new InvalidOperationException("The game window must not be null.");
            }

            this.gameWindow.Run();
        }

        /// <inheritdoc/>
        public void SwapBuffers()
        {
            if (this.gameWindow is null)
            {
                throw new InvalidOperationException("The game window must not be null.");
            }

            this.gameWindow.SwapBuffers();
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (this.gameWindow is null)
            {
                throw new InvalidOperationException("The game window must not be null.");
            }

            this.gameWindow.Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (!(this.gameWindow is null))
                    {
                        this.gameWindow.Dispose();
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
