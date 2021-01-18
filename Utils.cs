using System;
using Unit = System.ValueTuple;

public static partial class LFunctional {

    public abstract record Wraps<T> {

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected T Value {get; init;} 
 #pragma warning restore CS8618 

        public static implicit operator T(Wraps<T> a) => a.Value;
    }

    public static Unit Unit() => default;
    public static T id<T>(T t) => t;

    public static Unit Log(string s) { Console.WriteLine(s); return Unit();}
    private static Func<T, Unit> ActionToFunc<T>(Action<T> a) => t => { a(t); return Unit();};
}
