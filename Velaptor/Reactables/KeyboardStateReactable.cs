// <copyright file="KeyboardStateReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables;

using Input;
using Core;

/// <summary>
/// Creates a reactable to send push notifications to signal that the state of the keyboard has changed.
/// </summary>
internal sealed class KeyboardStateReactable : Reactable<(KeyCode key, bool isDown)>
{
}
