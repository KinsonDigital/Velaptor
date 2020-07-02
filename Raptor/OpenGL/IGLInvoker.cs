// <copyright file="IGLInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;

    /// <summary>
    /// Invokes OpenGL calls.
    /// </summary>
    public interface IGLInvoker
    {
        /// <summary>
        /// [requires: v1.0] Enable or disable server-side GL capabilities.
        /// </summary>
        /// <param name="cap">Specifies a symbolic constant indicating a GL capability.</param>
        void Enable(EnableCap cap);

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
        void BlendFunc(BlendingFactor sfactor, BlendingFactor dfactor);

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
        void ActiveTexture(TextureUnit texture);

        /// <summary>
        /// [requires: v2.0] Returns the location of a uniform variable.
        /// </summary>
        /// <param name="program">Specifies the program object to be queried.</param>
        /// <param name="name">
        ///     Points to a null terminated string containing the
        ///     name of the uniform variable whose location is to be queried.
        /// </param>
        /// <returns>The location/ID of the uniform.</returns>
        uint GetUniformLocation(uint program, string name);

        /// <summary>
        /// [requires: v1.1] Bind a named texture to a texturing target.
        /// </summary>
        /// <param name="target">
        ///     Specifies the target to which the texture is bound.Must be one of Texture1D,
        ///     Texture2D, Texture3D, Texture1DArray, Texture2DArray, TextureRectangle, TextureCubeMap,
        ///     TextureCubeMapArray, TextureBuffer, Texture2DMultisample or Texture2DMultisampleArray.
        /// </param>
        /// <param name="texture">Specifies the name of a texture.</param>
        void BindTexture(TextureTarget target, uint texture);

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
        void DrawElements(PrimitiveType mode, uint count, DrawElementsType type, IntPtr indices);

        /// <summary>
        /// [requires: v4.0 or ARB_gpu_shader_fp64|VERSION_4_0].
        /// </summary>
        /// <param name="location">Specifies the location of the uniform variable to be modified.</param>
        /// <param name="transpose">For the matrix commands, specifies whether to transpose the matrix as the values are loaded into the uniform variable.</param>
        /// <param name="matrix">The matrix data to send to the GPU.</param>
        void UniformMatrix4(uint location, bool transpose, ref Matrix4 matrix);

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
        /// <param name="programParams">[length: COMPSIZE(pname)] Returns the requested object parameter.</param>
        void GetProgram(uint program, GetProgramParameterName pname, out int programParams);

        /// <summary>
        /// Returns a value indicating if the program linking process was successful.
        /// </summary>
        /// <param name="program">The ID of the program to check.</param>
        /// <returns>True if the linking was successful.</returns>
        bool LinkProgramSuccess(uint program);

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
        uint CreateShader(ShaderType type);

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
        /// <param name="shaderParams">[length: COMPSIZE(pname)] Returns the requested object parameter.</param>
        void GetShader(uint shader, ShaderParameter pname, out int shaderParams);

        /// <summary>
        /// Returns a value indicating if the shader was compiled successfully.
        /// </summary>
        /// <param name="shaderID">The ID of the shader to check.</param>
        /// <returns>True if the shader compiled successfully.</returns>
        bool ShaderCompileSuccess(uint shaderID);

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
        /// [requires: v1.5] Updates a subset of a buffer object's data store.
        /// </summary>
        /// <typeparam name="T3">The type of data in the buffer.</typeparam>
        /// <param name="target">
        ///     Specifies the target buffer object. The symbolic constant must be ArrayBuffer,
        ///     AtomicCounterBuffer, CopyReadBuffer, CopyWriteBuffer, DrawIndirectBuffer, DispatchIndirectBuffer,
        ///     ElementArrayBuffer, PixelPackBuffer, PixelUnpackBuffer, QueryBuffer, ShaderStorageBuffer,
        ///     TextureBuffer, TransformFeedbackBuffer, or UniformBuffer.
        /// </param>
        /// <param name="offset">Specifies the offset into the buffer object's data store where data replacement will begin, measured in bytes.</param>
        /// <param name="size">Specifies the size in bytes of the data store region being replaced.</param>
        /// <param name="data">[length: size] Specifies a pointer to the new data that will be copied into the data store.</param>
        void BufferSubData<T3>(BufferTarget target, IntPtr offset, uint size, ref T3 data)
            where T3 : struct;

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
        void BindBuffer(BufferTarget target, uint buffer);

        /// <summary>
        /// [requires: v4.5 or ARB_direct_state_access|VERSION_4_5].
        /// </summary>
        /// <param name="vaobj">The ID of the vertex array object.</param>
        /// <param name="index">The index/location of the attribute.</param>
        void EnableVertexArrayAttrib(uint vaobj, uint index);

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
        ///     For glVertexAttribPointer, specifies whether fixed-point data values should be
        ///     normalized (True) or converted directly as fixed-point values (False) when they
        ///     are accessed.
        /// </param>
        /// <param name="stride">
        ///     Specifies the byte offset between consecutive generic vertex attributes. If stride
        ///     is 0, the generic vertex attributes are understood to be tightly packed in the
        ///     array. The initial value is 0.
        /// </param>
        /// <param name="offset">The byte offset into the buffer object's data store.</param>
        void VertexAttribPointer(uint index, uint size, VertexAttribPointerType type, bool normalized, uint stride, uint offset);

        /// <summary>
        /// [requires: v1.5] Generate buffer object names.
        /// </summary>
        /// <returns>The ID of the buffer object.</returns>
        uint GenBuffer();

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
        void BufferData(BufferTarget target, uint size, IntPtr data, BufferUsageHint usage);

        /// <summary>
        /// [requires: v1.5] Creates and initializes a buffer object's data store.
        /// </summary>
        /// <typeparam name="T2">The type of data in the buffer.</typeparam>
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
        void BufferData<T2>(BufferTarget target, uint size, T2[] data, BufferUsageHint usage)
            where T2 : struct;

        /// <summary>
        /// [requires: v3.0 or ARB_vertex_array_object|VERSION_3_0] Bind a vertex array object.
        /// </summary>
        /// <param name="array">Specifies the name of the vertex array to bind.</param>
        void BindVertexArray(uint array);

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
        /// [requires: v4.3 or KHR_debug|VERSION_4_3] Label a named object identified within a namespace.
        /// </summary>
        /// <param name="identifier">The namespace from which the name of the object is allocated.</param>
        /// <param name="name">The name of the object to label.</param>
        /// <param name="length">The length of the label to be used for the object.</param>
        /// <param name="label">[length: COMPSIZE(label,length)] The address of a string containing the label to assign to the object.</param>
        void ObjectLabel(ObjectLabelIdentifier identifier, uint name, int length, string label);

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
        void TexParameter(TextureTarget target, TextureParameterName pname, int param);

        /// <summary>
        /// [requires: v1.0] Specify a two-dimensional texture image.
        /// </summary>
        /// <typeparam name="T8">The type of data to load to the GPU vertex buffer.</typeparam>
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
        void TexImage2D<T8>(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, T8[] pixels)
            where T8 : struct;

        /// <summary>
        /// [requires: v4.3 or KHR_debug|VERSION_4_3] Specify a callback to receive debugging
        /// messages from the GL.
        /// </summary>
        /// <param name="callback">The address of a callback function that will be called when a debug message is generated.</param>
        /// <param name="userParam">A user supplied pointer that will be passed on each invocation of callback.</param>
        void DebugMessageCallback(DebugProc callback, IntPtr userParam);
    }
}
