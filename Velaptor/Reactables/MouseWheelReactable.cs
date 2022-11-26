// <copyright file="MouseWheelReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables;

using Input;
using Core;

/// <summary>
/// Creates a reactable to send push notifications to signal that the state of the mouse wheel has changed.
/// </summary>
internal sealed class MouseWheelReactable : Reactable<(MouseScrollDirection, int)>
{
}
