using System;

namespace Raptor
{
    /// <summary>
    /// Provides the core of a game which facilitates how the engine starts, stops,
    /// manages time and how the game loop runs.
    /// </summary>
    public interface IWindow : IDisposable
    {
        #region Props
        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        int Height { get; set; }

        string Title { get; set; }

        Action<FrameTime>? Update { get; set; }

        Action<FrameTime>? Draw { get; set; }

        Action? Init { get; set; }

        int UpdateFreq { get; set; }
        #endregion


        #region Methods
        /// <summary>
        /// Shows the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
        #endregion
    }
}
