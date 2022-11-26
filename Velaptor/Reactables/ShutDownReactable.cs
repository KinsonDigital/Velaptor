// <copyright file="ShutDownReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables;

using Core;
using ReactableData;

/// <summary>
/// Creates a reactable to send push notifications to signal that the application is shutting down.
/// </summary>
internal sealed class ShutDownReactable : Reactable<ShutDownData>
{
}
