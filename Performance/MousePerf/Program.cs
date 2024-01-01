// See https://aka.ms/new-console-template for more information

// ReSharper disable UnusedVariable
// ReSharper disable RedundantUsingDirective
#pragma warning disable S1481 // Remove the unused local variable
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MousePerf;

var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);

#if DEBUG

var benchmarks = new MouseBenchmarks();

#elif RELEASE

var results = BenchmarkRunner.Run<MouseBenchmarks>(config);
Console.WriteLine(results);

#endif

Console.ReadLine();
