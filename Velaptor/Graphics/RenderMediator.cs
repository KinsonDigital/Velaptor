// <copyright file="RenderMediator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using Carbonate;
using Guards;

/// <inheritdoc/>
internal sealed class RenderMediator : IRenderMediator
{
    private readonly IPushReactable reactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediator"/> class.
    /// </summary>
    /// <param name="reactable">Sends and receives push notifications.</param>
    public RenderMediator(IPushReactable reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.reactable = reactable;

        var batchEndName = this.GetExecutionMemberName(nameof(NotificationIds.RenderBatchEndedId));

        reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.RenderBatchEndedId,
            name: batchEndName,
            onReceive: CoordinateRenders));
    }

    /// <summary>
    /// Coordinates the rendering between each of the renderers.
    /// </summary>
    private void CoordinateRenders()
    {
        this.reactable.Push(NotificationIds.RenderTexturesId);
        this.reactable.Push(NotificationIds.RenderRectsId);
        this.reactable.Push(NotificationIds.RenderFontsId);
        this.reactable.Push(NotificationIds.RenderLinesId);
    }
}
