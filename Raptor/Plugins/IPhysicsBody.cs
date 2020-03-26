namespace Raptor.Plugins
{
    /// <summary>
    /// Represents a body in a world that obeys physics.
    /// </summary>
    public interface IPhysicsBody
    {
        #region Props
        /// <summary>
        /// The X vertices of the body's shape.
        /// </summary>
        float[] XVertices { get; set; }

        /// <summary>
        /// The X vertices of the body's shape.
        /// </summary>
        float[] YVertices { get; set; }

        /// <summary>
        /// The X coordinate of the body's location.
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// The Y coordinate of the body's location.
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// Gets or sets the angle of the body in degrees.
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// Gets or sets the density of the body.
        /// </summary>
        float Density { get; set; }

        /// <summary>
        /// Gets or sets the friction of the body.
        /// </summary>
        float Friction { get; set; }

        /// <summary>
        /// Gets or sets the restitution(bounciness) of the body.
        /// </summary>
        float Restitution { get; set; }

        /// <summary>
        /// Gets or sets the linear velocity in the X plane.
        /// </summary>
        float LinearVelocityX { get; set; }

        /// <summary>
        /// Gets or sets the linear velocity in the Y plane.
        /// </summary>
        float LinearVelocityY { get; set; }

        /// <summary>
        /// Gets or sets the linear deceleration.
        /// </summary>
        float LinearDeceleration { get; set; }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        float AngularVelocity { get; set; }

        /// <summary>
        /// Gets or sets the angular desceleration.
        /// </summary>
        float AngularDeceleration { get; set; }

        /// <summary>
        /// Gets or sets the list of actions to execute after the body has been added to the <see cref="IPhysicsWorld"/>.
        /// </summary>
        DeferredActions AfterAddedToWorldActions { get; set; }
        #endregion


        #region Methods
        /// <summary>
        /// Applies a linear impulse to the body using the
        /// the given <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the location to apply the impulse.</param>
        /// <param name="y">The Y coordinate of the location to apply the impulse.</param>
        void ApplyLinearImpulse(float x, float y);


        /// <summary>
        /// Applies an angular impulse to the body using
        /// the given <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the location to apply the impulse.</param>
        /// <param name="y">The Y coordinate of the location to apply the impulse.</param>
        void ApplyAngularImpulse(float value);


        /// <summary>
        /// Applies a force to the body in the X and Y planes at the given world location.
        /// </summary>
        /// <param name="forceX">The force to apply in the X direction.</param>
        /// <param name="forceY">The force to apply in the Y direction.</param>
        /// <param name="worldLocationX">The location in the world of where to apply this force.</param>
        /// <param name="worldLocationY">The location in the world of where to apply this force.</param>
        void ApplyForce(float forceX, float forceY, float worldLocationX, float worldLocationY);
        #endregion
    }
}
