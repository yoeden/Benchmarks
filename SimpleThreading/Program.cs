using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Threading.Tasks.Dataflow;

var summary = BenchmarkRunner.Run<Bench>();

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Bench
{
    public static readonly List<int[]> Numbers = new List<int[]>();

    static Bench()
    {
        const int Buckets = 8;
        const int From = 0;
        const int To = 100_000;

        Span<int> whole = Enumerable.Range(From, To).ToArray();
        var n = To / Buckets;

        for (var i = 0; i < Buckets; i++)
        {
            var slice = whole.Slice(i * n, n);
            Numbers.Add(slice.ToArray());
        }
    }

    //

    [Benchmark]
    public void Base()
    {
        var sum = 0;
        for (var i = 0; i < Numbers.Count; i++)
        {
            for (var j = 0; j < Numbers[i].Length; j++)
            {
                sum += Numbers[i][j];
            }
        }
    }

    [Benchmark]
    public void ParallelForEach()
    {
        var sum = 0;
        Parallel.ForEach(Numbers, (n) =>
        {
            var localSum = 0;
            for (var i = 0; i < n.Length; i++)
            {
                localSum += n[i];
            }

            Interlocked.Add(ref sum, localSum);
        });
    }

    [Benchmark]
    public void ParallelFor()
    {
        var sum = 0;
        Parallel.For(0, Numbers.Count, (index) =>
        {
            var localSum = 0;
            var bucket = Numbers[index];
            for (var i = 0; i < bucket.Length; i++)
            {
                localSum += bucket[i];
            }

            Interlocked.Add(ref sum, localSum);
        });
    }

    [Benchmark]
    public async Task TaskWhenAll()
    {
        var sum = 0;
        var tasks = new Task[Numbers.Count];
        for (var taskIndex = 0; taskIndex < tasks.Length; taskIndex++)
        {
            var index = taskIndex;
            tasks[taskIndex] = Task.Run(() =>
            {
                var localSum = 0;
                var bucket = Numbers[index];
                for (var i = 0; i < bucket.Length; i++)
                {
                    localSum += bucket[i];
                }

                Interlocked.Add(ref sum, localSum);
            });
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public void TaskWaitAll()
    {
        var sum = 0;
        var tasks = new Task[Numbers.Count];
        for (var taskIndex = 0; taskIndex < tasks.Length; taskIndex++)
        {
            var index = taskIndex;
            tasks[taskIndex] = Task.Run(() =>
            {
                var localSum = 0;
                var bucket = Numbers[index];
                for (var i = 0; i < bucket.Length; i++)
                {
                    localSum += bucket[i];
                }

                Interlocked.Add(ref sum, localSum);
            });
        }

        Task.WaitAll(tasks);
    }

    [Benchmark]
    public void Threads_Join()
    {
        var sum = 0;
        var threads = new Thread[Numbers.Count];
        var semaphore = new SemaphoreSlim(Numbers.Count);

        for (var threadIndex = 0; threadIndex < threads.Length; threadIndex++)
        {
            var index = threadIndex;
            threads[threadIndex] = new Thread(() =>
            {
                var localSum = 0;
                var bucket = Numbers[index];
                for (var i = 0; i < bucket.Length; i++)
                {
                    localSum += bucket[i];
                }

                Interlocked.Add(ref sum, localSum);
            });
            threads[threadIndex].Start();
        }

        for (var threadIndex = 0; threadIndex < threads.Length; threadIndex++)
        {
            threads[threadIndex].Join();
        }
    }

    [Benchmark]
    public async Task ActionBlock()
    {
        var sum = 0;
        var block = new ActionBlock<int[]>((numbers) =>
        {
            var localSum = 0;
            for (var i = 0; i < numbers.Length; i++)
            {
                localSum += numbers[i];
            }

            Interlocked.Add(ref sum, localSum);
        });

        for (var i = 0; i < Numbers.Count; i++)
        {
            block.Post(Numbers[i]);
        }

        block.Complete();
        await block.Completion;
    }

    [Benchmark]
    public async Task ThreadPoolQueueUserWorkItem()
    {
        var sum = 0;
        var countdown = new CountdownEvent(Numbers.Count);

        for (var i = 0; i < Numbers.Count; i++)
        {
            var index = i;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var numbers = Numbers[index];
                var localSum = 0;
                for (var i = 0; i < numbers.Length; i++)
                {
                    localSum += numbers[i];
                }

                Interlocked.Add(ref sum, localSum);
                countdown.Signal();
            });
        }

        countdown.Wait();
    }
}