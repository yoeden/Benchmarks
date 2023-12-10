# Reflection - Property Get

One of the challanges here is that reflection uses object to support all values without knowing their types, but it takes more runtime and memory to box and unbox.
The most generic possible way to achive this is ```T GetPropValue<T>(object instance,string Prop)```.

## Benchmark Result

| Method                                 | N     | Mean            | Error         | StdDev        | Median          | Completed Work Items | Lock Contentions | Allocated |
|--------------------------------------- |------ |----------------:|--------------:|--------------:|----------------:|---------------------:|-----------------:|----------:|
| Direct                                 | 1     |        45.95 ns |      2.195 ns |      12.78 ns |        40.00 ns |                    - |                - |      80 B |
| CreateDelegate                         | 1     |     3,201.54 ns |     47.270 ns |     281.18 ns |     3,140.00 ns |                    - |                - |     331 B |
| GetValue                               | 1     |       328.99 ns |      5.296 ns |      34.56 ns |       320.00 ns |                    - |                - |     104 B |
| MakeGenericDelegate                    | 1     |    29,293.07 ns |    419.753 ns |   2,470.59 ns |    28,580.00 ns |                    - |                - |    1054 B |
| GenericDynamicInvoke                   | 1     |    25,144.11 ns |    335.495 ns |   2,069.76 ns |    24,560.00 ns |                    - |                - |     643 B |
| MethodInfo                             | 1     |       305.09 ns |      5.029 ns |      33.10 ns |       300.00 ns |                    - |                - |     104 B |
| YieldProperty                          | 1     |       396.97 ns |      7.447 ns |      49.38 ns |       380.00 ns |                    - |                - |     144 B |
| ExpressionTrees                        | 1     |        73.95 ns |      2.064 ns |      13.41 ns |        80.00 ns |                    - |                - |      22 B |
| ExpressionTrees_FastExpressionCompiler | 1     |        78.92 ns |      1.901 ns |      12.11 ns |        80.00 ns |                    - |                - |      80 B |
| EmitIL                                 | 1     |        98.00 ns |      1.886 ns |      11.79 ns |       100.00 ns |                    - |                - |      80 B |
| Direct                                 | 1000  |     5,260.25 ns |     57.190 ns |     309.49 ns |     5,180.00 ns |                    - |                - |      80 B |
| CreateDelegate                         | 1000  |    16,597.58 ns |    128.443 ns |     376.70 ns |    16,460.00 ns |                    - |                - |     331 B |
| GetValue                               | 1000  |    25,399.57 ns |    786.622 ns |   3,618.41 ns |    25,800.00 ns |                    - |                - |   24080 B |
| MakeGenericDelegate                    | 1000  |   220,530.07 ns |  2,526.323 ns |   8,724.10 ns |   217,880.00 ns |                    - |                - |   56998 B |
| GenericDynamicInvoke                   | 1000  |   213,121.51 ns |  2,002.888 ns |   6,473.96 ns |   210,700.00 ns |                    - |                - |   56587 B |
| MethodInfo                             | 1000  |    23,502.01 ns |    576.041 ns |   3,510.38 ns |    24,890.00 ns |                    - |                - |   24080 B |
| YieldProperty                          | 1000  |    47,770.86 ns |    455.858 ns |   1,200.91 ns |    47,520.00 ns |                    - |                - |   64080 B |
| ExpressionTrees                        | 1000  |     8,109.92 ns |     71.069 ns |     329.05 ns |     7,980.00 ns |                    - |                - |      80 B |
| ExpressionTrees_FastExpressionCompiler | 1000  |     8,407.45 ns |     69.030 ns |     301.17 ns |     8,300.00 ns |                    - |                - |      80 B |
| EmitIL                                 | 1000  |     8,718.76 ns |    123.195 ns |     533.57 ns |     8,520.00 ns |                    - |                - |      80 B |
| Direct                                 | 10000 |     7,469.31 ns |     67.159 ns |     199.07 ns |     7,520.00 ns |                    - |                - |      80 B |
| CreateDelegate                         | 10000 |    30,849.77 ns |    210.694 ns |     580.31 ns |    30,830.00 ns |                    - |                - |     331 B |
| GetValue                               | 10000 |   134,416.32 ns |  1,093.447 ns |   4,165.24 ns |   132,260.00 ns |                    - |                - |  240080 B |
| MakeGenericDelegate                    | 10000 | 1,767,190.75 ns | 96,671.915 ns | 592,766.78 ns | 1,348,440.00 ns |                    - |                - |  560998 B |
| GenericDynamicInvoke                   | 10000 | 1,764,164.72 ns | 65,727.119 ns | 325,226.81 ns | 1,767,760.00 ns |                    - |                - |  560587 B |
| MethodInfo                             | 10000 |   129,534.35 ns |  1,044.047 ns |   3,448.44 ns |   128,250.00 ns |                    - |                - |  240080 B |
| YieldProperty                          | 10000 |   366,923.87 ns |  4,086.203 ns |  14,907.73 ns |   366,620.00 ns |                    - |                - |  640080 B |
| ExpressionTrees                        | 10000 |    26,844.06 ns |    124.709 ns |     301.19 ns |    26,740.00 ns |                    - |                - |      80 B |
| ExpressionTrees_FastExpressionCompiler | 10000 |    29,135.08 ns |    251.140 ns |     850.30 ns |    29,010.00 ns |                    - |                - |      80 B |
| EmitIL                                 | 10000 |    33,488.31 ns |    148.368 ns |     346.80 ns |    33,360.00 ns |                    - |                - |      80 B |

