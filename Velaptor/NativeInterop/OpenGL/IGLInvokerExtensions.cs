// <copyright file="IGLInvokerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    using System.Drawing;
    using System.Numerics;

    // TODO: Convert these into literal extension methods instead
    // This will prevent having to instantiate the implementation and injecting

    /// <summary>
    /// Provides OpenGL extensions/helper methods to improve OpenGL related functionality.
    /// </summary>
    internal interface IGLInvokerExtensions
    {
        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        /// <returns>The size of the viewport.</returns>
        Size GetViewPortSize();

        /// <summary>
        /// Sets the size of the view port.
        /// </summary>
        /// <param name="size">The vector representing the width and height of the viewport.</param>
        void SetViewPortSize(Size size);

        /// <summary>
        /// Gets the position of the viewport.
        /// </summary>
        /// <returns>The position of the viewport.</returns>
        Vector2 GetViewPortPosition();

        /// <summary>
        /// Returns a value indicating if the program linking process was successful.
        /// </summary>
        /// <param name="program">The ID of the program to check.</param>
        /// <returns><see langword="true"/> if the linking was successful.</returns>
        bool LinkProgramSuccess(uint program);

        /// <summary>
        /// Returns a value indicating if the shader was compiled successfully.
        /// </summary>
        /// <param name="shaderId">The ID of the shader to check.</param>
        /// <returns><see langword="true"/> if the shader compiled successfully.</returns>
        bool ShaderCompileSuccess(uint shaderId);
    }
}
