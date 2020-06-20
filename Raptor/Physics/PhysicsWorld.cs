// <copyright file="PhysicsWorld.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Physics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;

    /// <summary>
    /// Represents a world with simulated physics.
    /// </summary>
    public class PhysicsWorld
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicsWorld"/> class.
        /// </summary>
        /// <param name="gravity">The gravity of the world.</param>
        [ExcludeFromCodeCoverage]
        public PhysicsWorld(Vector2 gravity)
        {
        }

        /// <summary>
        /// Gets or sets the worlds gravity.
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// Adds the given <paramref name="body"/> to the world.
        /// </summary>
        /// <param name="body">The body to add.</param>
        public void AddBody(object body) // TODO: Need to figure out what the body type is
            => throw new NotImplementedException(); // _internalWorld.AddBody(body);

        /// <summary>
        /// Updates the physics world to keep the physics simulation moving ahead.
        /// </summary>
        /// <param name="dt">The time passed in milliseconds since the last frame.</param>
        public void Update(float dt) => throw new NotImplementedException(); // _internalWorld.Update(dt);
    }
}
