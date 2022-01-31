// <copyright file="GLContextData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.ObservableData
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier//

    /// <summary>
    /// Holds data for the <see cref="IReactable{T}"/> reactable.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct GLContextData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GLContextData"/> struct.
        /// </summary>
        /// <param name="data">The context data.</param>
        public GLContextData(object data) => Data = data;

        /// <summary>
        /// Gets the data.
        /// </summary>
        public object Data { get; }
    }
}
