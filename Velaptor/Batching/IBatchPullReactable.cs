// <copyright file="IBatchPullReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Batching;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.OneWay;
using OpenGL.Batching;

/// <summary>
/// Pulls data from batch managers for the use of sorting for layered rendering.
/// </summary>
/// <typeparam name="TBatchItem">The type of batch item to pull.</typeparam>
[SuppressMessage("ReSharper", "RedundantTypeDeclarationBody", Justification = "Intentional")]
internal interface IBatchPullReactable<TBatchItem> : IPullReactable<Memory<RenderItem<TBatchItem>>>
{
}
