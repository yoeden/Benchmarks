# Array Copy

What is the most effiencet way to copy array from place to another ?

## Result

| Method       | Mean      | Error     | StdDev    | Completed Work Items | Lock Contentions | Allocated |
|------------- |----------:|----------:|----------:|---------------------:|-----------------:|----------:|
| Manually     | 28.466 us | 0.1294 us | 0.1147 us |                    - |                - |         - |
| Unsafe       | 21.475 us | 0.1885 us | 0.1763 us |                    - |                - |         - |
| ArrayCopy    |  7.109 us | 0.0461 us | 0.0431 us |                    - |                - |         - |
| MemoryCopyTo |  7.062 us | 0.0353 us | 0.0331 us |                    - |                - |         - |
| SpanCopyTo   |  7.092 us | 0.0851 us | 0.0754 us |                    - |                - |         - |
| BufferBlock  |  7.110 us | 0.0573 us | 0.0536 us |                    - |                - |         - |

### Manually

Looping through all the items, copying values (in case of value types, duplicating the entire value) from src to dst.

#### Benchmark Code

```csharp
for (var i = 0; i < Src.Length; i++)
{
    Dst[i] = Src[i];
}
```

### Unsafe

Using pointers to loop through all the items, doing the same as a loop but iterating use a memory.


#### Benchmark Code

```csharp
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
```

### Memmove

All the other methods are using an internal call for `memmove`, which is a function provided by the runtime, probably like `memcpy` in c.
The copies an entire block of memory instead of iterating section by section in that memory.

```csharp
[LibraryImport(RuntimeHelpers.QCall, EntryPoint = "Buffer_MemMove")]
private static unsafe partial void __Memmove(byte* dest, byte* src, nuint len);
```