// <copyright file="SceneBaseFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using Velaptor.Content;
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
    /// <param name="contentLoader">Mocked content loader.</param>
    /// <param name="reactableFactory">Mocked factory.</param>
    public SceneBaseFake(IContentLoader contentLoader, IReactableFactory reactableFactory)
        : base(contentLoader, reactableFactory)
    {
    }
}
