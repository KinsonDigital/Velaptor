// <copyright file="WindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.GLFW;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using Velaptor.UI;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an instance of a Velaptor window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WindowFactory
    {
        /// <summary>
        /// Creates an instance of a Velaptor window implementation.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>A Velaptor framework window implementation.</returns>
        public static IWindow CreateWindow(uint width, uint height)
            => new GLWindow(
                width,
                height,
                IoC.Container.GetInstance<IGLInvoker>(),
                IoC.Container.GetInstance<IGLFWInvoker>(),
                IoC.Container.GetInstance<ISystemMonitorService>(),
                IoC.Container.GetInstance<IGameWindowFacade>(),
                IoC.Container.GetInstance<IPlatform>(),
                IoC.Container.GetInstance<ITaskService>(),
                ContentLoaderFactory.CreateContentLoader(),
                IoC.Container.GetInstance<IReactor<GLInitData>>(),
                IoC.Container.GetInstance<IReactor<ShutDownData>>());
    }
}
