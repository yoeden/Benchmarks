using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Bench>();

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class Bench
{
    private int _sharedValue = 0;
    private readonly object _lockObject = new();
    private readonly Semaphore _semaphore = new(1, 1);
    private readonly AutoResetEvent _autoResetEvent = new(true);
    private readonly ManualResetEventSlim _manualResetEventSlim = new(true);
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly Mutex _mutex = new(true);
    private readonly ReaderWriterLock _readerWriterLock = new();
    private readonly CountdownEvent _countdown = new(1);

    [Params(10, 100, 1000)] // Loop counts for different scenarios
    public int LoopSize { get; set; }

    [Benchmark]
    public void Lock()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            lock (_lockObject)
            {
                _sharedValue++;
            }
        }
    }

    [Benchmark]
    public void NoLock()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _sharedValue++;
        }
    }

    [Benchmark]
    public void InterLocked()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            Interlocked.Increment(ref _sharedValue);
        }
    }

    [Benchmark]
    public void Monitor()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            System.Threading.Monitor.Enter(_lockObject);
            try
            {
                _sharedValue++;
            }
            finally
            {
                System.Threading.Monitor.Exit(_lockObject);
            }
        }
    }

    [Benchmark]
    public void Semaphore()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _semaphore.WaitOne();
            try
            {
                _sharedValue++;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    [Benchmark]
    public void AutoResetEvent()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _autoResetEvent.WaitOne();
            try
            {
                _sharedValue++;
            }
            finally
            {
                _autoResetEvent.Set();
            }
        }
    }

    [Benchmark]
    public void ManualResetEventSlim()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _manualResetEventSlim.Wait();
            try
            {
                _sharedValue++;
            }
            finally
            {
                _manualResetEventSlim.Set();
            }
        }
    }

    [Benchmark]
    public void SemaphoreSlim()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _semaphoreSlim.Wait();
            try
            {
                _sharedValue++;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }

    [Benchmark]
    public void Mutex()
    {
        for (int i = 0; i < LoopSize; i++)
        {
            _mutex.WaitOne();
            try
            {
                _sharedValue++;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }


    //[Benchmark]
    //public void Countdown()
    //{
    //    for (int i = 0; i < LoopSize; i++)
    //    {
    //        _countdown.Wait();
    //        _countdown.AddCount();
    //        try
    //        {
    //            _sharedValue++;
    //        }
    //        finally
    //        {
    //            _countdown.Signal();
    //        }
    //    }
    //}
}