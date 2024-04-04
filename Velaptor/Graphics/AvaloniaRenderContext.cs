// <copyright file="AvaloniaRenderContext.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;
using Carbonate.OneWay;
using Factories;
using NativeInterop.Services;
using ReactableData;

/// <inheritdoc/>
internal sealed class AvaloniaRenderContext : IRenderContext
{
    private readonly IOpenGLService glService = IoC.Container.GetInstance<IOpenGLService>();
    private readonly IPushReactable<ViewPortSizeData> viewPortReactable = IoC.Container.GetInstance<IPushReactable<ViewPortSizeData>>();

    /// <inheritdoc/>
    public Size GetRenderAreaSize() => this.glService.GetViewPortSize();

    /// <inheritdoc/>
    public void SetRenderAreaSize(int width, int height)
    {
        var mainDisplay = HardwareFactory.GetMainDisplay();

        width = (int)(width * mainDisplay.HorizontalScale);
        height = (int)(height * mainDisplay.VerticalScale);

        this.glService.SetViewPortSize(new Size(width, height));

        this.viewPortReactable.Push(
            PushNotifications.ViewPortSizeChangedId,
            new ViewPortSizeData
            {
                Width = (uint)width, Height = (uint)height,
            });
    }
}