## Direct

No need to explain.

| Aspect                   |Level   |
| ------------------------ |------- |
| 📦 Boxing                |✔️ No |
| 🔍 Reflection            |✔️ None |
| 🤸 Flexability (Generic) |❌ None  |
| ⏱️ Perfomance            |✔️ High  |
| 🛠️ Maintainability       |✔️ High  |

#### Benchmark

```csharp
int value = Subject.Value;
```

## CreateDelegate

TODO

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                |✔️ No  |
| 🔍 Reflection            |✔️ Low  |
| 🤸 Flexability (Generic) |❌ Low  |
| ⏱️ Perfomance            |✔️ Mid |
| 🛠️ Maintainability       |✔️ Mid  |

#### Benchmark

```csharp
var funcType = typeof(Func<Subject, int>);
var getMethod =  typeof(Subject).GetProperty(nameof(Subject.Value)).GetMethod;
var info = (Func<Subject, int>)Delegate.CreateDelegate(funcType, getMethod);

int value = info.Invoke(SubjectInstance);
```

## GetValue

TODO

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ❌ Yes  |
| 🔍 Reflection            | ❌ Mid  |
| 🤸 Flexability (Generic) | ❌ Low  |
| ⏱️ Perfomance            | ❌ Low  |
| 🛠️ Maintainability       | ✔️ Mid  |

```csharp
var propInfo = typeof(Subject).GetProperty(nameof(Subject.Value));
int value = (int)propInfo.GetValue(SubjectInstance);
```


## MakeGenericDelegate

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ❌ Yes  |
| 🔍 Reflection            | ❌ Mid  |
| 🤸 Flexability (Generic) | ❌ Low  |
| ⏱️ Perfomance            | ❌ Low  |
| 🛠️ Maintainability       | ✔️ Mid  |

#### Benchmark
```csharp
var del = typeof(Subject)
    .GetProperty(nameof(Subject.Value))
    .GetMethod
    .CreateDelegate(typeof(Func<,>)
    .MakeGenericType(typeof(Subject), typeof(int)));

int value = (int)del.DynamicInvoke(SubjectInstance)
```

## MethodInfo

MethodInfo heavily relies on reflection constatly to keep seacrhing the method and invoke from the assembly type metadata.

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ❌ Yes  |
| 🔍 Reflection            | ❌ High  |
| 🤸 Flexability (Generic) | ✔️ High  |
| ⏱️ Perfomance            | ✔️ Mid  |
| 🛠️ Maintainability       | ✔️ Mid  |

#### Benchmark Code

```csharp
var methoInfo = typeof(Subject).GetProperty(nameof(Subject.Value)).GetMethod;
int value = (int)methoInfo.Invoke(SubjectInstance, Type.EmptyTypes);
```

## YieldProperty

