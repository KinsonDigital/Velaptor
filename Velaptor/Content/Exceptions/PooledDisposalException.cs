// <copyright file="PooledDisposalException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when a pooled <see cref="IContent"/> item is being disposed
    /// when calling <see cref="IDisposable.Dispose"/>.
    /// </summary>
    public class PooledDisposalException : Exception
    {
        private static new readonly string Message =
            @$"Cannot manually dispose of '{nameof(IContent)}' objects.
            \nTo override manual disposal of pooled objects, set the '{nameof(IContent.IsPooled)}' to a value of 'false'.
            \n!!WARNING!! It is not recommended to do this due to the object probably being used somewhere else in the application.
            \nThe benefit of object pooling is to improve performance and reusability of objects.";

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledDisposalException"/> class.
        /// </summary>
        public PooledDisposalException()
            : base(Message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledDisposalException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PooledDisposalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledDisposalException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public PooledDisposalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
