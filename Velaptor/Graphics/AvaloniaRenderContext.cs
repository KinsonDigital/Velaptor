// <copyright file="AvaloniaRenderContext.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Factories;
using ReactableData;

/// <inheritdoc/>
internal sealed class AvaloniaRenderContext : IRenderContext
{
    private readonly IPushReactable pushReactable = IoC.Container.GetInstance<IPushReactable>();
    private readonly IPushReactable<ViewPortSizeData> viewPortReactable = IoC.Container.GetInstance<IPushReactable<ViewPortSizeData>>();
    private readonly IPullReactable<WindowSizeData> pullWinSizeReactable = IoC.Container.GetInstance<IPullReactable<WindowSizeData>>();

    /// <inheritdoc/>
    public Size GetRenderAreaSize()
    {
        var winSize = this.pullWinSizeReactable.Pull(PullNotifications.GetWindowSizeId);
        return new Size((int)winSize.Width, (int)winSize.Height);
    }

    /// <inheritdoc/>
    public void SetRenderAreaSize(int width, int height)
    {
        var mainDisplay = HardwareFactory.GetMainDisplay();

        width = (int)(width * mainDisplay.HorizontalScale);
        height = (int)(height * mainDisplay.VerticalScale);

        // Signal the WebGPU batcher to reconfigure the swap chain for the new size.
        this.pushReactable.Push(PushNotifications.SurfaceReconfigureId);

        this.viewPortReactable.Push(
            PushNotifications.ViewPortSizeChangedId,
            new ViewPortSizeData
            {
                Width = (uint)width, Height = (uint)height,
            });
    }
}

