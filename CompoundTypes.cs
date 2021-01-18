using static SimpleTypes;
using static LFunctional;

public static partial class CompoundTypes {

    // No need for any cross properties validation
    public record Person(Name name, Age age);

    // Cross property validation at construction
    public record GuinnessPerson {

        public Name Name { get;}
        public Age  RealAge { get;}
        public Age  FakeAge { get;}

        private GuinnessPerson(Name name, Age realAge, Age fakeAge) =>
            (Name, RealAge, FakeAge) = (name, realAge, fakeAge);

        public static Result<GuinnessPerson> Of(string name, int realAge, int fakeAge)
            => (from n in Name.Of(name)
                from r in Age.Of(realAge)
                from f in Age.Of(fakeAge)
                where r < f 
                select new GuinnessPerson(n, r, f))
                .WhereError(
                    $"Error constructing {nameof(GuinnessPerson)}. RealAge({realAge}) >= FakeAge({fakeAge})");
    }
}