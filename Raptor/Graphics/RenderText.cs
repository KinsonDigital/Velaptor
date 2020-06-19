namespace Raptor.Graphics
{
    /// <summary>
    /// Provides game text that can be rendered to a graphics surface.
    /// </summary>
    public class RenderText
    {
        #region Props
        public IText? InternalText { get; set; }

        public string Text
        {
            get => InternalText is null ? "" : InternalText.Text;
            set { if (!(InternalText is null)) InternalText.Text = value ?? ""; }
        }

        public int Width => InternalText is null ? 0 : InternalText.Width;

        public int Height => InternalText is null ? 0 : InternalText.Height;

        public Color Color
        {
            get => InternalText is null ? new Color(0, 0, 0, 0) : InternalText.Color;
            set { if (!(InternalText is null)) InternalText.Color = value; }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Concatenates the text of 2 <see cref="RenderText"/> objects.
        /// </summary>
        /// <param name="textA">The first object.</param>
        /// <param name="textB">The second object.</param>
        /// <returns></returns>
        public static string Add(RenderText textA, RenderText textB) => textA + textB;


        /// <summary>
        /// Concatenates a <see cref="RenderText"/> object and a string.
        /// </summary>
        /// <param name="textA">The game text object to combine.</param>
        /// <param name="textB">The string to combine.</param>
        /// <returns></returns>
        public static string Add(RenderText textA, string textB) => textA + textB;


        /// <summary>
        /// Concatenates a string and a <see cref="RenderText"/> object.
        /// </summary>
        /// <param name="textA">The string to combine.</param>
        /// <param name="textB">The game text object to combine.</param>
        /// <returns></returns>
        public static string Add(string textA, RenderText textB) => textA + textB;
        #endregion


        #region Overloaded Operators
        /// <summary>
        /// Concatenates the text of 2 <see cref="RenderText"/> objects.
        /// </summary>
        /// <param name="textA">The first object.</param>
        /// <param name="textB">The second object.</param>
        /// <returns></returns>
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
        /// <param name="textA">The game text object to combine.</param>
        /// <param name="textB">The string to combine.</param>
        /// <returns></returns>
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
        /// <param name="textB">The game text object to combine.</param>
        /// <returns></returns>
        public static string operator +(string textA, RenderText textB)
        {
            var textBResult = string.Empty;

            if (!(textB is null))
                textBResult = textB.Text;


            return textA + textBResult;
        }
        #endregion
    }
}
