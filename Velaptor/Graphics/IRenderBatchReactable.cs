// <copyright file="IRenderBatchReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.OneWay;
using OpenGL.Batching;

/// <summary>
/// Sends batch data to a renderer for the purpose of rendering.
/// </summary>
/// <typeparam name="TBatchItem">The type of batch item to send.</typeparam>
[SuppressMessage("ReSharper", "RedundantTypeDeclarationBody", Justification = "Intentional")]
internal interface IRenderBatchReactable<TBatchItem> : IPushReactable<Memory<RenderItem<TBatchItem>>>
{
}
