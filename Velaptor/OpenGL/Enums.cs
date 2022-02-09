// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using Velaptor.NativeInterop.OpenGL;

    /// <summary>
    /// Represents the type of GPU buffer.
    /// </summary>
    internal enum BufferType
    {
        /// <summary>
        ///  A vertex buffer object.
        /// </summary>
        VertexBufferObject,

        /// <summary>
        /// An index array object.
        /// </summary>
        /// <remarks>
        ///     Also known as an EBO (Element Buffer Object) or
        ///     IBO (Index Buffer Object).
        /// </remarks>
        IndexArrayObject,
    }

    /// <summary>
    /// Specifies the program parameter.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.GetProgram(uint, GLProgramParameterName)"/>.
    /// </para>
    /// </summary>
    internal enum GLProgramParameterName
    {
        /// <summary>
        /// OpenGL enum name GL_LINK_STATUS.
        /// </summary>
        LinkStatus = 35714,
    }

    /// <summary>
    /// Specifies the shader parameter.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.GetShader(uint, GLShaderParameter)"/>.
    /// </para>
    /// </summary>
    internal enum GLShaderParameter
    {
        /// <summary>
        /// OpenGL enum name GL_COMPILE_STATUS.
        /// </summary>
        CompileStatus = 35713,
    }

    /// <summary>
    /// Specifies the target to which the buffer object is bound for the buffer data.
    /// <para>Used in the following OpenGL function calls.</para>
    /// <list type="bullet">
    ///     <item><see cref="IGLInvoker.BufferData(Velaptor.OpenGL.GLBufferTarget,float[],Velaptor.OpenGL.GLBufferUsageHint)"/></item>
    ///     <item><see cref="IGLInvoker.BufferSubData"/></item>
    /// </list>
    /// </summary>
    internal enum GLBufferTarget
    {
        /// <summary>
        /// OpenGL enum name GL_ARRAY_BUFFER.
        /// </summary>
        ArrayBuffer = 34962,

        /// <summary>
        /// OpenGL enum name GL_ELEMENT_ARRAY_BUFFER.
        /// </summary>
        ElementArrayBuffer = 34963,
    }

    /// <summary>
    /// Specifies the expected usage pattern of the data store.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.BufferData(Velaptor.OpenGL.GLBufferTarget,float[],Velaptor.OpenGL.GLBufferUsageHint)"/>.
    /// </para>
    /// </summary>
    internal enum GLBufferUsageHint
    {
        /// <summary>
        /// OpenGL enum name GL_DYNAMIC_DRAW.
        /// </summary>
        DynamicDraw = 35048,

        /// <summary>
        /// OpenGL enum name GL_STATIC_DRAW.
        /// </summary>
        StaticDraw = 35044,
    }

    /// <summary>
    /// The namespace from which the name of the object is allocated.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.ObjectLabel(GLObjectIdentifier, uint, uint, string)"/>.
    /// </para>
    /// </summary>
    internal enum GLObjectIdentifier
    {
        /// <summary>
        /// OpenGL enum name GL_TEXTURE.
        /// </summary>
        Texture = 5890,

        /// <summary>
        /// OpenGL enum name GL_SHADER.
        /// </summary>
        Shader = 33505,

        /// <summary>
        /// OpenGL enum name GL_PROGRAM.
        /// </summary>
        Program = 33506,

        /// <summary>
        /// OpenGL enum name GL_VERTEX_ARRAY.
        /// </summary>
        VertexArray = 32884,

        /// <summary>
        /// OpenGL enum name GL_BUFFER.
        /// </summary>
        Buffer = 33504,
    }

    /// <summary>
    /// Specifies the number of color components in the texture.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}"/>.
    /// </para>
    /// </summary>
    internal enum GLInternalFormat
    {
        /// <summary>
        /// OpenGL enum name GL_RGBA.
        /// </summary>
        Rgba = 6408,
    }

    /// <summary>
    /// Specifies the format of the pixel data.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}"/>.
    /// </para>
    /// </summary>
    internal enum GLPixelFormat
    {
        /// <summary>
        /// OpenGL enum name GL_RGBA.
        /// </summary>
        Rgba = 6408,
    }

    /// <summary>
    /// Specifies the format of the pixel data.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}"/>.
    /// </para>
    /// </summary>
    internal enum GLPixelType
    {
        /// <summary>
        /// OpenGL enum name GL_UNSIGNED_BYTE.
        /// </summary>
        UnsignedByte = 5121,
    }

    /// <summary>
    /// Specifies a symbolic constant indicating a GL capability.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.Enable(GLEnableCap)"/>.
    /// </para>
    /// </summary>
    internal enum GLEnableCap
    {
        /// <summary>
        /// OpenGL enum name GL_BLEND.
        /// </summary>
        Blend = 3042,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_OUTPUT_SYNCHRONOUS.
        /// </summary>
        DebugOutputSynchronous = 33346,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_OUTPUT.
        /// </summary>
        DebugOutput = 37600,
    }

    /// <summary>
    /// Specifies how the red, green, blue, and alpha source blending factors are computed.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.BlendFunc(GLBlendingFactor, GLBlendingFactor)"/>.
    /// </para>
    /// </summary>
    internal enum GLBlendingFactor
    {
        /// <summary>
        /// OpenGL enum name GL_SRC_ALPHA.
        /// </summary>
        SrcAlpha = 770,

        /// <summary>
        /// OpenGL enum name GL_SRC_COLOR.
        /// </summary>
        SrcColor = 768,

        /// <summary>
        /// OpenGL enum name GL_ONE_MINUS_SRC_ALPHA.
        /// </summary>
        OneMinusSrcAlpha = 771,
    }

    /// <summary>
    /// Specifies which texture unit to make active.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.ActiveTexture(GLTextureUnit)"/>.
    /// </para>
    /// </summary>
    internal enum GLTextureUnit
    {
        /// <summary>
        /// OpenGL enum name GL_TEXTURE0.
        /// </summary>
        Texture0 = 33984,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE1.
        /// </summary>
        Texture1,
    }

    /// <summary>
    /// Specifies the target for which the texture is bound.
    /// <para>Used in the following OpenGL function calls.</para>
    /// <list type="bullet">
    ///     <item><see cref="IGLInvoker.BindTexture(GLTextureTarget, uint)"/></item>
    ///     <item><see cref="IGLInvoker.TexParameter(Velaptor.OpenGL.GLTextureTarget,Velaptor.OpenGL.GLTextureParameterName,Velaptor.OpenGL.GLTextureWrapMode)"/></item>
    ///     <item><see cref="IGLInvoker.TexImage2D{T}"/></item>
    /// </list>
    /// </summary>
    internal enum GLTextureTarget
    {
        /// <summary>
        /// OpenGL enum name GL_TEXTURE_2D.
        /// </summary>
        Texture2D = 3553,
    }

    /// <summary>
    /// Specifies what kind of primitives to render.
    /// <para>
    ///     Unused in the <see cref="IGLInvoker.DrawElements(GLPrimitiveType, uint, GLDrawElementsType, System.IntPtr)"/>.
    /// </para>
    /// </summary>
    internal enum GLPrimitiveType
    {
        /// <summary>
        /// OpenGL enum name GL_TRIANGLES.
        /// </summary>
        Triangles = 4,
    }

    /// <summary>
    /// Specifies the type of values in the indices when drawing elements.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.DrawElements(GLPrimitiveType, uint, GLDrawElementsType, System.IntPtr)"/>.
    /// </para>
    /// </summary>
    internal enum GLDrawElementsType
    {
        /// <summary>
        /// OpenGL enum name GL_UNSIGNED_INT.
        /// </summary>
        UnsignedInt = 5125,
    }

    /// <summary>
    /// Specifies the parameter value to be returned for non-indexed versions of
    /// the <see cref="IGLInvoker.GetInteger(GLGetPName, int[])"/> and <see cref="IGLInvoker.GetFloat(GLGetPName, float[])"/> OpenGL calls.
    /// </summary>
    internal enum GLGetPName
    {
        /// <summary>
        /// OpenGL enum name GL_VIEWPORT.
        /// </summary>
        Viewport = 2978,

        /// <summary>
        /// OpenGL enum name GL_COLOR_CLEAR_VALUE.
        /// </summary>
        ColorClearValue = 3106,
    }

    /// <summary>
    /// Specifies the type of shader to be created.
    /// </summary>
    internal enum GLShaderType
    {
        /// <summary>
        /// OpenGL enum name GL_FRAGMENT_SHADER.
        /// </summary>
        FragmentShader = 35632,

        /// <summary>
        /// OpenGL enum name GL_VERTEX_SHADER.
        /// </summary>
        VertexShader = 35633,
    }

    /// <summary>
    /// Specifies the data type of each component in the array for the <see cref="IGLInvoker.VertexAttribPointer(uint, int, GLVertexAttribPointerType, bool, uint, uint)"/>.
    /// </summary>
    internal enum GLVertexAttribPointerType
    {
        /// <summary>
        /// OpenGL enum name GL_FLOAT.
        /// </summary>
        Float = 5126,
    }

    /// <summary>
    /// Used in GL.TexParameter, GL.TexParameterI and 6 other functions.
    /// </summary>
    internal enum GLTextureParameterName
    {
        /// <summary>
        /// OpenGL enum name GL_TEXTURE_MAG_FILTER.
        /// </summary>
        TextureMagFilter = 10240,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE_MIN_FILTER.
        /// </summary>
        TextureMinFilter = 10241,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE_WRAP_S.
        /// </summary>
        TextureWrapS = 10242,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE_WRAP_T.
        /// </summary>
        TextureWrapT = 10243,
    }

    /// <summary>
    /// Used in GL.Arb.DebugMessageControl, GL.Arb.DebugMessageInsert and 6 other functions.
    /// </summary>
    internal enum GLDebugSource
    {
        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_APPLICATION.
        /// </summary>
        DebugSourceApplication = 33354,
    }

    /// <summary>
    /// Bitwise OR of masks that indicate the buffers to be cleared using the follow masks.
    /// <list type="bullet">
    ///     <item><see cref="ColorBufferBit"/></item>
    /// </list>
    /// </summary>
    internal enum GLClearBufferMask
    {
        /// <summary>
        /// Indicates the buffers currently enabled for color writing.
        /// </summary>
        ColorBufferBit = 0x4000,
    }

    /// <summary>
    /// Defines behavior of how to render textures that are outside of
    /// the NDC (Normalized Device Coordinates) of 0 to 1.
    /// </summary>
    internal enum GLTextureWrapMode
    {
        /// <summary>
        /// Clamps the coordinates between 0 and 1.  The result is that higher coordinates
        /// become clamped to the edge, resulting in a stretched edge pattern.
        /// </summary>
        ClampToEdge = 33071,
    }

    /// <summary>
    /// Determines how pixels colors are dealt with when mapping them to texture coordinates.
    /// </summary>
    internal enum GLTextureMagFilter
    {
        /// <summary>
        /// Returns the weighted average of the four texture elements that are closest to the specified
        /// texture coordinates. These can include items wrapped or repeated from other parts of a texture,
        /// depending on the values of <see cref="GLTextureParameterName.TextureWrapS"/> and
        /// <see cref="GLTextureParameterName.TextureWrapT"/>, and on the exact mapping.
        /// </summary>
        Linear = 9729,
    }

    /// <summary>
    /// Determines how pixels colors are dealt with when mapping them to texture coordinates.
    /// </summary>
    internal enum GLTextureMinFilter
    {
        /// <summary>
        /// Returns the weighted average of the four texture elements that are closest to the specified
        /// texture coordinates. These can include items wrapped or repeated from other parts of a texture,
        /// depending on the values of <see cref="GLTextureParameterName.TextureWrapS"/> and
        /// <see cref="GLTextureParameterName.TextureWrapT"/>, and on the exact mapping.
        /// </summary>
        Linear = 9729,
    }
}
