using System;

namespace KDScorpionCore.Plugins
{
    /// <summary>
    /// Provides the core of a game engine which facilitates how the engine starts, stops,
    /// manages time and how the game loop runs.
    /// </summary>
    public interface IEngineCore : IDisposable, IEngineEvents
    {
        #region Props
        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        int WindowWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        int WindowHeight { get; set; }

        /// <summary>
        /// Gets or sets the renderer that renders graphics to the window.
        /// </summary>
        IRenderer Renderer { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Starts the engine.
        /// </summary>
        void Start();


        /// <summary>
        /// Stops the engine.
        /// </summary>
        void Stop();


        /// <summary>
        /// Sets how many frames the engine will process per second.
        /// </summary>
        /// <param name="value">The total number of frames.</param>
        void SetFPS(float value);


        /// <summary>
        /// Returns true if the engine is running.
        /// </summary>
        /// <returns></returns>
        bool IsRunning();
        #endregion
    }
}
