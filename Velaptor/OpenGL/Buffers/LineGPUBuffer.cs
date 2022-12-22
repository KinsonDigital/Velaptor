// <copyright file="LineGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Linq;
using Carbonate;
using Exceptions;
using GPUData;
using Guards;
using NativeInterop.OpenGL;
using Reactables.Core;
using Reactables.ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// Updates data in the line GPU buffer.
/// </summary>
[GPUBufferName("Line")]
internal sealed class LineGPUBuffer : GPUBufferBase<LineBatchItem>
{
    private const string BufferNotInitMsg = "The line buffer has not been initialized.";
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGPUBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
    /// <param name="reactable">Receives a push notification about the batch size.</param>
    /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public LineGPUBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactable<GLInitData> glInitReactable,
        IReactable reactable,
        IReactable<ShutDownData> shutDownReactable)
        : base(gl, openGLService, glInitReactable, shutDownReactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.unsubscriber = reactable.Subscribe(new Reactor(
            NotificationIds.BatchSizeId,
            onNext: msg =>
            {
                var batchSize = msg.GetData<BatchSizeData>()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException($"{nameof(LineGPUBuffer)}.Constructor()", NotificationIds.BatchSizeId);
                }

                BatchSize = batchSize.Value;
            },
            onCompleted: () => this.unsubscriber?.Dispose()));
    }

    /// <inheritdoc/>
    protected internal override void UploadVertexData(LineBatchItem lineData, uint batchIndex)
    {
        if (IsInitialized is false)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BeginGroup($"Update Line - BatchItem({batchIndex})");

        var data = default(LineGPUData);

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

        var totalBytes = LineGPUData.GetTotalBytes();
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
        var result = new List<float>();

        for (var i = 0; i < BatchSize; i++)
        {
            result.AddRange(new LineGPUData(default, default, default, default).ToArray());
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
