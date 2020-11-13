// <copyright file="VelcroBody.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.VelcroPhysicsImp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using VelcroPhysics.Collision.Shapes;
    using VelcroPhysics.Dynamics;
    using VelcroPhysics.Primitives;

    /// <summary>
    /// Represents a body in a world that obeys physics.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VelcroBody
    {
        private readonly PhysicsBodySettings tempSettings = new PhysicsBodySettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="VelcroBody"/> class.
        /// </summary>
        /// <param name="xVertices">The X vertices's of the body's shape.</param>
        /// <param name="yVertices">The Y vertices's of the body's shape.</param>
        /// <param name="xPosition">The X location of the body.</param>
        /// <param name="yPosition">The Y location of the body.</param>
        /// <param name="angle">The angle of the body.</param>
        /// <param name="density">The density of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="restitution">The restitution(bounciness) of the body.</param>
        /// <param name="isStatic">True if the body is a static body.  False for dynamic.</param>
        public VelcroBody(float[] xVertices, float[] yVertices, float xPosition, float yPosition, float angle, float density = 1, float friction = 0.2f, float restitution = 0, bool isStatic = false)
        {
            if (xVertices is null)
                throw new ArgumentNullException(nameof(xVertices), "The X vertices's must not be null.");

            if (yVertices is null)
                throw new ArgumentNullException(nameof(yVertices), "The Y vertices's must not be null.");

            if (xVertices.Length != yVertices.Length)
                throw new ArgumentOutOfRangeException($"The parameters {nameof(xVertices)} and {nameof(yVertices)} must have the same number of elements.");

            this.tempSettings.SetXVertices(xVertices);
            this.tempSettings.SetYVertices(yVertices);
            this.tempSettings.XPosition = xPosition;
            this.tempSettings.YPosition = yPosition;
            this.tempSettings.Angle = angle;
            this.tempSettings.Density = density;
            this.tempSettings.Friction = friction;
            this.tempSettings.Restitution = restitution;
            this.tempSettings.IsStatic = isStatic;
        }

        /// <summary>
        /// Gets the list of <see cref="DeferredActionsCollection"/> that will execute after the body has been added to a <see cref="World"/>.
        /// </summary>
        //public DeferredActionsCollection AfterAddedToWorldActions { get; } = new DeferredActionsCollection();

        /// <summary>
        /// Gets the X vertices's of the body's shape.
        /// </summary>
        public ReadOnlyCollection<float> XVertices
        {
            get
            {
                var result = new List<float>();
                var positionX = PolygonBody is null ? 0 : PolygonBody.Position.X; // In physics units

                if (PolygonShape == null)
                {
                    result.AddRange(this.tempSettings.XVertices);
                }
                else
                {
                    // This gets the vertices's as world vertices's
                    var xVertices = (from v in PolygonShape.Vertices
                                     select v.X + positionX).ToArray();

                    result.AddRange(xVertices.ToScreenUnit());
                }

                return new ReadOnlyCollection<float>(result);
            }
        }

        /// <summary>
        /// Gets the X vertices's of the body's shape.
        /// </summary>
        public ReadOnlyCollection<float> YVertices
        {
            get
            {
                var result = new List<float>();
                var positionY = PolygonBody is null ? 0 : PolygonBody.Position.Y; // In physics units

                if (PolygonShape == null)
                {
                    result.AddRange(this.tempSettings.YVertices);
                }
                else
                {
                    // This gets the vertices as world vertices
                    var yVertices = (from v in PolygonShape.Vertices
                                     select v.Y + positionY).ToArray();

                    result.AddRange(yVertices.ToScreenUnit());
                }

                return new ReadOnlyCollection<float>(result);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the body's location.
        /// </summary>
        public float X
        {
            get => PolygonBody == null ? this.tempSettings.XPosition : PolygonBody.Position.X.ToScreenUnits();
            set
            {
                if (PolygonBody == null)
                    throw new Exception("Body must be added to a world first");

                PolygonBody.Position = new Vector2(value.ToPhysicsUnit(), PolygonBody.Position.Y);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the body's location.
        /// </summary>
        public float Y
        {
            get => PolygonBody == null ? this.tempSettings.YPosition : PolygonBody.Position.Y.ToScreenUnits();
            set
            {
                if (PolygonBody == null)
                    throw new Exception("Body must be added to a world first");

                PolygonBody.Position = new Vector2(PolygonBody.Position.X, value.ToPhysicsUnit());
            }
        }

        /// <summary>
        /// Gets or sets the angle of the body in degrees.
        /// </summary>
        public float Angle
        {
            get => PolygonBody == null ? this.tempSettings.Angle : PolygonBody.Rotation.ToDegrees();
            set
            {
                if (PolygonBody == null)
                {
                    //AfterAddedToWorldActions.Add(() =>
                    //{
                    //    if (!(PolygonBody is null))
                    //        PolygonBody.Rotation = value.ToRadians();
                    //});
                }
                else
                {
                    PolygonBody.Rotation = value.ToRadians();
                }

                this.tempSettings.Angle = value; // Degrees
            }
        }

        /// <summary>
        /// Gets or sets the density of the body.
        /// </summary>
        public float Density
        {
            get => PolygonShape == null ? this.tempSettings.Density : PolygonShape.Density;
            set
            {
                this.tempSettings.Density = value;

                // TODO: We might be able to change the density after its been added, look into this.
                throw new Exception("Cannot set the density after the body has been added to the world");
            }
        }

        /// <summary>
        /// Gets or sets the friction of the body.
        /// </summary>
        public float Friction
        {
            get => this.tempSettings.Friction;
            set
            {
                if (PolygonBody is null)
                    return;

                PolygonBody.Friction = value;
            }
        }

        /// <summary>
        /// Gets or sets the restitution(bounciness) of the body.
        /// </summary>
        public float Restitution
        {
            get => this.tempSettings.Restitution;
            set
            {
                if (PolygonBody == null)
                {
                    //AfterAddedToWorldActions.Add(() =>
                    //{
                    //    if (!(PolygonBody is null))
                    //        PolygonBody.Restitution = value;
                    //});
                }
                else
                {
                    PolygonBody.Restitution = value;
                }

                this.tempSettings.Restitution = value;
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity in the X plane.
        /// </summary>
        public float LinearVelocityX
        {
            get => PolygonBody is null ? 0 : PolygonBody.LinearVelocity.X.ToScreenUnits();
            set
            {
                if (PolygonBody is null)
                    return;

                PolygonBody.LinearVelocity = new Vector2(value.ToPhysicsUnit(), PolygonBody.LinearVelocity.Y);
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity in the Y plane.
        /// </summary>
        public float LinearVelocityY
        {
            get => PolygonBody is null ? 0 : PolygonBody.LinearVelocity.Y.ToScreenUnits();
            set
            {
                if (PolygonBody is null)
                    return;

                PolygonBody.LinearVelocity = new Vector2(PolygonBody.LinearVelocity.X, value.ToPhysicsUnit());
            }
        }

        /// <summary>
        /// Gets or sets the linear deceleration.
        /// </summary>
        public float LinearDeceleration
        {
            get => PolygonBody is null ? 0 : PolygonBody.LinearDamping.ToScreenUnits();
            set
            {
                if (PolygonBody == null)
                {
                    //AfterAddedToWorldActions.Add(() =>
                    //{
                    //    if (!(PolygonBody is null))
                    //        PolygonBody.LinearDamping = value.ToPhysicsUnit();
                    //});
                }
                else
                {
                    PolygonBody.LinearDamping = value.ToPhysicsUnit();
                }
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public float AngularVelocity
        {
            get => PolygonBody is null ? 0 : PolygonBody.AngularVelocity.ToScreenUnits();
            set
            {
                if (PolygonBody is null)
                    return;

                PolygonBody.AngularVelocity = value.ToPhysicsUnit();
            }
        }

        /// <summary>
        /// Gets or sets the angular deceleration.
        /// </summary>
        public float AngularDeceleration
        {
            get => PolygonBody is null ? 0 : PolygonBody.AngularDamping.ToScreenUnits();
            set
            {
                if (PolygonBody == null)
                {
                    //AfterAddedToWorldActions.Add(() =>
                    //{
                    //    if (!(PolygonBody is null))
                    //        PolygonBody.AngularDamping = value.ToPhysicsUnit();
                    //});
                }
                else
                {
                    PolygonBody.AngularDamping = value.ToPhysicsUnit();
                }
            }
        }

        /// <summary>
        /// Gets or sets the velcro body for internal use.
        /// </summary>
        internal Body? PolygonBody { get; set; }

        /// <summary>
        /// Gets or sets the shape of the polygon body for internal use.
        /// </summary>
        internal PolygonShape? PolygonShape { get; set; }

        /// <summary>
        /// Applies a linear impulse to the body using the
        /// the given <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The X coordinate of the location to apply the impulse.</param>
        /// <param name="y">The Y coordinate of the location to apply the impulse.</param>
        public void ApplyLinearImpulse(float x, float y)
        {
            if (PolygonBody is null)
                return;

            PolygonBody.ApplyLinearImpulse(new Vector2(x.ToPhysicsUnit(), y.ToPhysicsUnit()));
        }

        /// <summary>
        /// Applies an angular impulse to the body using
        /// the given <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="value">The impulse value to apply.</param>
        public void ApplyAngularImpulse(float value)
        {
            if (PolygonBody is null)
                return;

            PolygonBody.ApplyAngularImpulse(value.ToPhysicsUnit());
        }

        /// <summary>
        /// Applies a force to the body in the X and Y planes at the given world location.
        /// </summary>
        /// <param name="forceX">The force to apply in the X direction.</param>
        /// <param name="forceY">The force to apply in the Y direction.</param>
        /// <param name="worldLocationX">The X location in the world of where to apply this force.</param>
        /// <param name="worldLocationY">The Y location in the world of where to apply this force.</param>
        public void ApplyForce(float forceX, float forceY, float worldLocationX, float worldLocationY)
        {
            if (PolygonBody is null)
                return;

            PolygonBody.ApplyForce(new Vector2(forceX.ToPhysicsUnit(), forceY.ToPhysicsUnit()), new Vector2(worldLocationX.ToPhysicsUnit(), worldLocationY.ToPhysicsUnit()));
        }

        /// <summary>
        /// Sets all of the X vertices's to the given <paramref name="xVertices"/> param.
        /// </summary>
        /// <param name="xVertices">The list of X vertices's.</param>
        public void SetXVertices(float[] xVertices) => throw new NotImplementedException();

        /// <summary>
        /// Sets all of the Y vertices's to the given <paramref name="xVertices"/> param.
        /// </summary>
        /// <param name="yVertices">The list of Y vertices's.</param>
        public void SetYVertices(float[] yVertices) => throw new NotImplementedException();
    }
}
