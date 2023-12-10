using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using FastExpressionCompiler;
using Sigil;

var summary = BenchmarkRunner.Run<Bench>();

public class Subject : IPropertyYielder
{
    public int Prop { get; set; }

    public IEnumerable<object> Properties()
    {
        yield return Prop;
    }
}

public interface IPropertyYielder
{
    IEnumerable<object> Properties();
}

[MarkdownExporter]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[SimpleJob(RunStrategy.Throughput, launchCount: 5, invocationCount: 5)]
public class Bench
{
    private static readonly Subject Subject = new()
    {
        Prop = 10,
    };

    [Params(1, 1000, 10000)]
    public int N;

    [Benchmark]
    public void Direct()
    {
        for (var i = 0; i < N; i++)
        {
            var value = Subject.Prop;
        }
    }

    [Benchmark]
    public void CreateDelegate()
    {
        var info = (Func<Subject, int>)Delegate.CreateDelegate(typeof(Func<Subject, int>), typeof(Subject).GetProperty(nameof(Subject.Prop)).GetMethod);
        for (var i = 0; i < N; i++)
        {
            var value = info.Invoke(Subject);
        }
    }

    [Benchmark]
    public void GetValue()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionPropertyInfo.GetProp<int>(Subject);
        }
    }

    [Benchmark]
    public void MakeGenericDelegate()
    {
        var del = typeof(Subject)
            .GetProperty(nameof(Subject.Prop))
            .GetMethod
            .CreateDelegate(typeof(Func<,>)
            .MakeGenericType(typeof(Subject), typeof(int)));

        for (var i = 0; i < N; i++)
        {
            var value = (int)del.DynamicInvoke(Subject);
        }
    }

    [Benchmark]
    public void GenericDynamicInvoke()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionGenericDynamicInvoke.GetProp<int>(Subject);
        }
    }

    [Benchmark]
    public void MethodInfo()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionMethodInfo.GetProp<int>(Subject);
        }
    }

    [Benchmark]
    public void YieldProperty()
    {
        for (var i = 0; i < N; i++)
        {
            var value = (int)Subject.Properties().First();
        }
    }

    [Benchmark]
    public void ExpressionTrees()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionMethodExpressionTree.GetProp<int>(Subject);
        }
    }

    [Benchmark]
    public void ExpressionTrees_FastExpressionCompiler()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionMethodExpressionTreeFastCompile.GetProp<int>(Subject);
        }
    }

    [Benchmark]
    public void EmitIL()
    {
        for (var i = 0; i < N; i++)
        {
            var value = ReflectionMethodEmitIL.GetProp<int>(Subject);
        }
    }
}

public static class ReflectionPropertyInfo
{
    private static PropertyInfo _info;

    public static T GetProp<T>(object obj)
    {
        if (_info == null)
        {
            _info = typeof(Subject).GetProperty(nameof(Subject.Prop));
        }

        return (T)_info.GetValue(obj);
    }
}

public static class ReflectionMethodInfo
{
    private static MethodInfo _info;

    public static T GetProp<T>(object obj)
    {
        if (_info == null)
        {
            _info = typeof(Subject).GetProperty(nameof(Subject.Prop)).GetMethod;
        }

        return (T)_info.Invoke(obj, Type.EmptyTypes);
    }
}

public static class ReflectionMethodEmitIL
{
    private static object _cachedFunc;

    public static T GetProp<T>(object obj)
    {
        if (_cachedFunc == null)
        {
            _cachedFunc = Emit<Func<object, int>>
            .NewDynamicMethod()
            .LoadArgument(0)
            .CastClass(typeof(Subject))
            .Call(typeof(Subject).GetProperty(nameof(Subject.Prop)).GetGetMethod())
            .Return()
            .CreateDelegate();
        }

        return ((Func<object, T>)_cachedFunc)(obj);
    }
}

public static class ReflectionGenericDynamicInvoke
{
    private static Delegate _del;

    public static T GetProp<T>(object obj)
    {
        if (_del == null)
        {
            _del = typeof(Subject)
                .GetProperty(nameof(Subject.Prop))
                .GetMethod
                .CreateDelegate(typeof(Func<,>)
                .MakeGenericType(typeof(Subject), typeof(int)));
        }

        return (T)_del.DynamicInvoke(obj);
    }
}

public static class ReflectionMethodExpressionTree
{
    private static object _cachedFunc;

    public static T GetProp<T>(object obj)
    {
        if (_cachedFunc == null)
        {
            var info = typeof(Subject).GetProperty(nameof(Subject.Prop));
            var target = Expression.Parameter(typeof(object), "target");

            Expression getProperty = Expression.Call(
                Expression.Convert(target, info.DeclaringType),
                info.GetGetMethod()
            );

            var getter = Expression.Lambda<Func<object, int>>(
                getProperty, target
            ).Compile();

            _cachedFunc = getter;
        }

        return ((Func<object, T>)_cachedFunc)(obj);
    }
}

public static class ReflectionMethodExpressionTreeFastCompile
{
    private static object _cachedFunc;

    public static T GetProp<T>(object obj)
    {
        if (_cachedFunc == null)
        {
            var info = typeof(Subject).GetProperty(nameof(Subject.Prop));
            var target = Expression.Parameter(typeof(object), "target");

            Expression getProperty = Expression.Call(
                Expression.Convert(target, info.DeclaringType),
                info.GetGetMethod()
            );

            var getter = Expression.Lambda<Func<object, int>>(
                getProperty, target
            ).CompileFast();

            _cachedFunc = getter;
        }

        return ((Func<object, T>)_cachedFunc)(obj);
    }
}
