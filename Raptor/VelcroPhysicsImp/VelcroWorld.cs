// <copyright file="VelcroWorld.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.VelcroPhysicsImp
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using VelcroPhysics.Dynamics;
    using VelcroPhysics.Factories;
    using VelcroPhysics.Primitives;
    using VelcroPhysics.Shared;

    /// <summary>
    /// Represents a world with simulated physics.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VelcroWorld
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VelcroWorld"/> class.
        /// </summary>
        /// <param name="gravityX">The gravity in the X plane.</param>
        /// <param name="gravityY">The gravity in the Y plane.</param>
        public VelcroWorld(float gravityX, float gravityY)
        {
            PhysicsWorld = new World(new Vector2(gravityX, gravityY));

            GravityX = gravityX;
            GravityY = gravityY;
        }

        /// <summary>
        /// Gets or sets the world's gravity in the X plane.
        /// </summary>
        public float GravityX { get; set; }

        /// <summary>
        /// Gets or sets the world's gravity in the Y plane.
        /// </summary>
        public float GravityY { get; set; }

        /// <summary>
        /// Gets or sets the velcro physics world that is used internally.
        /// </summary>
        internal static World? PhysicsWorld { get; set; }

        /// <summary>
        /// Updates the physics world.
        /// </summary>
        /// <param name="dt">The time that has passed for the current frame.</param>
        public void Update(float dt)
        {
            if (!(PhysicsWorld is null))
            {
                PhysicsWorld.Step(dt);
            }
        }

        /// <summary>
        /// Adds the given <paramref name="body"/> to the physics world.
        /// </summary>
        /// <typeparam name="T">The type of physics body to add.</typeparam>
        /// <param name="body">The body to add.</param>
        public void AddBody<T>(T body)
        {
            var velVertices = new Vertices();

            // TODO: Figure out how to pull the PhysicsBodySettings
            // data out of the IPhysicsBody
            // var bodySettings = body.GetData<PhysicsBodySettings>(1);
            var bodySettings = new PhysicsBodySettings();

            for (var i = 0; i < bodySettings.XVertices.Count; i++)
            {
                velVertices.Add(new Vector2(bodySettings.XVertices[i], bodySettings.YVertices[i]).ToPhysicsVector());
            }

            var physicsBody = BodyFactory.CreatePolygon(PhysicsWorld, velVertices, bodySettings.Density, new Vector2(bodySettings.XPosition, bodySettings.YPosition).ToPhysicsVector(), bodySettings.Angle.ToRadians(), bodySettings.IsStatic ? BodyType.Static : BodyType.Dynamic);
            physicsBody.Friction = bodySettings.Friction;
            physicsBody.Restitution = bodySettings.Restitution;

            // TODO: Figure out how this is going to work getting the
            // Body and PolygonShape into the IPhysicsBody
            // body.InjectData(physicsBody);
            // body.InjectData(polyShape);

            // Execute any deferred actions if any exist
            // body.AfterAddedToWorldActions.ExecuteAll();
        }

        /// <summary>
        /// Removes the body from the physics world.
        /// </summary>
        public void RemoveBody() => throw new NotImplementedException();

        /// <summary>
        /// Gets the from the world.
        /// </summary>
        /// <typeparam name="T">The type of body to return.</typeparam>
        /// <returns>The body object.</returns>
        public T GetBody<T>() => throw new NotImplementedException();
    }
}
