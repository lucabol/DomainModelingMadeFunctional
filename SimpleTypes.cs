/*
    Smart constructor record examples.
    You should be able to construct them just through the Create functions.
    1. Disable parameterless constructor
    2. Implicit conversion to the wrapped type (it is always safe)
*/

using static LFunctional;


public static partial class SimpleTypes {


    public record Wraps<T> {

 #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected T Value { get; init;}
 #pragma warning restore CS8618 

        public static implicit operator T(Wraps<T> a) => a.Value;
    }
    // 1st syntax
    public record Age: Wraps<int> {

        private Age() { }

        public static Result<Age, string> Create(int age)
            => ConstrainedInt(age, 1, 200, new Age { Value = age }, "Age outside [1,200]");
    }

    // 2nd syntax
    public record Name: Wraps<string> {

        private Name(string v) => Value = v;

        public static Result<Name, string> Create(string name)
            => ConstrainedString(name, 50, new Name (name), "Name more than 50 chars");
    }

    public record ZipCode: Wraps<string> {

        private ZipCode() { }

        public static Result<ZipCode, string> Create(string name)
            => StringLike(name, @"\d{5}", new ZipCode { Value = name}, "Not a zipcode");
    }

    private static Result<S, F> ConstrainedInt<S,F>(int v, int start, int end, S s, F f)
        => v >= start && v <= end ? Success<S,F>(s) : Failure<S, F>(f);

    private static Result<S, F> ConstrainedString<S,F>(string v, int maxChars, S s, F f)
        => v.Length <= maxChars ? Success<S,F>(s) : Failure<S, F>(f);

    private static Result<S, F> StringLike<S,F>(string v, string pattern, S s, F f)
        => System.Text.RegularExpressions.Regex.IsMatch(v, pattern) ? Success<S,F>(s) : Failure<S, F>(f);
}