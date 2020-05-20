using System;
using System.Diagnostics.CodeAnalysis;

namespace Raptor
{
    /// <summary>
    /// Holds timing information for a game loop.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GameTime : IGameTiming
    {
        #region Props
        /// <summary>
        /// The total time that has passed.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// The total time that has passed for the current frame.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }
        #endregion
    }
}
