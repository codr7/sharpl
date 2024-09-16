namespace Sharpl.Libs;

public class Time : Lib
{
    public Time() : base("time", null, [])
    {
        BindMethod("D", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Days);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Day);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromDays(n)));
            }
        });

        BindMethod("h", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Hours);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Hour);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromHours(n)));
            }
        });

        BindMethod("M", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Months);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Month);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(n, TimeSpan.FromTicks(0)));
            }
        });

        BindMethod("m", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Minutes);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Minute);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromMinutes(n)));
            }
        });

        BindMethod("ms", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Milliseconds);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Millisecond);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromMilliseconds(n)));
            }
        });

        BindMethod("now", [], (loc, target, vm, stack, arity) =>
              stack.Push(Core.Timestamp, DateTime.Now));

        BindMethod("s", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Seconds);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Second);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromSeconds(n)));
            }
        });

        BindMethod("to-local", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Core.Timestamp).ToLocalTime()));

        BindMethod("to-utc", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Core.Timestamp).ToUniversalTime()));

        BindMethod("trunc", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Core.Timestamp).Date));

        BindMethod("us", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Microseconds);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Microsecond);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromMicroseconds(n)));
            }
        });

        BindMethod("W", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Days / 7);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).IsoWeek());
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(0, TimeSpan.FromDays(n * 7)));
            }
        });

        BindMethod("Y", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Months % 12);
            }
            else if (arity == 1 && stack.Peek().Type == Core.Timestamp)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Timestamp).Year);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, new Duration(n * 12, TimeSpan.FromTicks(0)));
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