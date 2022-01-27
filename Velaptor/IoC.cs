// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Velaptor.Observables.ObservableData;

[assembly: InternalsVisibleTo("VelaptorTests", AllInternalsVisible = true)]

namespace Velaptor
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using SimpleInjector;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Factories;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Factories;
    using Velaptor.Input;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.NativeInterop.GLFW;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides dependency injection for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly FileSystem FileSystem = new ();
        private static readonly IFileStreamFactory FileStream = FileSystem.FileStream;
        private static readonly Container IoCContainer = new ();
        private static bool isInitialized;

        /// <summary>
        /// Gets the inversion of control container used to get instances of objects.
        /// </summary>
        public static Container Container
        {
            get
            {
                if (!isInitialized)
                {
                    SetupContainer();
                }

                return IoCContainer;
            }
        }

        /// <summary>
        /// Sets up the IoC container.
        /// </summary>
        private static void SetupContainer()
        {
            SetupNativeInterop();

            SetupReactors();

            SetupCaching();

            SetupFactories();

            SetupServices();

            SetupContent();

            IoCContainer.Register<IKeyboardInput<KeyCode, KeyboardState>, Keyboard>(Lifestyle.Singleton);
            IoCContainer.Register<IMouseInput<MouseButton, MouseState>, Mouse>(Lifestyle.Singleton);
            IoCContainer.Register<IFontMetaDataParser, FontMetaDataParser>(Lifestyle.Singleton);

            isInitialized = true;
        }

        /// <summary>
        /// Sets up container registration related to OpenGL.
        /// </summary>
        private static void SetupNativeInterop()
        {
            IoCContainer.Register(() => FileSystem.File, Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.Directory, Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.Path, Lifestyle.Singleton);
            IoCContainer.Register(() => FileStream, Lifestyle.Singleton);
            IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);

            IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLInvokerExtensions, GLInvokerExtensions>(Lifestyle.Singleton);

            IoCContainer.Register<GLFWMonitors>(suppressDisposal: true);

            IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGameWindowFacade, GLWindowFacade>(Lifestyle.Singleton, suppressDisposal: true);

            IoCContainer.Register<IFreeTypeInvoker, FreeTypeInvoker>(Lifestyle.Singleton);
        }

        /// <summary>
        /// Sets up container registration related to observables.
        /// </summary>
        private static void SetupReactors()
        {
            IoCContainer.Register<IReactor<GLInitData>, OpenGLInitReactor>(Lifestyle.Singleton);
            IoCContainer.Register<IReactor<ShutDownData>, ShutDownReactor>(Lifestyle.Singleton);
            IoCContainer.Register<IReactor<GLContextData>, OpenGLContextReactor>(Lifestyle.Singleton);
            IoCContainer.Register<IReactor<DisposeTextureData>, DisposeTexturesReactor>(Lifestyle.Singleton);
            IoCContainer.Register<IReactor<DisposeSoundData>, DisposeSoundsReactor>(Lifestyle.Singleton);
            IoCContainer.Register<IReactor<RemoveBatchItemData>, RemoveBatchItemReactor>(Lifestyle.Singleton);
        }

        /// <summary>
        /// Sets up container registration related to caching.
        /// </summary>
        private static void SetupCaching() => IoCContainer.Register<IItemCache<string, ITexture>, TextureCache>(Lifestyle.Singleton);

        /// <summary>
        /// Sets up container registration related to factories.
        /// </summary>
        private static void SetupFactories()
        {
            IoCContainer.Register<ISoundFactory, SoundFactory>();
            IoCContainer.Register<ITextureFactory, TextureFactory>(Lifestyle.Singleton);
            IoCContainer.Register<IAtlasDataFactory, AtlasDataFactory>(Lifestyle.Singleton);
            IoCContainer.Register<IFontFactory, FontFactory>();
        }

        /// <summary>
        /// Sets up container registration related to services.
        /// </summary>
        private static void SetupServices()
        {
            IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
            IoCContainer.Register<IEmbeddedResourceLoaderService<string>, TextResourceLoaderService>(Lifestyle.Singleton);
            IoCContainer.Register<ITemplateProcessorService, ShaderTemplateProcessorService>(Lifestyle.Singleton);
            IoCContainer.Register<IShaderLoaderService<uint>, TextureShaderResourceLoaderService>(Lifestyle.Singleton);
            IoCContainer.Register<ISystemMonitorService, SystemMonitorService>(Lifestyle.Singleton);
            IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);
            IoCContainer.Register<IJSONService, JSONService>(Lifestyle.Singleton);
            IoCContainer.Register<IEmbeddedResourceLoaderService<Stream?>, EmbeddedFontResourceService>(Lifestyle.Singleton);
            IoCContainer.Register<IFontService, FontService>(Lifestyle.Singleton);
            IoCContainer.Register<IBatchManagerService<SpriteBatchItem>, TextureBatchService>();

            IoCContainer.Register<IFontStatsService>(
                () => new FontStatsService(
                    IoCContainer.GetInstance<IFontService>(),
                    PathResolverFactory.CreateFontPathResolver(),
                    PathResolverFactory.CreateSystemFontPathResolver(),
                    IoCContainer.GetInstance<IDirectory>(),
                    IoCContainer.GetInstance<IPath>()), Lifestyle.Singleton);

            IoCContainer.Register<ITaskService, TaskService>();
            IoCContainer.SuppressDisposableTransientWarning<ITaskService>();
        }

        /// <summary>
        /// Sets up container registration related to content.
        /// </summary>
        private static void SetupContent() => IoCContainer.Register<AtlasTexturePathResolver>();
    }
}
