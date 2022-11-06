// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;


using Velaptor.Content.Fonts;
using Velaptor.Guards;



/// <summary>
/// Loads content.
/// </summary>
public sealed class ContentLoader : IContentLoader
{
    private readonly ILoader<ITexture> textureLoader;
    private readonly ILoader<ISound> soundLoader;
    private readonly ILoader<IAtlasData> atlasLoader;
    private readonly ILoader<IFont> fontLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentLoader"/> class.
    /// </summary>
    /// <param name="textureLoader">Loads textures.</param>
    /// <param name="soundLoader">Loads sounds.</param>
    /// <param name="atlasLoader">Loads a texture atlas.</param>
    /// <param name="fontLoader">Loads fonts for rendering test.</param>
    public ContentLoader(
        ILoader<ITexture> textureLoader,
        ILoader<ISound> soundLoader,
        ILoader<IAtlasData> atlasLoader,
        ILoader<IFont> fontLoader)
    {
        EnsureThat.ParamIsNotNull(textureLoader);
        EnsureThat.ParamIsNotNull(soundLoader);
        EnsureThat.ParamIsNotNull(atlasLoader);
        EnsureThat.ParamIsNotNull(fontLoader);

        this.textureLoader = textureLoader;
        this.soundLoader = soundLoader;
        this.atlasLoader = atlasLoader;
        this.fontLoader = fontLoader;
    }

    /// <inheritdoc/>
    public ITexture LoadTexture(string nameOrFilePath) => this.textureLoader.Load(nameOrFilePath);

    /// <inheritdoc/>
    public ISound LoadSound(string nameOrFilePath) => this.soundLoader.Load(nameOrFilePath);

    /// <inheritdoc/>
    public IAtlasData LoadAtlas(string nameOrFilePath) => this.atlasLoader.Load(nameOrFilePath);

    /// <inheritdoc/>
    public IFont LoadFont(string nameOrFilePath, uint size) => this.fontLoader.Load($"{nameOrFilePath}|size:{size}");

    /// <inheritdoc/>
    public void UnloadTexture(ITexture content) => this.textureLoader.Unload(content.FilePath);

    /// <inheritdoc/>
    public void UnloadSound(ISound content) => this.soundLoader.Unload(content.FilePath);

    /// <inheritdoc/>
    public void UnloadAtlas(IAtlasData content) => this.atlasLoader.Unload(content.FilePath);

    /// <inheritdoc/>
    public void UnloadFont(IFont content) => this.fontLoader.Unload($"{content.FilePath}|size:{content.Size}");
}
