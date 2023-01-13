// <copyright file="RenderMediator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using Carbonate.NonDirectional;
using Factories;
using Guards;

/// <inheritdoc/>
internal sealed class RenderMediator : IRenderMediator
{
    private readonly IPushReactable pushReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediator"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public RenderMediator(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.pushReactable = reactableFactory.CreateNoDataReactable();

        var batchEndName = this.GetExecutionMemberName(nameof(NotificationIds.RenderBatchEndedId));

        this.pushReactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderBatchEndedId,
            name: batchEndName,
            onReceive: CoordinateRenders));
    }

    /// <summary>
    /// Coordinates the rendering between each of the renderers.
    /// </summary>
    private void CoordinateRenders()
    {
        this.pushReactable.Push(NotificationIds.RenderTexturesId);
        this.pushReactable.Push(NotificationIds.RenderRectsId);
        this.pushReactable.Push(NotificationIds.RenderFontsId);
        this.pushReactable.Push(NotificationIds.RenderLinesId);
    }
}
