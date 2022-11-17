// <copyright file="ShutDownReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.Reactables;

/// <summary>
/// Creates a reactable to send push notifications to signal that the application is shutting down.
/// </summary>
internal sealed class ShutDownReactable : Reactable<ShutDownData>
{
}
