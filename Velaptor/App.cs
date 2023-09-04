// <copyright file="App.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Graphics;
using ReactableData;
using Silk.NET.OpenGL;

/// <summary>
/// Represents a Velaptor application.
/// </summary>
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
        glReactable.Push(gl, PushNotifications.GLContextCreatedId);
        glReactable.Unsubscribe(PushNotifications.GLContextCreatedId);

        var initReactable = IoC.Container.GetInstance<IPushReactable>();
        var viewPortReactable = IoC.Container.GetInstance<IPushReactable<ViewPortSizeData>>();

        initReactable.Push(PushNotifications.GLInitializedId);
        initReactable.Unsubscribe(PushNotifications.GLInitializedId);

        viewPortReactable.Push(
            new ViewPortSizeData
            {
                Width = renderAreaWidth, Height = renderAreaHeight,
            }, PushNotifications.ViewPortSizeChangedId);

        return IoC.Container.GetInstance<IRenderContext>();
    }
}
