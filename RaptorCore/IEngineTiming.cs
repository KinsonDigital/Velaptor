using System;

namespace RaptorCore
{
    /// <summary>
    /// Represents the timing of a game engine.
    /// </summary>
    public interface IEngineTiming
    {
        #region Props
        /// <summary>
        /// The total engine time that has passed.
        /// </summary>
        TimeSpan TotalEngineTime { get; set; }

        /// <summary>
        /// The total time that has passed for the current frame.
        /// </summary>
        TimeSpan ElapsedEngineTime { get; set; }
        #endregion
    }
}
