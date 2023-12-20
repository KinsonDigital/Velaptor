// <copyright file="BatchPullReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.OneWay;
using OpenGL.Batching;

/// <inheritdoc cref="IBatchPullReactable{TBatchItem}"/>
[SuppressMessage("ReSharper", "RedundantTypeDeclarationBody", Justification = "Intentional")]
internal sealed class BatchPullReactable<TBatchItem> : PullReactable<Memory<RenderItem<TBatchItem>>>, IBatchPullReactable<TBatchItem>
{
}
