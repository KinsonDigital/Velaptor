using Raptor.Plugins;
using System;
using System.Diagnostics.CodeAnalysis;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Primitives;
using VelcroPhysics.Shared;

namespace Raptor.VelcroPhysicsImp
{
    /// <summary>
    /// Represents a world with simulated physics.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VelcroWorld : IPhysicsWorld
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="VelcroWorld"/>.
        /// </summary>
        /// <param name="gravityX">The gravity in the X plane.</param>
        /// <param name="gravityY">The gravity in the Y plane.</param>
        public VelcroWorld(float gravityX, float gravityY)
        {
            PhysicsWorld = new World(new Vector2(gravityX, gravityY));

            GravityX = gravityX;
            GravityY = gravityY;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the velcro physics world that is used internally.
        /// </summary>
        internal static World? PhysicsWorld { get; set; }

        /// <summary>
        /// Gets or sets the world's gravity in the X plane.
        /// </summary>
        public float GravityX { get; set; }

        /// <summary>
        /// Gets or sets the world's gravity in the Y plane.
        /// </summary>
        public float GravityY { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Updates the physics world.
        /// </summary>
        /// <param name="dt">The time that has passed for the current frame.</param>
        public void Update(float dt)
        {
            if (!(PhysicsWorld is null))
                PhysicsWorld.Step(dt);
        }


        /// <summary>
        /// Adds the given <paramref name="body"/> to the physics world.
        /// </summary>
        /// <typeparam name="T">The type of physics body to add.</typeparam>
        /// <param name="body">The body to add.</param>
        public void AddBody<T>(T body) where T : IPhysicsBody
        {
            var velVertices = new Vertices();

            //TODO: Figure out how to pull the PhysicsBodySettings
            //data out of the IPhysicsBody
            //var bodySettings = body.GetData<PhysicsBodySettings>(1);
            var bodySettings = new PhysicsBodySettings();

            for (int i = 0; i < bodySettings.XVertices.Count; i++)
            {
                velVertices.Add(new Vector2(bodySettings.XVertices[i], bodySettings.YVertices[i]).ToPhysics());
            }

            var physicsBody = BodyFactory.CreatePolygon(PhysicsWorld, velVertices, bodySettings.Density, new Vector2(bodySettings.XPosition, bodySettings.YPosition).ToPhysics(), bodySettings.Angle.ToRadians(), bodySettings.IsStatic ? BodyType.Static : BodyType.Dynamic);
            physicsBody.Friction = bodySettings.Friction;
            physicsBody.Restitution = bodySettings.Restitution;
            var polyShape = (PolygonShape)physicsBody.FixtureList[0].Shape;
            
            //TODO: Figure out how this is going to work getting the 
            //Body and PolygonShape into the IPhysicsBody
            //body.InjectData(physicsBody);
            //body.InjectData(polyShape);

            //Execute any deferred actions if any exist
            body.AfterAddedToWorldActions.ExecuteAll();
        }


        /// <summary>
        /// Removes the body from the physics world.
        /// </summary>
        public void RemoveBody() => throw new NotImplementedException();


        /// <summary>
        /// Gets the from the world.
        /// </summary>
        public T GetBody<T>() => throw new NotImplementedException();



        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Used to pass in options for the <see cref="GetData{T}(int)"/> implementation to process.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns></returns>
        //TODO: This mehod is planning on being removed
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        public static T GetData<T>(int option) where T : class => throw new NotImplementedException(option.ToString());


        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        //TODO: This mehod is planning on being removed
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InjectData<T>(T data) where T : class => throw new NotImplementedException();
        #endregion
    }
}
