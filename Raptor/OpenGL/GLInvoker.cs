// <copyright file="GLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GLInvoker : IGLInvoker
    {
        /// <inheritdoc/>
        public void Enable(EnableCap cap) => GL.Enable(cap);

        /// <inheritdoc/>
        public void BlendFunc(BlendingFactor sfactor, BlendingFactor dfactor) => GL.BlendFunc(sfactor, dfactor);

        /// <inheritdoc/>
        public void Clear(ClearBufferMask mask) => GL.Clear(mask);

        /// <inheritdoc/>
        public void ClearColor(float red, float green, float blue, float alpha) => GL.ClearColor(red, green, blue, alpha);

        /// <inheritdoc/>
        public void ActiveTexture(TextureUnit texture) => GL.ActiveTexture(texture);

        /// <inheritdoc/>
        public uint GetUniformLocation(uint program, string name) => (uint)GL.GetUniformLocation(program, name);

        /// <inheritdoc/>
        public void BindTexture(TextureTarget target, uint texture) => GL.BindTexture(target, texture);

        /// <inheritdoc/>
        public void DrawElements(PrimitiveType mode, uint count, DrawElementsType type, IntPtr indices) => GL.DrawElements(mode, (int)count, type, indices);

        /// <inheritdoc/>
        public void UniformMatrix4(uint location, bool transpose, ref Matrix4 matrix) => GL.UniformMatrix4((int)location, transpose, ref matrix);

        /// <inheritdoc/>
        public void GetProgram(uint program, GetProgramParameterName pname, out int programParams) => GL.GetProgram(program, pname, out programParams);

        /// <inheritdoc/>
        public void GetInteger(GetPName pname, int[] data) => GL.GetInteger(GetPName.Viewport, data);

        /// <inheritdoc/>
        public Vector2 GetViewPortSize()
        {
            var data = new int[4];

            GetInteger(GetPName.Viewport, data);

            return new Vector2(data[2], data[3]);
        }

        /// <inheritdoc/>
        public void SetViewPortSize(Vector2 vector)
        {
            var data = new int[4];

            GetInteger(GetPName.Viewport, data);

            Viewport(data[0], data[1], (int)vector.X, (int)vector.Y);
        }

        /// <inheritdoc/>
        public Vector2 GetViewPortPosition()
        {
            var data = new int[4];

            GetInteger(GetPName.Viewport, data);

            return new Vector2(data[0], data[1]);
        }

        /// <inheritdoc/>
        public void Viewport(int x, int y, int width, int height) => GL.Viewport(x, y, width, height);

        /// <inheritdoc/>
        public bool LinkProgramSuccess(uint program)
        {
            GetProgram(program, GetProgramParameterName.LinkStatus, out var statusCode);

            return statusCode >= 1;
        }

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
        public uint CreateShader(ShaderType type) => (uint)GL.CreateShader(type);

        /// <inheritdoc/>
        public void ShaderSource(uint shader, string sourceCode) => GL.ShaderSource((int)shader, sourceCode);

        /// <inheritdoc/>
        public void DetachShader(uint program, uint shader) => GL.DetachShader(program, shader);

        /// <inheritdoc/>
        public void CompileShader(uint shader) => GL.CompileShader(shader);

        /// <inheritdoc/>
        public void GetShader(uint shader, ShaderParameter pname, out int shaderParams) => GL.GetShader(shader, pname, out shaderParams);

        /// <inheritdoc/>
        public bool ShaderCompileSuccess(uint shaderID)
        {
            GetShader(shaderID, ShaderParameter.CompileStatus, out var statusCode);

            return statusCode >= 1;
        }

        /// <inheritdoc/>
        public string GetShaderInfoLog(uint shader) => GL.GetShaderInfoLog((int)shader);

        /// <inheritdoc/>
        public void DeleteShader(uint shader) => GL.DeleteShader(shader);

        /// <inheritdoc/>
        public uint GenVertexArray() => (uint)GL.GenVertexArray();

        /// <inheritdoc/>
        public void BufferSubData<T3>(BufferTarget target, IntPtr offset, uint size, ref T3 data)
            where T3 : struct => GL.BufferSubData(target, offset, (int)size, ref data);

        /// <inheritdoc/>
        public void DeleteVertexArray(uint arrays) => GL.DeleteVertexArray(arrays);

        /// <inheritdoc/>
        public void DeleteBuffer(uint buffers) => GL.DeleteBuffer(buffers);

        /// <inheritdoc/>
        public void BindBuffer(BufferTarget target, uint buffer) => GL.BindBuffer(target, buffer);

        /// <inheritdoc/>
        public void EnableVertexArrayAttrib(uint vaobj, uint index) => GL.EnableVertexArrayAttrib((int)vaobj, (int)index);

        /// <inheritdoc/>
        public void VertexAttribPointer(uint index, uint size, VertexAttribPointerType type, bool normalized, uint stride, uint offset)
            => GL.VertexAttribPointer(index, (int)size, type, normalized, (int)stride, (int)offset);

        /// <inheritdoc/>
        public uint GenBuffer() => (uint)GL.GenBuffer();

        /// <inheritdoc/>
        public void BufferData(BufferTarget target, uint size, IntPtr data, BufferUsageHint usage)
            => GL.BufferData(target, (int)size, data, usage);

        /// <inheritdoc/>
        public void BufferData<T2>(BufferTarget target, uint size, T2[] data, BufferUsageHint usage)
            where T2 : struct => GL.BufferData(target, (int)size, data, usage);

        /// <inheritdoc/>
        public void BindVertexArray(uint array) => GL.BindVertexArray(array);

        /// <inheritdoc/>
        public uint GenTexture() => (uint)GL.GenTexture();

        /// <inheritdoc/>
        public void DeleteTexture(uint textures) => GL.DeleteTexture(textures);

        /// <inheritdoc/>
        public void ObjectLabel(ObjectLabelIdentifier identifier, uint name, int length, string label)
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
