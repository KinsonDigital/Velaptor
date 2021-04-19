// <copyright file="ObservableFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Fakes
{
    using Raptor.Observables.Core;

    /// <summary>
    /// Used for test the abstract <see cref="Observable{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of notification to set.</typeparam>
    public class ObservableFake<T> : Observable<T>
    {
    }
}
