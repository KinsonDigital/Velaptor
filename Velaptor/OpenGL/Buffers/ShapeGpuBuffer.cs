// <copyright file="ShapeGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Batching;
using Carbonate;
using Exceptions;
using ExtensionMethods;
using Factories;
using GpuData;
using Graphics;
using NativeInterop.OpenGL;
using NativeInterop.Services;

/// <summary>
/// Updates data in the shape GPU buffer.
/// </summary>
[GpuBufferName("Shape")]
[SuppressMessage("csharpsquid", "S101", Justification = "GPU is an acceptable acronym.")]
internal sealed class ShapeGpuBuffer : GpuBufferBase<ShapeBatchItem>
{
    private const string BufferNotInitMsg = "The shape buffer has not been initialized.";
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public ShapeGpuBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, reactableFactory)
    {
        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        this.unsubscriber = batchSizeReactable.CreateOneWayReceive(
            PushNotifications.BatchSizeChangedId,
            (data) =>
            {
                if (data.TypeOfBatch != BatchType.Rect)
                {
                    return;
                }

                BatchSize = data.BatchSize;

                if (IsInitialized)
                {
                    ResizeBatch();
                }
            },
            () => this.unsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    protected internal override void UploadVertexData(ShapeBatchItem shape, uint batchIndex)
    {
        OpenGLService.BeginGroup($"Update Shape - BatchItem({batchIndex})");

        /*
         * Always have the smallest value between the width and height (divided by 2)
         * as the maximum limit of what the border thickness can be.
         * If the value was allowed to be larger than the smallest value between
         * the width and height, it would produce unintended rendering artifacts.
         */

        shape = ProcessBorderThicknessLimit(shape);
        shape = ProcessCornerRadiusLimits(shape);

        var data = ShapeGpuData.Empty();

        var left = shape.Position.X - shape.HalfWidth;
        var bottom = shape.Position.Y + shape.HalfHeight;
        var right = shape.Position.X + shape.HalfWidth;
        var top = shape.Position.Y - shape.HalfHeight;

        var topLeft = new Vector2(left, top).ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var bottomLeft = new Vector2(left, bottom).ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var bottomRight = new Vector2(right, bottom).ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var topRight = new Vector2(right, top).ToNDC(ViewPortSize.Width, ViewPortSize.Height);

        data = data.SetVertexPos(topLeft, VertexNumber.One);
        data = data.SetVertexPos(bottomLeft, VertexNumber.Two);
        data = data.SetVertexPos(topRight, VertexNumber.Three);
        data = data.SetVertexPos(bottomRight, VertexNumber.Four);

        data = data.SetRectangle(new Vector4(shape.Position.X, shape.Position.Y, shape.Width, shape.Height));

        data = ApplyColor(data, shape);

        data = data.SetAsSolid(shape.IsSolid);

        data = data.SetBorderThickness(shape.BorderThickness);
        data = data.SetTopLeftCornerRadius(shape.CornerRadius.TopLeft);
        data = data.SetBottomLeftCornerRadius(shape.CornerRadius.BottomLeft);
        data = data.SetBottomRightCornerRadius(shape.CornerRadius.BottomRight);
        data = data.SetTopRightCornerRadius(shape.CornerRadius.TopRight);

        var totalBytes = ShapeGpuData.GetTotalBytes();
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
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BindVAO(VAO);
    }

    /// <inheritdoc/>
    protected internal override float[] GenerateData()
    {
        var result = new List<float>();

        for (var i = 0u; i < BatchSize; i++)
        {
            var vertexData = GenerateVertexData();
            result.AddRange(new ShapeGpuData(vertexData[0], vertexData[1], vertexData[2], vertexData[3]).ToArray());
        }

        return result.ToArray();
    }

    /// <inheritdoc/>
    protected internal override void SetupVAO()
    {
        var stride = ShapeVertexData.GetStride();

        var attrComponentSizes = new[]
        {
            2u, // Vertex Position
            4u, // Rectangle
            4u, // Color
            1u, // IsSolid
            1u, // Border Thickness
            1u, // Top Left Corner Radius
            1u, // Bottom Left Corner Radius
            1u, // Bottom Right Corner Radius
            1u, // Top Right Corner Radius
        };

        var prevAttrByteSize = 0u;
        for (var i = 0u; i < attrComponentSizes.Length; i++)
        {
            var totalAttrComponents = attrComponentSizes[i];

            var offset = i == 0 ? 0 : prevAttrByteSize;
            var attrByteSize = totalAttrComponents * sizeof(float);

            GL.VertexAttribPointer(i, (int)totalAttrComponents, GLVertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(i);

            prevAttrByteSize += attrByteSize;
        }
    }

    /// <inheritdoc/>
    protected internal override uint[] GenerateIndices()
    {
        var result = new List<uint>();

        for (var i = 0u; i < BatchSize; i++)
        {
            var maxIndex = result.Count <= 0 ? 0 : result.Max() + 1;

            result.AddRange(
            [
                maxIndex,
                maxIndex + 1u,
                maxIndex + 2u,
                maxIndex + 2u,
                maxIndex + 1u,
                maxIndex + 3u
            ]);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Generates default <see cref="ShapeVertexData"/> for all four vertices that make
    /// up a rectangular rendering area.
    /// </summary>
    /// <returns>The four vertex data items.</returns>
    private static ShapeVertexData[] GenerateVertexData() =>
    [
        ShapeVertexData.New(-1.0f, 1.0f),
            ShapeVertexData.New(-1.0f, -1.0f),
            ShapeVertexData.New(1.0f, 1.0f),
            ShapeVertexData.New(1.0f, 1.0f)
    ];

    /// <summary>
    /// Applies the color of the given <paramref name="shape"/> shape to the shape
    /// data being sent to the GPU.
    /// </summary>
    /// <param name="data">The data to apply the color to.</param>
    /// <param name="shape">The shape that holds the color to apply to the data.</param>
    /// <returns>The original GPU <paramref name="data"/> with the color applied.</returns>
    /// <exception cref="InvalidEnumArgumentException">
    ///     Thrown if the <see cref="ColorGradient"/> of the given <paramref name="shape"/>
    ///     is an invalid value.
    /// </exception>
    private static ShapeGpuData ApplyColor(ShapeGpuData data, ShapeBatchItem shape)
    {
        switch (shape.GradientType)
        {
            case ColorGradient.None:
                return data.SetColor(shape.Color);
            case ColorGradient.Horizontal:
                data = data.SetColor(shape.GradientStart, VertexNumber.One); // BOTTOM LEFT
                data = data.SetColor(shape.GradientStart, VertexNumber.Two); // BOTTOM RIGHT
                data = data.SetColor(shape.GradientStop, VertexNumber.Three); // TOP RIGHT
                data = data.SetColor(shape.GradientStop, VertexNumber.Four); // BOTTOM RIGHT
                break;
            case ColorGradient.Vertical:
                data = data.SetColor(shape.GradientStart, VertexNumber.One); // BOTTOM LEFT
                data = data.SetColor(shape.GradientStop, VertexNumber.Two); // BOTTOM RIGHT
                data = data.SetColor(shape.GradientStart, VertexNumber.Three); // TOP RIGHT
                data = data.SetColor(shape.GradientStop, VertexNumber.Four); // BOTTOM RIGHT
                break;
            default:
                const string argName = $"{nameof(shape)}.{nameof(shape.GradientType)}";
                throw new InvalidEnumArgumentException(argName, (int)shape.GradientType, typeof(ColorGradient));
        }

        return data;
    }

    /// <summary>
    /// Process the border thickness by checking that the value is within limits.
    /// If it is not within limits, it will force the value to be within limits.
    /// </summary>
    /// <param name="shape">The shape containing the border thickness to set within a limit.</param>
    /// <remarks>
    ///     This is done to prevent any undesired rendering artifacts from occuring.
    /// </remarks>
    private static ShapeBatchItem ProcessBorderThicknessLimit(ShapeBatchItem shape)
    {
        var largestValueAllowed = (shape.Width <= shape.Height ? shape.Width : shape.Height) / 2f;

        var newBorderThickness = shape.BorderThickness > largestValueAllowed
            ? largestValueAllowed
            : shape.BorderThickness;
        newBorderThickness = newBorderThickness < 1f ? 1f : newBorderThickness;

        shape = new ShapeBatchItem(
            shape.Position,
            shape.Width,
            shape.Height,
            shape.Color,
            shape.IsSolid,
            newBorderThickness,
            shape.CornerRadius,
            shape.GradientType,
            shape.GradientStart,
            shape.GradientStop);

        return shape;
    }

    /// <summary>
    /// Processes the corner radius by checking each corner radius value and making sure they
    /// are within limits.  If it is not within limits, it will force the values to be within limits.
    /// </summary>
    /// <param name="shape">The shape containing the radius values to process.</param>
    /// <returns>The shape with the corner radius values set within limits.</returns>
    /// <remarks>
    ///     This is done to prevent any undesired rendering artifacts from occuring.
    /// </remarks>
    private static ShapeBatchItem ProcessCornerRadiusLimits(ShapeBatchItem shape)
    {
        /*
         * Always have the smallest value between the width and height (divided by 2)
         * as the maximum limit of what any corner radius can be.
         * If the value was allowed to be larger than the smallest value between
         * the width and height, it would produce unintended rendering artifacts.
         */
        var largestValueAllowed = (shape.Width <= shape.Height ? shape.Width : shape.Height) / 2f;

        var clampedCornerRadius = shape.CornerRadius.Clamp(0, largestValueAllowed);

        shape = new ShapeBatchItem(
            shape.Position,
            shape.Width,
            shape.Height,
            shape.Color,
            shape.IsSolid,
            shape.BorderThickness,
            clampedCornerRadius,
            shape.GradientType,
            shape.GradientStart,
            shape.GradientStop);

        return shape;
    }
}
