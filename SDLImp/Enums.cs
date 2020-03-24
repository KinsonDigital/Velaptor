namespace SDLScorpPlugin
{
    /// <summary>
    /// Represents the different types of game loops.
    /// </summary>
    public enum TimeStepType
    {
        /// <summary>
        /// Represents a game loop that runs at a specific frequency/FPS
        /// </summary>
        Fixed = 1,
        /// <summary>
        /// Represents a game loop that runs as fast as possible.  Provides
        /// the smoothest results.
        /// </summary>
        Variable = 2
    }
}
