// See https://aka.ms/new-console-template for more information

using MeasureTextPerf;

#if DEBUG

var benchMark = new Benchmarks
{
    TotalCharacters = 100,
    TotalLines = 10
};
benchMark.IterationSetup();
benchMark.MeasureString();

#elif RELEASE

var result = BenchmarkRunner.Run<Benchmarks>();

Console.WriteLine(result);
Console.ReadLine();

#endif
