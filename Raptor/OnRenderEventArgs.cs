using KDScorpionCore.Plugins;
using System;

namespace KDScorpionCore
{
    /// <summary>
    /// Holds information about the <see cref="IEngineEvents.OnRender"/> event.
    /// </summary>
    public class OnRenderEventArgs : EventArgs
    {
        #region Props
        /// <summary>
        /// Used to render to the screen.
        /// </summary>
        public IRenderer Renderer { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="OnRenderEventArgs"/>.
        /// </summary>
        /// <param name="renderer">The game renderer.</param>
        public OnRenderEventArgs(IRenderer renderer) => Renderer = renderer;
        #endregion
    }
}
