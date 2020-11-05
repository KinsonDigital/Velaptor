// <copyright file="RenderText.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System.Drawing;

    /// <summary>
    /// Provides text that can be rendered to a graphics surface.
    /// </summary>
    public class RenderText
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets or sets the foreground color of the text.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Concatenates the text of 2 <see cref="RenderText"/> objects.
        /// </summary>
        /// <param name="textA">The first object.</param>
        /// <param name="textB">The second object.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string operator +(RenderText textA, RenderText textB)
        {
            var textAResult = string.Empty;
            var textBResult = string.Empty;

            if (!(textA is null))
                textAResult = textA.Text;

            if (!(textB is null))
                textBResult = textB.Text;

            return textAResult + textBResult;
        }

        /// <summary>
        /// Concatenates a <see cref="RenderText"/> object and a string.
        /// </summary>
        /// <param name="textA">The text object to combine.</param>
        /// <param name="textB">The string to combine.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string operator +(RenderText textA, string textB)
        {
            var textAResult = string.Empty;

            if (!(textA is null))
                textAResult = textA.Text;

            return textAResult + textB;
        }

        /// <summary>
        /// Concatenates a string and a <see cref="RenderText"/> object.
        /// </summary>
        /// <param name="textA">The string to combine.</param>
        /// <param name="textB">The text object to combine.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string operator +(string textA, RenderText textB)
        {
            var textBResult = string.Empty;

            if (!(textB is null))
                textBResult = textB.Text;

            return textA + textBResult;
        }

        /// <summary>
        /// Concatenates the text of 2 <see cref="RenderText"/> objects.
        /// </summary>
        /// <param name="textA">The first object.</param>
        /// <param name="textB">The second object.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string Add(RenderText textA, RenderText textB) => textA + textB;

        /// <summary>
        /// Concatenates a <see cref="RenderText"/> object and a string.
        /// </summary>
        /// <param name="textA">The text object to combine.</param>
        /// <param name="textB">The string to combine.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string Add(RenderText textA, string textB) => textA + textB;

        /// <summary>
        /// Concatenates a string and a <see cref="RenderText"/> object.
        /// </summary>
        /// <param name="textA">The string to combine.</param>
        /// <param name="textB">The text object to combine.</param>
        /// <returns>Result of both text values concatenated.</returns>
        public static string Add(string textA, RenderText textB) => textA + textB;
    }
}
