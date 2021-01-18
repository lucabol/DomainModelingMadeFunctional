using System;
using static System.Console;

using static LFunctional;
using static SimpleTypes;
using static CompoundTypes;

TestResult();
TestSimpleTypes();
TestCompoundTypes();

void TestCompoundTypes() {
    var r = GuinnessPerson.Of("right", 3, 5);
    var w = GuinnessPerson.Of("wrong", 5, 3);

    r.ForEach(s => Log($"{s.Name} is right"));
    w.Match(s => Log($"{s.Name} is wrong"), e => Log($"Error {e}"));
}

void TestSimpleTypes() {
    var s = from bob in Age.Of(80)
            from jon in Age.Of(50)
            select jon + bob;

    Log($"bob + jon: {s}");

    // Compiler errors ...
    //var a = new Age();
    //var a = new Age(400);
    //var b = new Name();
    //var b = new Name("bob");

    var mathus = Age.Of(400);

    var t = from bob in Age.Of(80)
            from jon in Age.Of(50)
            from m in mathus
            select jon + bob + m;

    t.ForEach(t => Log($"{t} shouldn't print"));
    t.Match(v => Log($"{v} shouldn't print"), e => Log(e));

    var n = Name.Of("luca");

    // strings and ints can be added
    var z = from bob in Age.Of(80)
            from jon in Age.Of(50)
            from m in Name.Of("luca")
            select jon + bob + m; // Implicit conversion, no need to do a .Value

    z.ForEach(WriteLine);

    mathus.Match(m => throw new Exception(m.ToString()), e => Log(e));

    var p = Age.Of(32);
    var pp = p.Match(id, e => throw new Exception(e));

    // This made not to compile, otherwise it would bypass the Create function
    // var p2 = pp with { Value = 3};

    var zip = ZipCode.Of("ffessf");
    var rip = ZipCode.Of("12345");

    var zips = from r in zip
               from w in rip
               select r + ";" + w;

    zips.ForEachFailure(e => Log(e));
}

void TestResult() {
    var r1 = Success(1);
    var r2 = Success(2);

    var x = from i1 in r1
            from i2 in r2
            select i1 + i2;

    Log($"Result: {x}");

    var r3 = Failure<int>("Strange failure");

    var y = from i1 in r1
            from i2 in r2
            from i3 in r3
            select i1 + i2 + i3;

    Log($"Failure: {y}");

    var k1 = x.Match(
        success: v => v + 1,
        failure: e => 0);

    var p1 = Success(2);
    var p2 = Success(3);
    var p3 = Failure<int>("It's a fail");

    var pr = from i in p1
             from j in p2
             from u in p3
             let w = i + j
             select i + u;

    Log($"Failure: {pr}");

    p1.ForEach(i => Log($"{i}"));
}
