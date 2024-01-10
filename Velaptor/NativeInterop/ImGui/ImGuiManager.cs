// <copyright file="ImGuiManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.ImGui;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.Fluent;
using Carbonate.OneWay;
using ReactableData;
using Silk.NET.OpenGL.Extensions.ImGui;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Contains direct ImGui calls.")]
internal sealed class ImGuiManager : IImGuiManager
{
    private readonly IDisposable? unsubscriber;
    /* NOTE: Do not dispose of the 'ImGuiController' object.  It destroys the OpenGL context
     * and the context of the entire application is the same context used in the controller.
    */
    private ImGuiController? controller;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiManager"/> class.
    /// </summary>
    /// <param name="glObjectsReactable">Receives various OpenGL objects.</param>
    // public ImGuiManager(IPushReactable<ImGuiController> imGuiControllerReactable)
    public ImGuiManager(IPushReactable<GLObjectsData> glObjectsReactable)
    {
        var subscription = ISubscriptionBuilder
            .Create()
            .WithId(PushNotifications.GLObjectsCreatedId)
            .WithName($"{nameof(ImGuiManager)}.Ctor")
            .WhenUnsubscribing(() => this.unsubscriber?.Dispose())
            .BuildOneWayReceive<GLObjectsData>(glObjData =>
            {
                this.controller = new ImGuiController(glObjData.GL, glObjData.Window, glObjData.InputContext);
            });

        this.unsubscriber = glObjectsReactable.Subscribe(subscription);
    }

    /// <inheritdoc/>
    /// <param name="timeSeconds"></param>
    public void Update(double timeSeconds)
    {
        ArgumentNullException.ThrowIfNull(this.controller);
        this.controller.Update((float)timeSeconds);
    }

    /// <inheritdoc/>
    public void Render()
    {
        ArgumentNullException.ThrowIfNull(this.controller);
        this.controller.Render();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.unsubscriber?.Dispose();

        this.isDisposed = true;
    }
}
