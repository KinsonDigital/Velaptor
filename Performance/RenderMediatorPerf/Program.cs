// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnusedVariable
// ReSharper disable RedundantUsingDirective
#pragma warning disable S1481 // Remove the unused local variable
#pragma warning disable SA1200
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using RenderMediatorPerf;

var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);

#if DEBUG

var benchmarks = new RenderMediatorBenchmarks();
benchmarks.IterationSetup();
benchmarks.CoordinateRenders();
benchmarks.IterationCleanup();
Console.WriteLine("Debugging complete.");

#elif RELEASE

var results = BenchmarkRunner.Run<RenderMediatorBenchmarks>(config);
Console.WriteLine(results);

#endif

Console.ReadLine();
