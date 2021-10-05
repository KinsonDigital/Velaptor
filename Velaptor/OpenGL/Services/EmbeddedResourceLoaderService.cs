// <copyright file="EmbeddedResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Velaptor.Services;

    /// <summary>
    /// Loads embedded text file resources.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class EmbeddedResourceLoaderService : IEmbeddedResourceLoaderService
    {
        /// <inheritdoc cref="IEmbeddedResourceLoaderService.LoadResource"/>
        public string LoadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resources = assembly.GetManifestResourceNames();

            var shaderSrcResource = (from r in resources
                                     where r.EndsWith(name, StringComparison.Ordinal)
                                     select r).SingleOrDefault();

            if (resources is null || resources.Length <= 0 || string.IsNullOrEmpty(shaderSrcResource))
            {
                // TODO: Change this to a custom exception
                throw new Exception($"The embedded shader source code resource '{name}' does not exist.");
            }

            using (var stream = assembly.GetManifestResourceStream(shaderSrcResource))
            {
                if (stream is not null)
                {
                    using var reader = new StreamReader(stream);

                    return reader.ReadToEnd();
                }
            }

            return string.Empty;
        }
    }
}
