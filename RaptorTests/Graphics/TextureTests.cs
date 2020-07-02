// <copyright file="TextureTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Graphics
{
    using System.Collections.Generic;
    using System.Drawing;
    using Moq;
    using OpenToolkit.Graphics.OpenGL4;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Texture"/> class.
    /// </summary>
    public class TextureTests
    {
        private readonly Mock<IGLInvoker> mockGL;
        private readonly byte[] pixelData;
        private readonly uint textureID = 1234;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureTests"/> class.
        /// </summary>
        public TextureTests()
        {
            var byteData = new List<byte>();

            // Rows
            for (var row = 0; row < 3; row++)
            {
                // Columns
                for (var col = 0; col < 2; col++)
                {
                    // If the first row
                    switch (row)
                    {
                        case 0: // Row 1
                            byteData.AddRange(ToByteArray(Color.Red));
                            break;
                        case 1: // Row 2
                            byteData.AddRange(ToByteArray(Color.Green));
                            break;
                        case 2: // Row 3
                            byteData.AddRange(ToByteArray(Color.Blue));
                            break;
                    }
                }
            }

            this.pixelData = byteData.ToArray();

            this.mockGL = new Mock<IGLInvoker>();
            this.mockGL.Setup(m => m.GenTexture()).Returns(this.textureID);
        }

        [Fact]
        public void Ctor_WhenInvoked_UploadsTextureDataToGPU()
        {
            // Act
            var texture = new Texture(this.mockGL.Object, "test-texture.png", this.pixelData, 2, 3);

            // Assert
            this.mockGL.Verify(m => m.ObjectLabel(ObjectLabelIdentifier.Texture, this.textureID, -1, "test-texture.png"), Times.Once());
            this.mockGL.Verify(m => m.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear), Times.Once());

            this.mockGL.Verify(m => m.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMinFilter.Linear), Times.Once());

            this.mockGL.Verify(m => m.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int)TextureWrapMode.ClampToEdge), Times.Once());

            this.mockGL.Verify(m => m.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int)TextureWrapMode.ClampToEdge), Times.Once());

            this.mockGL.Verify(m => m.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                2,
                3,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                this.pixelData));
        }

        [Fact]
        public void Dispose_WhenUnmanagedResourcesIsNotDisposed_DisposesOfUnmanagedResources()
        {
            // Arrange
            var texture = new Texture(this.mockGL.Object, "test-texture", this.pixelData, 2, 3);

            // Act
            texture.Dispose();
            texture.Dispose();

            // Assert
            this.mockGL.Verify(m => m.DeleteTexture(this.textureID), Times.Once());
        }

        private static byte[] ToByteArray(Color clr) => new[] { clr.A, clr.R, clr.G, clr.B };
    }
}
