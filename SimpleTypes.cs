﻿/*
    Smart constructor record examples.
    You should be able to construct them exclusively through the Create functions.
    1. Disable parameterless constructor
    2. Implicit conversion to the wrapped type in the Wraps type
*/

using static LFunctional;


public static partial class SimpleTypes {

    // 1st syntax -> using empty constructor
    public record Age: Wraps<int> {

        private Age() { }

        public static Result<Age> Of(int age)
            => ConstrainedInt(age, 1, 200, new Age { Value = age }, "Age outside [1,200]");
    }

    // 2nd syntax -> using non empty constructor
    public record Name: Wraps<string> {

        private Name(string v) => Value = v;

        public static Result<Name> Of(string name)
            => ConstrainedString(name, 50, new Name (name), "Name more than 50 chars");
    }

    public record ZipCode: Wraps<string> {

        private ZipCode() { }

        public static Result<ZipCode> Of(string name)
            => StringLike(name, @"\d{5}", new ZipCode { Value = name}, "Not a zipcode");
    }

    private static Result<S> ConstrainedInt<S>(int v, int start, int end, S s, Error f)
        => v >= start && v <= end ? s : Failure<S>(f);

    private static Result<S> ConstrainedString<S>(string v, int maxChars, S s, Error f)
        => v.Length <= maxChars ? s : Failure<S>(f);

    private static Result<S> StringLike<S>(string v, string pattern, S s, Error f)
        => System.Text.RegularExpressions.Regex.IsMatch(v, pattern) ? s : Failure<S>(f);
}