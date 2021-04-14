// <copyright file="WindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Input;
    using Raptor.NativeInterop;
    using Raptor.OpenGL;
    using Raptor.Services;
    using Raptor.UI;

    /// <summary>
    /// Creates an instance of a raptor window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WindowFactory
    {
        /// <summary>
        /// Creates a single instance of a raptor window implementation.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>A raptor framework window implementation.</returns>
        public static IWindow CreateWindow(int width, int height)
            => new GLWindow(
                width,
                height,
                IoC.Container.GetInstance<IGLInvoker>(),
                IoC.Container.GetInstance<ISystemMonitorService>(),
                IoC.Container.GetInstance<IGameWindowFacade>(),
                IoC.Container.GetInstance<IPlatform>(),
                IoC.Container.GetInstance<ITaskService>(),
                IoC.Container.GetInstance<IKeyboardInput<KeyCode, KeyboardState>>(),
                IoC.Container.GetInstance<IMouseInput<MouseButton, MouseState>>(),
                ContentLoaderFactory.CreateContentLoader());
    }
}
