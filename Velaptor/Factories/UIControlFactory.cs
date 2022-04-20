// <copyright file="UIControlFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content.Fonts;
    using Velaptor.UI;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates UI controls.
    /// </summary>
    [ExcludeFromCodeCoverage]

    // TODO: Left as internal to prevent user from using this clas until it is finished
    internal static class UIControlFactory
    {
        /// <summary>
        /// Creates a new <see cref="Label"/> control to display text.
        /// </summary>
        /// <param name="labelText">The text to display in the label.</param>
        /// <returns>The label to render.</returns>
        public static Label CreateLabel(string labelText)
        {
            var label = new Label(
                ContentLoaderFactory.CreateContentLoader(),
                IoC.Container.GetInstance<IFont>()) { Text = labelText };

            return label;
        }
    }
}
