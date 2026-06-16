// <copyright file="ImGuiManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Carbonate;
using Carbonate.Fluent;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using ImGuiNET;
using ReactableData;
using Silk.NET.OpenGL.Extensions.ImGui;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Contains direct ImGui native calls that cannot be unit tested.")]
internal sealed class ImGuiManager : IImGuiManager
{
    private readonly IDisposable? glObjectsUnsubscriber;
    private readonly IDisposable? glInitUnsubscriber;
    private readonly IDisposable? winSizeUnsubscriber;
    /* NOTE: Do not dispose of the 'ImGuiController' object.  It destroys the OpenGL context
     * and the context of the entire application is the same context used in the controller.
    */
    private ImGuiController? controller;
    private Vector2 windowSize = new (1920, 1080);
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiManager"/> class.
    /// </summary>
    /// <param name="glObjectsReactable">Receives various OpenGL objects.</param>
    /// <param name="pushReactable">Receives non-data push notifications.</param>
    /// <param name="winSizeReactable">Receives window size change notifications.</param>
    public ImGuiManager(
        IPushReactable<GLObjectsData> glObjectsReactable,
        IPushReactable pushReactable,
        IPushReactable<WindowSizeData> winSizeReactable)
    {
        // OpenGL mode: create the Silk.NET ImGuiController once the GL context and window are ready.
        var glObjectsSubscription = ISubscriptionBuilder
            .Create()
            .WithId(PushNotifications.GLObjectsCreatedId)
            .WithName($"{nameof(ImGuiManager)}.Ctor - GL Objects")
            .WhenUnsubscribing(() => this.glObjectsUnsubscriber?.Dispose())
            .BuildOneWayReceive<GLObjectsData>(glObjData =>
            {
                this.controller = new ImGuiController(glObjData.GL, glObjData.Window, glObjData.InputContext);
            });

        this.glObjectsUnsubscriber = glObjectsReactable.Subscribe(glObjectsSubscription);

        // Track the actual window dimensions so io.DisplaySize stays accurate.
        this.winSizeUnsubscriber = winSizeReactable.CreateOneWayReceive(
            PushNotifications.WindowSizeChangedId,
            data => this.windowSize = new Vector2(data.Width, data.Height),
            () => this.winSizeUnsubscriber?.Dispose());

        // Both modes: when the window has finished loading (GLInitializedId covers both OpenGL and
        // WebGPU), bootstrap a minimal ImGui context if we are in WebGPU mode (i.e. the GL
        // controller was never created).  This keeps UI libraries such as KdGui functional —
        // they call ImGui freely inside draw callbacks, so a valid context and a per-frame
        // NewFrame/Render cycle are required even when draw data is never sent to the GPU.
        this.glInitUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.GLInitializedId,
            () =>
            {
                if (this.controller is not null)
                {
                    return; // OpenGL mode: ImGuiController already owns the context.
                }

                // WebGPU mode: create a context-only ImGui setup without a GPU rendering backend.
                ImGui.CreateContext();
                var io = ImGui.GetIO();
                io.DisplaySize = this.windowSize;
                io.DeltaTime = 1.0f / 60.0f;
            },
            () => this.glInitUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    public bool IsOpenGlMode => this.controller is not null;

    /// <inheritdoc/>
    public void Update(double timeSeconds)
    {
        if (this.controller is not null)
        {
            this.controller.Update((float)timeSeconds);
        }
        else
        {
            // WebGPU mode: advance the ImGui frame manually so any UI library that calls
            // ImGui.Begin/Text/etc. within the draw callback finds a valid frame in progress.
            var io = ImGui.GetIO();
            io.DisplaySize = this.windowSize;
            io.DeltaTime = (float)timeSeconds;
            ImGui.NewFrame();
        }
    }

    /// <inheritdoc/>
    public void Render()
    {
        if (this.controller is not null)
        {
            this.controller.Render();
        }
        else
        {
            // WebGPU mode: finalise the ImGui frame so internal state is consistent.
            // The resulting draw data is intentionally discarded — no GPU upload occurs.
            ImGui.Render();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.glObjectsUnsubscriber?.Dispose();
        this.glInitUnsubscriber?.Dispose();
        this.winSizeUnsubscriber?.Dispose();

        this.isDisposed = true;
    }
}
