// <copyright file="GLAssertHelpers.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Helpers
{
    using System;
    using System.Drawing;
    using Moq;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using Raptor.NativeInterop;
    using Raptor.OpenGL;
    using Xunit.Sdk;

    /// <summary>
    /// Provides assertion extensions for OpenGL functions.
    /// </summary>
    //[ExcludeFromCodeCoverage]
    internal static class GLAssertHelpers
    {
        // TODO: Look into creating a custom assertion that checks how many times a method has
        // been invoked against a single argument value by giving the argument type and value
        // expected number of times of execution.  This should be a generic method

        /// <summary>
        /// Verifies that the transform update been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="transform">The updated transform data to send to the GPU.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyTransformIsUpdated(this Mock<IGLInvoker> mock, Matrix4 transform, Times times)
        {
            mock.Verify(m => m.UniformMatrix4(It.IsAny<uint>(), true, ref transform), times);
        }

        /// <summary>
        /// Verifies that a texture with the given <paramref name="textureId"/> is being bound
        /// the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="textureId">The ID of the texture that is being bound.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void Verify2DTextureIsBound(this Mock<IGLInvoker> mock, uint textureId, Times times)
        {
            var totalExecutions = 0;
            var invalidId = 0u;

            Func<uint, bool> isCorrectId = (uint arg) =>
            {
                if (arg == textureId)
                {
                    totalExecutions++;
                }
                else
                {
                    invalidId = arg;
                }

                return arg == textureId;
            };

            try
            {
                mock.Verify(m => m.BindTexture(TextureTarget.Texture2D, It.Is<uint>(arg => isCorrectId(arg))), times);

                if (textureId != invalidId)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGLInvoker.BindTexture)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (textureId != invalidId)
                {
                    assertMsg += $"\nThe texture id was '{invalidId}' but was suppose to be '{textureId}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies if the rendering was performed as a primivite triangle type.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyDrawingWithTriangles(this Mock<IGLInvoker> mock, Times times)
        {
            var totalExecutions = 0;

            Func<PrimitiveType, bool> isTriangle = (PrimitiveType arg) =>
            {
                if (arg == PrimitiveType.Triangles)
                {
                    totalExecutions++;
                }

                return arg == PrimitiveType.Triangles;
            };

            try
            {
                mock.Verify(m => m.DrawElements(It.Is<PrimitiveType>((arg) => isTriangle(arg)),
                                                It.IsAny<uint>(),
                                                It.IsAny<DrawElementsType>(),
                                                It.IsAny<IntPtr>()),
                    times);
            }
            catch (MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = nameof(IGLInvoker.DrawElements);

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        public static void VerifyTotalBatchItemsDrawn(this Mock<IGLInvoker> mock, uint totalBatchItems, Times times)
        {
            /*NOTES:
             * The reason for multiplying the total batch items by six is because we are rendering the elements
             * as triangles and we are trying to render a quad which has 2 triangles.  Each triangle has 3 vertices
             * so 3 verticies multiplied by 2 triangles for 1 quad.
             *
             * quad * verticies per quad = total vertices per quad
             *      Example: 1 * 6 = 6
             *      Example: 2 * 6 = 12
             */
            mock.Verify(m => m.DrawElements(PrimitiveType.Triangles,
                                            6 * totalBatchItems,
                                            DrawElementsType.UnsignedInt,
                                            IntPtr.Zero),
                times);
        }

        /// <summary>
        /// Verifies that a quad has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdated(this Mock<IGPUBuffer> mock, Times times)
        {
            var totalExecutions = 0;

            Func<uint, bool> isCorrectId = (arg) =>
            {
                totalExecutions++;

                return true;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.Is<uint>(arg => isCorrectId(arg)),
                              It.IsAny<Rectangle>(),
                              It.IsAny<int>(),
                              It.IsAny<int>(),
                              It.IsAny<Color>()),
                              times);
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies that a quad with given <paramref name="quadId"/> has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="quadId">The ID of the quad being updated.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdatedWithCorrectQuadID(this Mock<IGPUBuffer> mock, uint quadId, Times times)
        {
            var totalExecutions = 0;
            var invalidId = 0u;

            Func<uint, bool> isCorrectId = (arg) =>
            {
                if (arg == quadId)
                {
                    totalExecutions++;
                }
                else
                {
                    invalidId = arg;
                }

                return arg == quadId;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.Is<uint>(arg => isCorrectId(arg)),
                              It.IsAny<Rectangle>(),
                              It.IsAny<int>(),
                              It.IsAny<int>(),
                              It.IsAny<Color>()),
                              times);

                if (quadId != invalidId)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (quadId != invalidId)
                {
                    assertMsg += $"\nThe quad id was '{invalidId}' but was suppose to be '{quadId}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies that a quand with given <paramref name="srcRect"/> has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="srcRect">The source rectangle.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdatedWithCorrectSrcRectangle(this Mock<IGPUBuffer> mock, Rectangle srcRect, Times times)
        {
            var totalExecutions = 0;
            Rectangle invalidRect = default;

            Func<Rectangle, bool> isCorrectRect = (arg) =>
            {
                if (arg == srcRect)
                {
                    totalExecutions++;
                    invalidRect = srcRect;
                }
                else
                {
                    invalidRect = arg;
                }

                return arg == srcRect;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.IsAny<uint>(),
                                              It.Is<Rectangle>(arg => isCorrectRect(arg)),
                                              It.IsAny<int>(),
                                              It.IsAny<int>(),
                                              It.IsAny<Color>()),
                                              times);

                if (srcRect != invalidRect)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (srcRect != invalidRect)
                {
                    assertMsg += $"\nThe source rectangle was '{invalidRect}' but was suppose to be '{srcRect}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="textureWidth"/> has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="textureWidth">The width of the texture.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdatedWithCorrectTextureWidth(this Mock<IGPUBuffer> mock, int textureWidth, Times times)
        {
            var totalExecutions = 0;
            var invalidWidth = 0;

            Func<int, bool> isCorrectWidth = (arg) =>
            {
                if (arg == textureWidth)
                {
                    totalExecutions++;
                    invalidWidth = textureWidth;
                }
                else
                {
                    invalidWidth = arg;
                }

                return arg == textureWidth;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.IsAny<uint>(),
                                              It.IsAny<Rectangle>(),
                                              It.Is<int>(arg => isCorrectWidth(arg)),
                                              It.IsAny<int>(),
                                              It.IsAny<Color>()),
                                              times);

                if (textureWidth != invalidWidth)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (textureWidth != invalidWidth)
                {
                    assertMsg += $"\nThe texture width was '{invalidWidth}' but was suppose to be '{textureWidth}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="textureHeight"/> has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="textureHeight">The width of the texture.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdatedWithCorrectTextureHeight(this Mock<IGPUBuffer> mock, int textureHeight, Times times)
        {
            var totalExecutions = 0;
            var invalidHeight = 0;

            Func<int, bool> isCorrectHeight = (arg) =>
            {
                if (arg == textureHeight)
                {
                    totalExecutions++;
                    invalidHeight = textureHeight;
                }
                else
                {
                    invalidHeight = arg;
                }

                return arg == textureHeight;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.IsAny<uint>(),
                                              It.IsAny<Rectangle>(),
                                              It.IsAny<int>(),
                                              It.Is<int>(arg => isCorrectHeight(arg)),
                                              It.IsAny<Color>()),
                                              times);

                if (textureHeight != invalidHeight)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (textureHeight != invalidHeight)
                {
                    assertMsg += $"\nThe texture height was '{invalidHeight}' but was suppose to be '{textureHeight}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="color"/> has been executed the given amount of <paramref name="times"/>.
        /// </summary>
        /// <param name="mock">The mock being verified.</param>
        /// <param name="color">The color of the quad.</param>
        /// <param name="times">The total number of times to expected the invocation to occur.</param>
        public static void VerifyQuadIsUpdatedWithCorrectColor(this Mock<IGPUBuffer> mock, Color color, Times times)
        {
            var totalExecutions = 0;
            Color invalidColor = default;

            Func<Color, bool> isCorrectColor = (arg) =>
            {
                if (arg == color)
                {
                    totalExecutions++;
                    invalidColor = color;
                }
                else
                {
                    invalidColor = arg;
                }

                return arg == color;
            };

            try
            {
                mock.Verify(m => m.UpdateQuad(It.IsAny<uint>(),
                                              It.IsAny<Rectangle>(),
                                              It.IsAny<int>(),
                                              It.IsAny<int>(),
                                              It.Is<Color>(arg => isCorrectColor(arg))),
                                              times);

                if (color != invalidColor)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) when (ex is Exception || ex is MockException)
            {
                times.Deconstruct(out var from, out var to);
                var assertMsg = string.Empty;

                var glFunctionName = $"OpenGL function '{nameof(IGPUBuffer.UpdateQuad)}'";

                assertMsg = BuildAssertionMessage(glFunctionName, from, to, totalExecutions);

                if (color != invalidColor)
                {
                    assertMsg += $"\nThe color was '{invalidColor}' but was suppose to be '{color}'.\n";
                }

                throw new AssertActualExpectedException(
                    from,
                    totalExecutions,
                    assertMsg,
                    expectedTitle: "Expected Executions",
                    actualTitle: "  Actual Executions");
            }
        }

        /// <summary>
        /// Builds an assertion message based on the given invocation stats.
        /// </summary>
        /// <param name="methodName">The name of the method that is being invocated.</param>
        /// <param name="from">The lower bound of executions that has occurred.</param>
        /// <param name="to">The upper bound of executions that has occurred.</param>
        /// <param name="totalExecutions">The total number of executions that has actually occurred.</param>
        /// <returns>The assertion message.</returns>
        private static string BuildAssertionMessage(string methodName, int from, int to, int totalExecutions)
        {
            if (from != int.MaxValue && to == int.MaxValue)
            {
                return $"{methodName} was executed {totalExecutions} times but must be at least {from} times.";
            }
            else if (from == 0 && to == 0)
            {
                return $"{methodName} should have never executed but was executed {totalExecutions} {(totalExecutions == 1 ? "time" : "times")}.";
            }
            else if (from == to)
            {
                return $"{methodName} was executed {totalExecutions} times but must be executed exactly {from} {(from == 1 ? "time" : "times")}.";
            }
            else if (from < to)
            {
                return $"{methodName} was executed {totalExecutions} times but was only a maximum amount of '{to}'.";
            }

            return string.Empty;
        }
    }
}
