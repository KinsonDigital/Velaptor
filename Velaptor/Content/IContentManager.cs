// <copyright file="IContentManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System.Diagnostics.CodeAnalysis;
using Fonts;

/// <summary>
/// Loads various types of content.
/// </summary>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1629:Documentation text should end with a period",
    Justification = "Code blocks do not need a period.")]
public interface IContentManager
{
    /// <summary>
    /// Loads the specified <paramref name="pathOrName"/> for the given type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="pathOrName">The content file path or name.</param>
    /// <typeparam name="T">The type of content to use.</typeparam>
    /// <returns>The content object.</returns>
    /// <remarks>
    /// <para>
    /// When loading audio content, the default <see cref="AudioBuffer.Full"/> used will be used.
    /// To specify a different buffer type, use the <see cref="LoadAudio(string, AudioBuffer)"/> method.
    /// </para>
    ///
    /// <para>
    /// When loading font content, the default size of <c>12</c> will be used.
    /// To specify a different size, use the <see cref="LoadFont(string, uint)"/> method.
    /// </para>
    ///
    /// <para>
    /// If the <paramref name="pathOrName"/> is a fully qualified file path, an attempt will be made to load the content
    /// using the file path directly.  If the <paramref name="pathOrName"/> is just the name of the content item,
    /// the content manager system will try to find and resolve the location of the content item automatically.
    /// </para>
    ///
    /// <para>Only loads the following built in <see cref="IContent"/> types.</para>
    /// <list type="bullet">
    ///     <item>
    ///         <see cref="ITexture"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IAudio"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IFont"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IAtlasData"/>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <code>
    ///     var contentManager = ContentManager.Create();
    ///
    ///     // Loading a texture
    ///     contentManager.Load&lt;ITexture&gt;("my-texture");
    ///     contentManager.Load&lt;ITexture&gt;("C:/custom-content-location/my-texture.png");
    ///
    ///     // Loading audio
    ///     contentManager.Load&lt;IAudio&gt;("my-sound");
    ///     contentManager.Load&lt;IAudio&gt;("C:/custom-content-location/my-sound.ogg");
    ///
    ///     // Loading font
    ///     contentManager.Load&lt;IFont&gt;("my-font");
    ///     contentManager.Load&lt;IFont&gt;("C:/custom-content-location/my-font.ttf");
    ///
    ///     // Loading texture atlas
    ///     contentManager.Load&lt;IAtlasData&gt;("my-atlas");
    ///     contentManager.Load&lt;IAtlasData&gt;("C:/custom-content-location/my-atlas.ttf");
    /// </code>
    T Load<T>(string pathOrName)
        where T : IContent;

    /// <summary>
    /// Loads a font with the given <paramref name="pathOrName"/> and <paramref name="size"/>.
    /// </summary>
    /// <param name="pathOrName">The font file path or name.</param>
    /// <param name="size">The size of the font.</param>
    /// <returns>The loaded font.</returns>
    /// <remarks>
    /// If the <paramref name="pathOrName"/> is a fully qualified file path, an attempt will be made to load the content
    /// using the file path directly.  If the <paramref name="pathOrName"/> is just the name of the content item,
    /// the content manager system will try to find and resolve the location of the content item automatically.
    /// </remarks>
    /// <code>
    ///     var contentManager = ContentManager.Create();
    ///     contentManager.LoadFont("my-texture", 12);
    ///     // or
    ///     contentManager.LoadFont("C:/custom-content-location/my-texture.png", 24);
    /// </code>
    IFont LoadFont(string pathOrName, uint size);

    /// <summary>
    /// Loads a font with the given <paramref name="pathOrName"/> and <paramref name="bufferType"/>.
    /// </summary>
    /// <param name="pathOrName">The font file path or name.</param>
    /// <param name="bufferType">The buffer mode of the audio.</param>
    /// <returns>The loaded audio.</returns>
    /// <remarks>
    /// If the <paramref name="pathOrName"/> is a fully qualified file path, an attempt will be made to load the content
    /// using the file path directly.  If the <paramref name="pathOrName"/> is just the name of the content item,
    /// the content manager system will try to find and resolve the location of the content item automatically.
    /// </remarks>
    IAudio LoadAudio(string pathOrName, AudioBuffer bufferType);

    /// <summary>
    /// Loads atlas data with the given <paramref name="pathOrName"/>.
    /// </summary>
    /// <param name="pathOrName">The atlas file path or name.</param>
    /// <returns>The loaded atlas data.</returns>
    /// <remarks>
    /// If the <paramref name="pathOrName"/> is a fully qualified file path, an attempt will be made to load the content
    /// using the file path directly.  If the <paramref name="pathOrName"/> is just the name of the content item,
    /// the content manager system will try to find and resolve the location of the content item automatically.
    /// </remarks>
    IAtlasData LoadAtlas(string pathOrName);

    /// <summary>
    /// Unloads the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The content item.</param>
    /// <typeparam name="T">The type of content item to unload.</typeparam>
    /// <remarks>
    /// <para>Only unloads the following built in <see cref="IContent"/> types.</para>
    /// <list type="bullet">
    ///     <item>
    ///         <see cref="ITexture"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IAudio"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IFont"/>
    ///     </item>
    ///     <item>
    ///         <see cref="IAtlasData"/>
    ///     </item>
    /// </list>
    /// </remarks>
    void Unload<T>(T item)
        where T : IContent;
}
