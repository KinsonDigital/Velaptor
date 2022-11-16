// <copyright file="App.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Velaptor.Input;
using Velaptor.NativeInterop.GLFW;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Velaptor.Services;
using Velaptor.UI;

namespace Velaptor.Factories;

/// <summary>
/// Velaptor application specific functionality.
/// </summary>
[ExcludeFromCodeCoverage]
public static class App
{
    /// <summary>
    /// Creates an instance of a Velaptor window implementation.
    /// </summary>
    /// <returns>A Velaptor framework window implementation.</returns>
    /// <remarks>
    ///     The window width and height are set by the application settings.
    /// </remarks>
    public static IWindow CreateWindow()
    {
        var appSettings = IoC.Container.GetInstance<IAppSettingsService>();

        return CreateWindow(appSettings.Settings.WindowWidth, appSettings.Settings.WindowHeight);
    }

    /// <summary>
    /// Creates an instance of a Velaptor window implementation.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <returns>A Velaptor framework window implementation.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for library users.")]
    public static IWindow CreateWindow(uint width, uint height)
        => new GLWindow(
            width,
            height,
            IoC.Container.GetInstance<IWindowFactory>(),
            IoC.Container.GetInstance<INativeInputFactory>(),
            IoC.Container.GetInstance<IGLInvoker>(),
            IoC.Container.GetInstance<IGLFWInvoker>(),
            IoC.Container.GetInstance<ISystemMonitorService>(),
            IoC.Container.GetInstance<IPlatform>(),
            IoC.Container.GetInstance<ITaskService>(),
            ContentLoaderFactory.CreateContentLoader(),
            RendererFactory.CreateRenderer(width, height),
            IoC.Container.GetInstance<IReactable<GLContextData>>(),
            IoC.Container.GetInstance<IReactable<GLInitData>>(),
            IoC.Container.GetInstance<IReactable<(KeyCode, bool)>>(),
            IoC.Container.GetInstance<IReactable<(int, int)>>(),
            IoC.Container.GetInstance<IReactable<(MouseButton, bool)>>(),
            IoC.Container.GetInstance<IReactable<(MouseScrollDirection, int)>>(),
            IoC.Container.GetInstance<IReactable<ShutDownData>>());
}
