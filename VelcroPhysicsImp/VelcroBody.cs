using Raptor;
using Raptor.Plugins;
using System;
using System.Linq;
using System.Collections.Generic;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace VelcroPhysicsImp
{
    /// <summary>
    /// Represents a body in a world that obeys physics.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VelcroBody : IPhysicsBody
    {
        #region Private Fields
        private readonly PhysicsBodySettings _tempSettings = new PhysicsBodySettings();
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="VelcroBody"/>.
        /// NOTE: Required for the plugin system to work. The IoC container must have a parameterless constructor.
        /// </summary>
        public VelcroBody() { }


        /// <summary>
        /// Creates a new instance of <see cref="VelcroBody"/>.
        /// </summary>
        /// <param name="xVertices">The X vertices of the body's shape.</param>
        /// <param name="yVertices">The Y vertices of the body's shape.</param>
        /// <param name="xPosition">The X location of the body.</param>
        /// <param name="yPosition">The Y location of the body.</param>
        /// <param name="angle">The angle of the body.</param>
        /// <param name="density">The density of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="restitution">The restituion(bounciness) of the body.</param>
        /// <param name="isStatic">True if the body is a static body.  False for dynamice.</param>
        public VelcroBody(float[] xVertices, float[] yVertices, float xPosition, float yPosition, float angle, float density = 1, float friction = 0.2f, float restitution = 0, bool isStatic = false)
        {
            if (xVertices.Length != yVertices.Length)
                throw new ArgumentOutOfRangeException($"The params {nameof(xVertices)} and {nameof(yVertices)} must have the same number of elements.");

            _tempSettings.XVertices = xVertices;
            _tempSettings.YVertices = yVertices;
            _tempSettings.XPosition = xPosition;
            _tempSettings.YPosition = yPosition;
            _tempSettings.Angle = angle;
            _tempSettings.Density = density;
            _tempSettings.Friction = friction;
            _tempSettings.Restitution = restitution;
            _tempSettings.IsStatic = isStatic;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the velcro body for internal use.
        /// </summary>
        internal Body PolygonBody { get; set; }

        /// <summary>
        /// Gets or sets the shape of the polygon body for internal use.
        /// </summary>
        internal PolygonShape PolygonShape { get; set; }

        /// <summary>
        /// The list of <see cref="DeferredActions"/> that will execute after the body has been added to a <see cref="World"/>.
        /// </summary>
        public DeferredActions AfterAddedToWorldActions { get; set; } = new DeferredActions();

        /// <summary>
        /// The X vertices of the body's shape.
        /// </summary>
        public float[] XVertices
        {
            get
            {
                var result = new List<float>();
                var positionX = PolygonBody.Position.X;//In physics units

                if (PolygonShape == null)
                {
                    result.AddRange(_tempSettings.XVertices);
                }
                else
                {
                    //This gets the vertices as world vertices
                    var xVertices = (from v in PolygonShape.Vertices
                                     select v.X + positionX).ToArray();

                    result.AddRange(xVertices.ToPixels());
                }


                return result.ToArray();
            }
            set { }
        }

        /// <summary>
        /// The X vertices of the body's shape.
        /// </summary>
        public float[] YVertices
        {
            get
            {
                var result = new List<float>();
                var positionY = PolygonBody.Position.Y;//In physics units

                if (PolygonShape == null)
                {
                    result.AddRange(_tempSettings.YVertices);
                }
                else
                {
                    //This gets the vertices as world vertices
                    var yVertices = (from v in PolygonShape.Vertices
                                     select v.Y + positionY).ToArray();

                    result.AddRange(yVertices.ToPixels());
                }


                return result.ToArray();
            }
            set { }
        }

        /// <summary>
        /// The X coordinate of the body's location.
        /// </summary>
        public float X
        {
            get => PolygonBody == null ? _tempSettings.XPosition : PolygonBody.Position.X.ToPixels();
            set
            {
                if (PolygonBody == null)
                    throw new Exception("Body must be added to a world first");

                PolygonBody.Position = new Vector2(value.ToPhysics(), PolygonBody.Position.Y);
            }
        }

        /// <summary>
        /// The Y coordinate of the body's location.
        /// </summary>
        public float Y
        {
            get => PolygonBody == null ? _tempSettings.YPosition : PolygonBody.Position.Y.ToPixels();
            set
            {
                if (PolygonBody == null)
                    throw new Exception("Body must be added to a world first");

                PolygonBody.Position = new Vector2(PolygonBody.Position.X, value.ToPhysics());
            }
        }

        /// <summary>
        /// Gets or sets the angle of the body in degrees.
        /// </summary>
        public float Angle
        {
            get => PolygonBody == null ? _tempSettings.Angle : PolygonBody.Rotation.ToDegrees();
            set
            {
                if (PolygonBody == null)
                {
                    AfterAddedToWorldActions.Add(() =>
                    {
                        PolygonBody.Rotation = value.ToRadians();
                    });
                }
                else
                {
                    PolygonBody.Rotation = value.ToRadians();
                }

                _tempSettings.Angle = value;//Degrees
            }
        }

        /// <summary>
        /// Gets or sets the density of the body.
        /// </summary>
        public float Density
        {
            get => PolygonShape == null ? _tempSettings.Density : PolygonShape.Density;
            set
            {
                _tempSettings.Density = value;
                //TODO: We might be able to change the density after its been added, look into this.
                throw new Exception("Cannot set the density after the body has been added to the world");
            }
        }

        /// <summary>
        /// Gets or sets the friction of the body.
        /// </summary>
        public float Friction
        {
            get => _tempSettings.Friction;
            set => PolygonBody.Friction = value;
        }

        /// <summary>
        /// Gets or sets the restitution(bounciness) of the body.
        /// </summary>
        public float Restitution
        {
            get => _tempSettings.Restitution;
            set
            {
                if (PolygonBody == null)
                {
                    AfterAddedToWorldActions.Add(() =>
                    {
                        PolygonBody.Restitution = value;
                    });
                }
                else
                {
                    PolygonBody.Restitution = value;
                }

                _tempSettings.Restitution = value;
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity in the X plane.
        /// </summary>
        public float LinearVelocityX
        {
            get => PolygonBody.LinearVelocity.X.ToPixels();
            set
            {
                PolygonBody.LinearVelocity = new Vector2(value.ToPhysics(), PolygonBody.LinearVelocity.Y);
            }
        }

        /// <summary>
        /// Gets or sets the linear velocity in the Y plane.
        /// </summary>
        public float LinearVelocityY
        {
            get => PolygonBody.LinearVelocity.Y.ToPixels();
            set
            {
                PolygonBody.LinearVelocity = new Vector2(PolygonBody.LinearVelocity.X, value.ToPhysics());
            }
        }

        /// <summary>
        /// Gets or sets the linear deceleration.
        /// </summary>
        public float LinearDeceleration
        {
            get => PolygonBody.LinearDamping.ToPixels();
            set
            {
                if (PolygonBody == null)
                {
                    AfterAddedToWorldActions.Add(() =>
                    {
                        PolygonBody.LinearDamping = value.ToPhysics();
                    });
                }
                else
                {
                    PolygonBody.LinearDamping = value.ToPhysics();
                }
            }
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public float AngularVelocity
        {
            get => PolygonBody.AngularVelocity.ToPixels();
            set
            {
                PolygonBody.AngularVelocity = value.ToPhysics();
            }
        }

        /// <summary>
        /// Gets or sets the angular desceleration.
        /// </summary>
        public float AngularDeceleration
        {
            get => PolygonBody.AngularDamping.ToPixels();
            set
            {
                if (PolygonBody == null)
                {
                    AfterAddedToWorldActions.Add(() =>
                    {
                        PolygonBody.AngularDamping = value.ToPhysics();
                    });
                }
                else
                {
                    PolygonBody.AngularDamping = value.ToPhysics();
                }
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Applies a linear impulse to the body using the
        /// the given <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The X coordinate of the location to apply the impulse.</param>
        /// <param name="y">The Y coordinate of the location to apply the impulse.</param>
        public void ApplyLinearImpulse(float x, float y) => PolygonBody.ApplyLinearImpulse(new Vector2(x.ToPhysics(), y.ToPhysics()));


        /// <summary>
        /// Applies an angular impulse to the body using
        /// the given <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The X coordinate of the location to apply the impulse.</param>
        /// <param name="y">The Y coordinate of the location to apply the impulse.</param>
        public void ApplyAngularImpulse(float value) => PolygonBody.ApplyAngularImpulse(value.ToPhysics());


        /// <summary>
        /// Applies a force to the body in the X and Y planes at the given world location.
        /// </summary>
        /// <param name="forceX">The force to apply in the X direction.</param>
        /// <param name="forceY">The force to apply in the Y direction.</param>
        /// <param name="worldLocationX">The location in the world of where to apply this force.</param>
        /// <param name="worldLocationY">The location in the world of where to apply this force.</param>
        public void ApplyForce(float forceX, float forceY, float worldLocationX, float worldLocationY) =>
            PolygonBody.ApplyForce(new Vector2(forceX.ToPhysics(), forceY.ToPhysics()), new Vector2(worldLocationX.ToPhysics(), worldLocationY.ToPhysics()));


        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        /// <exception cref="Exception">Thrown if the '<paramref name="data"/>' parameter is not of type <see cref="Body"/> or <see cref="PolygonShape"/>.</exception>
        public void InjectData<T>(T data) where T : class
        {
            if (data.GetType() == typeof(Body))
            {
                PolygonBody = data as Body;
            }
            else if (data.GetType() == typeof(PolygonShape))
            {
                PolygonShape = data as PolygonShape;
            }
            else
            {
                throw new Exception($"Data getting injected into {nameof(VelcroBody)} is not of type {nameof(Body)} or {nameof(PolygonShape)}.  Incorrect type is '{data.GetType().ToString()}'");
            }
        }


        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Used to pass in options for the <see cref="GetData{T}(int)"/> implementation to process.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <exception cref="Exception">Thrown if the <paramref name="option"/> value is not the value of
        /// type '1' for the type <see cref="PhysicsBodySettings"/>.</exception>
        public T GetData<T>(int option) where T : class
        {
            if (option == 1)
                return _tempSettings as T;


            throw new Exception($"Do not recognize the option '{option}'");
        }
        #endregion
    }
}
