// <copyright file="LineGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Linq;
using Batching;
using Carbonate.Fluent;
using Exceptions;
using ExtensionMethods;
using Factories;
using GpuData;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using ReactableData;

/// <summary>
/// Updates data in the line GPU buffer.
/// </summary>
[GpuBufferName("Line")]
internal sealed class LineGpuBuffer : GpuBufferBase<LineBatchItem>
{
    private const string BufferNotInitMsg = "The line buffer has not been initialized.";

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public LineGpuBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, reactableFactory)
    {
        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var subscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchSizeChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId)))
            .BuildOneWayReceive<BatchSizeData>(data =>
            {
                if (data.TypeOfBatch != BatchType.Line)
                {
                    return;
                }

                BatchSize = data.BatchSize;

                if (IsInitialized)
                {
                    ResizeBatch();
                }
            });

        batchSizeReactable.Subscribe(subscription);
    }

    /// <inheritdoc/>
    protected internal override void UploadVertexData(LineBatchItem lineData, uint batchIndex)
    {
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BeginGroup($"Update Line - BatchItem({batchIndex})");

        var data = default(LineGpuData);

        var lineRectPoints = lineData.CreateRectFromLine().ToArray();

        var topLeft = lineRectPoints[0].ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var topRight = lineRectPoints[1].ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var bottomRight = lineRectPoints[2].ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var bottomLeft = lineRectPoints[3].ToNDC(ViewPortSize.Width, ViewPortSize.Height);

        data = data.SetVertexPos(topLeft, VertexNumber.One);
        data = data.SetVertexPos(bottomLeft, VertexNumber.Two);
        data = data.SetVertexPos(topRight, VertexNumber.Three);
        data = data.SetVertexPos(bottomRight, VertexNumber.Four);

        data = data.SetColor(lineData.Color);

        var totalBytes = LineGpuData.GetTotalBytes();
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

        for (var i = 0; i < BatchSize; i++)
        {
            result.AddRange(new LineGpuData(default, default, default, default).ToArray());
        }

        return result.ToArray();
    }

    /// <inheritdoc/>
    protected internal override void SetupVAO()
    {
        var stride = LineVertexData.GetStride();

        // Vertex Position
        const uint vertexPosSize = 2u * sizeof(float);
        GL.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, stride, 0u);
        GL.EnableVertexAttribArray(0);

        // Color
        GL.VertexAttribPointer(1, 4, GLVertexAttribPointerType.Float, false, stride, vertexPosSize);
        GL.EnableVertexAttribArray(1);
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
}
