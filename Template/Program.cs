using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Bench>();

[MemoryDiagnoser]
[ThreadingDiagnoser]
[Config(typeof(Config))]
public class Bench
{
    [Benchmark(Baseline = true)]
    public void Baseline()
    {

    }

    [Benchmark]
    public void Second()
    {

    }
}

class Config : ManualConfig
{
    public Config()
    {
        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
    }
}