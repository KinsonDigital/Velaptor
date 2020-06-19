using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Raptor.Physics
{
    /// <summary>
    /// Represents a world with simulated physics.
    /// </summary>
    public class PhysicsWorld
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="PhysicsWorld"/> that will
        /// hold all instances of <see cref="PhysicsBody"/> objects and also
        /// control the physics of those bodies.
        /// </summary>
        /// <param name="gravity">The gravity of the world.</param>
        [ExcludeFromCodeCoverage]
        public PhysicsWorld(Vector2 gravity)
        {
            var otherGravity = gravity.X;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the worlds gravity.
        /// </summary>
        public Vector2 Gravity { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Adds the given <paramref name="body"/> to the world.
        /// </summary>
        /// <param name="body">The body to add.</param>
        public void AddBody(object body)//TODO: Need to figure out what the body type is
        {
            throw new NotImplementedException();
            //_internalWorld.AddBody(body);
        }


        /// <summary>
        /// Updates the physics world to keep the physics simulation moving ahead.
        /// </summary>
        /// <param name="dt">The time passed in milliseconds since the last frame.</param>
        public void Update(float dt)
        {
            throw new NotImplementedException();
            //_internalWorld.Update(dt);
        }
        #endregion
    }
}
