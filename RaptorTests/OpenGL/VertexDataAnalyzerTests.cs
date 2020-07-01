using Raptor.OpenGL;
using RaptorTests.Helpers;
using System.Drawing;
using System;
using Xunit;
using OpenToolkit.Mathematics;
using OpenToolkit.Graphics.OpenGL4;

namespace RaptorTests.OpenGL
{
    public class VertexDataAnalyzerTests
    {
        [Fact]
        public void GetTotalBytesForStruct_WithNullParam_ThrowsException()
        {
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                VertexDataAnalyzer.GetTotalBytesForStruct(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }, "The argument must not be null (Parameter 'type')");
        }

        [Fact]
        public void GetTotalBytesForStruct_WhenInvoked_ReturnsCorrectResult()
        {
            //Act
            var actual = VertexDataAnalyzer.GetTotalBytesForStruct(typeof(VertexData));

            //Assert
            Assert.Equal(40u, actual);
        }

        [Fact]
        public void GetPrimitiveByteSize_WhenUsingNonPrimitiveType_ThrowsException()
        {
            //Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                VertexDataAnalyzer.GetPrimitiveByteSize(typeof(Rectangle));
            }, "The param 'type' must be a primitive type.");
        }

        [Theory]
        [InlineData(typeof(byte), sizeof(byte))]
        [InlineData(typeof(sbyte), sizeof(sbyte))]
        [InlineData(typeof(short), sizeof(short))]
        [InlineData(typeof(ushort), sizeof(ushort))]
        [InlineData(typeof(int), sizeof(int))]
        [InlineData(typeof(uint), sizeof(uint))]
        [InlineData(typeof(float), sizeof(float))]
        [InlineData(typeof(long), sizeof(long))]
        [InlineData(typeof(ulong), sizeof(ulong))]
        [InlineData(typeof(double), sizeof(double))]
        public void GePrimitiveByteSize_WhenInvoked_ReturnsCorrectValue(Type type, uint expected)
        {
            //Act
            var actual = VertexDataAnalyzer.GetPrimitiveByteSize(type);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(byte), 1)]
        [InlineData(typeof(sbyte), 1)]
        [InlineData(typeof(short), 1)]
        [InlineData(typeof(ushort), 1)]
        [InlineData(typeof(int), 1)]
        [InlineData(typeof(uint), 1)]
        [InlineData(typeof(float), 1)]
        [InlineData(typeof(long), 1)]
        [InlineData(typeof(ulong), 1)]
        [InlineData(typeof(double), 1)]
        [InlineData(typeof(Vector2), 2)]
        [InlineData(typeof(Vector2i), 2)]
        [InlineData(typeof(Vector3), 3)]
        [InlineData(typeof(Vector3i), 3)]
        [InlineData(typeof(Vector2d), 2)]
        [InlineData(typeof(Vector4), 4)]
        [InlineData(typeof(Vector4i), 4)]
        [InlineData(typeof(Vector3d), 3)]
        [InlineData(typeof(Vector4d), 3)]
        [InlineData(typeof(Matrix4), 16)]
        public void TotalDataElementsForType_WhenInvoked_ReturnsCorrectResult(Type type, uint expected)
        {
            //Act
            var actual = VertexDataAnalyzer.TotalDataElementsForType(type);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(Vector2))]
        [InlineData(typeof(Vector3))]
        [InlineData(typeof(Vector4))]
        public void GetVertexPointerType_WhenInvoked_ReturnsCorrectResult(Type type)
        {
            //Act
            var actual = VertexDataAnalyzer.GetVertexPointerType(typeof(float));

            //Assert
            Assert.Equal(VertexAttribPointerType.Float, actual);
        }
    }
}
