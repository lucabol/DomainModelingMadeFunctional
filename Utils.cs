using System;
using Unit = System.ValueTuple;

public static partial class LFunctional {
    public static Unit Unit() => default;
    public static T id<T>(T t) => t;

    public static Unit Log(string s) { Console.WriteLine(s); return Unit();}
    private static Func<T, Unit> ActionToFunc<T>(Action<T> a) => t => { a(t); return Unit();};
}
