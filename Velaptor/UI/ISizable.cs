namespace Velaptor.UI
{
    /// <summary>
    /// Represents the size of an object by its width and height.
    /// </summary>
    public interface ISizable
    {
        /// <summary>
        /// Gets the width of the <see cref="IControl"/>.
        /// </summary>
        uint Width { get; }

        /// <summary>
        /// Gets the height of the <see cref="IControl"/>.
        /// </summary>
        uint Height { get; }
    }
}
