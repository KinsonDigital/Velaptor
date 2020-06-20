// <copyright file="PhysicsBody.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Physics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    /// Represents a body in a world that obeys physics.
    /// </summary>
    public class PhysicsBody
    {
        private object[]? ctorParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicsBody"/> class.
        /// </summary>
        /// <param name="vertices">The vertices that give the body shape.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="angle">The angle in degress that the body is rotated.</param>
        /// <param name="density">The density of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="restitution">The restitution of the body.</param>
        /// <param name="isStatic">True if the body is a static body.</param>
        [ExcludeFromCodeCoverage]
        public PhysicsBody(Vector2[] vertices, Vector2 position, float angle = 0, float density = 1, float friction = 0.2f, float restitution = 0, bool isStatic = false) =>
            Setup(vertices, position, angle, density, friction, restitution, isStatic);

        /// <summary>
        /// Gets or sets the vertices that maake up the shape of the body.
        /// </summary>
        public ReadOnlyCollection<Vector2> Vertices { get; set; } = new ReadOnlyCollection<Vector2>(Array.Empty<Vector2>());

        // {
        //    get
        //    {
        //        var result = new List<Vector2>();

        // if (InternalPhysicsBody is null || InternalPhysicsBody.XVertices == null || InternalPhysicsBody.YVertices == null)
        //            return new ReadOnlyCollection<Vector2>(Array.Empty<Vector2>());

        // for (int i = 0; i < InternalPhysicsBody.XVertices.Count; i++)
        //        {
        //            result.Add(new Vector2(InternalPhysicsBody.XVertices[i], InternalPhysicsBody.YVertices[i]));
        //        }

        // return new ReadOnlyCollection<Vector2>(result);
        //    }
        // }

        /// <summary>
        /// Gets or sets the X coordinate of the body's location.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the body's location.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the angle of the body in degrees.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Gets or sets the density of the body.
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Gets or sets the friction of the body.
        /// </summary>
        public float Friction { get; set; }

        /// <summary>
        /// Gets or sets the restitution(bounciness) of the body.
        /// </summary>
        public float Restitution { get; set; }

        /// <summary>
        /// Gets or sets the linear velocity in the Y plane.
        /// </summary>
        public Vector2 LinearVelocity { get; set; }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// Gets or sets the linear deceleration.
        /// </summary>
        public float LinearDeceleration { get; set; }

        /// <summary>
        /// Gets or sets the angular desceleration.
        /// </summary>
        public float AngularDeceleration { get; set; }

        /// <summary>
        /// Sets up the internal <see cref="IPhysicsBody"/>.
        /// </summary>
        /// <param name="vertices">The vertices that give the body shape.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="angle">The angle in degress that the body is rotated.</param>
        /// <param name="density">The density of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="restitution">The restitution of the body.</param>
        /// <param name="isStatic">True if the body is a static body.</param>
        private void Setup(Vector2[] vertices, Vector2 position, float angle, float density, float friction, float restitution, bool isStatic)
        {
            this.ctorParams = new object[9];

            this.ctorParams[0] = (from v in vertices select v.X).ToArray();
            this.ctorParams[1] = (from v in vertices select v.Y).ToArray();
            this.ctorParams[2] = position.X;
            this.ctorParams[3] = position.Y;
            this.ctorParams[4] = angle;
            this.ctorParams[5] = density;
            this.ctorParams[6] = friction;
            this.ctorParams[7] = restitution;
            this.ctorParams[8] = isStatic;
        }
    }
}
