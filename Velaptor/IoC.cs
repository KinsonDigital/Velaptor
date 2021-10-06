// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VelaptorTests", AllInternalsVisible = true)]

namespace Velaptor
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using SimpleInjector;
    using Velaptor.Content;
    using Velaptor.Factories;
    using Velaptor.Graphics;
    using Velaptor.Input;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.NativeInterop.GLFW;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Services;
    using Velaptor.Services;

    /// <summary>
    /// Provides dependency injection for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly FileSystem FileSystem = new ();
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
            SetupOpenGL();

            SetupServices();

            SetupContent();

            IoCContainer.Register<IFreeTypeInvoker, FreeTypeInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IFreeTypeExtensions, FreeTypeExtensions>(Lifestyle.Singleton);

            IoCContainer.Register<IKeyboardInput<KeyCode, KeyboardState>, Keyboard>(Lifestyle.Singleton);
            IoCContainer.Register<IMouseInput<MouseButton, MouseState>, Mouse>(Lifestyle.Singleton);

            IoCContainer.Register<ISoundFactory, SoundFactory>();

            IoCContainer.Register<OpenGLInitObservable>(Lifestyle.Singleton);
            IoCContainer.Register<OpenGLContextObservable>(Lifestyle.Singleton);

            isInitialized = true;
        }

        /// <summary>
        /// Setup container registration related to OpenGL.
        /// </summary>
        private static void SetupOpenGL()
        {
            IoCContainer.Register(() => FileSystem.File, Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.Directory, Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.Path, Lifestyle.Singleton);
            IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);

            IoCContainer.Register<IGLInvokerExtensions, GLInvokerExtensions>(Lifestyle.Singleton);

            IoCContainer.Register<GLFWMonitors>(suppressDisposal: true);

            IoCContainer.Register<IGPUBuffer, GPUBuffer<VertexData>>(Lifestyle.Singleton);

            IoCContainer.Register<IShaderProgram, ShaderProgram>(Lifestyle.Singleton);

            IoCContainer.Register<ISpriteBatch, SpriteBatch>(Lifestyle.Singleton);

            IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGameWindowFacade, GLWindowFacade>(Lifestyle.Singleton, suppressDisposal: true);
        }

        /// <summary>
        /// Setup container registration related to services.
        /// </summary>
        private static void SetupServices()
        {
            IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
            IoCContainer.Register<IEmbeddedResourceLoaderService, EmbeddedResourceLoaderService>(Lifestyle.Singleton);
            IoCContainer.Register<ITemplateProcessorService, ShaderTemplateProcessorService>(Lifestyle.Singleton);
            IoCContainer.Register<IShaderLoaderService<uint>, TextureShaderResourceLoaderService>(Lifestyle.Singleton);
            IoCContainer.Register<ISystemMonitorService, SystemMonitorService>(Lifestyle.Singleton);
            IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);

            IoCContainer.Register<ITaskService, TaskService>();
            IoCContainer.SuppressDisposableTransientWarning<ITaskService>();

            IoCContainer.Register<IBatchManagerService, BatchManagerService>(Lifestyle.Singleton);
        }

        /// <summary>
        /// Setup container registration related to content.
        /// </summary>
        private static void SetupContent() => IoCContainer.Register<AtlasTexturePathResolver>();
    }
}
