// <copyright file="RectGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL.Exceptions;
using Velaptor.OpenGL.GPUData;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.OpenGL.Buffers;

/// <summary>
/// Updates data in the rectangle GPU buffer.
/// </summary>
[GPUBufferName("Rectangle")]
[BatchSize(IRenderer.BatchSize)]
internal sealed class RectGPUBuffer : GPUBufferBase<RectShape>
{
    private const string BufferNotInitMsg = "The rectangle buffer has not been initialized.";

    /// <summary>
    /// Initializes a new instance of the <see cref="RectGPUBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
    /// <param name="shutDownReactable">Receives a notification that the application is shutting down.</param>
    public RectGPUBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactable<GLInitData> glInitReactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, glInitReactable, shutDownReactable)
    {
    }

    /// <inheritdoc/>
    protected internal override void UploadVertexData(RectShape rectShape, uint batchIndex)
    {
        OpenGLService.BeginGroup($"Update Rectangle - BatchItem({batchIndex})");

        /*
         * Always have the smallest value between the width and height (divided by 2)
         * as the maximum limit of what the border thickness can be.
         * If the value was allowed to be larger than the smallest value between
         * the width and height, it would produce unintended rendering artifacts.
         */

        rectShape = ProcessBorderThicknessLimit(rectShape);
        rectShape = ProcessCornerRadiusLimits(rectShape);

        var data = RectGPUData.Empty();
        var halfWidth = rectShape.Width / 2f;
        var halfHeight = rectShape.Height / 2f;

        var left = rectShape.Position.X - halfWidth;
        var bottom = rectShape.Position.Y + halfHeight;
        var right = rectShape.Position.X + halfWidth;
        var top = rectShape.Position.Y - halfHeight;

        var topLeft = new Vector2(left, top);
        var bottomLeft = new Vector2(left, bottom);
        var bottomRight = new Vector2(right, bottom);
        var topRight = new Vector2(right, top);

        data = data.SetVertexPos(topLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height), VertexNumber.One);
        data = data.SetVertexPos(bottomLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height), VertexNumber.Two);
        data = data.SetVertexPos(topRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height), VertexNumber.Three);
        data = data.SetVertexPos(bottomRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height), VertexNumber.Four);

        data = data.SetRectangle(new Vector4(rectShape.Position.X, rectShape.Position.Y, rectShape.Width, rectShape.Height));

        data = ApplyColor(data, rectShape);

        data = data.SetIsFilled(rectShape.IsFilled);

        data = data.SetBorderThickness(rectShape.BorderThickness);
        data = data.SetTopLeftCornerRadius(rectShape.CornerRadius.TopLeft);
        data = data.SetBottomLeftCornerRadius(rectShape.CornerRadius.BottomLeft);
        data = data.SetBottomRightCornerRadius(rectShape.CornerRadius.BottomRight);
        data = data.SetTopRightCornerRadius(rectShape.CornerRadius.TopRight);

        var totalBytes = RectGPUData.GetTotalBytes();
        var rawData = data.ToArray();
        var offset = totalBytes * batchIndex;

        OpenGLService.BindVBO(VBO);

        GL.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, totalBytes, rawData);

        OpenGLService.UnbindVBO();

        OpenGLService.EndGroup();
    }

    /// <inheritdoc/>
    protected internal override void PrepareForUpload()
    {
        if (IsInitialized is false)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BindVAO(VAO);
    }

    /// <inheritdoc/>
    protected internal override float[] GenerateData()
    {
        var result = new List<RectGPUData>();

        for (var i = 0u; i < BatchSize; i++)
        {
            var vertexData = GenerateVertexData();
            var gpuData = new RectGPUData(vertexData[0], vertexData[1], vertexData[2], vertexData[3]);
            result.Add(gpuData);
        }

        return OpenGLExtensionMethods.ToArray(result);
    }

    /// <inheritdoc/>
    protected internal override void SetupVAO()
    {
        var stride = RectVertexData.GetTotalBytes();

        // Vertex Position
        const uint vertexPosSize = 2u * sizeof(float);
        GL.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        // Rectangle
        const uint rectOffset = vertexPosSize;
        const uint rectSize = 4u * sizeof(float);
        GL.VertexAttribPointer(1, 4, GLVertexAttribPointerType.Float, false, stride, rectOffset);
        GL.EnableVertexAttribArray(1);

        // Color
        const uint colorOffset = rectOffset + rectSize;
        const uint colorSize = 4u * sizeof(float);
        GL.VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, stride, colorOffset);
        GL.EnableVertexAttribArray(2);

        // IsFilled
        const uint isFilledOffset = colorOffset + colorSize;
        const uint isFilledSize = 1u * sizeof(float);
        GL.VertexAttribPointer(3, 1, GLVertexAttribPointerType.Float, false, stride, isFilledOffset);
        GL.EnableVertexAttribArray(3);

        // Border Thickness
        const uint borderThicknessOffset = isFilledOffset + isFilledSize;
        const uint borderThicknessSize = 1u * sizeof(float);
        GL.VertexAttribPointer(4, 1, GLVertexAttribPointerType.Float, false, stride, borderThicknessOffset);
        GL.EnableVertexAttribArray(4);

        // Top Left Corner Radius
        const uint topLeftRadiusOffset = borderThicknessOffset + borderThicknessSize;
        const uint topLeftRadiusSize = 1u * sizeof(float);
        GL.VertexAttribPointer(5, 1, GLVertexAttribPointerType.Float, false, stride, topLeftRadiusOffset);
        GL.EnableVertexAttribArray(5);

        // Bottom Left Corner Radius
        const uint bottomLeftRadiusOffset = topLeftRadiusOffset + topLeftRadiusSize;
        const uint bottomLeftRadiusSize = 1u * sizeof(float);
        GL.VertexAttribPointer(6, 1, GLVertexAttribPointerType.Float, false, stride, bottomLeftRadiusOffset);
        GL.EnableVertexAttribArray(6);

        // Bottom Right Corner Radius
        const uint bottomRightRadiusOffset = bottomLeftRadiusOffset + bottomLeftRadiusSize;
        const uint bottomRightRadiusSize = 1u * sizeof(float);
        GL.VertexAttribPointer(7, 1, GLVertexAttribPointerType.Float, false, stride, bottomRightRadiusOffset);
        GL.EnableVertexAttribArray(7);

        // Top Right Corner Radius
        const uint topRightRadiusOffset = bottomRightRadiusOffset + bottomRightRadiusSize;
        GL.VertexAttribPointer(8, 1, GLVertexAttribPointerType.Float, false, stride, topRightRadiusOffset);
        GL.EnableVertexAttribArray(8);
    }

    /// <inheritdoc/>
    protected internal override uint[] GenerateIndices()
    {
        var result = new List<uint>();

        for (var i = 0u; i < BatchSize; i++)
        {
            var maxIndex = result.Count <= 0 ? 0 : result.Max() + 1;

            result.AddRange(new[]
            {
                maxIndex,
                maxIndex + 1u,
                maxIndex + 2u,
                maxIndex + 2u,
                maxIndex + 1u,
                maxIndex + 3u,
            });
        }

        return result.ToArray();
    }

    /// <summary>
    /// Generates default <see cref="RectVertexData"/> for all 4 vertices that make up a rectangle.
    /// </summary>
    /// <returns>The four vertex data items.</returns>
    private static RectVertexData[] GenerateVertexData()
    {
        var vertex1 = new RectVertexData(
            new Vector2(-1.0f, 1.0f),
            Vector4.Zero,
            Color.Empty,
            false,
            1f,
            0f,
            0f,
            0f,
            0f);

        var vertex2 = new RectVertexData(
            new Vector2(-1.0f, -1.0f),
            Vector4.Zero,
            Color.Empty,
            false,
            1f,
            0f,
            0f,
            0f,
            0f);

        var vertex3 = new RectVertexData(
            new Vector2(1.0f, 1.0f),
            Vector4.Zero,
            Color.Empty,
            false,
            1f,
            0f,
            0f,
            0f,
            0f);

        var vertex4 = new RectVertexData(
            new Vector2(1.0f, 11.0f),
            Vector4.Zero,
            Color.Empty,
            false,
            1f,
            0f,
            0f,
            0f,
            0f);

        return new[] { vertex1, vertex2, vertex3, vertex4 };
    }

    /// <summary>
    /// Applies the color of the given <paramref name="rect"/> shape to the rectangle
    /// data being sent to the GPU.
    /// </summary>
    /// <param name="data">The data to apply the color to.</param>
    /// <param name="rect">The rect that holds the color to apply to the data.</param>
    /// <returns>The original GPU <paramref name="data"/> with the color applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the <see cref="ColorGradient"/> of the given <paramref name="rect"/>
    ///     is an invalid value.
    /// </exception>
    private static RectGPUData ApplyColor(RectGPUData data, RectShape rect)
    {
        switch (rect.GradientType)
        {
            case ColorGradient.None:
                return data.SetColor(rect.Color);
            case ColorGradient.Horizontal:
                data = data.SetColor(rect.GradientStart, VertexNumber.One); // BOTTOM LEFT
                data = data.SetColor(rect.GradientStart, VertexNumber.Two); // BOTTOM RIGHT
                data = data.SetColor(rect.GradientStop, VertexNumber.Three); // TOP RIGHT
                data = data.SetColor(rect.GradientStop, VertexNumber.Four); // BOTTOM RIGHT
                break;
            case ColorGradient.Vertical:
                data = data.SetColor(rect.GradientStart, VertexNumber.One); // BOTTOM LEFT
                data = data.SetColor(rect.GradientStop, VertexNumber.Two); // BOTTOM RIGHT
                data = data.SetColor(rect.GradientStart, VertexNumber.Three); // TOP RIGHT
                data = data.SetColor(rect.GradientStop, VertexNumber.Four); // BOTTOM RIGHT
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rect.GradientType), "The gradient type is invalid.");
        }

        return data;
    }

    /// <summary>
    /// Process the border thickness by checking that the value is within limits.
    /// If it is not within limits, it will force the value to be within limits.
    /// </summary>
    /// <param name="rect">The rectangle containing the border thickness to set within a limit.</param>
    /// <remarks>
    ///     This is done to prevent any undesired rendering artifacts from occuring.
    /// </remarks>
    private static RectShape ProcessBorderThicknessLimit(RectShape rect)
    {
        var largestValueAllowed = (rect.Width <= rect.Height ? rect.Width : rect.Height) / 2f;

        rect.BorderThickness = rect.BorderThickness > largestValueAllowed
            ? largestValueAllowed
            : rect.BorderThickness;

        rect.BorderThickness = rect.BorderThickness < 1f ? 1f : rect.BorderThickness;

        return rect;
    }

    /// <summary>
    /// Processes the corner radius by checking each corner radius value and making sure they
    /// are within limits.  If it is not within limits, it will force the values to be within limits.
    /// </summary>
    /// <param name="rect">The rectangle containing the radius values to process.</param>
    /// <returns>The rect with the corner radius values set within limits.</returns>
    /// <remarks>
    ///     This is done to prevent any undesired rendering artifacts from occuring.
    /// </remarks>
    private static RectShape ProcessCornerRadiusLimits(RectShape rect)
    {
        /*
             * Always have the smallest value between the width and height (divided by 2)
             * as the maximum limit of what any corner radius can be.
             * If the value was allowed to be larger than the smallest value between
             * the width and height, it would produce unintended rendering artifacts.
             */
        var largestValueAllowed = (rect.Width <= rect.Height ? rect.Width : rect.Height) / 2f;

        rect.CornerRadius = rect.CornerRadius.TopLeft > largestValueAllowed ? RectShape.SetTopLeft(rect.CornerRadius, largestValueAllowed) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.BottomLeft > largestValueAllowed ? RectShape.SetBottomLeft(rect.CornerRadius, largestValueAllowed) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.BottomRight > largestValueAllowed ? RectShape.SetBottomRight(rect.CornerRadius, largestValueAllowed) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.TopRight > largestValueAllowed ? RectShape.SetTopRight(rect.CornerRadius, largestValueAllowed) : rect.CornerRadius;

        rect.CornerRadius = rect.CornerRadius.TopLeft < 0 ? RectShape.SetTopLeft(rect.CornerRadius, 0) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.BottomLeft < 0 ? RectShape.SetBottomLeft(rect.CornerRadius, 0) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.BottomRight < 0 ? RectShape.SetBottomRight(rect.CornerRadius, 0) : rect.CornerRadius;
        rect.CornerRadius = rect.CornerRadius.TopRight < 0 ? RectShape.SetTopRight(rect.CornerRadius, 0) : rect.CornerRadius;

        return rect;
    }
}
