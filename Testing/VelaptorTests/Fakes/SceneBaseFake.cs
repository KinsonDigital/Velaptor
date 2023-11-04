// <copyright file="SceneBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.Factories;
using Velaptor.Scene;

/// <summary>
/// Used for testing the abstract <see cref="SceneBase"/> class.
/// </summary>
internal sealed class SceneBaseFake : SceneBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBaseFake"/> class.
    /// </summary>
    /// <param name="reactableFactory">Mocked factory.</param>
    public SceneBaseFake(IReactableFactory reactableFactory)
        : base(reactableFactory)
    {
    }
}
