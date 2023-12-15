using System.Runtime.InteropServices.JavaScript;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<RefTypeBench>();

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class RefTypeBench
{
    private static readonly int[] Src;
    private static readonly int[] Dst;

    static RefTypeBench()
    {
        Src = Enumerable.Range(0, 100_000).Select(x => x).ToArray();
        Dst = new int[100_000];
    }

    [Benchmark]
    public void Manually()
    {
        for (var i = 0; i < Src.Length; i++)
        {
            Dst[i] = Src[i];
        }
    }

    [Benchmark]
    public void ArrayCopy()
    {
        Array.Copy(Src, Dst, Dst.Length);
    }

    [Benchmark]
    public void MemoryCopyTo()
    {
        var src = Src.AsMemory();
        src.CopyTo(Dst);
    }

    [Benchmark]
    public void SpanCopyTo()
    {
        var src = Src.AsSpan();
        src.CopyTo(Dst);
    }

    //Viable only on ValueTypes since it copies bytes
    [Benchmark]
    public void BufferBlock()
    {
        Buffer.BlockCopy(Src, 0, Dst, 0, Src.Length * sizeof(int));
    }

    [Benchmark]
    public unsafe void Unsafe()
    {
        fixed (int* src = Src, dest = Dst)
        {
            // Using pointers to copy elements
            var source = src;
            var destination = dest;

            for (var i = 0; i < Src.Length; i++)
            {
                *destination = *source;
                source++;
                destination++;
            }
        }
    }
}