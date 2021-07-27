// <copyright file="ExposedInternals.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(assemblyName: "VelaptorTests", AllInternalsVisible = true)]

// Used to expose internal types to the Moq mocking framework
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2", AllInternalsVisible = true)]
