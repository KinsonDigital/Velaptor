namespace KDScorpionCore
{
    /// <summary>
    /// Represents different ways that a scene should run.
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// This makes an scene run continously.  Used for standard game running through frames.
        /// </summary>
        Continuous = 1,

        /// <summary>
        /// This gives fine control to run the game a set amount of frames at a time.
        /// </summary>
        FrameStack = 2
    }
}
