﻿// <copyright file="IRenderer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers;

using System;
using System.Drawing;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Exceptions;
using NativeInterop.OpenGL;
using OpenGL;
using ReactableData;

/// <summary>
/// Provides basic rendering functionality.
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// <c>true</c> if initialization has been executed.
    /// </summary>
    private static bool isInitialized;

    /// <summary>
    /// The batch size.
    /// </summary>
    private const uint InitialBatchSize = 1000;

    /// <summary>
    /// The OpenGL invoker.
    /// </summary>
    private static readonly IGLInvoker GLInvoker;

    /// <summary>
    /// A push reactable to push messages without data.
    /// </summary>
    private static readonly IPushReactable PushReactable;

    /// <summary>
    /// The unsubscriber for OpenGL the initialization event.
    /// </summary>
    private static readonly IDisposable? GLInitUnsubscriber;

    /// <summary>
    /// Caches the color used to clear the screen during the rendering process.
    /// </summary>
    private static CachedValue<Color>? cachedClearColor;

    /// <summary>
    /// Initializes static members of the <see cref="IRenderer"/> class.
    /// </summary>
    static IRenderer()
    {
        GLInvoker = IoC.Container.GetInstance<IGLInvoker>();
        PushReactable = IoC.Container.GetInstance<IPushReactable>();
        var batchSizeReactable = IoC.Container.GetInstance<IPushReactable<BatchSizeData>>();

        const string glInitName = $"{nameof(IRenderer)}.Ctor - {nameof(PushNotifications.GLInitializedId)}";
        GLInitUnsubscriber = PushReactable.Subscribe(new ReceiveReactor(
            eventId: PushNotifications.GLInitializedId,
            name: glInitName,
            onReceive: () =>
            {
                if (isInitialized)
                {
                    return;
                }

                GLInvoker.Enable(GLEnableCap.Blend);
                GLInvoker.BlendFunc(GLBlendingFactor.SrcAlpha, GLBlendingFactor.OneMinusSrcAlpha);

                foreach (var batchType in Enum.GetValues<BatchType>())
                {
                    batchSizeReactable.Push(
                        new BatchSizeData { BatchSize = InitialBatchSize, TypeOfBatch = batchType },
                        PushNotifications.BatchSizeChangedId);
                }

                PushReactable.Unsubscribe(PushNotifications.BatchSizeChangedId);

                if (cachedClearColor is not null)
                {
                    cachedClearColor.IsCaching = false;
                }

                isInitialized = true;
            },
            onUnsubscribe: () => GLInitUnsubscriber?.Dispose()));

        SetupCaches();
    }

    /// <summary>
    /// Gets or sets the color of the back buffer when cleared.
    /// </summary>
    static Color ClearColor
    {
        get => cachedClearColor?.GetValue() ?? Color.Empty;
        set => cachedClearColor?.SetValue(value);
    }

    /// <summary>
    /// Gets a value indicating whether or not the batch process has begun.
    /// </summary>
    static bool HasBegun { get; private set;  }

    /// <summary>
    /// Initializes the renderer.  This kicks off the static ctor which in turn pushes init notifications
    /// across the application.
    /// </summary>
    internal static void Init()
    {
        // Left empty on purpose.  This is to invoke the static ctor.
    }

    /// <summary>
    /// Starts the batch rendering process.  Must be called before invoking any render methods.
    /// </summary>
    static void Begin()
    {
        if (!isInitialized)
        {
            throw new RendererException($"The '{nameof(IRenderer)}' is not initialized.");
        }

        HasBegun = true;
        PushReactable.Push(PushNotifications.BatchHasBegunId);
    }

    /// <summary>
    /// Clears the buffers.
    /// </summary>
    /// <remarks>
    ///     It is best to clear the buffer before rendering all of the textures.
    ///     This is to make sure smearing does not occur during texture
    ///     movement or animation.
    /// </remarks>
    static void Clear()
    {
        if (!isInitialized)
        {
            throw new RendererException($"The '{nameof(IRenderer)}' is not initialized.");
        }

        GLInvoker.Clear(GLClearBufferMask.ColorBufferBit);
    }

    /// <summary>
    /// Ends the batch process.  Calling this will render any textures
    /// still in the batch.
    /// </summary>
    static void End()
    {
        if (!isInitialized)
        {
            throw new RendererException($"The '{nameof(IRenderer)}' is not initialized.");
        }

        HasBegun = false;
        PushReactable.Push(PushNotifications.BatchHasEndedId);
    }

    /// <summary>
    /// Setup all of the caching for the properties that need caching.
    /// </summary>
    private static void SetupCaches() =>
        cachedClearColor = new CachedValue<Color>(
            Color.FromArgb(255, 16, 29, 36),
            () =>
            {
                var colorValues = new float[4];
                GLInvoker.GetFloat(GLGetPName.ColorClearValue, colorValues);

                var red = colorValues[0].MapValue(0, 1, 0, 255);
                var green = colorValues[1].MapValue(0, 1, 0, 255);
                var blue = colorValues[2].MapValue(0, 1, 0, 255);
                var alpha = colorValues[3].MapValue(0, 1, 0, 255);

                return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
            },
            value =>
            {
                var red = value.R.MapValue(0f, 255f, 0f, 1f);
                var green = value.G.MapValue(0f, 255f, 0f, 1f);
                var blue = value.B.MapValue(0f, 255f, 0f, 1f);
                var alpha = value.A.MapValue(0f, 255f, 0f, 1f);

                GLInvoker.ClearColor(red, green, blue, alpha);
            });
}
