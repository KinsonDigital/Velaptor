// <copyright file="BatchPullReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using Carbonate.UniDirectional;
using OpenGL.Batching;

/// <inheritdoc cref="IBatchPullReactable{TBatchItem}"/>
internal sealed class BatchPullReactable<TBatchItem> : PullReactable<Memory<RenderItem<TBatchItem>>>, IBatchPullReactable<TBatchItem>
{
}
