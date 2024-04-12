// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable once RedundantUsingDirective
#pragma warning disable SA1200
using BenchmarkDotNet.Running;
using MeasureTextPerf;

#if DEBUG

var benchMark = new MeasureTextBenchmarks
{
    TotalCharacters = 100,
    TotalLines = 10,
};
benchMark.IterationSetup();
benchMark.MeasureString();

#elif RELEASE

var result = BenchmarkRunner.Run<MeasureTextBenchmarks>();

Console.WriteLine(result);
Console.ReadLine();

#endif
