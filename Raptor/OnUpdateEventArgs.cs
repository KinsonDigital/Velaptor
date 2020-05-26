using System;

namespace Raptor
{
    /// <summary>
    /// Holds information about the <see cref="IGameEvents.OnUpdate"/> event.
    /// </summary>
    public class OnUpdateEventArgs : EventArgs
    {
        #region Props
        /// <summary>
        /// Holds elapsed time information of when the game loop last ran.
        /// </summary>
        public FrameTime EngineTime { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="OnUpdateEventArgs"/>.
        /// </summary>
        /// <param name="engineTime">The game engine time.</param>
        public OnUpdateEventArgs(FrameTime engineTime) => EngineTime = engineTime;
        #endregion
    }
}
