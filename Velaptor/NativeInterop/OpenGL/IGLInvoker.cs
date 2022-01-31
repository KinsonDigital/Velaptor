// <copyright file="IGLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    using System;
    using System.Numerics;
    using Velaptor.OpenGL;

    // TODO: Create an OpenGLService method for binding and unbinding various buffers
    // Example: BindVBO() - This would call the raw GL function with the proper parameters to bind the buffer.
    // Once these are created, the bind and unbind calls in the GPUBufferBase class can be removed and just used directly.

    /// <summary>
    /// Invokes OpenGL calls.
    /// </summary>
    internal interface IGLInvoker : IDisposable
    {
        /// <summary>
        /// Invoked when there is an OpenGL related error.
        /// </summary>
        event EventHandler<GLErrorEventArgs> GLError;

        /// <summary>
        /// Sets up the error callback.
        /// </summary>
        /// <remarks>
        ///     This cannot be invoked until the OpenGL context has been created.
        /// </remarks>
        void SetupErrorCallback();

        /// <summary>
        /// Push a named debug group into the command stream.
        /// </summary>
        /// <param name="source">The source of the debug message.</param>
        /// <param name="id">The identifier of the message.</param>
        /// <param name="length">The length of the message to be sent to the debug output stream.</param>
        /// <param name="message">The string containing the message to be sent to the debug output stream.</param>
        void PushDebugGroup(GLDebugSource source, uint id, uint length, string message);

        /// <summary>
        /// Pop the active debug group.
        /// </summary>
        void PopDebugGroup();

        /// <summary>
        /// Label a named object identified within a namespace.
        /// </summary>
        /// <param name="identifier">The namespace from which the name of the object is allocated.</param>
        /// <param name="name">The name of the object to label.</param>
        /// <param name="length">The length of the label to be used for the object.</param>
        /// <param name="label">The label to assign to the object.</param>
        void ObjectLabel(GLObjectIdentifier identifier, uint name, uint length, string label);

        /// <summary>
        /// [requires: v1.0] Enable or disable server-side GL capabilities.
        /// </summary>
        /// <param name="cap">Specifies a symbolic constant indicating a GL capability.</param>
        void Enable(GLEnableCap cap);

        /// <summary>
        /// Enable or disable server-side GL capabilities.
        /// </summary>
        /// <param name="cap">Specifies a symbolic constant indicating a GL capability.</param>
        void Disable(GLEnableCap cap);

        /// <summary>
        /// [requires: v1.0] Specify pixel arithmetic.
        /// </summary>
        /// <param name="sfactor">
        ///     Specifies how the red, green, blue, and alpha source blending factors are computed.
        ///     The initial value is One.
        /// </param>
        /// <param name="dfactor">
        ///     Specifies how the red, green, blue, and alpha destination blending factors are
        ///     computed. The following symbolic constants are accepted: Zero, One, SrcColor,
        ///     OneMinusSrcColor, DstColor, OneMinusDstColor, SrcAlpha, OneMinusSrcAlpha, DstAlpha,
        ///     OneMinusDstAlpha. ConstantColor, OneMinusConstantColor, ConstantAlpha, and OneMinusConstantAlpha.
        ///     The initial value is Zero.
        /// </param>
        void BlendFunc(GLBlendingFactor sfactor, GLBlendingFactor dfactor);

        /// <summary>
        /// Clear buffers to preset values.
        /// </summary>
        /// <param name="mask">
        ///     Bitwise OR of masks that indicate the buffers to be cleared. The three masks
        ///     are ColorBufferBit, DepthBufferBit, and StencilBufferBit.
        /// </param>
        void Clear(GLClearBufferMask mask);

        /// <summary>
        /// [requires: v1.0] Specify clear values for the color buffers.
        /// </summary>
        /// <param name="red">Specify the red value used when the color buffers are cleared. The initial values are all 0.</param>
        /// <param name="green">Specify the green value used when the color buffers are cleared. The initial values are all 0.</param>
        /// <param name="blue">Specify the blue value used when the color buffers are cleared. The initial values are all 0.</param>
        /// <param name="alpha">Specify the alpha value used when the color buffers are cleared. The initial values are all 0.</param>
        void ClearColor(float red, float green, float blue, float alpha);

        /// <summary>
        /// [requires: v1.3] Select active texture unit.
        /// </summary>
        /// <param name="texture">
        ///     Specifies which texture unit to make active.The number of texture units is implementation
        ///     dependent, but must be at least 80. texture must be one of Texturei, where i
        ///     ranges from zero to the value of MaxCombinedTextureImageUnits minus one. The
        ///     initial value is Texture0.
        /// </param>
        void ActiveTexture(GLTextureUnit texture);

        /// <summary>
        /// [requires: v2.0] Returns the location of a uniform variable.
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="name">
        ///     Points to a null terminated string containing the
        ///     name of the uniform variable whose location is to be queried.
        /// </param>
        /// <returns>The location/ID of the uniform.</returns>
        int GetUniformLocation(uint program, string name);

        /// <summary>
        /// [requires: v1.1] Bind a named texture to a texturing target.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target to which the texture is bound.Must be one of Texture1D,
        ///     Texture2D, Texture3D, Texture1DArray, Texture2DArray, TextureRectangle, TextureCubeMap,
        ///     TextureCubeMapArray, TextureBuffer, Texture2DMultisample or Texture2DMultisampleArray.
        /// </param>
        /// <param name="texture">Specifies the name of a texture.</param>
        void BindTexture(GLTextureTarget target, uint texture);

        /// <summary>
        /// [requires: v1.1] Render primitives from array data.
        /// </summary>
        /// <param name="mode">
        ///     Specifies what kind of primitives to render. Symbolic constants Points, LineStrip,
        ///     LineLoop, Lines, LineStripAdjacency, LinesAdjacency, TriangleStrip, TriangleFan,
        ///     Triangles, TriangleStripAdjacency, TrianglesAdjacency and Patches are accepted.
        /// </param>
        /// <param name="count">Specifies the number of elements to be rendered.</param>
        /// <param name="type">Specifies the type of the values in indices. Must be one of UnsignedByte, UnsignedShort, or UnsignedInt.</param>
        /// <param name="indices">[length: COMPSIZE(count,type)] Specifies a pointer to the location where the indices are stored.</param>
        void DrawElements(GLPrimitiveType mode, uint count, GLDrawElementsType type, nint indices);

        /// <summary>
        /// [requires: v4.0 or ARB_gpu_shader_fp64|VERSION_4_0].
        /// </summary>
        /// <param name="location">Specifies the location of the uniform variable to be modified.</param>
        /// <param name="count">
        ///     For the vector (glUniform*v) commands, specifies the number of elements that are to be modified.
        ///     This should be 1 if the targeted uniform variable is not an array, and 1 or more if it is an array.
        /// </param>
        /// <param name="transpose">For the matrix commands, specifies whether or not to transpose the matrix as the values are loaded into the uniform variable.</param>
        /// <param name="matrix">The matrix data to send to the GPU.</param>
        void UniformMatrix4(int location, uint count, bool transpose, Matrix4x4 matrix);

        /// <summary>
        /// [requires: v2.0] Returns a parameter from a program object.
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="pname">
        ///     Specifies the object parameter. Accepted symbolic names are DeleteStatus, LinkStatus,
        ///     ValidateStatus, InfoLogLength, AttachedShaders, ActiveAtomicCounterBuffers, ActiveAttributes,
        ///     ActiveAttributeMaxLength, ActiveUniforms, ActiveUniformBlocks, ActiveUniformBlockMaxNameLength,
        ///     ActiveUniformMaxLength, ComputeWorkGroupSizeProgramBinaryLength, TransformFeedbackBufferMode,
        ///     TransformFeedbackVaryings, TransformFeedbackVaryingMaxLength, GeometryVerticesOut,
        ///     GeometryInputType, and GeometryOutputType.
        /// </param>
        /// <returns>
        ///     [length: COMPSIZE(pname)] Returns the requested object parameter.
        /// </returns>
        int GetProgram(uint program, GLProgramParameterName pname);

        /// <summary>
        /// Returns parameter values of type integer.
        /// </summary>
        /// <param name="pname">
        ///     Specifies the parameter value to be returned for non-indexed versions of glGet.
        ///     The symbolic constants in the list below are accepted.
        /// </param>
        /// <param name="data">Returns the values of the specified parameter.</param>
        /// <remarks>
        ///     Refer to http://docs.gl/gl4/glGet for more information.
        /// </remarks>
        void GetInteger(GLGetPName pname, int[] data);

        /// <summary>
        /// Returns the floating point values of the given parameter name.
        /// </summary>
        /// <param name="pname">The parameter name.</param>
        /// <param name="data">The values to return.</param>
        void GetFloat(GLGetPName pname, float[] data);

        /// <summary>
        /// Set the viewport.
        /// </summary>
        /// <param name="x">Specify the X coordinate of the lower left corner of the viewport rectangle, in pixels. The initial value is (0,0).</param>
        /// <param name="y">Specify the Y coordinate of the lower left corner of the viewport rectangle, in pixels. The initial value is (0,0).</param>
        /// <param name="width">
        ///     Specify the width of the viewport. When a GL context is first attached to a window,
        ///     width are set to the width dimension of that window.
        /// </param>
        /// <param name="height">
        ///     Specify the width of the viewport.When a GL context is first attached to a window,
        ///     width are set to the width dimension of that window.
        /// </param>
        void Viewport(int x, int y, uint width, uint height);

        /// <summary>
        /// [requires: v2.0] Installs a program object as part of current rendering state.
        /// </summary>
        /// <param name="program">Specifies the handle of the program object whose executables are to be used as part of current rendering state.</param>
        void UseProgram(uint program);

        /// <summary>
        /// [requires: v2.0] Deletes a program object.
        /// </summary>
        /// <param name="program">Specifies the program object to be deleted.</param>
        void DeleteProgram(uint program);

        /// <summary>
        /// [requires: v2.0] Creates a program object.
        /// </summary>
        /// <returns>The ID of the program.</returns>
        uint CreateProgram();

        /// <summary>
        /// [requires: v2.0] Attaches a shader object to a program object.
        /// </summary>
        /// <param name="program">Specifies the program object to which a shader object will be attached.</param>
        /// <param name="shader">Specifies the shader object that is to be attached.</param>
        void AttachShader(uint program, uint shader);

        /// <summary>
        /// [requires: v2.0] Links a program object.
        /// </summary>
        /// <param name="program">Specifies the handle of the program object to be linked.</param>
        void LinkProgram(uint program);

        /// <summary>
        /// [requires: v2.0] Returns the information log for a program object.
        /// </summary>
        /// <param name="program">Specifies the program object whose information log is to be queried.</param>
        /// <returns>The log information.</returns>
        string GetProgramInfoLog(uint program);

        /// <summary>
        /// [requires: v2.0] Creates a shader object.
        /// </summary>
        /// <param name="type">
        ///     Specifies the type of shader to be created.Must be one of ComputeShader, VertexShader,
        ///     TessControlShader, TessEvaluationShader, GeometryShader, or FragmentShader.
        /// </param>
        /// <returns>The ID of the shader.</returns>
        uint CreateShader(GLShaderType type);

        /// <summary>
        /// [requires: v2.0] Replaces the source code in a shader object.
        /// </summary>
        /// <param name="shader">Specifies the handle of the shader object whose source code is to be replaced.</param>
        /// <param name="sourceCode">
        ///     [length: count] Specifies an array of pointers to strings containing the source
        ///     code to be loaded into the shader.
        /// </param>
        void ShaderSource(uint shader, string sourceCode);

        /// <summary>
        /// [requires: v2.0] Detaches a shader object from a program object to which it is.
        /// </summary>
        /// <param name="program">Specifies the program object from which to detach the shader object.</param>
        /// <param name="shader">Specifies the shader object to be detached.</param>
        void DetachShader(uint program, uint shader);

        /// <summary>
        /// [requires: v2.0] Compiles a shader object.
        /// </summary>
        /// <param name="shader">Specifies the shader object to be compiled.</param>
        void CompileShader(uint shader);

        /// <summary>
        /// [requires: v2.0] Returns a parameter from a shader object.
        /// </summary>
        /// <param name="shader">Specifies the shader object to be queried.</param>
        /// <param name="pname">
        ///     Specifies the object parameter. Accepted symbolic names are ShaderType, DeleteStatus,
        ///     CompileStatus, InfoLogLength, ShaderSourceLength.
        /// </param>
        /// <returns>
        ///     [length: COMPSIZE(pname)] Returns the requested object parameter.
        /// </returns>
        int GetShader(uint shader, GLShaderParameter pname);

        /// <summary>
        /// [requires: v2.0] Returns the information log for a shader object.
        /// </summary>
        /// <param name="shader">Specifies the shader object whose information log is to be queried.</param>
        /// <returns>The log information.</returns>
        string GetShaderInfoLog(uint shader);

        /// <summary>
        /// [requires: v2.0] Deletes a shader object.
        /// </summary>
        /// <param name="shader">Specifies the shader object to be deleted.</param>
        void DeleteShader(uint shader);

        /// <summary>
        /// [requires: v3.0 or ARB_vertex_array_object|VERSION_3_0] Generate vertex array object names.
        /// </summary>
        /// <returns>The vertex object name.</returns>
        uint GenVertexArray();

        /// <summary>
        /// Creates and initializes a buffer object's data store.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target buffer object. The symbolic constant must be ArrayBuffer,
        ///     AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer, DispatchIndirectBuffer,
        ///     ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer, QueryBuffer, ShaderStorageBuffer,
        ///     TextureBuffer, TransformFeedbackBuffer, or UniformBuffer.
        /// </param>
        /// <param name="data">
        ///     The data that will be copied into the data
        ///     store for initialization, or Null if no data is to be copied.
        /// </param>
        /// <param name="usage">
        ///     Specifies the expected usage pattern of the data store. The symbolic constant
        ///     must be StreamDraw, StreamRead, StreamCopy, StaticDraw, StaticRead, StaticCopy,
        ///     DynamicDraw, DynamicRead, or DynamicCopy.
        /// </param>
        void BufferData(GLBufferTarget target, float[] data, GLBufferUsageHint usage);

        /// <summary>
        /// [requires: v1.5] Creates and initializes a buffer object's data store.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target buffer object. The symbolic constant must be ArrayBuffer,
        ///     AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer, DispatchIndirectBuffer,
        ///     ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer, QueryBuffer, ShaderStorageBuffer,
        ///     TextureBuffer, TransformFeedbackBuffer, or UniformBuffer.
        /// </param>
        /// <param name="size">Specifies the size in bytes of the buffer object's new data store.</param>
        /// <param name="data">
        ///     [length: size] Specifies a pointer to data that will be copied into the data
        ///     store for initialization, or Null if no data is to be copied.
        /// </param>
        /// <param name="usage">
        ///     Specifies the expected usage pattern of the data store. The symbolic constant
        ///     must be StreamDraw, StreamRead, StreamCopy, StaticDraw, StaticRead, StaticCopy,
        ///     DynamicDraw, DynamicRead, or DynamicCopy.
        /// </param>
        void BufferData(GLBufferTarget target, uint size, nint data, GLBufferUsageHint usage);

        /// <summary>
        /// Creates and initializes a buffer object's data store.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target buffer object. The symbolic constant must be ArrayBuffer,
        ///     AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer, DispatchIndirectBuffer,
        ///     ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer, QueryBuffer, ShaderStorageBuffer,
        ///     TextureBuffer, TransformFeedbackBuffer, or UniformBuffer.
        /// </param>
        /// <param name="data">
        ///     The data that will be copied into the data
        ///     store for initialization, or Null if no data is to be copied.
        /// </param>
        /// <param name="usage">
        ///     Specifies the expected usage pattern of the data store. The symbolic constant
        ///     must be StreamDraw, StreamRead, StreamCopy, StaticDraw, StaticRead, StaticCopy,
        ///     DynamicDraw, DynamicRead, or DynamicCopy.
        /// </param>
        void BufferData(GLBufferTarget target, uint[] data, GLBufferUsageHint usage);

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

        /// <summary>
        /// [requires: v1.5] Updates a subset of a buffer object's data store.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target buffer object. The symbolic constant must be ArrayBuffer,
        ///     AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer, DispatchIndirectBuffer,
        ///     ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer, QueryBuffer, ShaderStorageBuffer,
        ///     TextureBuffer, TransformFeedbackBuffer, or UniformBuffer.
        /// </param>
        /// <param name="offset">Specifies the offset into the buffer object's data store where data replacement will begin, measured in bytes.</param>
        /// <param name="size">Specifies the size in bytes of the data store region being replaced.</param>
        /// <param name="data">The new data that will be copied into the data store.</param>
        void BufferSubData(GLBufferTarget target, nint offset, nuint size, float[] data);

        /// <summary>
        /// [requires: v3.0 or ARB_vertex_array_object|VERSION_3_0] Delete vertex array objects.
        /// </summary>
        /// <param name="arrays">[length: n] Specifies the address of an array containing the n names of the objects to be deleted.</param>
        void DeleteVertexArray(uint arrays);

        /// <summary>
        /// [requires: v1.5] Delete named buffer objects.
        /// </summary>
        /// <param name="buffers">[length: n] Specifies an array of buffer objects to be deleted.</param>
        void DeleteBuffer(uint buffers);

        /// <summary>
        /// [requires: v1.5] Bind a named buffer object.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target to which the buffer object is bound. The symbolic constant
        ///     must be ArrayBuffer, AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer,
        ///     DispatchIndirectBuffer, ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer,
        ///     QueryBuffer, ShaderStorageBuffer, TextureBuffer, TransformFeedbackBuffer, or
        ///     UniformBuffer.
        /// </param>
        /// <param name="buffer">Specifies the name of a buffer object.</param>
        void BindBuffer(GLBufferTarget target, uint buffer);

        /// <summary>
        /// Enable or disable a generic vertex attribute array.
        /// </summary>
        /// <param name="index">Specifies the index of the generic vertex attribute to be enabled or disabled.</param>
        void EnableVertexAttribArray(uint index);

        /// <summary>
        /// [requires: v2.0] Define an array of generic vertex attribute data.
        /// </summary>
        /// <param name="index">Specifies the index of the generic vertex attribute to be modified.</param>
        /// <param name="size">
        ///     Specifies the number of components per generic vertex attribute. Must be 1, 2,
        ///     3, 4. Additionally, the symbolic constant Bgra is accepted by glVertexAttribPointer.
        ///     The initial value is 4.
        /// </param>
        /// <param name="type">
        ///     Specifies the data type of each component in the array. The symbolic constants
        ///     Byte, UnsignedByte, Short, UnsignedShort, Int, and UnsignedInt are accepted by
        ///     glVertexAttribPointer and glVertexAttribIPointer. Additionally HalfFloat, Float,
        ///     Double, Fixed, Int2101010Rev, UnsignedInt2101010Rev and UnsignedInt10F11F11FRev
        ///     are accepted by glVertexAttribPointer. Double is also accepted by glVertexAttribLPointer
        ///     and is the only token accepted by the type parameter for that function. The initial
        ///     value is Float.
        /// </param>
        /// <param name="normalized">
        ///     For glVertexAttribPointer, specifies whether or not fixed-point data values should be
        ///     normalized <see langword="true"/> or converted directly as fixed-point values <see langword="false"/> when they
        ///     are accessed.
        /// </param>
        /// <param name="stride">
        ///     Specifies the byte offset between consecutive generic vertex attributes. If stride
        ///     is 0, the generic vertex attributes are understood to be tightly packed in the
        ///     array. The initial value is 0.
        /// </param>
        /// <param name="offset">The byte offset into the buffer object's data store.</param>
        void VertexAttribPointer(uint index, int size, GLVertexAttribPointerType type, bool normalized, uint stride, uint offset);

        /// <summary>
        /// [requires: v1.5] Generate buffer object names.
        /// </summary>
        /// <returns>The ID of the buffer object.</returns>
        uint GenBuffer();

        /// <summary>
        /// [requires: v3.0 or ARB_vertex_array_object|VERSION_3_0] Bind a vertex array object.
        /// </summary>
        /// <param name="array">Specifies the name of the vertex array to bind.</param>
        void BindVertexArray(uint array);

        /// <summary>
        /// Modifies the value of a uniform variable or a uniform variable array. The location of the uniform
        /// variable to be modified is specified by location, which should be a value returned by GetUniformLocation.
        /// Operates on the program object that was made part of current state by calling <see cref="UseProgram"/>.
        /// </summary>
        /// <param name="location">Specifies the location of the uniform variable to be modified.</param>
        /// <param name="value">
        /// For the vector and matrix commands, specifies a pointer to an array of count values that
        /// will be used to update the specified uniform variable.
        /// </param>
        void Uniform1(int location, int value);

        /// <summary>
        /// [requires: v1.1] Generate texture names.
        /// </summary>
        /// <returns>The ID of the texture.</returns>
        uint GenTexture();

        /// <summary>
        /// [requires: v1.1] Delete named textures.
        /// </summary>
        /// <param name="textures">[length: n] Specifies an array of textures to be deleted.</param>
        void DeleteTexture(uint textures);

        /// <summary>
        /// [requires: v1.0] Set texture parameters.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target texture, which must be either Texture1D, Texture2D, Texture3D,
        ///     Texture1DArray, Texture2DArray, TextureRectangle, or TextureCubeMap.
        /// </param>
        /// <param name="pname">
        ///     Specifies the symbolic name of a single-valued texture parameter. pname can be
        ///     one of the following: DepthStencilTextureMode, TextureBaseLevel, TextureCompareFunc,
        ///     TextureCompareMode, TextureLodBias, TextureMinFilter, TextureMagFilter, TextureMinLod,
        ///     TextureMaxLod, TextureMaxLevel, TextureSwizzleR, TextureSwizzleG, TextureSwizzleB,
        ///     TextureSwizzleA, TextureWrapS, TextureWrapT, or TextureWrapR. For the vector
        ///     commands (glTexParameter*v), pname can also be one of TextureBorderColor or TextureSwizzleRgba.
        /// </param>
        /// <param name="param">For the scalar commands, specifies the value of pname.</param>
        void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureWrapMode param);

        /// <summary>
        /// [requires: v1.0] Set texture parameters.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target texture, which must be either Texture1D, Texture2D, Texture3D,
        ///     Texture1DArray, Texture2DArray, TextureRectangle, or TextureCubeMap.
        /// </param>
        /// <param name="pname">
        ///     Specifies the symbolic name of a single-valued texture parameter. pname can be
        ///     one of the following: DepthStencilTextureMode, TextureBaseLevel, TextureCompareFunc,
        ///     TextureCompareMode, TextureLodBias, TextureMinFilter, TextureMagFilter, TextureMinLod,
        ///     TextureMaxLod, TextureMaxLevel, TextureSwizzleR, TextureSwizzleG, TextureSwizzleB,
        ///     TextureSwizzleA, TextureWrapS, TextureWrapT, or TextureWrapR. For the vector
        ///     commands (glTexParameter*v), pname can also be one of TextureBorderColor or TextureSwizzleRgba.
        /// </param>
        /// <param name="param">For the scalar commands, specifies the value of pname.</param>
        void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMinFilter param);

        /// <summary>
        /// [requires: v1.0] Set texture parameters.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target texture, which must be either Texture1D, Texture2D, Texture3D,
        ///     Texture1DArray, Texture2DArray, TextureRectangle, or TextureCubeMap.
        /// </param>
        /// <param name="pname">
        ///     Specifies the symbolic name of a single-valued texture parameter. pname can be
        ///     one of the following: DepthStencilTextureMode, TextureBaseLevel, TextureCompareFunc,
        ///     TextureCompareMode, TextureLodBias, TextureMinFilter, TextureMagFilter, TextureMinLod,
        ///     TextureMaxLod, TextureMaxLevel, TextureSwizzleR, TextureSwizzleG, TextureSwizzleB,
        ///     TextureSwizzleA, TextureWrapS, TextureWrapT, or TextureWrapR. For the vector
        ///     commands (glTexParameter*v), pname can also be one of TextureBorderColor or TextureSwizzleRgba.
        /// </param>
        /// <param name="param">For the scalar commands, specifies the value of pname.</param>
        void TexParameter(GLTextureTarget target, GLTextureParameterName pname, GLTextureMagFilter param);

        /// <summary>
        /// [requires: v1.0] Specify a two-dimensional texture image.
        /// </summary>
        /// <typeparam name="T">The type of data to load to the GPU vertex buffer.</typeparam>
        /// <param name="target">
        ///     Specifies the target texture. Must be Texture2D, ProxyTexture2D, Texture1DArray,
        ///     ProxyTexture1DArray, TextureRectangle, ProxyTextureRectangle, TextureCubeMapPositiveX,
        ///     TextureCubeMapNegativeX, TextureCubeMapPositiveY, TextureCubeMapNegativeY, TextureCubeMapPositiveZ,
        ///     TextureCubeMapNegativeZ, or ProxyTextureCubeMap.
        /// </param>
        /// <param name="level">
        ///     Specifies the level-of-detail number. Level 0 is the base image level. Level
        ///     n is the nth mipmap reduction image. If target is TextureRectangle or ProxyTextureRectangle,
        ///     level must be 0.
        /// </param>
        /// <param name="internalformat">
        ///     Specifies the number of color components in the texture. Must be one of base
        ///     internal formats given in Table 1, one of the sized internal formats given in
        ///     Table 2, or one of the compressed internal formats given in Table 3, below.
        /// </param>
        /// <param name="width">
        ///     Specifies the width of the texture image. All implementations support texture
        ///     images that are at least 1024 texels wide.
        /// </param>
        /// <param name="height">
        ///     Specifies the height of the texture image, or the number of layers in a texture
        ///     array, in the case of the Texture1DArray and ProxyTexture1DArray targets. All
        ///     implementations support 2D texture images that are at least 1024 texels high,
        ///     and texture arrays that are at least 256 layers deep.
        /// </param>
        /// <param name="border">This value must be 0.</param>
        /// <param name="format">
        ///     Specifies the format of the pixel data. The following symbolic values are accepted:
        ///     Red, Rg, Rgb, Bgr, Rgba, Bgra, RedInteger, RgInteger, RgbInteger, BgrInteger,
        ///     RgbaInteger, BgraInteger, StencilIndex, DepthComponent, DepthStencil.
        /// </param>
        /// <param name="type">
        ///     Specifies the data type of the pixel data. The following symbolic values are
        ///     accepted: UnsignedByte, Byte, UnsignedShort, Short, UnsignedInt, Int, Float,
        ///     UnsignedByte332, UnsignedByte233Rev, UnsignedShort565, UnsignedShort565Rev, UnsignedShort4444,
        ///     UnsignedShort4444Rev, UnsignedShort5551, UnsignedShort1555Rev, UnsignedInt8888,
        ///     UnsignedInt8888Rev, UnsignedInt1010102, and UnsignedInt2101010Rev.
        /// </param>
        /// <param name="pixels">[length: COMPSIZE(format,type,width,height)] Specifies a pointer to the image data in memory.</param>
        void TexImage2D<T>(GLTextureTarget target, int level, GLInternalFormat internalformat, uint width, uint height, int border, GLPixelFormat format, GLPixelType type, byte[] pixels)
            where T : unmanaged;
    }
}
