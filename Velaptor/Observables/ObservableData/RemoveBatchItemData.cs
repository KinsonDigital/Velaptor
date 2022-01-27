// <copyright file="RemoveBatchItemData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.ObservableData
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds da ta for the <see cref="IReactor{T}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct RemoveBatchItemData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveBatchItemData"/> struct.
        /// </summary>
        /// <param name="id">The ID of the batch item to remove.</param>
        /// <remarks>This is actually an OpenGL texture ID.</remarks>
        public RemoveBatchItemData(uint id) => Id = id;

        /// <summary>
        /// Gets the texture ID to send with the push notification.
        /// </summary>
        public uint Id { get; }
    }
}
