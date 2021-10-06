// <copyright file="ExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Services
{
    using Velaptor.OpenGL.Services;
    using Xunit;

    public class ExtensionMethodsTests
    {

        [Theory]
        [InlineData("${{TEST_VAR}}", "}}", ' ', "${{TEST_VAR}}")]
        [InlineData("${{TEST_VAR }}", "}}", ' ', "${{TEST_VAR}}")]
        [InlineData("${{TEST_VAR    }}", "}}", ' ', "${{TEST_VAR}}")]
        [InlineData("${{TEST_VAR~}}", "}}", '~', "${{TEST_VAR}}")]
        [InlineData("${{TEST_VAR~~}}", "}}", '~', "${{TEST_VAR}}")]
        public void TrimLeftOf_WhenInvoked_ReturnsCorrectResult(
            string content,
            string value,
            char trimChar,
            string expected)
        {
            // Arrange & Act
            var actual = content.TrimLeftOf(value, trimChar);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("${{TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
        [InlineData("${{ TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
        [InlineData("${{    TEST_VAR}}", "${{", ' ', "${{TEST_VAR}}")]
        [InlineData("${{~TEST_VAR}}", "${{", '~', "${{TEST_VAR}}")]
        [InlineData("${{~~TEST_VAR}}", "${{", '~', "${{TEST_VAR}}")]
        public void TrimRightOf_WhenInvoked_ReturnsCorrectResult(
            string content,
            string value,
            char trimChar,
            string expected)
        {
            // Arrange & Act
            var actual = content.TrimRightOf(value, trimChar);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
