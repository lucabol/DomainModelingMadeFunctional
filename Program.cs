using System;
using static System.Console;

using static LFunctional;
using static SimpleTypes;

TestResult();
TestSimpleTypes();

void TestSimpleTypes() {
    var s = from bob in Age.Create(80)
            from jon in Age.Create(50)
            select jon + bob;

    Log($"bob + jon: {s}");

    //var a = new Age(); // compiler error
    // var a = new Age(400) // compiler error

    var mathus = Age.Create(400);

    var t = from bob in Age.Create(80)
            from jon in Age.Create(50)
            from m in mathus
            select jon + bob + m;

    t.Iter(t => Log($"{t} shouldn't print"));
    t.Match(v => Log($"{v} shouldn't print"), e => Log(e));

    var n = Name.Create("luca");

    // strings and ints can be added
    var z = from bob in Age.Create(80)
            from jon in Age.Create(50)
            from m in Name.Create("luca")
            select jon + bob + m; // Implicit conversion, no need to do a .Value

    z.Iter(WriteLine);

    mathus.Match(m => throw new Exception(m.ToString()), Log);

    var p = Age.Create(32);
    var pp = p.Match(id, e => throw new Exception(e));

    // This made not to compile, otherwise it would bypass the Create function
    // var p2 = pp with { Value = 3};

    var zip = ZipCode.Create("ffessf");
    var rip = ZipCode.Create("12345");

    var zips = from r in zip
               from w in rip
               select r + ";" + w;

    zips.IterFailure(WriteLine);
}

void TestResult() {
    var r1 = Success<int, string>(1);
    var r2 = Success<int, string>(2);

    var x = from i1 in r1
            from i2 in r2
            select i1 + i2;

    Log($"Result: {x}");

    var r3 = Failure<int, string>("Strange failure");

    var y = from i1 in r1
            from i2 in r2
            from i3 in r3
            select i1 + i2 + i3;

    Log($"Failure: {y}");

    var k = x switch {
        Result<int,string>.Success s => s.Value,
        Result<int,string>.Failure _ => 0,
        _                            => -1
    };
    Log($"Success: {k}");

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

    p1.Iter(i => Log($"{i}"));
}
