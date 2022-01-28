// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.ObservableData
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Holds data for the <see cref="IReactor{T}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct DisposeSoundData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeSoundData"/> struct.
        /// </summary>
        /// <param name="soundId">The ID of the sound.</param>
        public DisposeSoundData(uint soundId) => SoundId = soundId;

        /// <summary>
        /// Gets the sound ID.
        /// </summary>
        public uint SoundId { get; }
    }
}
