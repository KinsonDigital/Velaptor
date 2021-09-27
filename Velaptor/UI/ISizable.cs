namespace Velaptor.UI
{
    public interface ISizable
    {
        /// <summary>
        /// Gets the width of the <see cref="IControl"/>.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the <see cref="IControl"/>.
        /// </summary>
        int Height { get; }
    }
}
