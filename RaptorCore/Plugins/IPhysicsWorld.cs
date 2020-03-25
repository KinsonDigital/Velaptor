namespace RaptorCore.Plugins
{
    /// <summary>
    /// Represents a world with simulated physics.
    /// </summary>
    public interface IPhysicsWorld
    {
        #region Props
        /// <summary>
        /// Gets or sets the gravity in the X plane.
        /// </summary>
        float GravityX { get; set; }

        /// <summary>
        /// Gets or sets the gravity in the Y plane.
        /// </summary>
        float GravityY { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Updates the physics world.
        /// </summary>
        /// <param name="dt">The time passed in milliseconds since the last frame.</param>
        void Update(float dt);


        /// <summary>
        /// Adds the given <paramref name="body"/> to the physics world.
        /// </summary>
        /// <typeparam name="T">The type of physics body to add.</typeparam>
        /// <param name="body">The body to add.</param>
        void AddBody<T>(T body) where T : IPhysicsBody;


        /// <summary>
        /// Removes the body from the physics world.
        /// </summary>
        void RemoveBody();


        /// <summary>
        /// Gets the from the world.
        /// </summary>
        T GetBody<T>();
        #endregion
    }
}
