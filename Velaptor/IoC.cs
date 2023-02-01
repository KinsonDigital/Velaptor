// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Content;
using Content.Caching;
using Content.Factories;
using Content.Fonts.Services;
using Factories;
using Graphics;
using Graphics.Renderers;
using Input;
using NativeInterop.FreeType;
using NativeInterop.GLFW;
using NativeInterop.OpenGL;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Services;
using ReactableData;
using Services;
using Silk.NET.OpenGL;
using SimpleInjector;

/// <summary>
/// Provides dependency injection for the application.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the '{nameof(SimpleInjector)}' library.")]
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

        SetupBuffers();

        SetupCaching();

        SetupFactories();

        SetupServices();

        SetupContent();

        SetupReactables();

        IoCContainer.Register<IComparer<RenderItem<TextureBatchItem>>, RenderItemComparer<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<FontGlyphBatchItem>>, RenderItemComparer<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<RectBatchItem>>, RenderItemComparer<RectBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<LineBatchItem>>, RenderItemComparer<LineBatchItem>>(Lifestyle.Singleton);

        IoCContainer.Register<IBatchingManager, BatchingManager>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<KeyboardState>, Keyboard>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<MouseState>, Mouse>(Lifestyle.Singleton);
        IoCContainer.Register<IFontMetaDataParser, FontMetaDataParser>(Lifestyle.Singleton);

        isInitialized = true;
    }

    /// <summary>
    /// Sets up the container registration related to OpenGL.
    /// </summary>
    private static void SetupNativeInterop()
    {
        IoCContainer.Register(() => FileSystem.File, Lifestyle.Singleton);
        IoCContainer.Register(() => FileSystem.Directory, Lifestyle.Singleton);
        IoCContainer.Register(() => FileSystem.Path, Lifestyle.Singleton);
        IoCContainer.Register(() => FileStream, Lifestyle.Singleton);
        IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);

        IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IOpenGLService, OpenGLService>(Lifestyle.Singleton);
        IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IFreeTypeInvoker, FreeTypeInvoker>(Lifestyle.Singleton);

        IoCContainer.Register<GLFWMonitors>(Lifestyle.Singleton);
        IoCContainer.Register<IMonitors, GLFWMonitors>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to the GPU buffers.
    /// </summary>
    private static void SetupBuffers()
    {
        IoCContainer.Register<IGPUBuffer<TextureBatchItem>, TextureGPUBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGPUBuffer<FontGlyphBatchItem>, FontGPUBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGPUBuffer<RectBatchItem>, RectGPUBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGPUBuffer<LineBatchItem>, LineGPUBuffer>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to caching.
    /// </summary>
    private static void SetupCaching()
    {
        IoCContainer.Register<IItemCache<string, ITexture>, TextureCache>(Lifestyle.Singleton);
        IoCContainer.Register<IItemCache<string, ISound>, SoundCache>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to factories.
    /// </summary>
    private static void SetupFactories()
    {
        IoCContainer.Register<IWindowFactory, SilkWindowFactory>(Lifestyle.Singleton);
        IoCContainer.Register<INativeInputFactory, NativeInputFactory>(Lifestyle.Singleton);
        IoCContainer.Register<ISoundFactory, SoundFactory>(Lifestyle.Singleton);
        IoCContainer.Register<ITextureFactory, TextureFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IAtlasDataFactory, AtlasDataFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IShaderFactory, ShaderFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IFontFactory, FontFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderMediator, RenderMediator>(Lifestyle.Singleton);
        IoCContainer.Register<IRendererFactory, RendererFactory>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to services.
    /// </summary>
    private static void SetupServices()
    {
        IoCContainer.Register<IConsoleService, ConsoleService>(Lifestyle.Singleton);
        IoCContainer.Register<IDateTimeService, DateTimeService>(Lifestyle.Singleton);
        IoCContainer.Register<IConsoleLoggerService, ConsoleLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IFileLoggerService, FileLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IEventLoggerService, EventLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<ILoggingService, LoggingService>(Lifestyle.Singleton);
        IoCContainer.Register<IAppSettingsService, AppSettingsService>(Lifestyle.Singleton);
        IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<string>, TextResourceLoaderService>(Lifestyle.Singleton);
        IoCContainer.Register<IShaderLoaderService, TextureShaderResourceLoaderService>(Lifestyle.Singleton);
        IoCContainer.Register<ISystemMonitorService, SystemMonitorService>(Lifestyle.Singleton);
        IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);
        IoCContainer.Register<IJSONService, JSONService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<Stream?>, EmbeddedFontResourceService>(Lifestyle.Singleton);
        IoCContainer.Register<IFontService, FontService>(Lifestyle.Singleton);

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
    /// Sets up the container registration related to content.
    /// </summary>
    private static void SetupContent() => IoCContainer.Register<AtlasTexturePathResolver>();

    /// <summary>
    /// Sets up the container registration related to reactables.
    /// </summary>
    private static void SetupReactables()
    {
        IoCContainer.Register<IReactableFactory, ReactableFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable, PushReactable>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<GL>, PushReactable<GL>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<BatchSizeData>, PushReactable<BatchSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<ViewPortSizeData>, PushReactable<ViewPortSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<MouseStateData>, PushReactable<MouseStateData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<KeyboardKeyStateData>, PushReactable<KeyboardKeyStateData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<DisposeTextureData>, PushReactable<DisposeTextureData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<DisposeSoundData>, PushReactable<DisposeSoundData>>(Lifestyle.Singleton);

        IoCContainer.Register<IBatchPullReactable<TextureBatchItem>, BatchPullReactable<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<FontGlyphBatchItem>, BatchPullReactable<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<RectBatchItem>, BatchPullReactable<RectBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<LineBatchItem>, BatchPullReactable<LineBatchItem>>(Lifestyle.Singleton);

        IoCContainer.Register<IRenderBatchReactable<TextureBatchItem>, RenderBatchReactable<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<FontGlyphBatchItem>, RenderBatchReactable<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<RectBatchItem>, RenderBatchReactable<RectBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<LineBatchItem>, RenderBatchReactable<LineBatchItem>>(Lifestyle.Singleton);
    }
}
