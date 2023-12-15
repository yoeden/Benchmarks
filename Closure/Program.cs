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
    public int Closure()
    {
        var a = 23;
        var f = () => a + a / a; 
        return f();
    }

    [Benchmark]
    public int Parameter()
    {
        var a = 23;
        Func<int, int> f = (x) => x + x / x;
        return f(a);
    }

    [Benchmark]
    public int StaticParameter()
    {
        var a = 23;
        Func<int, int> f = static (x) => x + x / x;
        return f(a);
    }

    [Benchmark]
    public int LocalFunction()
    {
        var a = 23;

        int Func(int x)
        {
            return x + x / x;
        }

        return Func(a);
    }


    [Benchmark]
    public int LocalStaticFunction()
    {
        var a = 23;

        static int Func(int x)
        {
            return x + x / x;
        }

        return Func(a);
    }

    [Benchmark]
    public int InstanceMethod()
    {
        var a = 23;
        return InstanceMethod(a);
    }

    [Benchmark]
    public int StaticMethod()
    {
        var a = 23;
        return StaticMethod(a);
    }

    private int InstanceMethod(int x)
    {
        return x + x / x;
    }

    private static int StaticMethod(int x)
    {
        return x + x / x;
    }
}

class Config : ManualConfig
{
    public Config()
    {
        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
    }
}