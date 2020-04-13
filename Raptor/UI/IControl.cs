using System.Numerics;

namespace Raptor.UI
{
    /// <summary>
    /// A user interface object that can be updated and rendered to the screen.
    /// </summary>
    interface IControl : IUpdatable, IInitialize, IContentLoadable
    {
        #region Props
        /// <summary>
        /// Gets or sets the position of the <see cref="IControl"/> on the screen.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="IControl"/>.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets or sets the height of the <see cref="IControl"/>.
        /// </summary>
        int Height { get; }
        #endregion
    }
}
