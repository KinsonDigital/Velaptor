// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System.Diagnostics.CodeAnalysis;
    using FileIO.Core;
    using FileIO.File;
    using Raptor.Content;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using SimpleInjector;
    using SimpleInjector.Diagnostics;

    /// <summary>
    /// Provides dependency injection for the applcation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly Container IocContainer = new Container();
        private static bool isInitialized;

        /// <summary>
        /// The inversion of control container used to get instances of objects.
        /// </summary>
        public static Container Container
        {
            get
            {
                if (!isInitialized)
                    SetupContainer();

                return IocContainer;
            }
        }

        /// <summary>
        /// Sets up the IoC container.
        /// </summary>
        private static void SetupContainer()
        {
            IocContainer.Register<ITextFile, TextFile>();
            IocContainer.Register<IImageFile, ImageFile>();
            IocContainer.Register<IGLInvoker, GLInvoker>();
            IocContainer.Register<ILoader<ITexture>, TextureLoader>();
            IocContainer.Register<ILoader<AtlasRegionRectangle[]>, AtlasDataLoader<AtlasRegionRectangle>>();

            /*NOTE:
             * The suppression of the SimpleInjector warning of DiagnosticType.DisposableTransientComponent is for
             * classes that are disposable.  This tells simple injector that the disposing of the object will be
             * handled manually by the application/library instead of by simple injector.
             */
            IocContainer.Register<IGPUBuffer, GPUBuffer<VertexData>>();
            var bufferRegistration = IocContainer.GetRegistration(typeof(IGPUBuffer))?.Registration;
            bufferRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IocContainer.Register<IShaderProgram, ShaderProgram>();
            var shaderRegistration = IocContainer.GetRegistration(typeof(IShaderProgram))?.Registration;
            shaderRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IocContainer.Register<ISpriteBatch, SpriteBatch>();
            var spriteBatchRegistration = IocContainer.GetRegistration(typeof(ISpriteBatch))?.Registration;
            spriteBatchRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            isInitialized = true;
        }
    }
}
