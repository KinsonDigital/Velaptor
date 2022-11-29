// <copyright file="ReactableFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.Reactables.Core;

/// <summary>
/// Used for testing the abstract <see cref="Reactable{TData}"/> class.
/// </summary>
/// <typeparam name="T">The type of notification to set.</typeparam>
public class ReactableFake<T> : Reactable<T>
{
}
