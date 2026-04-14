// <copyright file="ContentPathResolverFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Velaptor.Content;
using Velaptor.Services;

/// <summary>
/// Used to test the abstract <see cref="ContentPathResolver"/> class.
/// </summary>
[SuppressMessage("ReSharper", "RedundantTypeDeclarationBody", Justification = "Intentional")]
internal sealed class ContentPathResolverFake : ContentPathResolver
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public ContentPathResolverFake(IAppService appService, IFile file, IPath path)
        : base(appService, file, path)
    {
    }
}
