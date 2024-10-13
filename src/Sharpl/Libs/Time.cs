using Sharpl.Ops;

namespace Sharpl.Libs;

public class Time : Lib
{
    public Time(VM vm) : base("time", null, [])
    {
        Value[] mts = [
            Value._,
            Value.Make(Core.Sym, vm.Intern("jan")),
            Value.Make(Core.Sym, vm.Intern("feb")),
            Value.Make(Core.Sym, vm.Intern("mar")),
            Value.Make(Core.Sym, vm.Intern("apr")),
            Value.Make(Core.Sym, vm.Intern("may")),
            Value.Make(Core.Sym, vm.Intern("jun")),
            Value.Make(Core.Sym, vm.Intern("jul")),
            Value.Make(Core.Sym, vm.Intern("aug")),
            Value.Make(Core.Sym, vm.Intern("sep")),
            Value.Make(Core.Sym, vm.Intern("oct")),
            Value.Make(Core.Sym, vm.Intern("nov")),
            Value.Make(Core.Sym, vm.Intern("dec")),
        ];

        Bind("MONTHS", Value.Make(Core.Array, mts));

        var wds = new Value[7];
        for (var i = 0; i < 7; i++) { wds[i] = Value.Make(Core.Sym, vm.Intern(((DayOfWeek)i).ToString().ToLower()[0..2])); }
        Bind("WEEKDAYS", Value.Make(Core.Array, wds));

        BindMethod("D", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Days));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Day));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromDays(n))));
            }
        });

        BindMethod("h", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Hours));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Hour));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromHours(n))));
            }
        });

        BindMethod("M", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Months));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Month));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(n, TimeSpan.FromTicks(0))));
            }
        });

        BindMethod("m", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Minutes));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Minute));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromMinutes(n))));
            }
        });

        BindMethod("ms", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Milliseconds));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Millisecond));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromMilliseconds(n))));
            }
        });

        BindMethod("now", [], (vm, target, arity, result, loc) =>
              vm.Set(result, Value.Make(Core.Timestamp, DateTime.Now)));

        BindMethod("s", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Seconds));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Second));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromSeconds(n))));
            }
        });

        BindMethod("to-local", ["t"], (vm, target, arity, result, loc) =>
           vm.Set(result, Value.Make(Core.Timestamp, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp, loc).ToLocalTime())));

        BindMethod("to-utc", ["t"], (vm, target, arity, result, loc) =>
           vm.Set(result, Value.Make(Core.Timestamp, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp, loc).ToUniversalTime())));

        BindMethod("trunc", ["t"], (vm, target, arity, result, loc) =>
           vm.Set(result, Value.Make(Core.Timestamp, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp, loc).Date)));

        BindMethod("us", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Microseconds));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Microsecond));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromMicroseconds(n))));
            }
        });

        BindMethod("WD", ["n?"], (vm, target, arity, result, loc) =>
            vm.Set(result, Value.Make(Core.Int, (int)vm.GetRegister(0, 0).CastUnbox(Core.Timestamp, loc).DayOfWeek)));

        BindMethod("W", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Days / 7));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).IsoWeek()));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(0, TimeSpan.FromDays(n * 7))));
            }
        });

        BindMethod("Y", ["n?"], (vm, target, arity, result, loc) =>
        {
            if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Duration)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Duration).Months % 12));
            else if (arity == 1 && vm.GetRegister(0, 0).Type == Core.Timestamp)
                vm.Set(result, Value.Make(Core.Int, vm.GetRegister(0, 0).CastUnbox(Core.Timestamp).Year));
            else
            {
                var n = (arity == 0) ? 1 : vm.GetRegister(0, 0).CastUnbox(Core.Int, loc);
                vm.Set(result, Value.Make(Core.Duration, new Duration(n * 12, TimeSpan.FromTicks(0))));
            }
        });
    }

    protected override void OnInit(VM vm)
    {
        Import(vm.CoreLib);

        vm.Eval("""
          (^frac [t]
            (- t (trunc t)))

          (^today []
            (trunc (now)))
        """);
    }
}