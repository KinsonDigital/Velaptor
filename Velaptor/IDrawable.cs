// <copyright file="IDrawable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor
{
    using Velaptor.Graphics;

    /// <summary>
    /// Provides the ability for an object to be rendered.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Renders the object.
        /// </summary>
        /// <param name="spriteBatch">Renders sprites.</param>
        void Render(ISpriteBatch spriteBatch);
    }
}
