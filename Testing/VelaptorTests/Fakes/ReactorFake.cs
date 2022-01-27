// <copyright file="ReactorFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes
{
    using Velaptor.Observables.Core;

    /// <summary>
    /// Used for test the abstract <see cref="Reactor{TData}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of notification to set.</typeparam>
    public class ReactorFake<T> : Reactor<T>
    {
        public override void PushNotification(T data) => throw new System.NotImplementedException();
    }
}
