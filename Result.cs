using System;

static partial class LFunctional {

    public record Result<S,F>() {
        public record Success(S Value)   : Result<S,F>;
        public record Failure(F Error)   : Result<S,F>;
    }

    public static Result<S,F> Success<S, F>(S s) => new Result<S,F>.Success(s);
    public static Result<S,F> Failure<S, F>(F e) => new Result<S,F>.Failure(e);

    public static Result<S,string> Success<S>(S s) => new Result<S,string>.Success(s);
    public static Result<S,string> Failure<S>(string e) => new Result<S,string>.Failure(e);

    public static K Match<S, F, K>(this Result<S,F> @this, Func<S,K> success, Func<F, K> failure)
        => @this switch {
            Result<S, F>.Success s => success(s.Value),
            Result<S, F>.Failure e => failure(e.Error),
            _                      => throw new Exception("Either success or failure.") };

    static public Result<S1, F> Map<S, S1,F>(this Result<S,F> @this, Func<S,S1> f)
        => @this.Match(
            v => Success<S1,F>(f(v)),
            e => Failure<S1, F>(e)
        ); 

    static public Result<S, F1> MapError<S, F, F1>(this Result<S, F> @this, Func<F, F1> f)
        => @this.Match(
            s => Success<S,F1>(s),
            e => Failure<S,F1>(f(e)));

    static public Result<S1, F> Bind<S, S1, F>(this Result<S, F> @this, Func<S, Result<S1,F>> f)
        => @this.Match(
            s => f(s),
            e => Failure<S1, F>(e));

    // for LINQ
    public static Result<S1,F> Select<S, S1, F>(this Result<S,F> @this, Func<S, S1> f)
        => @this.Map(f);

    public static Result<S2,F> SelectMany<S, S1, S2, F>
        (this Result<S,F> @this, Func<S, Result<S1,F>> bind, Func<S,S1,S2> proj)
        => @this.Match(
            s => bind(s).Match(
                s1 => Success<S2, F>(proj(s, s1)),
                e  => Failure<S2, F>(e)
            ),
            e => Failure<S2, F>(e) );

    public static void Iter<S, F>(this Result<S, F> @this, Action<S> a) {
        if(@this is Result<S,F>.Success s) a(s.Value);
    }
    public static void IterFailure<S, F>(this Result<S, F> @this, Action<F> a) {
        if(@this is Result<S,F>.Failure f) a(f.Error);
    }
        
    static private Result<S,F> Throw<S,F>() => throw new Exception("Either success or failure.");
}