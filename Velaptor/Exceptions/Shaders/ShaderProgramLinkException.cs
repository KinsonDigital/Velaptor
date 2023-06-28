// <copyright file="ShaderProgramLinkException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions.Shaders
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides an exception that is thrown when a shader fails to link.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public sealed class ShaderProgramLinkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramLinkException"/> class.
        /// </summary>
        public ShaderProgramLinkException()
            : this("An error occurred while linking a shader program.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramLinkException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ShaderProgramLinkException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramLinkException"/> class.
        /// </summary>
        /// <param name="shaderProgramId">The ID of the shader program that caused the exception to be thrown.</param>
        /// <param name="errorInformation">The error information related to the linking error.</param>
        public ShaderProgramLinkException(uint shaderProgramId, string? errorInformation)
            : this($"An error occurred while linking program with ID '{shaderProgramId}'\n{errorInformation}")
        {
            ShaderProgramId = shaderProgramId;
            ErrorInformation = errorInformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramLinkException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        public ShaderProgramLinkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgramLinkException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public ShaderProgramLinkException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the ID of the shader program that failed to link.
        /// </summary>
        /// <value>
        /// The ID of the shader program that caused the exception to be thrown.
        /// </value>
        public uint ShaderProgramId { get; }

        /// <summary>
        /// Gets the error information related to the linking error.
        /// </summary>
        /// <value>
        /// The error information that is given as a result of the shader program failing to link.
        /// </value>
        public string? ErrorInformation { get; }
    }
}
