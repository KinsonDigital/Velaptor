// <copyright file="TextResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Velaptor.Exceptions;

namespace Velaptor.Services;

/// <summary>
/// Loads embedded text file resources.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class TextResourceLoaderService : IEmbeddedResourceLoaderService<string>
{
    /// <summary>
    /// Loads embedded text file resources that match the given <param name="name"></param>.
    /// </summary>
    /// <param name="name">The name of the embedded text file.</param>
    /// <returns>The embedded text file content.</returns>
    /// <exception cref="LoadEmbeddedResourceException">
    ///     Occurs when the embedded resource with the given <paramref name="name"/> does not exist.
    /// </exception>
    public string LoadResource(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resources = assembly.GetManifestResourceNames();

        var shaderSrcResource = (from r in resources
            where r.EndsWith(name.ToLower(), StringComparison.Ordinal)
            select r).SingleOrDefault();

        if (resources is null || resources.Length <= 0 || string.IsNullOrEmpty(shaderSrcResource))
        {
            throw new LoadEmbeddedResourceException($"The embedded text file resource with the name '{name}' does not exist.");
        }

        using var stream = assembly.GetManifestResourceStream(shaderSrcResource);

        if (stream is null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
