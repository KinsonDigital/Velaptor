// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using Raptor.NativeInterop;

    /// <summary>
    /// Specifies the program parameter.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.GetProgram(uint, GLProgramParameterName, out int)"/>.
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
    ///     Used for the OpenGL function <see cref="IGLInvoker.GetShader(uint, GLShaderParameter, out int)"/>.
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
    ///     <item><see cref="IGLInvoker.BufferData(GLBufferTarget, uint, nint, GLBufferUsageHint)"/></item>
    ///     <item><see cref="IGLInvoker.BufferSubData{T}(GLBufferTarget, nint, nuint, ref T)"/></item>
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
    ///     Used for the OpenGL function <see cref="IGLInvoker.BufferData(GLBufferTarget, uint, nint, GLBufferUsageHint)"/>.
    /// </para>
    /// </summary>
    internal enum GLBufferUsageHint
    {
        /// <summary>
        /// OpenGL enum name GL_DYNAMIC_DRAW.
        /// </summary>
        DynamicDraw = 35048,
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
    }

    /// <summary>
    /// Specifies the number of color components in the texture.
    /// <para>
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}(GLTextureTarget, int, GLInternalFormat, uint, uint, int, GLPixelFormat, GLPixelType, void*)"/>.
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
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}(GLTextureTarget, int, GLInternalFormat, uint, uint, int, GLPixelFormat, GLPixelType, void*)"/>.
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
    ///     Used for the OpenGL function <see cref="IGLInvoker.TexImage2D{T}(GLTextureTarget, int, GLInternalFormat, uint, uint, int, GLPixelFormat, GLPixelType, void*)"/>.
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
    /// Specifices how the red, green, blue, and alpha source blending factors are computed.
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

        /// <summary>
        /// OpenGL enum name GL_TEXTURE2.
        /// </summary>
        Texture2,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE3.
        /// </summary>
        Texture3,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE.
        /// </summary>
        Texture4,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE5.
        /// </summary>
        Texture5,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE6.
        /// </summary>
        Texture6,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE7.
        /// </summary>
        Texture7,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE8.
        /// </summary>
        Texture8,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE9.
        /// </summary>
        Texture9,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE10.
        /// </summary>
        Texture10,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE11.
        /// </summary>
        Texture11,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE12.
        /// </summary>
        Texture12,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE13.
        /// </summary>
        Texture13,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE14.
        /// </summary>
        Texture14,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE15.
        /// </summary>
        Texture15,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE16.
        /// </summary>
        Texture16,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE17.
        /// </summary>
        Texture17,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE18.
        /// </summary>
        Texture18,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE19.
        /// </summary>
        Texture19,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE20.
        /// </summary>
        Texture20,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE21.
        /// </summary>
        Texture21,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE22.
        /// </summary>
        Texture22,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE23.
        /// </summary>
        Texture23,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE24.
        /// </summary>
        Texture24,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE25.
        /// </summary>
        Texture25,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE26.
        /// </summary>
        Texture26,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE27.
        /// </summary>
        Texture27,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE28.
        /// </summary>
        Texture28,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE29.
        /// </summary>
        Texture29,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE30.
        /// </summary>
        Texture30,

        /// <summary>
        /// OpenGL enum name GL_TEXTURE31.
        /// </summary>
        Texture31,
    }

    /// <summary>
    /// Specifies the target for which the texture is bound.
    /// <para>Used in the following OpenGL function calls.</para>
    /// <list type="bullet">
    ///     <item><see cref="IGLInvoker.BindTexture(GLTextureTarget, uint)"/></item>
    ///     <item><see cref="IGLInvoker.TexParameter(GLTextureTarget, GLTextureParameterName, int)"/></item>
    ///     <item><see cref="IGLInvoker.TexImage2D{T}(GLTextureTarget, int, GLInternalFormat, uint, uint, int, GLPixelFormat, GLPixelType, void*)"/></item>
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
    ///     Unsed in the <see cref="IGLInvoker.DrawElements(GLPrimitiveType, uint, GLDrawElementsType, System.IntPtr)"/>.
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
        /// OpenGL enum name GL_DEBUG_SOURCE_API.
        /// </summary>
        DebugSourceApi = 33350,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_WINDOW_SYSTEM.
        /// </summary>
        DebugSourceWindowSystem = 33351,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_SHADER_COMPILER.
        /// </summary>
        DebugSourceShaderCompiler = 33352,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_THIRD_PARTY.
        /// </summary>
        DebugSourceThirdParty = 33353,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_APPLICATION.
        /// </summary>
        DebugSourceApplication = 33354,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SOURCE_OTHER.
        /// </summary>
        DebugSourceOther = 33355,
    }

    /// <summary>
    /// Used in GL.Arb.DebugMessageControl, GL.Arb.DebugMessageInsert and 6 other functions.
    /// </summary>
    internal enum GLDebugType
    {
        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_ERROR.
        /// </summary>
        DebugTypeError = 33356,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR.
        /// </summary>
        DebugTypeDeprecatedBehavior = 33357,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR.
        /// </summary>
        DebugTypeUndefinedBehavior = 33358,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_PORTABILITY.
        /// </summary>
        DebugTypePortability = 33359,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_PERFORMANCE.
        /// </summary>
        DebugTypePerformance = 33360,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_OTHER.
        /// </summary>
        DebugTypeOther = 33361,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_MARKER.
        /// </summary>
        DebugTypeMarker = 33384,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_PUSH_GROUP.
        /// </summary>
        DebugTypePushGroup = 33385,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_TYPE_POP_GROUP.
        /// </summary>
        DebugTypePopGroup = 33386,
    }

    /// <summary>
    /// Used in GL.Arb.DebugMessageControl, GL.Arb.DebugMessageInsert and 6 other functions.
    /// </summary>
    internal enum GLDebugSeverity
    {
        /// <summary>
        /// OpenGL enum name GL_DEBUG_SEVERITY_NOTIFICATION
        /// </summary>
        DebugSeverityNotification = 33387,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SEVERITY_HIGH.
        /// </summary>
        DebugSeverityHigh = 37190,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SEVERITY_MEDIUM.
        /// </summary>
        DebugSeverityMedium = 37191,

        /// <summary>
        /// OpenGL enum name GL_DEBUG_SEVERITY_LOW.
        /// </summary>
        DebugSeverityLow = 37192,
    }

    /// <summary>
    /// Error codes, used in the GLFW error callback.
    /// </summary>
    internal enum GLFWErrorCode
    {
        /// <summary>
        /// Everything is running as intended. Yay!
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Called a function before calling IGlfw.Init. Initialize GLFW and then try again.
        /// </summary>
        NotInitialized = 65537,

        /// <summary>
        /// No OpenGL/OpenGL ES context on this thread.
        /// </summary>
        NoContext = 65538,

        /// <summary>
        /// Used an invalid enum value on a function.
        /// </summary>
        /// <remarks>
        /// This should hopefully never happen in the bindings, due to the
        /// added type safety of C# enums VS. GLFW's native #defines.
        /// </remarks>
        InvalidEnum = 65539,

        /// <summary>
        /// Called a function with an invalid argument.
        /// </summary>
        /// <remarks>
        ///     This can happen if you request an OpenGL version that doesn't exist, like 2.7.
        ///     If you request a version of OpenGL that exists, but isn't supported by this graphics
        ///     card, it will return VersionUnavailable instead.
        /// </remarks>
        InvalidValue = 65540,

        /// <summary>
        /// A memory allocation failed on GLFW's end.
        /// </summary>
        /// <remarks>
        ///     Report this to the GLFW issue tracker if encountered.
        /// </remarks>
        OutOfMemory = 65541,

        /// <summary>
        /// The requested API is not available on the system.
        /// </summary>
        ApiUnavailable = 65542,

        /// <summary>
        /// The requested OpenGL version is not available on the system.
        /// </summary>
        VersionUnavailable = 65543,

        /// <summary>
        /// A platform-specific error occurred that doesn't fit into any more specific category.
        /// </summary>
        /// <remarks>
        ///     Report this to the GLFW issue tracker if encountered.
        /// </remarks>
        PlatformError = 65544,

        /// <summary>
        /// The requested format is unavailable.
        /// </summary>
        /// <remarks>
        ///     If emitted during window creation, the requested pixel format isn't available.
        ///     If emitted when using the clipboard, the contents of the clipboard couldn't be
        ///     converted to the requested format.
        /// </remarks>
        FormatUnavailable = 65545,

        /// <summary>
        /// There is no OpenGL/OpenGL ES context attached to this window.
        /// </summary>
        NoWindowContext = 65546,
    }

    /// <summary>
    /// Bitwise OR of masks that indicate the buffers to be cleared using the follow masks.
    /// <list type="bullet"
    ///     <item><see cref="ColorBufferBit"/></item>
    ///     <item><see cref="DepthBufferBit"/></item>
    ///     <item><see cref="StencilBufferBit"/></item>
    /// </list>
    /// </summary>
    internal enum GLClearBufferMask
    {
        /// <summary>
        /// Indicates the depth buffer.
        /// </summary>
        DepthBufferBit = 0x100,

        /// <summary>
        /// Indicates the stencil buffer.
        /// </summary>
        StencilBufferBit = 0x400,

        /// <summary>
        /// Indicates the buffers currently enabled for color writing.
        /// </summary>
        ColorBufferBit = 0x4000,

        /// <summary>
        /// Unknown.
        /// </summary>
        CoverageBufferBitNV = 0x8000,
    }

    /// <summary>
    /// Defineds behavior of how to render textures that are outside of
    /// the NDC (Normalized Device Coordinates) of 0 to 1.
    /// </summary>
    internal enum GLTextureWrapMode
    {
        /// <summary>
        /// Clamps the coordinates between 0 and 1.  The result is that higher coordinates
        /// become clamped tot he edge, resulting in a streched edge pattern.
        /// </summary>
        ClampToEdge = 33071,
    }

    /// <summary>
    /// Determines how pixels colors are dealt with when mapping them to texture coordinates.
    /// </summary>
    internal enum GLTextureMagFilter
    {
        /// <summary>
        /// Returns the value of the texture element that is nearest (in Manhattan distance) to the specified texture coordinates.
        /// </summary>
        Nearest = 9728,

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
        /// Returns the value of the texture element that is nearest (in Manhattan distance) to the specified texture coordinates.
        /// </summary>
        Nearest = 9728,

        /// <summary>
        /// Returns the weighted average of the four texture elements that are closest to the specified
        /// texture coordinates. These can include items wrapped or repeated from other parts of a texture,
        /// depending on the values of <see cref="GLTextureParameterName.TextureWrapS"/> and
        /// <see cref="GLTextureParameterName.TextureWrapT"/>, and on the exact mapping.
        /// </summary>
        Linear = 9729,
    }
}
