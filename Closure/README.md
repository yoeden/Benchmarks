# Function Invoking

SUMMARY

## Result

| Method              | Mean      | Error     | StdDev    | Median    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------:|----------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
| Closure             | 6.3021 ns | 0.1442 ns | 0.1543 ns | 6.2531 ns | baseline |         | 0.0053 |      88 B |             |
| Parameter           | 0.2132 ns | 0.0028 ns | 0.0024 ns | 0.2137 ns |   -96.6% |    2.7% |      - |         - |       -100% |
| StaticParameter     | 0.2168 ns | 0.0024 ns | 0.0021 ns | 0.2164 ns |   -96.6% |    2.5% |      - |         - |       -100% |
| LocalFunction       | 0.0013 ns | 0.0023 ns | 0.0022 ns | 0.0000 ns |  -100.0% |  165.3% |      - |         - |       -100% |
| LocalStaticFunction | 0.0018 ns | 0.0022 ns | 0.0019 ns | 0.0021 ns |  -100.0% |  105.2% |      - |         - |       -100% |
| InstanceMethod      | 0.0016 ns | 0.0028 ns | 0.0026 ns | 0.0000 ns |  -100.0% |  162.8% |      - |         - |       -100% |
| StaticMethod        | 0.0005 ns | 0.0010 ns | 0.0009 ns | 0.0000 ns |  -100.0% |  171.1% |      - |         - |       -100% |
### Closure

Closure in terms of functions, remarks the "capturing" of an _outside_ (a higher scope) variable for the _internal_ use of the lambda.

In the compiled code we can see that the compiler is generated a class, `<>c__DisplayClass0_0` (the ugly name indicates it was generated).
We can see our lambda logic in this class `<Closure>b__0`, and the variable `a` sitting in there.
Basically, capturing the variable `a` through a object.

Thus, we allocating a hidden class and a new Func for that matter.

#### Benchmark Code

```csharp
var a = 23;
var f = () => a + a / a; 
```

#### Compiled Code

```csharp
private sealed class <>c__DisplayClass0_0
{
    public int a;

    internal int <Closure>b__0()
    {
        return a + a / a;
    }
}

public int Closure()
{
    <>c__DisplayClass0_0 <>c__DisplayClass0_ = new <>c__DisplayClass0_0();
    <>c__DisplayClass0_.a = 23;
    Func<int> func = new Func<int>(<>c__DisplayClass0_.<Closure>b__0);
    return func();
}
```

### Passing parameter to lambda

This time around we see the compiler still generating a class behind the scens, but this time
the class is sort of a Singleton of the lambda.

The single allocation comes from the singleton allocation for the ` <>9__0_0` field refrencing the 
`<Parameter>b__0_0` method, thus making it a long lived object, reducing allocation to none.

#### Benchmark Code

```csharp
var a = 23;
Func<int, int> f = (x) => x + x / x;
return f(a);
```

#### Compiled Code

```csharp
private sealed class <>c
{
    public static readonly <>c <>9 = new <>c();

    public static Func<int, int> <>9__0_0;

    internal int <Parameter>b__0_0(int x)
    {
        return x + x / x;
    }
}

public int Parameter()
{
    int arg = 23;
    Func<int, int> func = <>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Func<int, int>(<>c.<>9.<Parameter>b__0_0));
    return func(arg);
}
```

### Static lambda

It compiles exactly the same as the method before.
`Allow a 'static' modifier on lambdas and anonymous methods, which disallows capture of locals or instance state from containing scopes.`.
Basically, the static modifiers on lambda is there to help us protect ourself against closure.

#### Benchmark Code

```csharp
var a = 23;
Func<int, int> f = static (x) => x + x / x;
return f(a);
```

### Local method

We can see that the compiler extract the local method that a static internal one.
So basically its the same as calling a static method.

#### Benchmark Code

```csharp
var a = 23;

int Func(int x)
{
    return x + x / x;
}

return Func(a);
```

#### Compiled Code

```csharp
public int LocalFunction()
{
    int x = 23;
    return <LocalFunction>g__Func|0_0(x);
}

[CompilerGenerated]
internal static int <LocalFunction>g__Func|0_0(int x)
{
    return x + x / x;
}
```

### Instance vs Static method

Even though its trivial to understand that instance and static method gets native perfomance and zero allocations, 
Static method seems to have a slight faster runtime than instance method.
This is due the `this` parameter of instance method, being passed to every method that is not static in a class, addin a little bit of overhead.