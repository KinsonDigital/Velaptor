// <copyright file="MouseWheelReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Input;
using Velaptor.Reactables.Core;

namespace Velaptor.Reactables;

/// <summary>
/// Creates a reactable to send push notifications to signal that the state of the mouse wheel has changed.
/// </summary>
internal sealed class MouseWheelReactable : Reactable<(MouseScrollDirection, int)>
{
}
