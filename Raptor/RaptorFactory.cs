// <copyright file="RaptorFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using Raptor.Graphics;

    public static class RaptorFactory
    {
        public static ISpriteBatch CreateSpriteBatch(int renderSurfaceWidth, int renderSurfaceHeight)
        {
            var result = IoC.Container.GetInstance<ISpriteBatch>();

            result.RenderSurfaceWidth = renderSurfaceWidth;
            result.RenderSurfaceHeight = renderSurfaceHeight;

            return result;
        }
    }
}
