// <copyright file="UIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.UI;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates UI controls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class UIControlFactory
    {
        /// <summary>
        /// Creates a new <see cref="Label"/> control to display text.
        /// </summary>
        /// <param name="labelText">The text to display in the label.</param>
        /// <returns>The label to render.</returns>
        public static Label CreateLabel(string labelText)
        {
            var contentLoader = ContentLoaderFactory.CreateContentLoader();
            var label = new Label(contentLoader);
            label.Text = labelText;

            return label;
        }
    }
}
