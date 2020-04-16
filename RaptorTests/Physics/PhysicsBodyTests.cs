#nullable disable
using System;
using Moq;
using Xunit;
using Raptor.Plugins;
using Raptor.Physics;
using System.Numerics;
using System.Collections.ObjectModel;

namespace RaptorTests.Physics
{
    /// <summary>
    /// Unit tests to test the <see cref="PhysicsBody"/> class.
    /// </summary>
    public class PhysicsBodyTests : IDisposable
    {
        #region Private Fields
        private Mock<IPhysicsBody> _mockPhysicsBody;
        #endregion


        #region Constructors
        public PhysicsBodyTests()
        {
            _mockPhysicsBody = new Mock<IPhysicsBody>();
            _mockPhysicsBody.SetupProperty(m => m.Angle);
            _mockPhysicsBody.SetupProperty(m => m.AngularVelocity);
            _mockPhysicsBody.SetupProperty(m => m.X);
            _mockPhysicsBody.SetupProperty(m => m.Y);
            _mockPhysicsBody.SetupProperty(m => m.Density);
            _mockPhysicsBody.SetupProperty(m => m.Friction);
            _mockPhysicsBody.SetupProperty(m => m.Restitution);
            _mockPhysicsBody.SetupProperty(m => m.LinearDeceleration);
            _mockPhysicsBody.SetupProperty(m => m.AngularDeceleration);
            _mockPhysicsBody.SetupProperty(m => m.LinearVelocityX);
            _mockPhysicsBody.SetupProperty(m => m.LinearVelocityY);
            _mockPhysicsBody.SetupProperty(m => m.XVertices);
            _mockPhysicsBody.SetupProperty(m => m.YVertices);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void Vertices_WhenGettingValue_GetsCorrectValue()
        {
            //Arrange
            var expectedVertices = new Vector2[]
            {
                new Vector2(11, 22),
                new Vector2(33, 44),
                new Vector2(55, 66),
            };
            _mockPhysicsBody.SetupGet(p => p.XVertices).Returns(new ReadOnlyCollection<float>(new float[] { 11, 33, 55 }));
            _mockPhysicsBody.SetupGet(p => p.YVertices).Returns(new ReadOnlyCollection<float>(new float[] { 22, 44, 66 }));

            //Act
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var actualVertices = body.Vertices;

            //Assert
            Assert.NotNull(actualVertices);
            Assert.Equal(expectedVertices, actualVertices);
        }


        [Fact]
        public void X_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 1234.4321f;

            //Act
            body.X = 1234.4321f;
            var actual = body.X;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Y_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 456.654f;

            //Act
            body.Y = 456.654f;
            var actual = body.Y;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Angle_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 90.12f;

            //Act
            body.Angle = 90.12f;
            var actual = body.Angle;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Density_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 14.7f;

            //Act
            body.Density = 14.7f;
            var actual = body.Density;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Friction_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 43.73f;

            //Act
            body.Friction = 43.73f;
            var actual = body.Friction;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Restitution_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 8.4f;

            //Act
            body.Restitution = 8.4f;
            var actual = body.Restitution;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void LinearDeceleration_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 3.22f;

            //Act
            body.LinearDeceleration = 3.22f;
            var actual = body.LinearDeceleration;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void AngularDeceleration_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 43.73f;

            //Act
            body.AngularDeceleration = 43.73f;
            var actual = body.AngularDeceleration;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void LinearVelocity_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = new Vector2(123.321f, 789.987f);

            //Act
            body.LinearVelocity = new Vector2(123.321f, 789.987f);
            var actual = body.LinearVelocity;

            //Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void AngularVelocity_WhenGettingAndSettingValue_GetsCorrectValue()
        {
            //Arrange
            var body = new PhysicsBody(_mockPhysicsBody.Object);
            var expected = 1973.3791f;

            //Act
            body.AngularVelocity = 1973.3791f;
            var actual = body.AngularVelocity;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Public Methods
        public void Dispose() => _mockPhysicsBody = null;
        #endregion
    }
}
