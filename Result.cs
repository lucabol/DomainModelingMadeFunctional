using System;

static partial class LFunctional {

    public record Error(string Message) {
        public static implicit operator Error(string s) => new Error(s);
        public static implicit operator string(Error e) => e.Message;

        public static Error DefaultError = new Error("Unknown error");
    }

    // The elevated type
    public record Result<S>() {
        internal record Success(S Value)   : Result<S>;
        internal record Failure(Error Error)   : Result<S>;

        public static implicit operator Result<S>(S s)      => Success(s);
        public static implicit operator Result<S>(Error e)  => Failure<S>(e);
        public static implicit operator Result<S>(string s) => Failure<S>(s);

    }

    // Return functions
    public static Result<S> Success<S>(S s)         => new Result<S>.Success(s);
    public static Result<S> Failure<S>(Error e)     => new Result<S>.Failure(e);
    public static Result<S> Failure<S>(string s)    => new Result<S>.Failure(s);


    // Extractor
    public static R Match<S,R>(this Result<S> @this, Func<S,R> success, Func<Error, R> failure)
        => @this switch {
            Result<S>.Success s => success(s.Value),
            Result<S>.Failure e => failure(e.Error),
            _                      => throw new Exception("Either success or failure.") };

    // Classical functional lore
    public static Result<R> Map<S,R>(this Result<S> @this, Func<S,R> f)
        => @this.Match(
            v => f(v),
            Failure<R>); 

    public static Result<S> MapError<S>(this Result<S> @this, Func<Error, Error> f)
        => @this.Match(
            s => s,
            e => Failure<S>(f(e)));

    public static Result<R> Bind<S,R>(this Result<S> @this, Func<S,Result<R>> f)
        => @this.Match(
            s => f(s),
            Failure<R>);

    public static Result<R> Apply<S,R>(Result<Func<S,R>> fR, Result<S> xR) =>
        (fR, xR) switch {
            (Result<Func<S,R>>.Success sf,Result<S>.Success sx)
                                                    => sf.Value(sx.Value),
            (Result<Func<S,R>>.Failure sf, _)       => Failure<R>(sf.Error),
            (_ ,Result<S>.Failure sx)               => Failure<R>(sx.Error),
            _                                       => Throw<R>()
        };

    // Playing nice with Actions
    public static void ForEach<S>(this Result<S> @this, Action<S> a) {
        if(@this is Result<S>.Success s) a(s.Value);
    }
    public static void ForEachFailure<S>(this Result<S> @this, Action<Error> a) {
        if(@this is Result<S>.Failure f) a(f.Error);
    }

    // for LINQ
    public static Result<S1> Select<S,S1>(this Result<S> @this, Func<S, S1> f)
        => @this.Map(f);

    public static Result<R> SelectMany<S,S1,R>
        (this Result<S> @this, Func<S,Result<S1>> bind, Func<S,S1,R> proj)
        => @this.Match(
            s => bind(s).Match(
                s1 => proj(s, s1),
                e  => Failure<R>(e)
            ),
            e => Failure<R>(e) );

    // Unfortunately no way to specify an error if the Where clause is not satisfied
    public static Result<S> Where<S>(this Result<S> @this, Func<S, bool> pred)
        => @this.Match(
            s => pred(s) ? Success<S>(s) : Failure<S>(Error.DefaultError), e => Failure<S>(e));

    public static Result<S> WhereError<S>(this Result<S> @this, Error e) =>
        @this.Match(
            _ => @this,
            te => te == Error.DefaultError ? Failure<S>(e) : @this);

    // Utilities 
    static private Result<S> Throw<S>() => throw new Exception("Either success or failure.");
}