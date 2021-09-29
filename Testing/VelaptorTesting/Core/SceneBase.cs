// <copyright file="SceneBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTesting.Core
{
    using System;
    using System.Collections.Generic;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.UI;

    /// <summary>
    /// A base scene to be used for creating new custom scenes.
    /// </summary>
    public abstract class SceneBase : IScene
    {
        private readonly List<IControl> controls = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneBase"/> class.
        /// </summary>
        /// <param name="contentLoader">Loads content for a scene.</param>
        protected SceneBase(IContentLoader contentLoader)
        {
            ContentLoader = contentLoader;
            IsActive = false;
        }

        /// <inheritdoc cref="IScene.Name"/>
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc cref="IScene.ID"/>
        public Guid ID { get; } = Guid.NewGuid();

        /// <inheritdoc cref="IScene.Name"/>
        public bool IsLoaded { get; private set; }

        /// <inheritdoc cref="IScene.IsActive"/>
        public bool IsActive { get; set; }

        protected IContentLoader ContentLoader { get; }

        /// <summary>
        /// <inheritdoc cref="IScene"/>
        /// </summary>
        public virtual void Load()
        {
            if (IsLoaded)
            {
                return;
            }

            IsLoaded = true;
        }

        public void AddControl(IControl control)
        {
            this.controls.Add(control);
        }

        /// <summary>
        /// <inheritdoc cref="IScene"/>
        /// </summary>
        public virtual void Unload()
        {
            IsLoaded = false;
        }

        /// <inheritdoc cref="IScene"/>
        public virtual void Update(FrameTime frameTime)
        {
            if (IsLoaded is false || IsActive)
            {
                return;
            }

            foreach (var control in this.controls)
            {
                control.Update(frameTime);
            }
        }

        /// <inheritdoc cref="IScene"/>
        public virtual void Render(ISpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
            {
                throw new ArgumentNullException(nameof(spriteBatch), "The parameter must not be null.");
            }

            if (IsLoaded is false)
            {
                return;
            }

            foreach (var control in this.controls)
            {
                control.Render(spriteBatch);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable"/>
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var control in this.controls)
            {
                control.Dispose();
            }

            this.controls.Clear();
        }
    }
}
