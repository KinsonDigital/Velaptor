// <copyright file="FileSystemStreamFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Fakes;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

/// <summary>
/// Used for unit testing.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Used for unit testing only.")]
public class FileSystemStreamFake : FileSystemStream
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemStreamFake"/> class.
    /// </summary>
    public FileSystemStreamFake()
        : base(null, "test-path", false)
    {
    }
}
