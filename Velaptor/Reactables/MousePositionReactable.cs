// <copyright file="MousePositionReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables;

using Core;

/// <summary>
/// Creates a reactable to send push notifications to signal that the position of the mouse has changed.
/// </summary>
internal sealed class MousePositionReactable : Reactable<(int x, int y)>
{
}
