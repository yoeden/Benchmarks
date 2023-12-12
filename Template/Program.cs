using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Bench>();

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Bench
{
    [Benchmark]
    public void First()
    {

    }

    [Benchmark]
    public void Second()
    {

    }
}