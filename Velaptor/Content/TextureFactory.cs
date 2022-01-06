// <copyright file="TextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.Diagnostics.CodeAnalysis;
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;

    /// <summary>
    /// Creates <see cref="ITexture"/> objects for rendering.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class TextureFactory : ITextureFactory
    {
        /// <inheritdoc/>
        public ITexture Create(string name, string filePath, ImageData imageData, bool isPooled)
        {
            var gl = IoC.Container.GetInstance<IGLInvoker>();
            var glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();

            return new Texture(gl, glExtensions, name, filePath, imageData) { IsPooled = true };
        }
    }
}
