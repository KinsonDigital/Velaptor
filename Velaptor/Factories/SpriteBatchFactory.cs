// <copyright file="SpriteBatchFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;

namespace Velaptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;

    /// <summary>
    /// Creates instances of the type <see cref="SpriteBatch"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SpriteBatchFactory
    {
        private static ISpriteBatch? spriteBatch;

        /// <summary>
        /// Initializes and instance of a <see cref="ISpriteBatch"/>.
        /// </summary>
        /// <param name="renderSurfaceWidth">The width of the render surface.</param>
        /// <param name="renderSurfaceHeight">The height of the render surface.</param>
        /// <returns>A Velaptor implemented sprite batch.</returns>
        public static ISpriteBatch CreateSpriteBatch(int renderSurfaceWidth, int renderSurfaceHeight)
        {
            // TODO: Make this static class field
            spriteBatch = IoC.Container.GetInstance<ISpriteBatch>();

            if (spriteBatch is null)
            {
                throw new NullReferenceException("There were issues creating the sprite batch.");
            }

            spriteBatch.RenderSurfaceWidth = renderSurfaceWidth;
            spriteBatch.RenderSurfaceHeight = renderSurfaceHeight;

            return spriteBatch;
        }
    }
}
