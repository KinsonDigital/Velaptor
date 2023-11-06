﻿// <copyright file="WindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using NativeInterop.GLFW;
using NativeInterop.OpenGL;
using OpenGL;
using Scene;
using Services;
using UI;

/// <summary>
/// Velaptor application specific functionality.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal static class WindowFactory
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
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IWindowFactory>(),
            IoC.Container.GetInstance<INativeInputFactory>(),
            IoC.Container.GetInstance<IGLInvoker>(),
            IoC.Container.GetInstance<IGlfwInvoker>(),
            IoC.Container.GetInstance<ISystemDisplayService>(),
            IoC.Container.GetInstance<IPlatform>(),
            IoC.Container.GetInstance<ITaskService>(),
            IoC.Container.GetInstance<ISceneManager>(),
            IoC.Container.GetInstance<IReactableFactory>(),
            IoC.Container.GetInstance<ITimerService>());
}
