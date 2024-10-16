﻿// <copyright file="App.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System.Diagnostics.CodeAnalysis;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Graphics;
using ReactableData;
using Silk.NET.OpenGL;

/// <summary>
/// Represents a Velaptor application.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "This is a static class that doesn't need to be tested.")]
public static class App
{
    /// <summary>
    /// Sets Velaptor to render to an Avalonia control.
    /// </summary>
    /// <param name="gl">The <see cref="Silk"/> OpenGL object.</param>
    /// <param name="renderAreaWidth">The width of the render area.</param>
    /// <param name="renderAreaHeight">The height of the render area.</param>
    /// <returns>The Avalonia render context.</returns>
    public static IRenderContext UseAvaloniaRenderContext(GL gl, uint renderAreaWidth, uint renderAreaHeight)
    {
        var glReactable = IoC.Container.GetInstance<IPushReactable<GL>>();
        glReactable.Push(PushNotifications.GLContextCreatedId, gl);
        glReactable.Unsubscribe(PushNotifications.GLContextCreatedId);

        var initReactable = IoC.Container.GetInstance<IPushReactable>();
        var viewPortReactable = IoC.Container.GetInstance<IPushReactable<ViewPortSizeData>>();

        initReactable.Push(PushNotifications.GLInitializedId);
        initReactable.Unsubscribe(PushNotifications.GLInitializedId);

        viewPortReactable.Push(
            PushNotifications.ViewPortSizeChangedId,
            new ViewPortSizeData
            {
                Width = renderAreaWidth, Height = renderAreaHeight,
            });

        return IoC.Container.GetInstance<IRenderContext>();
    }
}
