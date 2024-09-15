namespace Sharpl.Libs;

public class Time : Lib
{
    public Time() : base("time", null, [])
    {
        BindMethod("d", ["n"], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Duration, TimeSpan.FromDays(stack.Pop().CastUnbox(loc, Core.Int))));

        BindMethod("h", ["n"], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Duration, TimeSpan.FromHours(stack.Pop().CastUnbox(loc, Core.Int))));

        BindMethod("m", ["n"], (loc, target, vm, stack, arity) =>
             stack.Push(Core.Duration, TimeSpan.FromMinutes(stack.Pop().CastUnbox(loc, Core.Int))));

        BindMethod("ms", ["n"], (loc, target, vm, stack, arity) =>
             stack.Push(Core.Duration, TimeSpan.FromMilliseconds(stack.Pop().CastUnbox(loc, Core.Int))));

        BindMethod("now", [], (loc, target, vm, stack, arity) =>
              stack.Push(Core.Timestamp, DateTime.Now));

        BindMethod("s", ["n"], (loc, target, vm, stack, arity) =>
             stack.Push(Core.Duration, TimeSpan.FromSeconds(stack.Pop().CastUnbox(loc, Core.Int))));

        BindMethod("today", [], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Timestamp, DateTime.Now.Date));

        BindMethod("to-local", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Core.Timestamp).ToLocalTime()));

        BindMethod("to-utc", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Core.Timestamp).ToUniversalTime()));

        BindMethod("us", ["n"], (loc, target, vm, stack, arity) =>
             stack.Push(Core.Duration, TimeSpan.FromMicroseconds(stack.Pop().CastUnbox(loc, Core.Int))));
    }
}