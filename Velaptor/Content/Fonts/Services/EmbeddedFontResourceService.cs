// <copyright file="EmbeddedFontResourceService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Velaptor.Exceptions;
using Velaptor.Services;

namespace Velaptor.Content.Fonts.Services;

/// <summary>
/// Loads embedded font file resources.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class EmbeddedFontResourceService : IEmbeddedResourceLoaderService<Stream?>
{
    private const string FontResourcePath = "Velaptor.Content.Fonts.EmbeddedResources.";

    /// <summary>
    /// Loads a <see cref="Stream"/> to an embedded font file resource that matches the given <param name="name"></param>.
    /// </summary>
    /// <param name="name">The name of the embedded font file.</param>
    /// <returns>The stream to the embedded file data.</returns>
    /// <exception cref="LoadEmbeddedResourceException">
    ///     Thrown if the given embedded file resource does not exist.
    /// </exception>
    public Stream? LoadResource(string name)
    {
        if (string.IsNullOrEmpty(Path.GetDirectoryName(name)) is false)
        {
            throw new ArgumentException(
                "The parameter must not contain a directory path when extracting embedded font files.",
                nameof(name));
        }

        var assembly = Assembly.GetExecutingAssembly();

        var resources = assembly.GetManifestResourceNames();

        var shaderSrcResource = (from r in resources
            where r.EndsWith(name, StringComparison.Ordinal)
            select r).SingleOrDefault();

        if (resources is null || resources.Length <= 0 || string.IsNullOrEmpty(shaderSrcResource))
        {
            throw new LoadEmbeddedResourceException($"The embedded text file resource with the name '{name}' does not exist.");
        }

        var fullyQualifiedResourcePath = $"{FontResourcePath}{name}";
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedResourcePath);

        return resource;
    }
}
