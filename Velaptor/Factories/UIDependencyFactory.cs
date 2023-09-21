// <copyright file="UIDependencyFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Carbonate.OneWay;
using ReactableData;
using UI;

/// <inheritdoc/>
internal class UIDependencyFactory : IUIDependencyFactory
{
    /// <inheritdoc/>
    public ITextSelection CreateTextSelection(IPushReactable<TextBoxStateData> textBoxDataReactable) => new TextSelection(textBoxDataReactable);

    /// <inheritdoc/>
    public ITextCursor CreateTextCursor(IPushReactable<TextBoxStateData> textBoxDataReactable) => new TextCursor(textBoxDataReactable);
}
