// <copyright file="InvalidRenderEffectsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Exceptions
{
    using System;
    using Raptor.Graphics;

    /// <summary>
    /// Thrown when an invalid <see cref="RenderEffects"/> value is used.
    /// </summary>
    public class InvalidRenderEffectsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
        /// </summary>
        public InvalidRenderEffectsException()
            : base($"{nameof(RenderEffects)} value invalid.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRenderEffectsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public InvalidRenderEffectsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
