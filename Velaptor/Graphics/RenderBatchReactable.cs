// <copyright file="RenderBatchReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using Carbonate.OneWay;
using OpenGL.Batching;

/// <inheritdoc cref="IRenderBatchReactable{TBatchItem}"/>
/// <typeparam name="TBatchItem">The type of batch item to send.</typeparam>
internal sealed class RenderBatchReactable<TBatchItem> : PushReactable<Memory<RenderItem<TBatchItem>>>, IRenderBatchReactable<TBatchItem>
{
}
