// <copyright file="GPUBufferNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System;
    using Velaptor.OpenGL;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="GPUBufferNameAttribute"/>.
    /// </summary>
    public class GPUBufferNameAttributeTests
    {
        #region Constructor Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_WithNullOrEmptyNameParam_ThrowsException(string value)
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new GPUBufferNameAttribute(value);
            }, "The parameter must not be null or empty. (Parameter 'name')");
        }
        #endregion
    }
}
