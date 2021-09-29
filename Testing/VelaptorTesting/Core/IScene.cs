// <copyright file="IScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Core
{
    using System;
    using Velaptor;
    using Velaptor.UI;

    /// <summary>
    /// Represents a single scene that can be rendered to the screen.
    /// </summary>
    public interface IScene : IUpdatable, IDrawable, IDisposable
    {
        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The unique ID of the scene.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Gets a value indicating whether a value indicating if the scene has been loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the scene is the current active scene.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Loads the scene content.
        /// </summary>
        void Load();

        /// <summary>
        /// Adds a control to the scene to be updated and rendered.
        /// </summary>
        /// <param name="control">The control to add to the scene.</param>
        void AddControl(IControl control);

        /// <summary>
        /// Unloads the scene's content.
        /// </summary>
        void Unload();
    }
}
