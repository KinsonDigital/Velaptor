// <copyright file="TextResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Loads embedded text file resources.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class TextResourceLoaderService : IEmbeddedResourceLoaderService<string>
    {
        /// <summary>
        /// Loads embedded text file resources that match the given <param name="name"></param>.
        /// </summary>
        /// <param name="name">The name of the embedded text file.</param>
        /// <returns>The embedded text file content.</returns>
        /// <exception cref="Exception"></exception>
        public string LoadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resources = assembly.GetManifestResourceNames();

            var shaderSrcResource = (from r in resources
                                          where r.EndsWith(name.ToLower(), StringComparison.Ordinal)
                                          select r).SingleOrDefault();

            if (resources is null || resources.Length <= 0 || string.IsNullOrEmpty(shaderSrcResource))
            {
                // TODO: Change this to a custom exception
                throw new Exception($"The embedded shader source code resource '{name}' does not exist.");
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
}