It may seem like an intuitive solution since we basically yielding the properties values.
But behind the scenes the `IEnumerable` is forcing us to create a `IEnumerator`, and the `First()` is forcing us to use LINQ.
So it turns out to be less perfomant than expected, plus were still bounded by boxing.

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ❌ Yes  |
| 🔍 Reflection            | ✔️ None  |
| 🤸 Flexability (Generic) | ✔️ High  |
| ⏱️ Perfomance            | ❌ Low  |
| 🛠️ Maintainability       | ❌ Mid  |

#### Benchmark Code

```csharp
int value = (int)Subject.Properties().First();

public class Subject : IPropertyYielder
{
    public int Value { get; set; }

    public IEnumerable<object> Properties()
    {
        yield return Prop;
    }
}

public interface IPropertyYielder
{
    IEnumerable<object> Properties();
}
```

## ExpressionTrees

_`Expression trees represent code in a tree-like data structure`_ - Microsoft.
We can compile it to native IL code (let the runtime handle the emitting).
The maintability suffer a little bit but keeps us in the C# scope and not the IL like the [EmitIL](#EmitIL) Solution.

We basically compile a method call, 
that calls the getter method, 
receving object and `Convert` it to our type, returning int straight ahead, avoiding boxing.

Because we create a method calling the get method instead of calling it directly, we suffer a slight perofmance hit.


| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ✔️ No  |
| 🔍 Reflection            | ✔️ Low  |
| 🤸 Flexability (Generic) | ✔️ High  |
| ⏱️ Perfomance            | ✔️ High  |
| 🛠️ Maintainability       | ✔️ Mid  |

#### Benchmark

```csharp
var info = typeof(Subject).GetProperty(nameof(Subject.Value));
var target = Expression.Parameter(typeof(object), "target");

Expression getProperty = Expression.Call(
    Expression.Convert(target, info.DeclaringType),
    info.GetGetMethod()
);

var getter = Expression.Lambda<Func<object, int>>(
    getProperty, target
).Compile();

int value = getter(SubjectInstance);
```

## ExpressionTrees_FastExpressionCompiler

Works exactly like [ExpressionTrees](#ExpressionTrees), but uses the library [FastExpressionCompiler](https://www.nuget.org/packages/FastExpressionCompiler), which magically adds a little bit of performance.

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ✔️ No  |
| 🔍 Reflection            | ✔️ Low  |
| 🤸 Flexability (Generic) | ✔️ High  |
| ⏱️ Perfomance            | ✔️ High  |
| 🛠️ Maintainability       | ✔️ Mid  |

#### Benchmark

```csharp
var info = typeof(Subject).GetProperty(nameof(Subject.Value));
var target = Expression.Parameter(typeof(object), "target");

Expression getProperty = Expression.Call(
    Expression.Convert(target, info.DeclaringType),
    info.GetGetMethod()
);

var getter = Expression.Lambda<Func<object, int>>(
    getProperty, target
).CompileFast();

int value = getter(SubjectInstance);
```

## EmitIL

Emitting _should be_ the fastest way, and its on par with expression tree.
Its not worth the risk of emitting wrong IL code, and the maintability will suffer as well, requiring the developers to be familiar with IL.

The IL of getting g property : 
```csharp
IL_0000: nop
IL_0001: ldarg.0
IL_0002: callvirt instance int32 Subject::get_Value()
IL_0007: stloc.0
IL_0008: br.s IL_000a

IL_000a: ldloc.0
IL_000b: ret
```

_Since this is a static method we use `ldarg.0`, apparently `ldarg.0` is reserved for this point in an instance method._

| Aspect                   | Level   |
| ------------------------ | ------- |
| 📦 Boxing                | ✔️ No  |
| 🔍 Reflection            | ✔️ Low  |
| 🤸 Flexability (Generic) | ✔️ High  |
| ⏱️ Perfomance            | ✔️ High  |
| 🛠️ Maintainability       | ❌ High  |

#### Benchmark

```csharp
var getter = Emit<Func<object, int>>
              .NewDynamicMethod()
              .LoadArgument(0)
              .CastClass(typeof(Subject))
              .Call(typeof(Subject).GetProperty(nameof(Subject.Value)).GetGetMethod())
              .Return()
              .CreateDelegate();

int value = getter(SubjectInstance);
```

