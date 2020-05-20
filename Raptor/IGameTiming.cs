using System;

namespace Raptor
{
    /// <summary>
    /// Holds timing information for a game loop.
    /// </summary>
    public interface IGameTiming
    {
        #region Props
        /// <summary>
        /// The total time that has passed.
        /// </summary>
        TimeSpan TotalTime { get; set; }

        /// <summary>
        /// The total time that has passed for the current frame.
        /// </summary>
        TimeSpan ElapsedTime { get; set; }
        #endregion
    }
}
