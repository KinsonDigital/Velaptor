// <copyright file="GLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public class GLInvoker : IGLInvoker
    {
        /// <inheritdoc/>
        public void Enable(EnableCap cap) => GL.Enable(cap);

        /// <inheritdoc/>
        public void BlendFunc(BlendingFactor sfactor, BlendingFactor dfactor) => GL.BlendFunc(sfactor, dfactor);

        /// <inheritdoc/>
        public void ClearColor(float red, float green, float blue, float alpha) => GL.ClearColor(red, green, blue, alpha);

        /// <inheritdoc/>
        public void ActiveTexture(TextureUnit texture) => GL.ActiveTexture(texture);

        /// <inheritdoc/>
        public int GetUniformLocation(int program, string name) => GL.GetUniformLocation(program, name);

        /// <inheritdoc/>
        public void BindTexture(TextureTarget target, int texture) => GL.BindTexture(target, texture);

        /// <inheritdoc/>
        public void DrawElements(PrimitiveType mode, int count, DrawElementsType type, IntPtr indices) => GL.DrawElements(mode, count, type, indices);

        /// <inheritdoc/>
        public void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix) => GL.UniformMatrix4(location, transpose, ref matrix);

        /// <inheritdoc/>
        public void GetProgram(int program, GetProgramParameterName pname, out int programParams) => GL.GetProgram(program, pname, out programParams);

        /// <inheritdoc/>
        public bool LinkProgramSuccess(int program)
        {
            GetProgram(program, GetProgramParameterName.LinkStatus, out var statusCode);

            return statusCode >= 1;
        }

        /// <inheritdoc/>
        public void UseProgram(int program) => GL.UseProgram(program);

        /// <inheritdoc/>
        public void DeleteProgram(int program) => GL.DeleteProgram(program);

        /// <inheritdoc/>
        public int CreateProgram() => GL.CreateProgram();

        /// <inheritdoc/>
        public void AttachShader(int program, int shader) => GL.AttachShader(program, shader);

        /// <inheritdoc/>
        public void LinkProgram(int program) => GL.LinkProgram(program);

        /// <inheritdoc/>
        public string GetProgramInfoLog(int program) => GL.GetProgramInfoLog(program);

        /// <inheritdoc/>
        public int CreateShader(ShaderType type) => GL.CreateShader(type);

        /// <inheritdoc/>
        public void ShaderSource(int shader, string sourceCode) => GL.ShaderSource(shader, sourceCode);

        /// <inheritdoc/>
        public void DetachShader(int program, int shader) => GL.DetachShader(program, shader);

        /// <inheritdoc/>
        public void CompileShader(int shader) => GL.CompileShader(shader);

        /// <inheritdoc/>
        public void GetShader(int shader, ShaderParameter pname, out int shaderParams) => GL.GetShader(shader, pname, out shaderParams);

        /// <inheritdoc/>
        public bool ShaderCompileSuccess(int shaderID)
        {
            GetShader(shaderID, ShaderParameter.CompileStatus, out var statusCode);

            return statusCode >= 1;
        }

        /// <inheritdoc/>
        public string GetShaderInfoLog(int shader) => GL.GetShaderInfoLog(shader);

        /// <inheritdoc/>
        public void DeleteShader(int shader) => GL.DeleteShader(shader);

        /// <inheritdoc/>
        public int GenVertexArray() => GL.GenVertexArray();

        /// <inheritdoc/>
        public void BufferSubData<T3>(BufferTarget target, IntPtr offset, int size, ref T3 data)
            where T3 : struct => GL.BufferSubData(target, offset, size, ref data);

        /// <inheritdoc/>
        public void DeleteVertexArray(int arrays) => GL.DeleteVertexArray(arrays);

        /// <inheritdoc/>
        public void DeleteBuffer(int buffers) => GL.DeleteBuffer(buffers);

        /// <inheritdoc/>
        public void BindBuffer(BufferTarget target, int buffer) => GL.BindBuffer(target, buffer);

        /// <inheritdoc/>
        public void EnableVertexArrayAttrib(int vaobj, int index) => GL.EnableVertexArrayAttrib(vaobj, index);

        /// <inheritdoc/>
        public void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
            => GL.VertexAttribPointer(index, size, type, normalized, stride, offset);

        /// <inheritdoc/>
        public int GenBuffer() => GL.GenBuffer();

        /// <inheritdoc/>
        public void BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint usage)
            => GL.BufferData(target, size, data, usage);

        /// <inheritdoc/>
        public void BufferData<T2>(BufferTarget target, int size, T2[] data, BufferUsageHint usage)
            where T2 : struct => GL.BufferData(target, size, data, usage);

        /// <inheritdoc/>
        public void BindVertexArray(int array) => GL.BindVertexArray(array);

        /// <inheritdoc/>
        public int GenTexture() => GL.GenTexture();

        /// <inheritdoc/>
        public void DeleteTexture(int textures) => GL.DeleteTexture(textures);

        /// <inheritdoc/>
        public void ObjectLabel(ObjectLabelIdentifier identifier, int name, int length, string label)
            => GL.ObjectLabel(identifier, name, length, label);

        /// <inheritdoc/>
        public void TexParameter(TextureTarget target, TextureParameterName pname, int param)
            => GL.TexParameter(target, pname, param);

        /// <inheritdoc/>
        public void TexImage2D<T8>(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, T8[] pixels)
            where T8 : struct => GL.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels);

        /// <inheritdoc/>
        public void DebugMessageCallback(DebugProc callback, IntPtr userParam) => GL.DebugMessageCallback(callback, userParam);
    }
}
