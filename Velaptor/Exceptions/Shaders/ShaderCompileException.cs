// <copyright file="ShaderCompileException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions.Shaders
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an exception that is thrown when shader compilation fails.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public sealed class ShaderCompileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
        /// </summary>
        public ShaderCompileException()
            : this("An error occurred while compiling a shader.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ShaderCompileException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
        /// </summary>
        /// <param name="shaderId">The ID of the shader that failed to compile.</param>
        /// <param name="errorInformation">The error information related to the shader compilation failure.</param>
        public ShaderCompileException(uint shaderId, string? errorInformation)
            : this($"An error occurred while compiling shader with ID '{shaderId}'\n{errorInformation}.")
        {
            ShaderId = shaderId;
            ErrorInformation = errorInformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        public ShaderCompileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public ShaderCompileException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the ID of the OpenGL shader that caused the exception to be thrown.
        /// </summary>
        /// <value>
        /// The shader ID that caused the compilation error.
        /// </value>
        public uint ShaderId { get; }

        /// <summary>
        /// Gets the error information related to the exception.
        /// </summary>
        /// <value>
        /// The error information that is given as a result of shader compilation failure.
        /// </value>
        public string? ErrorInformation { get; }
    }
}
