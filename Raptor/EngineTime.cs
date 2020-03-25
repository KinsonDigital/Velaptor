using System;
using System.Diagnostics.CodeAnalysis;

namespace Raptor
{
    /// <summary>
    /// Holds timing information of the game engine.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EngineTime : IEngineTiming
    {
        #region Props
        /// <summary>
        /// The total engine time that has passed.
        /// </summary>
        public TimeSpan TotalEngineTime { get; set; }

        /// <summary>
        /// The total time that has passed for the current frame.
        /// </summary>
        public TimeSpan ElapsedEngineTime { get; set; }
        #endregion
    }
}
