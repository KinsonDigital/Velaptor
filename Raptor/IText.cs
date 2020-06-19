using Raptor.Graphics;

namespace Raptor
{
    /// <summary>
    /// Text that can be rendered to the graphics surface.
    /// </summary>
    public interface IText
    {
        #region Props
        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        Color Color { get; set; }
        #endregion
    }
}
