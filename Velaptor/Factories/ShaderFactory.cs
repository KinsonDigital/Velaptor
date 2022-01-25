// <copyright file="ShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates instance of type <see cref="IShaderProgram"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ShaderFactory
    {
        private static IShaderProgram? textureShader;
        private static IShaderProgram? fontShader;

        /// <summary>
        /// Creates a shader for rendering textures.
        /// </summary>
        /// <returns>The shader program.</returns>
        public static IShaderProgram CreateTextureShader()
        {
            if (textureShader is not null)
            {
                return textureShader;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();

            textureShader = new TextureShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitObservable, shutDownObservable);

            return textureShader;
        }

        /// <summary>
        /// Creates a shader for rendering text using a font.
        /// </summary>
        /// <returns>The shader program.</returns>
        public static IShaderProgram CreateFontShader()
        {
            if (fontShader is not null)
            {
                return fontShader;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            var shaderLoaderService = IoC.Container.GetInstance<IShaderLoaderService<uint>>();
            var glInitObservable = IoC.Container.GetInstance<OpenGLInitObservable>();
            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();

            fontShader = new FontShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitObservable, shutDownObservable);

            return fontShader;
        }
    }
}
