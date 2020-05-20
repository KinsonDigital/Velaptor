namespace Raptor
{
    /// <summary>
    /// Provides the ability for an object to be updated.
    /// </summary>
    public interface IUpdatable
    {
        #region Methods
        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="engineTime">The game engine time.</param>
        void Update(GameTime engineTime);
        #endregion
    }
}
