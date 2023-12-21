// See https://aka.ms/new-console-template for more information

// ReSharper disable once RedundantUsingDirective
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
