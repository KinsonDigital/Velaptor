// <copyright file="OpenTKGLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using OpenTK.Graphics.OpenGL4;
    using Velaptor.OpenGL;

    /// <summary>
    /// Invokes OpenGL calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class OpenTKGLInvoker : IGLInvoker
    {
        private static DebugProc? debugCallback;
        private bool isDisposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="OpenTKGLInvoker"/> class.
        /// </summary>
        ~OpenTKGLInvoker()
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

                GL.DebugMessageCallback(debugCallback, Marshal.StringToHGlobalAnsi(string.Empty));
            }
        }

        /// <inheritdoc/>
        public void Enable(GLEnableCap cap) => GL.Enable((EnableCap)cap);

        /// <inheritdoc/>
        public void BlendFunc(GLBlendingFactor sfactor, GLBlendingFactor dfactor) => GL.BlendFunc((BlendingFactor)sfactor, (BlendingFactor)dfactor);

        /// <inheritdoc/>
        public void Clear(GLClearBufferMask mask) => GL.Clear((ClearBufferMask)mask);

        /// <inheritdoc/>
        public void ClearColor(float red, float green, float blue, float alpha) => GL.ClearColor(red, green, blue, alpha);

        /// <inheritdoc/>
        public void ActiveTexture(GLTextureUnit texture) => GL.ActiveTexture((TextureUnit)texture);

        /// <inheritdoc/>
        public int GetUniformLocation(uint program, string name) => GL.GetUniformLocation(program, name);

        /// <inheritdoc/>
        public void BindTexture(GLTextureTarget target, uint texture) => GL.BindTexture((TextureTarget)target, texture);

        /// <inheritdoc/>
        public void DrawElements(GLPrimitiveType mode, uint count, GLDrawElementsType type, nint indices) => GL.DrawElements((PrimitiveType)mode, (int)count, (DrawElementsType)type, indices);

        /// <inheritdoc/>
        public void UniformMatrix4(int location, uint count, bool transpose, Matrix4x4 matrix)
        {
            unsafe
            {
                GL.UniformMatrix4(location, (int)count, transpose, (float*)&matrix);
            }
        }

        /// <inheritdoc/>
        public void GetProgram(uint program, GLProgramParameterName pname, out int programParams) => GL.GetProgram(program, (GetProgramParameterName)pname, out programParams);

        /// <inheritdoc/>
        public void GetInteger(GLGetPName pname, int[] data) => GL.GetInteger((GetPName)pname, data);

        /// <inheritdoc/>
        public void GetFloat(GLGetPName pname, float[] data) => GL.GetFloat((GetPName)pname, data);

        /// <inheritdoc/>
        public void Viewport(int x, int y, uint width, uint height) => GL.Viewport(x, y, (int)width, (int)height);

        /// <inheritdoc/>
        public void UseProgram(uint program) => GL.UseProgram(program);

        /// <inheritdoc/>
        public void DeleteProgram(uint program) => GL.DeleteProgram(program);

        /// <inheritdoc/>
        public uint CreateProgram() => (uint)GL.CreateProgram();

        /// <inheritdoc/>
        public void AttachShader(uint program, uint shader) => GL.AttachShader(program, shader);

        /// <inheritdoc/>
        public void LinkProgram(uint program) => GL.LinkProgram(program);

        /// <inheritdoc/>
        public string GetProgramInfoLog(uint program) => GL.GetProgramInfoLog((int)program);

        /// <inheritdoc/>
        public uint CreateShader(GLShaderType type) => (uint)GL.CreateShader((ShaderType)type);

        /// <inheritdoc/>
        public void ShaderSource(uint shader, string sourceCode) => GL.ShaderSource((int)shader, sourceCode);

        /// <inheritdoc/>
        public void DetachShader(uint program, uint shader) => GL.DetachShader(program, shader);

        /// <inheritdoc/>
        public void CompileShader(uint shader) => GL.CompileShader(shader);

        /// <inheritdoc/>
        public void GetShader(uint shader, GLShaderParameter pname, out int shaderParams) => GL.GetShader(shader, (ShaderParameter)pname, out shaderParams);

        /// <inheritdoc/>
        public string GetShaderInfoLog(uint shader) => GL.GetShaderInfoLog((int)shader);

        /// <inheritdoc/>
        public void DeleteShader(uint shader) => GL.DeleteShader(shader);

        /// <inheritdoc/>
        public uint GenVertexArray() => (uint)GL.GenVertexArray();

        /// <inheritdoc/>
        public void BufferData(GLBufferTarget target, uint size, nint data, GLBufferUsageHint usage) => GL.BufferData((BufferTarget)target, (int)size, data, (BufferUsageHint)usage);

        /*
         * TODO: Need to eventually convert the BufferSubData<T> method `data` param from a `ref`
         * param to an `in` param.  Currently this cannot be done due to an issue found in NET 5 runtime.
         *
         * The issue was suppose to be fixed in NET 6 preview at the link below, but the issue seems to have
         * moved from the test failing at the actual method call to the `Mock<T>.Verify()` call in the
         * unit test `UpdateQuad_WhenInvoked_UpdatesGPUVertexBuffer`, and is should be passing.
         *
         * Links to GITHUB issues:
         *      1. https://github.com/moq/moq4/issues/555
         *      2. https://github.com/moq/moq4/issues/1136
         *          * This one is related to the first
         */

        /// <inheritdoc/>
        public void BufferSubData<T>(GLBufferTarget target, nint offset, nuint size, ref T data)
            where T : unmanaged
        {
            unsafe
            {
                fixed (T* dataPtr = &data)
                {
                    GL.BufferSubData((BufferTarget)target, offset, (int)size, (nint)dataPtr);
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteVertexArray(uint arrays) => GL.DeleteVertexArray(arrays);

        /// <inheritdoc/>
        public void DeleteBuffer(uint buffers) => GL.DeleteBuffer(buffers);

        /// <inheritdoc/>
        public void BindBuffer(GLBufferTarget target, uint buffer) => GL.BindBuffer((BufferTarget)target, buffer);

        /// <inheritdoc/>
        public void EnableVertexArrayAttrib(uint vaobj, uint index) => GL.EnableVertexArrayAttrib((int)vaobj, (int)index);

        /// <inheritdoc/>
        public void VertexAttribPointer(uint index, int size, GLVertexAttribPointerType type, bool normalized, uint stride, uint offset)
            => GL.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, (int)stride, (int)offset);

        /// <inheritdoc/>
        public uint GenBuffer() => (uint)GL.GenBuffer();

        /// <inheritdoc/>
        public void BindVertexArray(uint array) => GL.BindVertexArray(array);

        /// <inheritdoc/>
        public uint GenTexture() => (uint)GL.GenTexture();

        /// <inheritdoc/>
        public void DeleteTexture(uint textures) => GL.DeleteTexture(textures);

        /// <inheritdoc/>
        public void ObjectLabel(GLObjectIdentifier identifier, uint name, uint length, string label)
            => GL.ObjectLabel((ObjectLabelIdentifier)identifier, name, (int)length, label);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureWrapMode param)
            => GL.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMinFilter param)
            => GL.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMagFilter param)
            => GL.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);

        /// <inheritdoc/>
        public void TexImage2D<T>(GLTextureTarget target, int level, GLInternalFormat internalformat, uint width, uint height, int border, GLPixelFormat format, GLPixelType type, byte[] pixels)
            where T : unmanaged
        {
            unsafe
            {
                fixed (void* unmanagedPixelPtr = pixels)
                {
                    GL.TexImage2D(
                        target: (TextureTarget)target,
                        level: level,
                        internalformat: (PixelInternalFormat)internalformat,
                        width: (int)width,
                        height: (int)height,
                        border: border,
                        format: (PixelFormat)format,
                        type: (PixelType)type,
                        pixels: (nint)unmanagedPixelPtr);
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                debugCallback = null;
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Invoked when there is an OpenGL related error.
        /// </summary>
        /// <param name="src">The debug source.</param>
        /// <param name="type">The debug type.</param>
        /// <param name="id">The id of the error or message.</param>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="length">The length of the message.</param>
        /// <param name="message">The error message.</param>
        /// <param name="userParam">The OpenGL parameter related to the error.</param>
        private void DebugCallback(DebugSource src, DebugType type, int id, DebugSeverity severity, int length, nint message, nint userParam)
        {
            var errorMessage = Marshal.PtrToStringAnsi(message);

            errorMessage += $"\n\tSrc: {src}";
            errorMessage += $"\n\tType: {type}";
            errorMessage += $"\n\tID: {id}";
            errorMessage += $"\n\tSeverity: {severity}";
            errorMessage += $"\n\tLength: {length}";
            errorMessage += $"\n\tUser Param: {Marshal.PtrToStringAnsi(userParam)}";

            if (severity != DebugSeverity.DebugSeverityNotification)
            {
                GLError?.Invoke(this, new GLErrorEventArgs(errorMessage));
            }
        }
    }
}
