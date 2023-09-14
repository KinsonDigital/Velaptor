// <copyright file="IUIDependencyFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Carbonate.OneWay;
using ReactableData;
using UI;

/// <summary>
/// Manages dependencies for UI components.
/// </summary>
internal interface IUIDependencyFactory
{
    /// <summary>
    /// Creates a <see cref="ITextSelection"/> object for the <see cref="TextBox"/> control.
    /// </summary>
    /// <param name="textBoxDataReactable">The reactable used to push text box data notifications.</param>
    /// <returns>The text selection object.</returns>
    public ITextSelection CreateTextSelection(IPushReactable<TextBoxStateData> textBoxDataReactable);

    /// <summary>
    /// Creates a <see cref="ITextCursor"/> object for the <see cref="TextBox"/> control.
    /// </summary>
    /// <param name="textBoxDataReactable">The reactable used to push text box data notifications.</param>
    /// <returns>The text selection object.</returns>
    public ITextCursor CreateTextCursor(IPushReactable<TextBoxStateData> textBoxDataReactable);
}
