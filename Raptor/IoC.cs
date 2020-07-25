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
    using Raptor.Audio;
    using SimpleInjector;
    using SimpleInjector.Diagnostics;
    using Raptor.OpenAL;
    using FileIO.Directory;

    /// <summary>
    /// Provides dependency injection for the applcation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly Container IocContainer = new Container();
        private static bool isInitialized;

        /// <summary>
        /// Gets the inversion of control container used to get instances of objects.
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
            IocContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IocContainer.Register<ITextFile, TextFile>();
            IocContainer.Register<IImageFile, ImageFile>();
            IocContainer.Register<ILoader<ITexture>, TextureLoader>();
            IocContainer.Register<ILoader<ISound>, SoundLoader>();
            IocContainer.Register<IDirectory, Directory>();
            IocContainer.Register<IALInvoker, ALInvoker>();
            IocContainer.Register<ISoundDecoder<float>, OggSoundDecoder>();
            IocContainer.Register<ISoundDecoder<byte>, MP3SoundDecoder>();
            IocContainer.Register<IContentSource, ContentSource>();
            IocContainer.Register<IContentLoader, ContentLoader>();
            IocContainer.Register<ILoader<AtlasRegionRectangle[]>, AtlasDataLoader<AtlasRegionRectangle>>();

            /*NOTE:
             * The suppression of the SimpleInjector warning of DiagnosticType.DisposableTransientComponent is for
             * classes that are disposable.  This tells simple injector that the disposing of the object will be
             * handled manually by the application/library instead of by simple injector.
             */
            IocContainer.Register<IGPUBuffer>(() =>
            {
                return new GPUBuffer<VertexData>(IocContainer.GetInstance<IGLInvoker>());
            });
            var bufferRegistration = IocContainer.GetRegistration(typeof(IGPUBuffer))?.Registration;
            bufferRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IocContainer.Register<IShaderProgram>(() =>
            {
                return new ShaderProgram(IocContainer.GetInstance<IGLInvoker>(), IocContainer.GetInstance<ITextFile>());
            });
            var shaderRegistration = IocContainer.GetRegistration(typeof(IShaderProgram))?.Registration;
            shaderRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IocContainer.Register<ISpriteBatch>(() =>
            {
                return new SpriteBatch(IocContainer.GetInstance<IGLInvoker>(), IocContainer.GetInstance<IShaderProgram>(), IocContainer.GetInstance<IGPUBuffer>());
            });
            var spriteBatchRegistration = IocContainer.GetRegistration(typeof(ISpriteBatch))?.Registration;
            spriteBatchRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            isInitialized = true;
        }
    }
}
