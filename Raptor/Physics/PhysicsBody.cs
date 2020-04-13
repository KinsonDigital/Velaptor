using Raptor.Plugins;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Raptor.Physics
{
    /// <summary>
    /// Represents a body in a world that obeys physics.
    /// </summary>
    public class PhysicsBody
    {
        #region Private Fields
        private IPhysicsBody _internalPhysicsBody;
        private object[] _ctorParams;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="PhysicsBody"/> and
        /// injects the given <paramref name="mockedBody"/> for mocking and unit testing.
        /// </summary>
        /// <param name="mockedBody">The mocked body to inject.</param>
        public PhysicsBody(IPhysicsBody mockedBody)
        {
            InternalPhysicsBody = mockedBody;
            Setup(new Vector2[1] { Vector2.Zero }, Vector2.Zero, 0, 1, 0.2f, 0, false);
        }


        /// <summary>
        /// Creates a new instance of a <see cref="PhysicsBody"/>.
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
        #endregion


        #region Props
        /// <summary>
        /// The internal physics engine body.
        /// </summary>
        public IPhysicsBody InternalPhysicsBody
        {
            [ExcludeFromCodeCoverage]
            get
            {
                //TODO: Figure out how to get the proper implementation inside of this class
                return _internalPhysicsBody;
            }
            private set => _internalPhysicsBody = value;
        }

        /// <summary>
        /// Gets or sets the vertices that maake up the shape of the body.
        /// </summary>
        public Vector2[] Vertices
        {
            get
            {
                var result = new List<Vector2>();

                if (InternalPhysicsBody.XVertices == null || InternalPhysicsBody.YVertices == null)
                    return null;

                for (int i = 0; i < InternalPhysicsBody.XVertices.Length; i++)
                {
                    result.Add(new Vector2(InternalPhysicsBody.XVertices[i], InternalPhysicsBody.YVertices[i]));
                }


                return result.ToArray();
            }
            set
            {
                InternalPhysicsBody.XVertices = (from v in value select v.X).ToArray();
                InternalPhysicsBody.YVertices = (from v in value select v.Y).ToArray();
            }
        }

        /// <summary>
        /// The X coordinate of the body's location.
        /// </summary>
        public float X
        {
            get => InternalPhysicsBody.X;
            set => InternalPhysicsBody.X = value;
        }

        /// <summary>
        /// The Y coordinate of the body's location.
        /// </summary>
        public float Y
        {
            get => InternalPhysicsBody.Y;
            set => InternalPhysicsBody.Y = value;
        }

        /// <summary>
        /// Gets or sets the angle of the body in degrees.
        /// </summary>
        public float Angle
        {
            get => InternalPhysicsBody.Angle;
            set => InternalPhysicsBody.Angle = value;
        }

        /// <summary>
        /// Gets or sets the density of the body.
        /// </summary>
        public float Density
        {
            get => InternalPhysicsBody.Density;
            set => InternalPhysicsBody.Density = value;
        }

        /// <summary>
        /// Gets or sets the friction of the body.
        /// </summary>
        public float Friction
        {
            get => InternalPhysicsBody.Friction;
            set => InternalPhysicsBody.Friction = value;
        }

        /// <summary>
        /// Gets or sets the restitution(bounciness) of the body.
        /// </summary>
        public float Restitution
        {
            get => InternalPhysicsBody.Restitution;
            set => InternalPhysicsBody.Restitution = value;
        }

        /// <summary>
        /// Gets or sets the linear velocity in the Y plane.
        /// </summary>
        public Vector2 LinearVelocity
        {
            get => new Vector2(InternalPhysicsBody.LinearVelocityX, InternalPhysicsBody.LinearVelocityY);
            set
            {
                InternalPhysicsBody.LinearVelocityX = value.X;
                InternalPhysicsBody.LinearVelocityY = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public float AngularVelocity
        {
            get => InternalPhysicsBody.AngularVelocity;
            set => InternalPhysicsBody.AngularVelocity = value;
        }

        /// <summary>
        /// Gets or sets the linear deceleration.
        /// </summary>
        public float LinearDeceleration
        {
            get => InternalPhysicsBody.LinearDeceleration;
            set => InternalPhysicsBody.LinearDeceleration = value;
        }

        /// <summary>
        /// Gets or sets the angular desceleration.
        /// </summary>
        public float AngularDeceleration
        {
            get => InternalPhysicsBody.AngularDeceleration;
            set => InternalPhysicsBody.AngularDeceleration = value;
        }
        #endregion


        #region Private Methods
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
            _ctorParams = new object[9];

            _ctorParams[0] = (from v in vertices select v.X).ToArray();
            _ctorParams[1] = (from v in vertices select v.Y).ToArray();
            _ctorParams[2] = position.X;
            _ctorParams[3] = position.Y;
            _ctorParams[4] = angle;
            _ctorParams[5] = density;
            _ctorParams[6] = friction;
            _ctorParams[7] = restitution;
            _ctorParams[8] = isStatic;
        }
        #endregion
    }
}
