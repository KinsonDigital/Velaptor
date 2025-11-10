// <copyright file="ContentManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using Factories;
using Fonts;

/// <inheritdoc />
[ExcludeFromCodeCoverage(Justification = "Cannot test due to interaction with 'IoC' container.")]
public sealed class ContentManager : IContentManager
{
    private static IContentManager? instance;
    private static IContentLoaderFactory? loaderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentManager"/> class.
    /// </summary>
    /// <param name="loaderFactory">The factory used to create content loaders.</param>
    /// <remarks>Used to inject custom version of the loader factory and for testing.</remarks>
    internal ContentManager(IContentLoaderFactory loaderFactory)
    {
        ArgumentNullException.ThrowIfNull(loaderFactory);
        ContentManager.loaderFactory = loaderFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentManager"/> class.
    /// </summary>
    /// <remarks>
    ///     This is used to create the singleton instance of the <see cref="ContentManager"/>.
    /// </remarks>
    private ContentManager() => loaderFactory = IoC.Container.GetInstance<IContentLoaderFactory>();

    /// <summary>
    /// Creates a singleton instance of the <see cref="IContentManager"/>.
    /// </summary>
    /// <returns>The singleton instance of the <see cref="IContentManager"/>.</returns>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to interaction with 'IoC' container.")]
    public static IContentManager Create()
    {
        if (instance is not null && loaderFactory is not null)
        {
            return instance;
        }

        loaderFactory = IoC.Container.GetInstance<IContentLoaderFactory>();
        instance = new ContentManager();

        return instance;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown if the internal loader factory is null.</exception>
    /// <exception cref="NotSupportedException">Thrown if a content type of type <typeparamref name="T"/> is not a valid content type.</exception>
    public T Load<T>(string pathOrName)
        where T : IContent
    {
        if (loaderFactory is null)
        {
            throw new InvalidOperationException("The content loader factory has not been initialized.");
        }

        switch (typeof(T))
        {
            case { } t when t == typeof(ITexture):
                var textureLoader = loaderFactory.CreateTextureLoader();
                return (T)textureLoader.Load(pathOrName);
            case { } t when t == typeof(IAudio):
                var audioLoader = loaderFactory.CreateAudioLoader();
                return (T)audioLoader.Load(pathOrName, AudioBuffer.Full);
            case { } t when t == typeof(IFont):
                var fontLoader = loaderFactory.CreateFontLoader();
                return (T)fontLoader.Load(pathOrName, 12);
            case { } t when t == typeof(IAtlasData):
                var atlasLoader = loaderFactory.CreateAtlasLoader();
                return (T)atlasLoader.Load(pathOrName);
            default:
                throw new NotSupportedException($"The content type '{typeof(T)}' is not supported.");
        }
    }

    /// <inheritdoc />
    public IFont LoadFont(string pathOrName, uint size)
    {
        if (loaderFactory is null)
        {
            throw new InvalidOperationException("The content loader factory has not been initialized.");
        }

        var fontLoader = loaderFactory.CreateFontLoader();

        return fontLoader.Load(pathOrName, size);
    }

    /// <inheritdoc />
    public IAudio LoadAudio(string pathOrName, AudioBuffer bufferType)
    {
        if (loaderFactory is null)
        {
            throw new InvalidOperationException("The content loader factory has not been initialized.");
        }

        var audioLoader = loaderFactory.CreateAudioLoader();

        return audioLoader.Load(pathOrName, bufferType);
    }

    /// <inheritdoc />
    public IAtlasData LoadAtlas(string pathOrName)
    {
        if (loaderFactory is null)
        {
            throw new InvalidOperationException("The content loader factory has not been initialized.");
        }

        var atlasLoader = loaderFactory.CreateAtlasLoader();

        return atlasLoader.Load(pathOrName);
    }

    /// <inheritdoc />
    public void Unload<T>(T item)
        where T : IContent
    {
        if (loaderFactory is null)
        {
            throw new InvalidOperationException("The content loader factory has not been initialized.");
        }

        switch (typeof(T))
        {
            case { } t when t == typeof(ITexture):
                var textureLoader = loaderFactory.CreateTextureLoader();
                textureLoader.Unload((ITexture)item);
                break;
            case { } t when t == typeof(IAudio):
                var audioLoader = loaderFactory.CreateAudioLoader();
                audioLoader.Unload((IAudio)item);
                break;
            case { } t when t == typeof(IFont):
                var fontLoader = loaderFactory.CreateFontLoader();
                fontLoader.Unload((IFont)item);
                break;
            case { } t when t == typeof(IAtlasData):
                var atlasLoader = loaderFactory.CreateAtlasLoader();
                atlasLoader.Unload((IAtlasData)item);
                break;
            default:
                throw new NotSupportedException($"The content type '{typeof(T)}' is not supported.");
        }
    }
}
