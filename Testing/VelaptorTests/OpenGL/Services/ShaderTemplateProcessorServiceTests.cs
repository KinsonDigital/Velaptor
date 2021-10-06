// <copyright file="ShaderTemplateProcessorServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Services
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Velaptor.OpenGL.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ShaderTemplateProcessorService"/> class.
    /// </summary>
    public class ShaderTemplateProcessorServiceTests
    {
        /// <summary>
        /// Gets all of the test data for testing the <see cref="ShaderTemplateProcessorService.ProcessTemplateVariables"/> method.
        /// </summary>
        public static IEnumerable<object[]> InvalidVarNameChars
        {
            get
            {
                var result = new List<object[]>
                {
                    // Generate the data for starting with an underscore
                    new object[] { $"_MY_VAR", "Template variables cannot start with an '_' underscore character." },
                    // Generate the data for ending with an underscore
                    new object[] { $"MY_VAR_", "Template variables cannot end with an '_' underscore character." },
                };

                // Generate the lowercase numbers
                for (var i = 'a'; i <= 'z'; i++)
                {
                    result.Add(new object[] { $"MY_VAR{i}", "The lower case letters 'a' through 'z' are not a valid character in a template variable." });
                }

                // Generate the number digits
                for (var i = '0'; i <= '9'; i++)
                {
                    result.Add(new object[] { $"MY_VAR{i}", "The numbers '0' through '9' are not a valid character in a template variable." });
                }

                // Generate the first group of symbols
                for (var i = ' '; i <= '/'; i++)
                {
                    result.Add(new object[] { $"MY_VAR{i}", "All symbol characters besides the '_' underscore character are invalid." });
                }

                // Generate the second group of symbols
                for (var i = ':'; i <= '@'; i++)
                {
                    result.Add(new object[] { $"MY_VAR{i}", "All symbol characters besides the '_' underscore character are invalid." });
                }

                // Generate the third group of symbols
                for (var i = '['; i <= '`'; i++)
                {
                    // If the character is an underscore, ignore it.  This is allowed.
                    if (i == '_')
                    {
                        continue;
                    }

                    result.Add(new object[] { $"MY_VAR{i}", "All symbol characters besides the '_' underscore character are invalid." });
                }

                // Generate the fourth group of symbols
                for (var i = '{'; i <= '~'; i++)
                {
                    result.Add(new object[] { $"MY_VAR{i}", "All symbol characters besides the '_' underscore character are invalid." });
                }

                return result;
            }
        }

        /// <summary>
        /// Gets test data for the <see cref="ProcessTemplateVariables_WhenInvoked_ReturnsCorrectResult"/> test.
        /// </summary>
        public static IEnumerable<object[]> TemplateData =>
            new List<object[]>
            {
                new object[] // Single variable no spaces
                {
                    "${{NUM_ONE}} + 2 = 3",
                    new[] { ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
                new object[] // Single variable space on left side
                {
                    "${{ NUM_ONE}} + 2 = 3",
                    new[] { ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
                new object[] // Single variable space on right side
                {
                    "${{NUM_ONE }} + 2 = 3",
                    new[] { ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
                new object[] // Single variable space on both sides
                {
                    "${{ NUM_ONE }} + 2 = 3",
                    new[] { ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
                new object[] // Single variable with lots of space on both sides
                {
                    "${{              NUM_ONE                                    }} + 2 = 3",
                    new[] { ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
                new object[] // More than 1 variable
                {
                    "${{NUM_ONE}} + ${{NUM_TWO}} = 3",
                    new[] { ("NUM_ONE", "1"), ("NUM_TWO", "2") },
                    "1 + 2 = 3",
                },
                new object[] // More than 1 variable in variable list backwards
                {
                    "${{NUM_ONE}} + ${{NUM_TWO}} = 3",
                    new[] { ("NUM_TWO", "2"), ("NUM_ONE", "1") },
                    "1 + 2 = 3",
                },
            };

        #region Method Tests
        [Theory]
        [MemberData(nameof(InvalidVarNameChars))]
        public void ProcessTemplateVariables_WithInvalidName_ThrowsException(string varName, string expectedExceptionMsg)
        {
            // Arrange
            var service = CreateTemplateService();

            IEnumerable<(string, string)> vars = new[]
            {
                (varName, "test-value"),
            };
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                service.ProcessTemplateVariables(It.IsAny<string>(), vars);
            }, expectedExceptionMsg);
        }

        [Theory]
        [MemberData(nameof(TemplateData))]
        public void ProcessTemplateVariables_WhenInvoked_ReturnsCorrectResult(
            string content,
            (string, string)[] variables,
            string expected)
        {
            // Arrange
            var service = CreateTemplateService();

            // Act
            var actual = service.ProcessTemplateVariables(content, variables);

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ShaderTemplateProcessorService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The object instance to test.</returns>
        private ShaderTemplateProcessorService CreateTemplateService() => new ();
    }
}
