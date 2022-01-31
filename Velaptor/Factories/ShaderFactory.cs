// <copyright file="ShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;
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
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            textureShader = new TextureShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitReactor, shutDownReactor);

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
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            fontShader = new FontShader(glInvoker, glInvokerExtensions, shaderLoaderService, glInitReactor, shutDownReactor);

            return fontShader;
        }
    }
}
