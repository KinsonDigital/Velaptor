// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.GLFW;

/// <summary>
/// Error codes, used in the GLFW error callback.
/// </summary>
internal enum GlfwErrorCode
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
    /// This should never happen in the bindings, due to the
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
    /// A platform-specific error occurred that doesn't fit into a more specific category.
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
