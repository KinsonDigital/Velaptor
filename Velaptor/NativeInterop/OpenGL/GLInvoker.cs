// <copyright file="GLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using Silk.NET.OpenGL;
    using Silk.NET.Windowing;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL;

    /// <summary>
    /// Invokes OpenGL calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class GLInvoker : IGLInvoker
    {
        private static DebugProc? debugCallback;
        private readonly IDisposable glContextUnsubscriber;
        private bool isDisposed;
        private GL gl = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLInvoker"/> class.
        /// </summary>
        /// <param name="glContextObservable">
        ///     The OpenGL context observable to subscribe to get a push notification
        ///     that the OpenGL context has been created.
        /// </param>
        public GLInvoker(OpenGLContextObservable glContextObservable)
            => this.glContextUnsubscriber = glContextObservable.Subscribe(new Observer<object>(
                onNext: data =>
                {
                    if (data is IWindow window)
                    {
                        this.gl = window.CreateOpenGL();
                    }
                    else
                    {
                        var exceptionMessage = $"The parameter '{nameof(data)}' of the '{nameof(Observer<object>.OnNext)}()' action delegate must be of type '{nameof(IWindow)}'.";
                        exceptionMessage += $"\n\t{nameof(OpenGLContextObservable)} subscription location: {nameof(GLInvoker)}.Ctor()";

                        throw new Exception(exceptionMessage);
                    }
                },
                onCompleted: () =>
                {
                    this.glContextUnsubscriber?.Dispose();
                }));

        /// <summary>
        /// Finalizes an instance of the <see cref="GLInvoker"/> class.
        /// </summary>
        ~GLInvoker()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public event EventHandler<GLErrorEventArgs>? GLError;

        /// <inheritdoc/>
        public void SetupErrorCallback()
        {
            if (debugCallback == null)
            {
                debugCallback = DebugCallback;

                /*NOTE:
                 * This is here to help prevent an issue with an obscure System.ExecutionException from occurring.
                 * The garbage collector performs a collect on the delegate passed into GL.DebugMesageCallback()
                 * without the native system knowing about it which causes this exception. The GC.KeepAlive()
                 * method tells the garbage collector to not collect the delegate to prevent this from happening.
                 */
                GC.KeepAlive(debugCallback);

                this.gl.DebugMessageCallback(debugCallback, Marshal.StringToHGlobalAnsi(string.Empty));
            }
        }

        /// <inheritdoc/>
        public void Enable(GLEnableCap cap) => this.gl.Enable((EnableCap)cap);

        /// <inheritdoc/>
        public void Disable(GLEnableCap cap) => this.gl.Disable((EnableCap)cap);

        /// <inheritdoc/>
        public void BlendFunc(GLBlendingFactor sfactor, GLBlendingFactor dfactor) => this.gl.BlendFunc((BlendingFactor)sfactor, (BlendingFactor)dfactor);

        /// <inheritdoc/>
        public void Clear(GLClearBufferMask mask) => this.gl.Clear((uint)mask);

        /// <inheritdoc/>
        public void ClearColor(float red, float green, float blue, float alpha) => this.gl.ClearColor(red, green, blue, alpha);

        /// <inheritdoc/>
        public void ActiveTexture(GLTextureUnit texture) => this.gl.ActiveTexture((TextureUnit)texture);

        /// <inheritdoc/>
        public int GetUniformLocation(uint program, string name) => this.gl.GetUniformLocation(program, name);

        /// <inheritdoc/>
        public void BindTexture(GLTextureTarget target, uint texture) => this.gl.BindTexture((TextureTarget)target, texture);

        /// <inheritdoc/>
        public void DrawElements(GLPrimitiveType mode, uint count, GLDrawElementsType type, nint indices)
        {
            unsafe
            {
                this.gl.DrawElements((PrimitiveType)mode, count, (DrawElementsType)type, (void*)indices);
            }
        }

        /// <inheritdoc/>
        public void UniformMatrix4(int location, uint count, bool transpose, Matrix4x4 matrix)
        {
            unsafe
            {
                this.gl.UniformMatrix4(location, count, transpose, (float*)&matrix);
            }
        }

        /// <inheritdoc/>
        public void GetProgram(uint program, GLProgramParameterName pname, out int programParams) => this.gl.GetProgram(program, (ProgramPropertyARB)pname, out programParams);

        /// <inheritdoc/>
        public void GetInteger(GLGetPName pname, int[] data) => this.gl.GetInteger((GetPName)pname, data);

        /// <inheritdoc/>
        public void GetFloat(GLGetPName pname, float[] data) => this.gl.GetFloat((GetPName)pname, data);

        /// <inheritdoc/>
        public void Viewport(int x, int y, uint width, uint height) => this.gl.Viewport(x, y, width, height);

        /// <inheritdoc/>
        public void UseProgram(uint program) => this.gl.UseProgram(program);

        /// <inheritdoc/>
        public void DeleteProgram(uint program) => this.gl.DeleteProgram(program);

        /// <inheritdoc/>
        public uint CreateProgram() => this.gl.CreateProgram();

        /// <inheritdoc/>
        public void AttachShader(uint program, uint shader) => this.gl.AttachShader(program, shader);

        /// <inheritdoc/>
        public void LinkProgram(uint program) => this.gl.LinkProgram(program);

        /// <inheritdoc/>
        public string GetProgramInfoLog(uint program) => this.gl.GetProgramInfoLog(program);

        /// <inheritdoc/>
        public uint CreateShader(GLShaderType type) => this.gl.CreateShader((ShaderType)type);

        /// <inheritdoc/>
        public void ShaderSource(uint shader, string sourceCode) => this.gl.ShaderSource(shader, sourceCode);

        /// <inheritdoc/>
        public void DetachShader(uint program, uint shader) => this.gl.DetachShader(program, shader);

        /// <inheritdoc/>
        public void CompileShader(uint shader) => this.gl.CompileShader(shader);

        /// <inheritdoc/>
        public void GetShader(uint shader, GLShaderParameter pname, out int shaderParams) => this.gl.GetShader(shader, (ShaderParameterName)pname, out shaderParams);

        /// <inheritdoc/>
        public string GetShaderInfoLog(uint shader) => this.gl.GetShaderInfoLog(shader);

        /// <inheritdoc/>
        public void DeleteShader(uint shader) => this.gl.DeleteShader(shader);

        /// <inheritdoc/>
        public uint GenVertexArray() => this.gl.GenVertexArray();

        /// <inheritdoc/>
        public void BufferData(GLBufferTarget target, uint size, nint data, GLBufferUsageHint usage)
        {
            unsafe
            {
                this.gl.BufferData((BufferTargetARB)target, size, (void*)data, (BufferUsageARB)usage);
            }
        }

        /// <inheritdoc/>
        public void BufferSubData<T>(GLBufferTarget target, nint offset, nuint size, ref T data)
            where T : unmanaged
        {
            unsafe
            {
                fixed (T* dataPtr = &data)
                {
                    this.gl.BufferSubData((BufferTargetARB)target, offset, size, dataPtr);
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteVertexArray(uint arrays) => this.gl.DeleteVertexArray(arrays);

        /// <inheritdoc/>
        public void DeleteBuffer(uint buffers) => this.gl.DeleteBuffer(buffers);

        /// <inheritdoc/>
        public void BindBuffer(GLBufferTarget target, uint buffer) => this.gl.BindBuffer((BufferTargetARB)target, buffer);

        /// <inheritdoc/>
        public void EnableVertexArrayAttrib(uint vaobj, uint index) => this.gl.EnableVertexArrayAttrib(vaobj, index);

        /// <inheritdoc/>
        public void VertexAttribPointer(uint index, int size, GLVertexAttribPointerType type, bool normalized, uint stride, uint offset)
        {
            unsafe
            {
                this.gl.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, stride, (void*)offset);
            }
        }

        /// <inheritdoc/>
        public uint GenBuffer() => this.gl.GenBuffer();

        /// <inheritdoc/>
        public void BindVertexArray(uint array) => this.gl.BindVertexArray(array);

        /// <inheritdoc/>
        public uint GenTexture() => this.gl.GenTexture();

        /// <inheritdoc/>
        public void DeleteTexture(uint textures) => this.gl.DeleteTexture(textures);

        /// <inheritdoc/>
        public void ObjectLabel(GLObjectIdentifier identifier, uint name, uint length, string label)
            => this.gl.ObjectLabel((ObjectIdentifier)identifier, name, length, label);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureWrapMode param)
            => this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMinFilter param)
            => this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMagFilter param)
            => this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexImage2D<T>(GLTextureTarget target, int level, GLInternalFormat internalformat, uint width, uint height, int border, GLPixelFormat format, GLPixelType type, byte[] pixels)
            where T : unmanaged
        {
            unsafe
            {
                fixed (void* unmanagedPixelPtr = pixels)
                {
                    this.gl.TexImage2D(
                        target: (TextureTarget)target,
                        level: level,
                        internalformat: (int)internalformat,
                        width: width,
                        height: height,
                        border: border,
                        format: (PixelFormat)format,
                        type: (PixelType)type,
                        pixels: unmanagedPixelPtr);
                }
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.glContextUnsubscriber.Dispose();
                }

                debugCallback = null;
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Invoked when there is an OpenGL related error.
        /// </summary>
        /// <param name="source">The debug source.</param>
        /// <param name="type">The debug type.</param>
        /// <param name="id">The id of the error or message.</param>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="length">The length of the message.</param>
        /// <param name="message">The error message.</param>
        /// <param name="userParam">The OpenGL parameter related to the error.</param>
        private void DebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
        {
            var errorMessage = Marshal.PtrToStringAnsi(message);

            errorMessage += $"\n\tSrc: {source}";
            errorMessage += $"\n\tType: {type}";
            errorMessage += $"\n\tID: {id}";
            errorMessage += $"\n\tSeverity: {severity}";
            errorMessage += $"\n\tLength: {length}";
            errorMessage += $"\n\tUser Param: {Marshal.PtrToStringAnsi(userParam)}";

            if (severity != GLEnum.DebugSeverityNotification)
            {
                GLError?.Invoke(this, new GLErrorEventArgs(errorMessage));
            }
        }
    }
}
