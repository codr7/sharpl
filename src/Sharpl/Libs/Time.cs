namespace Sharpl.Libs;

public class Time : Lib
{
    public Time() : base("time", null, [])
    {
        BindMethod("d", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Days);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromDays(n));
            }
        });

        BindMethod("h", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Hours);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromHours(n));
            }
        });

        BindMethod("m", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Minutes);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromMinutes(n));
            }
        });

        BindMethod("ms", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Milliseconds);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromMilliseconds(n));
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
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromSeconds(n));
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
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromMicroseconds(n));
            }
        });

        BindMethod("w", ["n?"], (loc, target, vm, stack, arity) =>
        {
            if (arity == 1 && stack.Peek().Type == Core.Duration)
            {
                stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Duration).Days / 7);
            }
            else
            {
                var n = (arity == 0) ? 1 : stack.Pop().CastUnbox(loc, Core.Int);
                stack.Push(Core.Duration, TimeSpan.FromDays(n * 7));
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