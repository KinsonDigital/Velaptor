// <copyright file="MouseButtonReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Input;
using Velaptor.Reactables.Core;

namespace Velaptor.Reactables;

/// <summary>
/// Creates a reactable to send push notifications to signal changes to the state of the mouse buttons.
/// </summary>
internal sealed class MouseButtonReactable : Reactable<(MouseButton button, bool isDown)>
{
}
