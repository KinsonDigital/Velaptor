// <copyright file="GLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using Silk.NET.OpenGL;
    using Silk.NET.Windowing;
    using Velaptor.OpenGL;
    using Velaptor.Reactables;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Invokes OpenGL calls.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class GLInvoker : IGLInvoker
    {
        // ReSharper disable InconsistentNaming
#pragma warning disable SA1310
        private const int API_ID_RECOMPILE_FRAGMENT_SHADER = 2;
        private const int API_ID_RECOMPILE_VERTEX_SHADER = 131218;
#pragma warning restore SA1310

        // ReSharper restore InconsistentNaming
        private static readonly Queue<string> OpenGLCallStack = new ();
        private static DebugProc? debugCallback;
        private readonly IDisposable glContextUnsubscriber;
        private readonly ILoggingService loggingService;
        private bool isDisposed;

        // ReSharper disable once MemberInitializerValueIgnored
        private GL gl = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLInvoker"/> class.
        /// </summary>
        /// <param name="glContextReactable">
        ///     The OpenGL context reactable to subscribe to get a push notification
        ///     that the OpenGL context has been created.
        /// </param>
        /// <param name="loggingService">Logs messages to the console and files.</param>
        public GLInvoker(IReactable<GLContextData> glContextReactable, ILoggingService loggingService)
        {
            this.glContextUnsubscriber = glContextReactable.Subscribe(new Reactor<GLContextData>(
                onNext: data =>
                {
                    if (data.Data is IWindow window)
                    {
                        this.gl = window.CreateOpenGL();
                    }
                    else
                    {
                        var exceptionMessage =
                            $"The parameter '{nameof(data)}' of the '{nameof(Reactor<object>.OnNext)}()' action delegate must be of type '{nameof(IWindow)}'.";
                        exceptionMessage +=
                            $"{Environment.NewLine}\t{nameof(OpenGLContextReactable)} subscription location: {nameof(GLInvoker)}.Ctor()";

                        throw new Exception(exceptionMessage);
                    }
                },
                onCompleted: () =>
                {
                    this.glContextUnsubscriber?.Dispose();
                }));

            this.loggingService = loggingService;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GLInvoker"/> class.
        /// </summary>
        ~GLInvoker() => Dispose();

        /// <inheritdoc/>
        public event EventHandler<GLErrorEventArgs>? GLError;

        // ReSharper disable once UnusedMember.Global
#pragma warning disable SA1600
        /// <summary>
        /// Gets the list of OpenGL function calls.
        /// </summary>
        public static string[] GLCallStack => OpenGLCallStack.ToArray();
#pragma warning restore SA1600

        /// <inheritdoc/>
        public void SetupErrorCallback()
        {
            if (debugCallback != null)
            {
                return;
            }

            debugCallback = DebugCallback;

            /*NOTE:
                 * This is here to help prevent an issue with an obscure System.ExecutionException from occurring.
                 * The garbage collector performs a collect on the delegate passed into GL.DebugMessageCallback()
                 * without the native system knowing about it which causes this exception. The GC.KeepAlive()
                 * method tells the garbage collector to not collect the delegate to prevent this from happening.
                 */
            GC.KeepAlive(debugCallback);

            this.gl.DebugMessageCallback(debugCallback, Marshal.StringToHGlobalAnsi(string.Empty));
        }

        /// <inheritdoc/>
        public void PushDebugGroup(GLDebugSource source, uint id, uint length, string message)
        {
            AddToGLCallStack(nameof(PushDebugGroup));
            this.gl.PushDebugGroup((DebugSource)source, id, length, message);
        }

        /// <inheritdoc/>
        public void PopDebugGroup()
        {
            AddToGLCallStack(nameof(PopDebugGroup));
            this.gl.PopDebugGroup();
        }

        /// <inheritdoc/>
        public void ObjectLabel(GLObjectIdentifier identifier, uint name, uint length, string label)
        {
            AddToGLCallStack(nameof(ObjectLabel));
            this.gl.ObjectLabel((ObjectIdentifier)identifier, name, length, label);
        }

        /// <inheritdoc/>
        public void Enable(GLEnableCap cap)
        {
            AddToGLCallStack($"{nameof(Enable)} {cap.ToString()}");
            this.gl.Enable((EnableCap)cap);
        }

        /// <inheritdoc/>
        public void Disable(GLEnableCap cap)
        {
            AddToGLCallStack(nameof(Disable));
            this.gl.Disable((EnableCap)cap);
        }

        /// <inheritdoc/>
        public void BlendFunc(GLBlendingFactor sfactor, GLBlendingFactor dfactor)
        {
            AddToGLCallStack(nameof(BlendFunc));
            this.gl.BlendFunc((BlendingFactor)sfactor, (BlendingFactor)dfactor);
        }

        /// <inheritdoc/>
        public void Clear(GLClearBufferMask mask)
        {
            AddToGLCallStack(nameof(Clear));
            this.gl.Clear((uint)mask);
        }

        /// <inheritdoc/>
        public void ClearColor(float red, float green, float blue, float alpha)
        {
            AddToGLCallStack(nameof(ClearColor));
            this.gl.ClearColor(red, green, blue, alpha);
        }

        /// <inheritdoc/>
        public void ActiveTexture(GLTextureUnit texture)
        {
            AddToGLCallStack(nameof(ActiveTexture));
            this.gl.ActiveTexture((TextureUnit)texture);
        }

        /// <inheritdoc/>
        public int GetUniformLocation(uint program, string name)
        {
            AddToGLCallStack(nameof(Enable));
            return this.gl.GetUniformLocation(program, name);
        }

        /// <inheritdoc/>
        public void BindTexture(GLTextureTarget target, uint texture)
        {
            AddToGLCallStack($"{(texture <= 0 ? "Un" : string.Empty)}{nameof(BindTexture)}");
            this.gl.BindTexture((TextureTarget)target, texture);
        }

        /// <inheritdoc/>
        public void DrawElements(GLPrimitiveType mode, uint count, GLDrawElementsType type, nint indices)
        {
            AddToGLCallStack(nameof(DrawElements));
            unsafe
            {
                this.gl.DrawElements((PrimitiveType)mode, count, (DrawElementsType)type, (void*)indices);
            }
        }

        /// <inheritdoc/>
        public void UniformMatrix4(int location, uint count, bool transpose, Matrix4x4 matrix)
        {
            AddToGLCallStack(nameof(UniformMatrix4));
            unsafe
            {
                this.gl.UniformMatrix4(location, count, transpose, (float*)&matrix);
            }
        }

        /// <inheritdoc/>
        public int GetProgram(uint program, GLProgramParameterName pname)
        {
            AddToGLCallStack(nameof(GetProgram));
            this.gl.GetProgram(program, (ProgramPropertyARB)pname, out var programParams);

            return programParams;
        }

        /// <inheritdoc/>
        public void GetInteger(GLGetPName pname, int[] data)
        {
            AddToGLCallStack(nameof(GetInteger));
            this.gl.GetInteger((GetPName)pname, data);
        }

        /// <inheritdoc/>
        public void GetFloat(GLGetPName pname, float[] data)
        {
            AddToGLCallStack(nameof(GetFloat));
            this.gl.GetFloat((GetPName)pname, data);
        }

        /// <inheritdoc/>
        public void Viewport(int x, int y, uint width, uint height)
        {
            AddToGLCallStack(nameof(Viewport));
            this.gl.Viewport(x, y, width, height);
        }

        /// <inheritdoc/>
        public void UseProgram(uint program)
        {
            AddToGLCallStack(nameof(UseProgram));
            this.gl.UseProgram(program);
        }

        /// <inheritdoc/>
        public void DeleteProgram(uint program)
        {
            AddToGLCallStack(nameof(DeleteProgram));
            this.gl.DeleteProgram(program);
        }

        /// <inheritdoc/>
        public uint CreateProgram()
        {
            AddToGLCallStack(nameof(CreateProgram));
            return this.gl.CreateProgram();
        }

        /// <inheritdoc/>
        public void AttachShader(uint program, uint shader)
        {
            AddToGLCallStack(nameof(AttachShader));
            this.gl.AttachShader(program, shader);
        }

        /// <inheritdoc/>
        public void LinkProgram(uint program)
        {
            AddToGLCallStack(nameof(LinkProgram));
            this.gl.LinkProgram(program);
        }

        /// <inheritdoc/>
        public string GetProgramInfoLog(uint program)
        {
            AddToGLCallStack(nameof(GetProgramInfoLog));
            return this.gl.GetProgramInfoLog(program);
        }

        /// <inheritdoc/>
        public uint CreateShader(GLShaderType type)
        {
            AddToGLCallStack(nameof(CreateShader));
            return this.gl.CreateShader((ShaderType)type);
        }

        /// <inheritdoc/>
        public void ShaderSource(uint shader, string sourceCode)
        {
            AddToGLCallStack(nameof(ShaderSource));
            this.gl.ShaderSource(shader, sourceCode);
        }

        /// <inheritdoc/>
        public void DetachShader(uint program, uint shader)
        {
            AddToGLCallStack(nameof(DetachShader));
            this.gl.DetachShader(program, shader);
        }

        /// <inheritdoc/>
        public void CompileShader(uint shader)
        {
            AddToGLCallStack(nameof(CompileShader));
            this.gl.CompileShader(shader);
        }

        /// <inheritdoc/>
        public int GetShader(uint shader, GLShaderParameter pname)
        {
            AddToGLCallStack(nameof(GetShader));
            this.gl.GetShader(shader, (ShaderParameterName)pname, out var shaderParams);

            return shaderParams;
        }

        /// <inheritdoc/>
        public string GetShaderInfoLog(uint shader)
        {
            AddToGLCallStack(nameof(GetShaderInfoLog));
            return this.gl.GetShaderInfoLog(shader);
        }

        /// <inheritdoc/>
        public void DeleteShader(uint shader)
        {
            AddToGLCallStack(nameof(DeleteShader));
            this.gl.DeleteShader(shader);
        }

        /// <inheritdoc/>
        public uint GenVertexArray()
        {
            AddToGLCallStack(nameof(GenVertexArray));
            return this.gl.GenVertexArray();
        }

        /// <inheritdoc/>
        public void BufferData(GLBufferTarget target, float[] data, GLBufferUsageHint usage)
        {
            var dataSpan = new ReadOnlySpan<float>(data);
            var size = (nuint)(sizeof(float) * data.Length);

            AddToGLCallStack($"{nameof(BufferData)}(GLBufferTarget target, uint size, float[] data, GLBufferUsageHint usage)");
            this.gl.BufferData((BufferTargetARB)target, size, dataSpan, (BufferUsageARB)usage);
        }

        /// <inheritdoc/>
        public void BufferData(GLBufferTarget target, uint[] data, GLBufferUsageHint usage)
        {
            var dataSpan = new ReadOnlySpan<uint>(data);
            var size = (nuint)(sizeof(uint) * data.Length);

            AddToGLCallStack($"{nameof(BufferData)}(GLBufferTarget target, uint[] data, GLBufferUsageHint usage)");
            this.gl.BufferData((BufferTargetARB)target, size, dataSpan, (BufferUsageARB)usage);
        }

        /// <inheritdoc/>
        public void BufferData(GLBufferTarget target, uint size, nint data, GLBufferUsageHint usage)
        {
            AddToGLCallStack($"{nameof(BufferData)}(GLBufferTarget target, uint size, nint data, GLBufferUsageHint usage)");
            unsafe
            {
                this.gl.BufferData((BufferTargetARB)target, size, (void*)data, (BufferUsageARB)usage);
            }
        }

        /// <inheritdoc/>
        public void BufferSubData(GLBufferTarget target, nint offset, nuint size, float[] data)
        {
            AddToGLCallStack($"{nameof(BufferSubData)}(GLBufferTarget target, nint offset, nuint size, float[] data)");
            unsafe
            {
                fixed (void* dataPtr = data)
                {
                    this.gl.BufferSubData((BufferTargetARB)target, offset, size, dataPtr);
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteVertexArray(uint arrays)
        {
            AddToGLCallStack("Delete VAO");
            this.gl.DeleteVertexArray(arrays);
        }

        /// <inheritdoc/>
        public void DeleteBuffer(uint buffers)
        {
            AddToGLCallStack(nameof(DeleteBuffer));
            this.gl.DeleteBuffer(buffers);
        }

        /// <inheritdoc/>
        public void BindBuffer(GLBufferTarget target, uint buffer)
        {
            var firstSection = $"{(buffer <= 0u ? "Un" : string.Empty)}";
            var secondSection = target == GLBufferTarget.ArrayBuffer ? "VBO" : "EBO";

            AddToGLCallStack($"{firstSection}Bind {secondSection}");
            this.gl.BindBuffer((BufferTargetARB)target, buffer);
        }

        /// <inheritdoc/>
        public void EnableVertexAttribArray(uint index)
        {
            AddToGLCallStack(nameof(EnableVertexAttribArray));
            this.gl.EnableVertexAttribArray(index);
        }

        /// <inheritdoc/>
        public void VertexAttribPointer(uint index, int size, GLVertexAttribPointerType type, bool normalized, uint stride, uint offset)
        {
            AddToGLCallStack(nameof(VertexAttribPointer));
            unsafe
            {
                this.gl.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, stride, (void*)offset);
            }
        }

        /// <inheritdoc/>
        public uint GenBuffer()
        {
            AddToGLCallStack(nameof(GenBuffer));
            return this.gl.GenBuffer();
        }

        /// <inheritdoc/>
        public void BindVertexArray(uint array)
        {
            AddToGLCallStack($"{(array <= 0 ? "Unb" : "B")}ind VAO");
            this.gl.BindVertexArray(array);
        }

        /// <inheritdoc/>
        public void Uniform1(int location, int value)
        {
            AddToGLCallStack($"{nameof(Uniform1)} - Location({location}) - Value({value})");
            this.gl.Uniform1(location, value);
        }

        /// <inheritdoc/>
        public uint GenTexture()
        {
            AddToGLCallStack(nameof(GenTexture));
            return this.gl.GenTexture();
        }

        /// <inheritdoc/>
        public void DeleteTexture(uint textures)
        {
            AddToGLCallStack(nameof(DeleteTexture));
            this.gl.DeleteTexture(textures);
        }

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureWrapMode param)
        {
            AddToGLCallStack($"{nameof(TexParameter)}(GLTextureTarget target, GLTextureParameterName pname, GLTextureWrapMode param)");
            this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);
        }

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMinFilter param)
        {
            AddToGLCallStack($"{nameof(TexParameter)}(GLTextureTarget target, GLTextureParameterName pname, GLTextureMinFilter param)");
            this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);
        }

        /// <inheritdoc/>
        public void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMagFilter param)
        {
            AddToGLCallStack($"{nameof(TexParameter)}(GLTextureTarget target, GLTextureParameterName pname, GLTextureMagFilter param)");
            this.gl.TexParameter((TextureTarget)target, (TextureParameterName)pname, (int)param);
        }

        /// <inheritdoc/>
        public void TexImage2D<T>(GLTextureTarget target, int level, GLInternalFormat internalformat, uint width, uint height, int border, GLPixelFormat format, GLPixelType type, byte[] pixels)
            where T : unmanaged
        {
            AddToGLCallStack(nameof(TexImage2D));
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
            if (this.isDisposed)
            {
                return;
            }

            debugCallback = null;
            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Invoked when there is an OpenGL related error.
        /// </summary>
        /// <param name="source">The debug source.</param>
        /// <param name="type">The debug type.</param>
        /// <param name="id">The ID of the error or message.</param>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="length">The length of the message.</param>
        /// <param name="message">The error message.</param>
        /// <param name="userParam">The OpenGL parameter related to the error.</param>
        private void DebugCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
        {
            var openGLMessage = Marshal.PtrToStringAnsi(message);

            openGLMessage += $"{Environment.NewLine}\tSrc: {source}";
            openGLMessage += $"{Environment.NewLine}\tType: {type}";
            openGLMessage += $"{Environment.NewLine}\tID: {id}";
            openGLMessage += $"{Environment.NewLine}\tSeverity: {severity}";
            openGLMessage += $"{Environment.NewLine}\tLength: {length}";
            openGLMessage += $"{Environment.NewLine}\tUser Param: {Marshal.PtrToStringAnsi(userParam)}";

            // Ignore warnings about shader recompilation
            if (id is API_ID_RECOMPILE_VERTEX_SHADER or API_ID_RECOMPILE_FRAGMENT_SHADER)
            {
                return;
            }

            if (severity == GLEnum.NoError)
            {
                this.loggingService.Warning(openGLMessage);
            }
            else
            {
                if (severity != GLEnum.DebugSeverityNotification)
                {
                    this.loggingService.Error(openGLMessage);
                    this.GLError?.Invoke(this, new GLErrorEventArgs(openGLMessage));
                }
            }
        }

        // TODO: Add Debug precompiler logic here to only run this method code if in debug
        private void AddToGLCallStack(string glFunctionName)
        {
            OpenGLCallStack.Enqueue(glFunctionName);

            if (OpenGLCallStack.Count >= 200)
            {
                OpenGLCallStack.Dequeue();
            }
        }
    }
}
